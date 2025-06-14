using BTL_QLHD.Helpers;
using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.ServiceCategoryPages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.ServicePages;

public partial class DeleteServiceCategoryPopup : Popup
{
    public DeleteServiceCategoryPopup(int serviceCategoryId)
    {
        InitializeComponent();
        var vm = new DeleteServiceCategoryPopupViewModel(ServiceHelper.GetService<ServiceCategoryService>(), serviceCategoryId);
        vm.ClosePopupRequested += () => Close(true);
        BindingContext = vm;
    }

    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }
}