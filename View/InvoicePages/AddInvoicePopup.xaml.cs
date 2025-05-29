using BTL_QLHD.ViewModels.InvoicePages;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.InvoicePages;

public partial class AddInvoicePopup : Popup
{
    public AddInvoicePopup()
    {
        InitializeComponent();
        BindingContext = new AddInvoicePopupViewModel(
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