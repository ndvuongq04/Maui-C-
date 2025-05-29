using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;
using System.Linq;

namespace BTL_QLHD.ViewModels.InvoicePages
{
    public partial class ServiceUsageInput : ObservableObject
    {
        [ObservableProperty] private int serviceCategoryId;
        [ObservableProperty] private string serviceName = string.Empty;
        [ObservableProperty] private string unit = string.Empty;
        [ObservableProperty] private int usageValue;
        [ObservableProperty] private float price;

        // Thêm property này để binding ra XAML
        public string PriceWithUnit => $"{Price:N0} VND";

        // Sự kiện để ViewModel đăng ký
        public event Action? UsageValueChanged;

        partial void OnUsageValueChanged(int value)
        {
            UsageValueChanged?.Invoke();
        }
    }

    public partial class AddInvoicePopupViewModel : ObservableObject
    {
        private readonly InvoiceService _invoiceService;
        private readonly HouseService _houseService;
        private readonly ServiceCategoryService _serviceCategoryService;
        private readonly Popup _popup;
        private readonly ServiceUsageService _serviceUsageService;

        [ObservableProperty] private ObservableCollection<House> houses = new();
        [ObservableProperty] private House? selectedHouse;

        [ObservableProperty] private int month = DateTime.Now.Month;
        [ObservableProperty] private int year = DateTime.Now.Year;
        [ObservableProperty] private DateTime invoiceDate = DateTime.Now;
        [ObservableProperty] private string status = "Chưa thanh toán";
        [ObservableProperty] private string totalAmount = string.Empty;
        [ObservableProperty] private string description = string.Empty;

        [ObservableProperty] private ObservableCollection<ServiceUsageInput> serviceUsages = new();

        public List<int> MonthList { get; } = Enumerable.Range(1, 12).ToList();

        public AddInvoicePopupViewModel(
            InvoiceService invoiceService,
            HouseService houseService,
            ServiceCategoryService serviceCategoryService,
            ServiceUsageService serviceUsageService, // thêm vào đây
            Popup popup)
        {
            _invoiceService = invoiceService;
            _houseService = houseService;
            _serviceCategoryService = serviceCategoryService;
            _serviceUsageService = serviceUsageService; // gán vào đây
            _popup = popup;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var houseList = await _houseService.GetHousesAsync();
            Houses = new ObservableCollection<House>(houseList);

            var serviceList = await _serviceCategoryService.GetServiceCategoriesAsync();
            var usages = serviceList.Select(s => new ServiceUsageInput
            {
                ServiceCategoryId = s.Id,
                ServiceName = s.Name,
                Unit = s.Unit,
                UsageValue = 0,
                Price = s.Price
            }).ToList();

            // Đăng ký sự kiện cho từng usage
            foreach (var usage in usages)
            {
                usage.UsageValueChanged += UpdateTotalAmount;
            }

            ServiceUsages = new ObservableCollection<ServiceUsageInput>(usages);

            // Tính tổng tiền ban đầu
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            var sum = ServiceUsages.Sum(u => u.UsageValue * (decimal)u.Price);
            TotalAmount = sum.ToString("N0");
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (SelectedHouse == null || month <= 0 || year <= 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Lỗi", "Vui lòng chọn nhà và nhập tháng/năm.", "OK");
                return;
            }

            if (!decimal.TryParse(TotalAmount, out decimal amount))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Lỗi", "Tổng tiền không hợp lệ.", "OK");
                return;
            }

            if (await _invoiceService.ExistsInvoiceAsync(SelectedHouse.Id, year, month))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Lỗi", "Hóa đơn cho nhà này, tháng này, năm này đã tồn tại.", "OK");
                return;
            }

            var invoice = new Invoice
            {
                HouseId = SelectedHouse.Id,
                Month = month,
                Year = year,
                CreatedDate = invoiceDate,
                TotalAmount = amount,
                Status = status,
                Note = description
            };

            // Thêm invoice trước và lấy Id vừa tạo
            var invoiceId = await _invoiceService.AddInvoiceAsync(invoice);

            // Lưu các ServiceUsageInput thành ServiceUsage với đúng InvoiceId vừa tạo
            foreach (var usage in ServiceUsages)
            {
                if (usage.UsageValue > 0)
                {
                    var serviceUsage = new ServiceUsage
                    {
                        InvoiceId = invoice.Id, // Đảm bảo invoice.Id đã được cập nhật sau khi thêm
                        ServiceId = usage.ServiceCategoryId,
                        Year = year,
                        Month = month,
                        UsageValue = usage.UsageValue
                    };
                    await _serviceUsageService.AddServiceUsageAsync(serviceUsage);
                }
            }

            await Shell.Current.CurrentPage.DisplayAlert("Thành công", "Đã thêm hóa đơn mới.", "OK");
            _popup.Close();
        }
    }
}
