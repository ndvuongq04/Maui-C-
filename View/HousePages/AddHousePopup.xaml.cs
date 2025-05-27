using BTL_QLHD.Helpers;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;
namespace BTL_QLHD.View.HousePages;

public partial class AddHousePopup : Popup
{
    private readonly HouseService _houseService;

    public AddHousePopup()
    {
        InitializeComponent();
        // Lấy HouseService từ ServiceHelper
        _houseService = ServiceHelper.GetService<HouseService>();

    }

    private async void OnSaveClicked(object sender, System.EventArgs e)
    {
        // Kiểm tra nếu có ô nào bị bỏ trống
        if (string.IsNullOrWhiteSpace(soNhaEntry.Text) ||
            string.IsNullOrWhiteSpace(tenChuNhaEntry.Text) ||
            string.IsNullOrWhiteSpace(sdtEntry.Text) ||
            string.IsNullOrWhiteSpace(diaChiEntry.Text))
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ thông tin.", "OK");
            return;
        }

        // Nếu hợp lệ, tiến hành thêm nhà
        await _houseService.AddHouseAsync(new House
        {
            HouseNumber = soNhaEntry.Text.Trim(),
            OwnerName = tenChuNhaEntry.Text.Trim(),
            OwnerPhone = sdtEntry.Text.Trim(),
            Address = diaChiEntry.Text.Trim()
        });

        // Đóng popup sau khi thêm xong
        Close(true);
    }


    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        // Đóng popup
        Close(null);
    }
}