using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTL_QLHD.ViewModels.ServiceCategoryPages;

public partial class AddServiceCategoryPopupViewModel : ObservableObject
{
    private readonly ServiceCategoryService _serviceCategoryService;

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string unit = string.Empty;
    [ObservableProperty] private float price;

    public AddServiceCategoryPopupViewModel(ServiceCategoryService serviceCategoryService)
    {
        _serviceCategoryService = serviceCategoryService;
    }

    public event Action? ClosePopupRequested;

    [RelayCommand]
    public async Task Save()
    {
        // Kiểm tra dữ liệu đầu vào
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Unit))
        {
            await Shell.Current.DisplayAlert("Lỗi", "Vui lòng nhập đầy đủ thông tin.", "OK");
            return;
        }

        if (Price <= 0)
        {
            await Shell.Current.DisplayAlert("Lỗi", "Giá phải lớn hơn 0.", "OK");
            return;
        }

        try
        {
            var service = new ServiceCategory
            {
                Name = Name.Trim(),
                Price = Price,
                Unit = Unit.Trim()
            };

            var result = await _serviceCategoryService.AddServiceCategoryAsync(service);

            if (result > 0)
            {
                ClosePopupRequested?.Invoke();
            }
            else
            {
                await Shell.Current.DisplayAlert("Lỗi", "Không thể thêm dịch vụ. Vui lòng thử lại.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", $"Đã xảy ra lỗi: {ex.Message}", "OK");
        }
    }
}
