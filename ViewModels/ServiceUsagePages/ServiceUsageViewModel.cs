using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.ViewModels.ServiceUsagePages
{
    public partial class ServiceUsageViewModel : ObservableObject
    {
        private readonly ServiceUsageService _serviceUsageService;

        [ObservableProperty]
        private int houseId;

        [ObservableProperty]
        private int serviceId;

        [ObservableProperty]
        private int year;

        [ObservableProperty]
        private int month;

        [ObservableProperty]
        private int usageValue;

        public ServiceUsageViewModel(ServiceUsageService serviceUsageService)
        {
            _serviceUsageService = serviceUsageService;
            year = DateTime.Now.Year;
            month = DateTime.Now.Month;
        }

        [RelayCommand]
        public async Task AddAsync()
        {
            try
            {
                // Lấy InvoiceId từ houseId, year, month
                var invoiceService = Helpers.ServiceHelper.GetService<InvoiceService>();
                var invoice = await invoiceService.GetInvoiceByHouseAndTimeAsync(houseId, year, month);
                if (invoice == null)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Lỗi", "Không tìm thấy hóa đơn cho nhà, tháng, năm này.", "OK");
                    return;
                }

                // Kiểm tra trùng lặp usage theo InvoiceId và ServiceId
                bool exists = await _serviceUsageService.ExistsUsageAsync(invoice.Id, serviceId);
                if (exists)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Lỗi", "Dữ liệu sử dụng dịch vụ đã tồn tại cho hóa đơn này.", "OK");
                    return;
                }

                var usage = new ServiceUsage
                {
                    InvoiceId = invoice.Id,
                    ServiceId = serviceId,
                    Year = year,
                    Month = month,
                    UsageValue = usageValue
                };

                await _serviceUsageService.AddServiceUsageAsync(usage);
                await Shell.Current.CurrentPage.DisplayAlert("Thành công", "Đã thêm dịch vụ sử dụng", "OK");

                // Reset form sau khi thêm
                houseId = 0;
                serviceId = 0;
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                usageValue = 0;
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Lỗi", $"Không thể thêm: {ex.Message}", "OK");
            }
        }
    }
}