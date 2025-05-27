using BTL_QLHD.Helpers;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.HousePages;

public partial class UpdateHousePopup : Popup
{
    private readonly HouseService _houseService;
    private int _houseId; // Lưu id của house cần sửa
    private House _currentHouse; // Lưu thông tin house hiện tại

    // Nhận id qua constructor
    public UpdateHousePopup(int houseId)
    {
        InitializeComponent();
        _houseService = ServiceHelper.GetService<HouseService>();
        _houseId = houseId;
        LoadHouseData(); // Tải dữ liệu house lên form
    }

    // Hàm tải dữ liệu house lên form
    private async void LoadHouseData()
    {
        _currentHouse = await _houseService.GetHouseAsync(_houseId);
        if (_currentHouse != null)
        {
            idEntry.Text = _currentHouse.Id.ToString();
            soNhaEntry.Text = _currentHouse.HouseNumber;
            tenChuNhaEntry.Text = _currentHouse.OwnerName;
            sdtEntry.Text = _currentHouse.OwnerPhone;
            diaChiEntry.Text = _currentHouse.Address;
        }
    }

    private async void OnSaveClicked(object sender, System.EventArgs e)
    {
        // Cập nhật thông tin house
        _currentHouse.HouseNumber = soNhaEntry.Text;
        _currentHouse.OwnerName = tenChuNhaEntry.Text;
        _currentHouse.OwnerPhone = sdtEntry.Text;
        _currentHouse.Address = diaChiEntry.Text;

        await _houseService.UpdateHouseAsync(_currentHouse);

        // Đóng popup sau khi lưu
        Close(true);
    }

    private void OnCancelClicked(object sender, System.EventArgs e)
    {
        // Đóng popup
        Close(null);
    }
}