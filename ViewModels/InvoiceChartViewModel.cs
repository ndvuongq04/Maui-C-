using CommunityToolkit.Mvvm.ComponentModel;
using Microcharts;
using SkiaSharp;
using System.Globalization;
using BTL_QLHD.Models;
using BTL_QLHD.Services;

namespace BTL_QLHD.ViewModels;

// ViewModel cho trang biểu đồ hóa đơn
public partial class InvoiceChartViewModel : ObservableObject
{
    // Biểu đồ hóa đơn
    [ObservableProperty]
    private Chart? invoiceChart;

    // Tổng số nhà
    [ObservableProperty]
    private int totalHouses;

    // Tổng số dịch vụ
    [ObservableProperty]
    private int totalServices;

    // Số hóa đơn chưa thanh toán
    [ObservableProperty]
    private int unpaidInvoices;

    // Số hóa đơn đã thanh toán
    [ObservableProperty]
    private int paidInvoices;

    // Các service dùng để lấy dữ liệu từ database
    private readonly InvoiceService _invoiceService;
    private readonly HouseService _houseService;
    private readonly ServiceCategoryService _serviceCategoryService;

    // Hàm khởi tạo, inject các service
    public InvoiceChartViewModel(
        InvoiceService invoiceService,
        HouseService houseService,
        ServiceCategoryService serviceCategoryService)
    {
        _invoiceService = invoiceService;
        _houseService = houseService;
        _serviceCategoryService = serviceCategoryService;
        //LoadDataAsync(); // Gọi hàm lấy dữ liệu khi khởi tạo
    }

    // Thêm phương thức này vào trong class InvoiceChartViewModel
    public async Task RefreshAsync()
    {
        await LoadDataAsync();
    }

    // Sửa LoadDataAsync và LoadChartAsync thành async Task thay vì void
    private async Task LoadDataAsync()
    {
        // Lấy tổng số nhà
        TotalHouses = (await _houseService.GetHousesAsync()).Count;
        // Lấy tổng số dịch vụ
        TotalServices = (await _serviceCategoryService.GetServiceCategoriesAsync()).Count;

        // Lấy danh sách hóa đơn
        var invoices = await _invoiceService.GetInvoicesAsync();
        System.Diagnostics.Debug.WriteLine($"[DEBUG] Số hóa đơn lấy được: {invoices.Count}");

        // Đếm số hóa đơn đã thanh toán
        PaidInvoices = invoices.Count(i => i.Status == "Đã thanh toán");
        // Đếm số hóa đơn chưa thanh toán
        UnpaidInvoices = invoices.Count(i => i.Status != "Đã thanh toán");

        // Gọi hàm vẽ biểu đồ
        await LoadChartAsync();
    }

    private async Task LoadChartAsync()
    {
        // Lấy danh sách hóa đơn
        var invoices = await _invoiceService.GetInvoicesAsync();

        // Nếu không có hóa đơn thì trả về biểu đồ rỗng
        if (invoices.Count == 0)
        {
            InvoiceChart = new BarChart { Entries = new List<ChartEntry>() };
            return;
        }

        // Xác định tháng nhỏ nhất và lớn nhất trong dữ liệu
        var minYear = invoices.Min(i => i.Year);
        var minMonth = invoices.Where(i => i.Year == minYear).Min(i => i.Month);
        var maxYear = invoices.Max(i => i.Year);
        var maxMonth = invoices.Where(i => i.Year == maxYear).Max(i => i.Month);

        // Tạo danh sách tất cả các tháng trong khoảng dữ liệu
        var months = new List<(int year, int month)>();
        var year = minYear;
        var month = minMonth;
        while (year < maxYear || (year == maxYear && month <= maxMonth))
        {
            months.Add((year, month));
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
        }

        // Gom nhóm hóa đơn theo từng tháng/năm
        var groups = invoices
            .GroupBy(i => (i.Year, i.Month))
            .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

        // Tạo danh sách entry cho biểu đồ, mỗi entry là một tháng
        var entries = months.Select(m =>
            new ChartEntry(groups.ContainsKey((m.year, m.month)) ? (float)groups[(m.year, m.month)] : 0)
            {
                Label = $"{m.month:D2}/{m.year}",
                ValueLabel = groups.ContainsKey((m.year, m.month)) ? groups[(m.year, m.month)].ToString("N0") : "0",
                Color = SKColor.Parse("#3498db")
            }).ToList();

        // Gán dữ liệu cho biểu đồ
        InvoiceChart = new BarChart
        {
            Entries = entries,
            LabelTextSize = 28,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelOrientation = Orientation.Horizontal
        };
    }
}