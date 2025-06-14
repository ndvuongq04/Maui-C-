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

public partial class UpdateServiceCategoryPopupViewModel : ObservableObject
{
    private readonly ServiceCategoryService _serviceCategoryService;
    private readonly int _serviceCategoryId;
    private ServiceCategory? _originalServiceCategory;

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string unit = string.Empty;
    [ObservableProperty] private float price;

    public UpdateServiceCategoryPopupViewModel(ServiceCategoryService serviceCategoryService, int serviceCategoryId)
    {
        _serviceCategoryService = serviceCategoryService;
        _serviceCategoryId = serviceCategoryId;
    }

    public event Action? ClosePopupRequested;

    public async Task LoadAsync()
    {
        _originalServiceCategory = await _serviceCategoryService.GetServiceCategoryAsync(_serviceCategoryId);
        if (_originalServiceCategory != null)
        {
            Name = _originalServiceCategory.Name;
            Price = _originalServiceCategory.Price;
            Unit = _originalServiceCategory.Unit;
        }
    }

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
            if (_originalServiceCategory == null)
            {
                await Shell.Current.DisplayAlert("Lỗi", "Không tìm thấy dịch vụ để cập nhật.", "OK");
                return;
            }

            _originalServiceCategory.Name = Name.Trim();
            _originalServiceCategory.Price = Price;
            _originalServiceCategory.Unit = Unit.Trim();

            var result = await _serviceCategoryService.UpdateServiceCategoryAsync(_originalServiceCategory);

            if (result > 0)
            {
                ClosePopupRequested?.Invoke();
            }
            else
            {
                await Shell.Current.DisplayAlert("Lỗi", "Không thể cập nhật dịch vụ. Vui lòng thử lại.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Lỗi", $"Đã xảy ra lỗi: {ex.Message}", "OK");
        }
    }
}
