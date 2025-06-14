using BTL_QLHD.Helpers;
using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.ServiceCategoryPages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.ServicePages;

public partial class AddServiceCategoryPopup : Popup
{
    public AddServiceCategoryPopup()
    {
        InitializeComponent();
        var vm = new AddServiceCategoryPopupViewModel(ServiceHelper.GetService<ServiceCategoryService>());
        vm.ClosePopupRequested += () => Close(true);
        BindingContext = vm;
    }

    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }


}
