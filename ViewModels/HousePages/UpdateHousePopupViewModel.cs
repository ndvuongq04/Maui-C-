using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTL_QLHD.ViewModels.HousePages;

public partial class UpdateHousePopupViewModel : ObservableObject
{
    private readonly HouseService _houseService;
    private readonly int _houseId;
    private House? _originalHouse;

    [ObservableProperty] private string houseNumber = string.Empty;
    [ObservableProperty] private string ownerName = string.Empty;
    [ObservableProperty] private string ownerPhone = string.Empty;
    [ObservableProperty] private string address = string.Empty;

    public UpdateHousePopupViewModel(HouseService houseService, int houseId)
    {
        _houseService = houseService;
        _houseId = houseId;
    }

    public async Task LoadAsync()
    {
        _originalHouse = await _houseService.GetHouseAsync(_houseId);
        if (_originalHouse != null)
        {
            HouseNumber = _originalHouse.HouseNumber;
            OwnerName = _originalHouse.OwnerName;
            OwnerPhone = _originalHouse.OwnerPhone;
            Address = _originalHouse.Address;
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        try
        {
            if (_originalHouse == null) return;
            _originalHouse.HouseNumber = HouseNumber;
            _originalHouse.OwnerName = OwnerName;
            _originalHouse.OwnerPhone = OwnerPhone;
            _originalHouse.Address = Address;
            var result = await _houseService.UpdateHouseAsync(_originalHouse);
            if (result == 0)
                await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật dữ liệu!", "OK");
            else
                ClosePopupRequested?.Invoke();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", ex.Message, "OK");
        }
    }

    public event Action? ClosePopupRequested;
}
