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

    [ObservableProperty]
    private string currentSortColumn = string.Empty;

    [ObservableProperty]
    private bool isSortAscending = true;

    public HousePageViewModel(HouseService houseService)
    {
        _houseService = houseService;
        LoadHousesCommand.Execute(null);
    }

    partial void OnSearchKeywordChanged(string value)
    {
        Search();
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

    // SORT LOGIC FOR ALL COLUMNS
    [RelayCommand]
    public void SortId()
    {
        ApplySort("Id", h => h.Id);
    }

    [RelayCommand]
    public void SortSoNha()
    {
        ApplySort("HouseNumber", h => h.HouseNumber);
    }

    [RelayCommand]
    public void SortTenChuNha()
    {
        ApplySort("OwnerName", h => h.OwnerName);
    }

    [RelayCommand]
    public void SortSDT()
    {
        ApplySort("OwnerPhone", h => h.OwnerPhone);
    }

    [RelayCommand]
    public void SortDiaChi()
    {
        ApplySort("Address", h => h.Address);
    }

    private void ApplySort<T>(string column, Func<House, T> keySelector)
    {
        if (CurrentSortColumn == column)
            IsSortAscending = !IsSortAscending;
        else
        {
            CurrentSortColumn = column;
            IsSortAscending = true;
        }

        // Sử dụng FilteredHouses để sort kết quả đang hiển thị
        var sorted = IsSortAscending
            ? FilteredHouses.OrderBy(keySelector).ToList()
            : FilteredHouses.OrderByDescending(keySelector).ToList();

        FilteredHouses.Clear();
        foreach (var house in sorted)
            FilteredHouses.Add(house);

        // Notify icon for all columns
        OnPropertyChanged(nameof(IsSortIdIconVisible));
        OnPropertyChanged(nameof(SortIdIcon));
        OnPropertyChanged(nameof(IsSortSoNhaIconVisible));
        OnPropertyChanged(nameof(SortSoNhaIcon));
        OnPropertyChanged(nameof(IsSortTenChuNhaIconVisible));
        OnPropertyChanged(nameof(SortTenChuNhaIcon));
        OnPropertyChanged(nameof(IsSortSDTIconVisible));
        OnPropertyChanged(nameof(SortSDTIcon));
        OnPropertyChanged(nameof(IsSortDiaChiIconVisible));
        OnPropertyChanged(nameof(SortDiaChiIcon));
    }

    // ICON PROPERTIES FOR ALL COLUMNS
    public bool IsSortIdIconVisible => CurrentSortColumn == "Id";
    public string SortIdIcon => IsSortAscending ? "▲" : "▼";

    public bool IsSortSoNhaIconVisible => CurrentSortColumn == "HouseNumber";
    public string SortSoNhaIcon => IsSortAscending ? "▲" : "▼";

    public bool IsSortTenChuNhaIconVisible => CurrentSortColumn == "OwnerName";
    public string SortTenChuNhaIcon => IsSortAscending ? "▲" : "▼";

    public bool IsSortSDTIconVisible => CurrentSortColumn == "OwnerPhone";
    public string SortSDTIcon => IsSortAscending ? "▲" : "▼";

    public bool IsSortDiaChiIconVisible => CurrentSortColumn == "Address";
    public string SortDiaChiIcon => IsSortAscending ? "▲" : "▼";
}
