using BTL_QLHD.ViewModels.InvoicePages;

namespace BTL_QLHD.View.InvoicePages;

public partial class InvoicePage : ContentPage
{
    public InvoicePage()
    {
        InitializeComponent();
        BindingContext = new InvoicePageViewModel(
            Helpers.ServiceHelper.GetService<Services.InvoiceService>(),
            Helpers.ServiceHelper.GetService<Services.HouseService>());
    }
}