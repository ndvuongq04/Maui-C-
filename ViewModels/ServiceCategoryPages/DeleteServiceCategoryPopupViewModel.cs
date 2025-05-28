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

namespace BTL_QLHD.ViewModels.ServiceCategoryPages;

public partial class DeleteServiceCategoryPopupViewModel : ObservableObject
{
    private readonly ServiceCategoryService _serviceCategoryService;
    private readonly int _serviceCategoryId;

    public DeleteServiceCategoryPopupViewModel(ServiceCategoryService serviceCategoryService, int serviceCategoryId)
    {
        _serviceCategoryService = serviceCategoryService;
        _serviceCategoryId = serviceCategoryId;
    }

    public event Action? ClosePopupRequested;

    [RelayCommand]
    public async Task Delete()
    {
        await _serviceCategoryService.DeleteServiceCategoryByIdAsync(_serviceCategoryId);
        ClosePopupRequested?.Invoke();
    }
}
