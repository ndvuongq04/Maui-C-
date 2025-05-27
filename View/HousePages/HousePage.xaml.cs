//using Android.App.Job;
using BTL_QLHD.Helpers;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using BTL_QLHD.View.HousePages;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BTL_QLHD.View.HousePages
{
    public partial class HousePage : ContentPage
    {
        private readonly HouseService _houseService;
        // Danh sách gốc (dùng để reset hoặc tìm kiếm lại)
        private ObservableCollection<House> houses;

        // Danh sách hiển thị trên CollectionView, có thể bị lọc/sắp xếp
        private ObservableCollection<House> filteredHouses;

        // Enum đại diện các cột có thể sắp xếp
        public enum SortField
        {
            Id,
            SoNha,
            TenChuNha,
            SDTChuNha,
            DiaChi
        }

        // Dictionary để lưu trạng thái sắp xếp tăng/giảm của từng cột
        private Dictionary<SortField, bool> sortDirections = new()
        {
            { SortField.Id, true },
            { SortField.SoNha, true },
            { SortField.TenChuNha, true },
            { SortField.SDTChuNha, true },
            { SortField.DiaChi, true }
        };

        public HousePage()
        {
            InitializeComponent();
            _houseService = ServiceHelper.GetService<HouseService>();

            // Khởi tạo danh sách rỗng cho houses và filteredHouses
            houses = new ObservableCollection<House>();
            filteredHouses = new ObservableCollection<House>();

            // CollectionView luôn hiển thị filteredHouses (danh sách đã lọc)
            houseCollectionView.ItemsSource = filteredHouses;

            // Gọi hàm load dữ liệu từ DB 
            LoadHousesFromDb();
        }

        // Hàm load dữ liệu từ DB, đồng bộ cả houses và filteredHouses
        private async void LoadHousesFromDb()
        {
            var list = await _houseService.GetHousesAsync();

            houses.Clear();
            foreach (var house in list)
                houses.Add(house);

            filteredHouses.Clear();
            foreach (var house in houses)
                filteredHouses.Add(house);
        }

        // Hàm tìm kiếm: lọc houses theo keyword, kết quả gán vào filteredHouses
        private void OnSearchBarButtonPressed(object sender, System.EventArgs e)
        {
            string keyword = searchBar.Text?.ToLower() ?? "";

            // Lọc theo tất cả các trường, kiểm tra null để tránh lỗi
            var filtered = houses.Where(h =>
                (h.HouseNumber?.ToLower().Contains(keyword) ?? false) ||
                (h.OwnerName?.ToLower().Contains(keyword) ?? false) ||
                (h.OwnerPhone?.ToLower().Contains(keyword) ?? false) ||
                (h.Address?.ToLower().Contains(keyword) ?? false)
            ).ToList();

            filteredHouses.Clear();
            foreach (var house in filtered)
                filteredHouses.Add(house);
        }

        // Mở popup thêm mới, sau khi thêm xong load lại danh sách
        private async void OnAddClicked(object sender, System.EventArgs e)
        {
            var popup = new AddHousePopup();
            await this.ShowPopupAsync(popup);

            // Khi popup đóng, load lại danh sách nhà
            LoadHousesFromDb();
        }

        // Hàm mở popup sửa, lấy House từ CommandParameter của Button
        private async void OnEditItemClicked(object sender, System.EventArgs e)
        {
            // Lấy House từ CommandParameter của Button
            if (sender is Button btn && btn.CommandParameter is House selectedHouse)
            {
                // Truyền id sang popup sửa
                var popup = new UpdateHousePopup(selectedHouse.Id);
                await this.ShowPopupAsync(popup);

                // Sau khi sửa xong, load lại danh sách
                LoadHousesFromDb();
            }
        }

        // Làm mới lại danh sách, hiển thị lại toàn bộ houses
        private void OnRefreshClicked(object sender, System.EventArgs e)
        {
            filteredHouses.Clear();
            foreach (var house in houses)
                filteredHouses.Add(house);

            searchBar.Text = "";
        }

        // Hàm dùng chung để sắp xếp theo cột
        // Hàm dùng để sắp xếp danh sách theo một trường cụ thể (Id, Số nhà, Chủ nhà, SĐT, Địa chỉ)
        // Thêm biến lưu trữ thứ tự và chiều sắp xếp của các trường
        private List<SortField> sortPriority = new List<SortField>();

        private void SortByField(SortField field, Label sortIconLabel)
        {
            // Nếu trường đã có trong danh sách sortPriority thì đảo chiều sắp xếp,
            // đồng thời đưa trường đó lên đầu danh sách ưu tiên
            if (sortPriority.Contains(field))
            {
                sortDirections[field] = !sortDirections[field];
                sortPriority.Remove(field);
                sortPriority.Insert(0, field);
            }
            else
            {
                // Trường mới được chọn, mặc định giữ chiều hiện tại (true là tăng)
                sortPriority.Insert(0, field);
            }

            // Reset icon của các cột khác về ẩn hoặc rỗng
            SortIdIconLabel.Text = "";
            SortSoNhaIconLabel.Text = "";
            SortTenChuNhaIconLabel.Text = "";
            SortSDTIconLabel.Text = "";
            SortDiaChiIconLabel.Text = "";

            // Áp dụng sắp xếp đa cấp
            IOrderedEnumerable<House> sortedList = null;

            foreach (var f in sortPriority)
            {
                bool ascending = sortDirections[f];

                Func<House, object> keySelector = f switch
                {
                    SortField.Id => h => h.Id,
                    SortField.SoNha => h => h.HouseNumber,
                    SortField.TenChuNha => h => h.OwnerName,
                    SortField.SDTChuNha => h => h.OwnerPhone,
                    SortField.DiaChi => h => h.Address,
                    _ => h => h.Id
                };

                if (sortedList == null)
                {
                    sortedList = ascending
                        ? filteredHouses.OrderBy(keySelector)
                        : filteredHouses.OrderByDescending(keySelector);
                }
                else
                {
                    sortedList = ascending
                        ? sortedList.ThenBy(keySelector)
                        : sortedList.ThenByDescending(keySelector);
                }
            }

            // Cập nhật danh sách hiển thị
            var tempList = sortedList?.ToList() ?? filteredHouses.ToList();
            filteredHouses.Clear();
            foreach (var house in tempList)
                filteredHouses.Add(house);

            // Cập nhật icon mũi tên cho cột đang sort đầu tiên
            sortIconLabel.Text = sortDirections[field] ? "▲" : "▼";
            sortIconLabel.IsVisible = true;
        }

        // Các hàm xử lý sự kiện cho các cột
        private void OnSortIdClicked(object sender, EventArgs e) => SortByField(SortField.Id, SortIdIconLabel);
        private void OnSortSoNhaClicked(object sender, EventArgs e) => SortByField(SortField.SoNha, SortSoNhaIconLabel);
        private void OnSortTenChuNhaClicked(object sender, EventArgs e) => SortByField(SortField.TenChuNha, SortTenChuNhaIconLabel);
        private void OnSortSDTClicked(object sender, EventArgs e) => SortByField(SortField.SDTChuNha, SortSDTIconLabel);
        private void OnSortDiaChiClicked(object sender, EventArgs e) => SortByField(SortField.DiaChi, SortDiaChiIconLabel);

        // Thêm các hàm xử lý sự kiện cho cột Số nhà và Chủ nhà
        // id
        private void OnSortIdTapped(object sender, EventArgs e)
        {
            SortByField(SortField.Id, SortIdIconLabel);
        }
        // số nhà
        private void OnSortSoNhaTapped(object sender, EventArgs e)
        {
            SortByField(SortField.SoNha, SortSoNhaIconLabel);
        }
        // Tên chủ nhà
        private void OnSortTenChuNhaTapped(object sender, EventArgs e)
        {
            SortByField(SortField.TenChuNha, SortTenChuNhaIconLabel);
        }
        // SDT
        private void OnSortSDTTapped(object sender, EventArgs e)
        {
            SortByField(SortField.SDTChuNha, SortSDTIconLabel);
        }
        // Địa chỉ
        private void OnSortDiaChiTapped(object sender, EventArgs e)
        {
            SortByField(SortField.DiaChi, SortDiaChiIconLabel);
        }
    }
}
