using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Views;
using BTL_QLHD.View.HousePages;

namespace BTL_QLHD.ViewModels.HousePages;

public partial class HousePageViewModel : ObservableObject
{
    private readonly HouseService _houseService;

    [ObservableProperty]
    private ObservableCollection<House> houses = new();

    [ObservableProperty]
    private ObservableCollection<House> filteredHouses = new();

    [ObservableProperty]
    private string searchKeyword;

    public HousePageViewModel(HouseService houseService)
    {
        _houseService = houseService;
        LoadHousesCommand.Execute(null);
    }

    [RelayCommand]
    public async Task LoadHousesAsync()
    {
        var list = await _houseService.GetHousesAsync();
        Houses.Clear();
        foreach (var house in list)
            Houses.Add(house);

        FilteredHouses.Clear();
        foreach (var house in Houses)
            FilteredHouses.Add(house);
    }

    [RelayCommand]
    public void Search()
    {
        var keyword = SearchKeyword?.ToLower() ?? "";
        var filtered = Houses.Where(h =>
            (h.HouseNumber?.ToLower().Contains(keyword) ?? false) ||
            (h.OwnerName?.ToLower().Contains(keyword) ?? false) ||
            (h.OwnerPhone?.ToLower().Contains(keyword) ?? false) ||
            (h.Address?.ToLower().Contains(keyword) ?? false)
        ).ToList();

        FilteredHouses.Clear();
        foreach (var house in filtered)
            FilteredHouses.Add(house);
    }

    [RelayCommand]
    public async Task AddAsync()
    {
        var popup = new AddHousePopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        await LoadHousesAsync();
    }

    [RelayCommand]
    public async Task EditAsync(House house)
    {
        if (house == null) return;
        var popup = new UpdateHousePopup(house.Id);
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        await LoadHousesAsync();
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadHousesAsync();
        SearchKeyword = string.Empty;
    }

    [RelayCommand]
    public void SortId()
    {
        var sorted = Houses.OrderBy(h => h.Id).ToList();
        FilteredHouses.Clear();
        foreach (var house in sorted)
            FilteredHouses.Add(house);
    }

    [RelayCommand]
    public void SortSoNha()
    {
        var sorted = Houses.OrderBy(h => h.HouseNumber).ToList();
        FilteredHouses.Clear();
        foreach (var house in sorted)
            FilteredHouses.Add(house);
    }

    [RelayCommand]
    public void SortTenChuNha()
    {
        var sorted = Houses.OrderBy(h => h.OwnerName).ToList();
        FilteredHouses.Clear();
        foreach (var house in sorted)
            FilteredHouses.Add(house);
    }

    [RelayCommand]
    public void SortSDT()
    {
        var sorted = Houses.OrderBy(h => h.OwnerPhone).ToList();
        FilteredHouses.Clear();
        foreach (var house in sorted)
            FilteredHouses.Add(house);
    }

    [RelayCommand]
    public void SortDiaChi()
    {
        var sorted = Houses.OrderBy(h => h.Address).ToList();
        FilteredHouses.Clear();
        foreach (var house in sorted)
            FilteredHouses.Add(house);
    }
}
