using BTL_QLHD.ViewModels;

namespace BTL_QLHD.View;

public partial class InvoiceChartPage : ContentPage
{
    public InvoiceChartPage()
    {
        InitializeComponent();
    }

    public InvoiceChartPage(InvoiceChartViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // Gọi RefreshAsync mỗi khi trang xuất hiện để luôn lấy dữ liệu mới nhất
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine($"[DEBUG] BindingContext: {BindingContext?.GetType().Name}");
        if (BindingContext is InvoiceChartViewModel vm)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] Gọi RefreshAsync");
            await vm.RefreshAsync();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] BindingContext KHÔNG phải InvoiceChartViewModel");
        }
    }
}