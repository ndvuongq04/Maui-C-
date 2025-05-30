using BTL_QLHD.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BTL_QLHD.Models;
using CommunityToolkit.Maui.Views;
using BTL_QLHD.View.InvoicePages;
using System.Windows.Input;
using SQLite;

namespace BTL_QLHD.ViewModels.InvoicePages
{
    public partial class InvoicePageViewModel : ObservableObject
    {
        private readonly InvoiceService _invoiceService;
        private readonly HouseService _houseService;

        [ObservableProperty]
        private string searchKeyword;

        [ObservableProperty]
        private ObservableCollection<Invoice> filteredInvoices = new();

        [ObservableProperty]
        private string? selectedStatus = "Tất cả";

        [ObservableProperty]
        private string? selectedMonthYear = "Tất cả";

        public List<string> MonthYearList { get; set; } = new();

        public List<string> StatusList { get; } = new() { "Tất cả", "Đã thanh toán", "Chưa thanh toán" };

        public ICommand ViewHouseDetailCommand { get; }

        // Constructor mới nhận cả 2 service
        public InvoicePageViewModel(InvoiceService invoiceService, HouseService houseService)
        {
            _invoiceService = invoiceService;
            _houseService = houseService;

            // Initialize non-nullable fields/properties
            searchKeyword = string.Empty;
            //ViewDetailCommand = new RelayCommand<Invoice>(invoice => OnViewDetail(invoice));

            _ = LoadInvoicesAsync();

            ViewHouseDetailCommand = new RelayCommand<House>(OnViewHouseDetail!);
        }

        // Tải danh sách hóa đơn và gán thông tin House
        public async Task LoadInvoicesAsync()
        {
            var allInvoices = await _invoiceService.GetInvoicesAsync();
            var allHouses = await _houseService.GetHousesAsync();

            foreach (var invoice in allInvoices)
            {
                invoice.House = allHouses.FirstOrDefault(h => h.Id == invoice.HouseId) ?? new House();
            }

            // Tạo danh sách tháng/năm duy nhất từ DB
            MonthYearList = allInvoices
                .Select(i => $"{i.Month:D2}/{i.Year}")
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();
            MonthYearList.Insert(0, "Tất cả");
            OnPropertyChanged(nameof(MonthYearList));

            FilteredInvoices = new ObservableCollection<Invoice>(ApplyFilter(allInvoices, SearchKeyword, SelectedMonthYear, SelectedStatus));
        }

        // Áp dụng filter theo từ khóa, số nhà, tên chủ nhà, địa chỉ
        private IEnumerable<Invoice> ApplyFilter(IEnumerable<Invoice> invoices, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return invoices.OrderByDescending(i => i.CreatedDate);

            keyword = keyword.ToLower();
            return invoices.Where(i =>
                i.Month.ToString().Contains(keyword) ||
                i.Year.ToString().Contains(keyword) ||
                (i.Status != null && i.Status.ToLower().Contains(keyword)) ||
                // Tìm kiếm theo tổng hóa đơn (tổng tiền)
                i.TotalAmount.ToString("N0").Contains(keyword) ||
                i.TotalAmount.ToString().Contains(keyword) ||
                (i.House != null && (
                    (i.House.HouseNumber?.ToLower().Contains(keyword) ?? false) ||
                    (i.House.OwnerName?.ToLower().Contains(keyword) ?? false) ||
                    (i.House.Address?.ToLower().Contains(keyword) ?? false)
                ))
            ).OrderByDescending(i => i.CreatedDate);
        }

        private IEnumerable<Invoice> ApplyFilter(IEnumerable<Invoice> invoices, string keyword, string? monthYear, string? status)
        {
            var filtered = invoices;

            if (!string.IsNullOrWhiteSpace(monthYear) && monthYear != "Tất cả")
            {
                filtered = filtered.Where(i => $"{i.Month:D2}/{i.Year}" == monthYear);
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "Tất cả")
            {
                filtered = filtered.Where(i => i.Status != null && i.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lower = keyword.ToLower();
                filtered = filtered.Where(i =>
                    i.Month.ToString().Contains(lower) ||
                    i.Year.ToString().Contains(lower) ||
                    (i.Status != null && i.Status.ToLower().Contains(lower)) ||
                    i.TotalAmount.ToString("N0").Contains(lower) ||
                    i.TotalAmount.ToString().Contains(lower) ||
                    (i.House != null && (
                        (i.House.HouseNumber?.ToLower().Contains(lower) ?? false) ||
                        (i.House.OwnerName?.ToLower().Contains(lower) ?? false) ||
                        (i.House.Address?.ToLower().Contains(lower) ?? false)
                    ))
                );
            }

            return filtered.OrderByDescending(i => i.CreatedDate);
        }

        // Khi thay đổi từ khóa tìm kiếm
        partial void OnSearchKeywordChanged(string value)
        {
            _ = SearchAsync();
        }

        partial void OnSelectedMonthYearChanged(string? value)
        {
            _ = SearchAsync();
        }

        partial void OnSelectedStatusChanged(string? value)
        {
            _ = SearchAsync();
        }

        private async Task SearchAsync()
        {
            var allInvoices = await _invoiceService.GetInvoicesAsync();
            var allHouses = await _houseService.GetHousesAsync();

            foreach (var invoice in allInvoices)
            {
                invoice.House = allHouses.FirstOrDefault(h => h.Id == invoice.HouseId);
            }

            FilteredInvoices = new ObservableCollection<Invoice>(
                ApplyFilter(allInvoices, SearchKeyword, SelectedMonthYear, SelectedStatus)
            );
        }

        [RelayCommand]
        public async Task AddAsync()
        {
            var popup = new AddInvoicePopup();
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            await LoadInvoicesAsync(); // Reload sau khi thêm
        }

        [RelayCommand]
        public async Task ViewDetailAsync(Invoice invoice)
        {
            if (invoice == null) return;
            var popup = new ViewDetailInvoicePopup(
                invoice.Id, 
                _invoiceService, 
                _houseService, 
                new ServiceUsageService(GetConnectionFromInvoiceService(_invoiceService)), // Use helper method
                new ServiceCategoryService(GetConnectionFromInvoiceService(_invoiceService)) // Use helper method
            );
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }

        [RelayCommand]
        public async Task EditAsync(Invoice invoice)
        {
            if (invoice == null) return;
            var popup = new UpdateInvoicePopup(invoice.Id);
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            await LoadInvoicesAsync();
        }

        private async void OnViewHouseDetail(House? house)
        {
            if (house == null) return;

            var invoice = FilteredInvoices.FirstOrDefault(i => i.HouseId == house.Id);
            if (invoice == null) return;

            var popup = new ViewDetailInvoicePopup(
                invoice.Id, 
                _invoiceService, 
                _houseService, 
                new ServiceUsageService(GetConnectionFromInvoiceService(_invoiceService)), // Pass connection
                new ServiceCategoryService(GetConnectionFromInvoiceService(_invoiceService)) // Pass connection
            );
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }

        private async Task OnViewDetail(Invoice? invoice)
        {
            if (invoice == null) return;
            var popup = new ViewDetailInvoicePopup(
                invoice.Id,
                _invoiceService,
                _houseService,
                new ServiceUsageService(GetConnectionFromInvoiceService(_invoiceService)),
                new ServiceCategoryService(GetConnectionFromInvoiceService(_invoiceService))
            );
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }

        private SQLiteAsyncConnection GetConnectionFromInvoiceService(InvoiceService invoiceService)
        {
            // Assuming InvoiceService does not have a GetConnection method, 
            // expose the _connection field via a public property or method.
            return invoiceService.Connection;
        }
    }
}
