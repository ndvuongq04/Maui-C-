using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.InvoicePages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.InvoicePages;

public partial class UpdateInvoicePopup : Popup
{
    public UpdateInvoicePopup(int invoiceId)
    {
        InitializeComponent();
        BindingContext = new UpdateInvoicePopupViewModel(
            invoiceId,
            Helpers.ServiceHelper.GetService<InvoiceService>(),
            Helpers.ServiceHelper.GetService<HouseService>(),
            Helpers.ServiceHelper.GetService<ServiceCategoryService>(),
            Helpers.ServiceHelper.GetService<ServiceUsageService>(),
            this);
    }
    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }
}