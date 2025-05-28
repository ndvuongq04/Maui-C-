using BTL_QLHD.Helpers;
using BTL_QLHD.Models;
using BTL_QLHD.Services;
using BTL_QLHD.View.HousePages;
using BTL_QLHD.ViewModels.HousePages;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BTL_QLHD.View.HousePages
{
    public partial class HousePage : ContentPage
    {
        public HousePage()
        {
            InitializeComponent();
            BindingContext = new HousePageViewModel(Helpers.ServiceHelper.GetService<Services.HouseService>());
        }
    }
}
