using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.InvoicePages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.InvoicePages;

public partial class ViewDetailInvoicePopup : CommunityToolkit.Maui.Views.Popup
{
    public ViewDetailInvoicePopup(int invoiceId, InvoiceService invoiceService, HouseService houseService, ServiceUsageService serviceUsageService, ServiceCategoryService serviceCategoryService)
    {
        InitializeComponent();
        var vm = new ViewDetailInvoicePopupViewModel(invoiceId, invoiceService, houseService, serviceUsageService, serviceCategoryService);
        BindingContext = vm;
        // Gọi reload ngay sau khi gán BindingContext
        _ = vm.ReloadAsync(invoiceId);
    }

    private void OnCloseClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }
}