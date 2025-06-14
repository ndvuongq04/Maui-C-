using BTL_QLHD.Helpers;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.HousePages;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.HousePages;

public partial class AddHousePopup : Popup
{
    public AddHousePopup()
    {
        InitializeComponent();
        var vm = new AddHousePopupViewModel(ServiceHelper.GetService<HouseService>());
        vm.ClosePopupRequested += () => Close(true);
        BindingContext = vm;
    }

    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        Close(null);
    }
}