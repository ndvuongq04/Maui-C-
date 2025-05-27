using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace BTL_QLHD.View.ServicePages;

public partial class ServicePage : ContentPage
{
    private readonly ServiceCategoryService _serviceCategoryServiceService;
    // Danh sách gốc (dùng để reset hoặc tìm kiếm lại)
    private ObservableCollection<ServiceCategory> serviceCategories;

    // Danh sách hiển thị trên CollectionView
        private ObservableCollection<ServiceCategory> filteredServiceCategory;

    public ServicePage()
    {
        InitializeComponent();
        _serviceCategoryServiceService = Helpers.ServiceHelper.GetService<ServiceCategoryService>();
        // khởi tạo
        serviceCategories = new ObservableCollection<ServiceCategory>();
        filteredServiceCategory = new ObservableCollection<ServiceCategory>();
        // Gọi hàm load dữ liệu từ DB 
        LoadServiceFromDb();
    }
    private async void LoadServiceFromDb()
    {
        var list = await _serviceCategoryServiceService.GetServiceCategoriesAsync();

        serviceCategories.Clear();
        foreach (var service in list)
            serviceCategories.Add(service);

        filteredServiceCategory.Clear();
        foreach (var service in serviceCategories)
            filteredServiceCategory.Add(service);
    }



    private async void OnAddServiceClicked(object sender, EventArgs e)
    {
        //var popup = new AddServiceCategoryPopup();
        //await this.ShowPopupAsync(popup);

        //// Gọi hàm load dữ liệu từ DB 
        //LoadServiceFromDb();
    }

    
    private async void OnEditClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Thêm", "Chức năng sửa dịch vụ", "OK");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Thêm", "Chức xóa thêm dịch vụ", "OK");

    }
}
