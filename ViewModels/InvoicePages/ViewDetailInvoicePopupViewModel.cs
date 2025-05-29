using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using BTL_QLHD.Models;
using BTL_QLHD.Services;

namespace BTL_QLHD.ViewModels.InvoicePages
{
    public partial class ViewDetailInvoicePopupViewModel : ObservableObject
    {
        private readonly InvoiceService _invoiceService;
        private readonly HouseService _houseService;
        private readonly ServiceUsageService _serviceUsageService;
        private readonly ServiceCategoryService _serviceCategoryService;

        [ObservableProperty] private Invoice? invoice;
        [ObservableProperty] private House? house;
        [ObservableProperty] private ObservableCollection<ServiceUsageDetail> serviceUsages = new();

        [ObservableProperty] private int month;
        [ObservableProperty] private int year;
        [ObservableProperty] private DateTime invoiceDate;
        [ObservableProperty] private string status = string.Empty;
        [ObservableProperty] private string totalAmount = string.Empty;
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private House? selectedHouse;

        public ViewDetailInvoicePopupViewModel(
            int invoiceId,
            InvoiceService invoiceService,
            HouseService houseService,
            ServiceUsageService serviceUsageService,
            ServiceCategoryService serviceCategoryService)
        {
            _invoiceService = invoiceService;
            _houseService = houseService;
            _serviceUsageService = serviceUsageService;
            _serviceCategoryService = serviceCategoryService;
            _ = LoadAsync(invoiceId);
        }

        public ViewDetailInvoicePopupViewModel(Invoice invoice)
        {
            SelectedHouse = invoice.House;
            // Gán các thuộc tính khác nếu cần
        }

        private async Task LoadAsync(int invoiceId)
        {
            Invoice = await _invoiceService.GetInvoiceAsync(invoiceId);
            if (Invoice != null)
            {
                House = await _houseService.GetHouseAsync(Invoice.HouseId);

                // Gán House cho SelectedHouse để binding ra UI
                SelectedHouse = House;

                Month = Invoice.Month;
                Year = Invoice.Year;
                InvoiceDate = Invoice.CreatedDate;
                Status = Invoice.Status;
                TotalAmount = Invoice.TotalAmount.ToString("N0");
                Description = Invoice.Note;

                var usages = await _serviceUsageService.GetServiceUsagesByInvoiceAsync(Invoice.Id);
                var categories = await _serviceCategoryService.GetServiceCategoriesAsync();

                ServiceUsages = new ObservableCollection<ServiceUsageDetail>(
                    from usage in usages
                    join cat in categories on usage.ServiceId equals cat.Id
                    select new ServiceUsageDetail
                    {
                        ServiceName = cat.Name,
                        Unit = cat.Unit,
                        Price = cat.Price,
                        UsageValue = usage.UsageValue
                    });
            }
        }

        public async Task ReloadAsync()
        {
            if (Invoice != null)
            {
                await LoadAsync(Invoice.Id);
            }
        }
    }

    // Model view cho hiển thị dịch vụ sử dụng
    public class ServiceUsageDetail
    {
        public string ServiceName { get; set; }
        public string Unit { get; set; }
        public float Price { get; set; }
        public int UsageValue { get; set; }
        public string PriceWithUnit => $"{Price:N0} VND";
        public float Total => Price * UsageValue; // Tổng tiền cho usage này

        // Thêm property này nếu muốn binding ra XAML
        public string TotalWithUnit => $"{Total:N0} VND";
    }
}


