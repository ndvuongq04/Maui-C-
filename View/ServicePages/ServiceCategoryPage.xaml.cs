using BTL_QLHD.Helpers;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using BTL_QLHD.ViewModels.ServiceCategoryPages;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace BTL_QLHD.View.ServicePages;

public partial class ServiceCategoryPage : ContentPage
{
    public ServiceCategoryPage()
    {
        InitializeComponent();
        BindingContext = new ServiceCategoryPageViewModel(ServiceHelper.GetService<ServiceCategoryService>());
    }
}
