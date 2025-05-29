using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.InvoicePages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.InvoicePages;

public partial class ViewDetailInvoicePopup : Popup // <-- phải kế thừa từ Popup
{
    public ViewDetailInvoicePopup(int invoiceId)
    {
        InitializeComponent();
        BindingContext = new ViewDetailInvoicePopupViewModel(
            invoiceId,
            Helpers.ServiceHelper.GetService<InvoiceService>(),
            Helpers.ServiceHelper.GetService<HouseService>(),
            Helpers.ServiceHelper.GetService<ServiceUsageService>(),
            Helpers.ServiceHelper.GetService<ServiceCategoryService>());
    }

    private void OnCloseClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }
}