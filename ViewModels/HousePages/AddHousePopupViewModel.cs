using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
    
namespace BTL_QLHD.ViewModels.HousePages;

public partial class AddHousePopupViewModel : ObservableObject
{
    private readonly HouseService _houseService;

    [ObservableProperty]
    private string houseNumber;

    [ObservableProperty]
    private string ownerName;

    [ObservableProperty]
    private string ownerPhone;

    [ObservableProperty]
    private string address;

    public AddHousePopupViewModel(HouseService houseService)
    {
        _houseService = houseService;
    }

    public event Action? ClosePopupRequested;

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(HouseNumber) ||
            string.IsNullOrWhiteSpace(OwnerName) ||
            string.IsNullOrWhiteSpace(OwnerPhone) ||
            string.IsNullOrWhiteSpace(Address))
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ thông tin.", "OK");
            return;
        }

        await _houseService.AddHouseAsync(new House
        {
            HouseNumber = HouseNumber.Trim(),
            OwnerName = OwnerName.Trim(),
            OwnerPhone = OwnerPhone.Trim(),
            Address = Address.Trim()
        });

        ClosePopupRequested?.Invoke();
    }
}
