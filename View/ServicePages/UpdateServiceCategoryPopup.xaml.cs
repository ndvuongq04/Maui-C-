using BTL_QLHD.Helpers;
using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.ServiceCategoryPages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.ServicePages;

public partial class UpdateServiceCategoryPopup : Popup
{
    public UpdateServiceCategoryPopup(int serviceCategoryId)
    {
        InitializeComponent();
        var vm = new UpdateServiceCategoryPopupViewModel(ServiceHelper.GetService<ServiceCategoryService>(), serviceCategoryId);
        vm.ClosePopupRequested += () => Close(true);
        BindingContext = vm;
        _ = vm.LoadAsync();
    }

    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }
}