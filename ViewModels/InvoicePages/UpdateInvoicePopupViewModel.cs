using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.ViewModels.InvoicePages
{
    public partial class UpdateServiceUsageInput : ObservableObject
    {
        [ObservableProperty] private int serviceCategoryId;
        [ObservableProperty] private string serviceName = string.Empty;
        [ObservableProperty] private string unit = string.Empty;
        [ObservableProperty] private int usageValue;
        [ObservableProperty] private float price;
        public string PriceWithUnit => $"{Price:N0} VND";
        public event Action? UsageValueChanged;

        partial void OnUsageValueChanged(int value)
        {
            UsageValueChanged?.Invoke();
            OnPropertyChanged(nameof(UsageValue));
        }
    }

    public partial class UpdateInvoicePopupViewModel : ObservableObject
    {
        private readonly InvoiceService _invoiceService;
        private readonly HouseService _houseService;
        private readonly ServiceCategoryService _serviceCategoryService;
        private readonly ServiceUsageService _serviceUsageService;
        private readonly Popup _popup;

        [ObservableProperty] private Invoice? invoice;
        [ObservableProperty] private ObservableCollection<House> houses = new();
        [ObservableProperty] private House? selectedHouse;
        [ObservableProperty] private int month;
        [ObservableProperty] private int year;
        [ObservableProperty] private DateTime invoiceDate;
        [ObservableProperty] private string status = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string totalAmount = string.Empty;
        [ObservableProperty] private ObservableCollection<UpdateServiceUsageInput> serviceUsages = new();

        public List<string> StatusList { get; } = new() { "Đã thanh toán", "Chưa thanh toán" };
        public List<int> MonthList { get; } = Enumerable.Range(1, 12).ToList();

        public Action? OnReloadDetailInvoice { get; set; }

        public UpdateInvoicePopupViewModel(
            int invoiceId,
            InvoiceService invoiceService,
            HouseService houseService,
            ServiceCategoryService serviceCategoryService,
            ServiceUsageService serviceUsageService,
            Popup popup)
        {
            _invoiceService = invoiceService;
            _houseService = houseService;
            _serviceCategoryService = serviceCategoryService;
            _serviceUsageService = serviceUsageService;
            _popup = popup;
            _ = LoadDataAsync(invoiceId);
        }

        private async Task LoadDataAsync(int invoiceId)
        {
            Invoice = await _invoiceService.GetInvoiceAsync(invoiceId);
            if (Invoice == null) return;

            var houseList = await _houseService.GetHousesAsync();
            Houses = new ObservableCollection<House>(houseList);

            SelectedHouse = Houses.FirstOrDefault(h => h.Id == Invoice.HouseId);

            Month = Invoice.Month;
            Year = Invoice.Year;
            InvoiceDate = Invoice.CreatedDate;
            Status = Invoice.Status;
            Description = Invoice.Note;

            // Lấy usage đúng theo InvoiceId
            var usages = await _serviceUsageService.GetServiceUsagesByInvoiceAsync(Invoice.Id);
            var serviceList = await _serviceCategoryService.GetServiceCategoriesAsync();

            // Hiển thị tất cả dịch vụ, nếu chưa có usage thì tạo usage value = 0
            var usageInputs = serviceList.Select(cat =>
            {
                var usage = usages.FirstOrDefault(u => u.ServiceId == cat.Id);
                var input = new UpdateServiceUsageInput
                {
                    ServiceCategoryId = cat.Id,
                    ServiceName = cat.Name,
                    Unit = cat.Unit,
                    Price = cat.Price,
                    UsageValue = usage?.UsageValue ?? 0
                };
                input.UsageValueChanged += UpdateTotalAmount;
                return input;
            }).ToList();

            ServiceUsages = new ObservableCollection<UpdateServiceUsageInput>(usageInputs);

            // Đăng ký lại sự kiện khi ServiceUsages thay đổi
            foreach (var input in ServiceUsages)
            {
                input.UsageValueChanged -= UpdateTotalAmount;
                input.UsageValueChanged += UpdateTotalAmount;
            }
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            // Tính lại tổng tiền mỗi khi có thay đổi usage
            var sum = ServiceUsages.Sum(u => u.UsageValue * (decimal)u.Price);
            TotalAmount = sum.ToString("N0");
            if (Invoice != null)
            {
                Invoice.TotalAmount = sum;
            }
        }

        [RelayCommand]
        public async Task UpdateAsync()
        {
            if (Invoice == null) return;

            // Tính lại tổng tiền từ ServiceUsages
            var sum = ServiceUsages.Sum(u => u.UsageValue * (decimal)u.Price);
            Invoice.TotalAmount = sum;
            TotalAmount = sum.ToString("N0");

            if (!decimal.TryParse(TotalAmount, out decimal amount))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Lỗi", "Tổng tiền không hợp lệ.", "OK");
                return;
            }

            // Cập nhật lại HouseId nếu chọn nhà khác
            if (SelectedHouse != null)
            {
                Invoice.HouseId = SelectedHouse.Id;
                await _houseService.UpdateHouseAsync(SelectedHouse);
            }

            Invoice.Month = Month;
            Invoice.Year = Year;
            Invoice.CreatedDate = InvoiceDate;
            Invoice.Status = Status;
            Invoice.Note = Description;

            // 1. Cập nhật bảng invoice trước (đã có tổng tiền mới)
            await _invoiceService.UpdateInvoiceAsync(Invoice);

            // 2. Lấy lại usages cũ từ DB
            var oldUsages = await _serviceUsageService.GetServiceUsagesByInvoiceAsync(Invoice.Id);

            // 3. Cập nhật, thêm mới hoặc xóa usages theo ServiceUsages hiện tại
            foreach (var usage in ServiceUsages)
            {
                var old = oldUsages.FirstOrDefault(u => u.ServiceId == usage.ServiceCategoryId);
                if (usage.UsageValue > 0)
                {
                    if (old != null)
                    {
                        old.UsageValue = usage.UsageValue;
                        old.Year = Invoice.Year;
                        old.Month = Invoice.Month;
                        await _serviceUsageService.UpdateServiceUsageAsync(old);
                    }
                    else
                    {
                        var newUsage = new ServiceUsage
                        {
                            InvoiceId = Invoice.Id,
                            ServiceId = usage.ServiceCategoryId,
                            Year = Invoice.Year,
                            Month = Invoice.Month,
                            UsageValue = usage.UsageValue
                        };
                        await _serviceUsageService.AddServiceUsageAsync(newUsage);
                    }
                }
                else if (old != null)
                {
                    await _serviceUsageService.DeleteServiceUsageAsync(old.Id);
                }
            }

            if (OnReloadDetailInvoice != null)
            {
                OnReloadDetailInvoice.Invoke();
            }

            await Shell.Current.CurrentPage.DisplayAlert("Thành công", "Cập nhật hóa đơn thành công.", "OK");
            _popup.Close();
        }
    }
}
