using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Views;
using BTL_QLHD.View.ServicePages;

namespace BTL_QLHD.ViewModels.ServiceCategoryPages;

public partial class ServiceCategoryPageViewModel : ObservableObject
{
    private readonly ServiceCategoryService _serviceCategoryService;

    [ObservableProperty] private ObservableCollection<ServiceCategory> allServiceCategories = new();
    [ObservableProperty] private ObservableCollection<ServiceCategory> filteredServiceCategories = new();
    [ObservableProperty] private string searchKeyword = string.Empty;

    public ServiceCategoryPageViewModel(ServiceCategoryService serviceCategoryService)
    {
        _serviceCategoryService = serviceCategoryService;
        LoadServiceCategoriesCommand.Execute(null);
    }

    partial void OnSearchKeywordChanged(string value)
    {
        Search();
    }

    [RelayCommand]
    public async Task LoadServiceCategoriesAsync()
    {
        var list = await _serviceCategoryService.GetServiceCategoriesAsync();
        AllServiceCategories.Clear();
        foreach (var item in list)
            AllServiceCategories.Add(item);

        FilteredServiceCategories.Clear();
        foreach (var item in AllServiceCategories)
            FilteredServiceCategories.Add(item);
    }

    [RelayCommand]
    public void Search()
    {
        var keyword = SearchKeyword?.ToLower() ?? "";
        var filtered = AllServiceCategories.Where(s =>
            (s.Name?.ToLower().Contains(keyword) ?? false) ||
            (s.Unit?.ToLower().Contains(keyword) ?? false) ||
            s.Price.ToString().Contains(keyword)
        ).ToList();

        FilteredServiceCategories.Clear();
        foreach (var item in filtered)
            FilteredServiceCategories.Add(item);
    }

    [RelayCommand]
    public async Task AddAsync()
    {
        var popup = new AddServiceCategoryPopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        await LoadServiceCategoriesAsync();
    }

    [RelayCommand]
    public async Task EditAsync(ServiceCategory serviceCategory)
    {
        if (serviceCategory == null) return;
        var popup = new UpdateServiceCategoryPopup(serviceCategory.Id);
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        await LoadServiceCategoriesAsync();
    }

    [RelayCommand]
    public async Task DeleteAsync(ServiceCategory serviceCategory)
    {
        if (serviceCategory == null) return;
        var popup = new DeleteServiceCategoryPopup(serviceCategory.Id);
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        await LoadServiceCategoriesAsync();
    }
}


