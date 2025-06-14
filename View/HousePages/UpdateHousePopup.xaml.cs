using BTL_QLHD.Helpers;
using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.HousePages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.HousePages;

public partial class UpdateHousePopup : Popup
{
    public UpdateHousePopup(int houseId)
    {
        InitializeComponent();
        var vm = new UpdateHousePopupViewModel(ServiceHelper.GetService<HouseService>(), houseId);
        vm.ClosePopupRequested += () => Close(true);
        BindingContext = vm;
        _ = vm.LoadAsync();
    }

    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }
}