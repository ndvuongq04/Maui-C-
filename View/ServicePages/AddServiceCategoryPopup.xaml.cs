using BTL_QLHD.Models;
using BTL_QLHD.Services;
using CommunityToolkit.Maui.Views;

namespace BTL_QLHD.View.ServicePages
{
    public partial class AddServiceCategoryPopup : Popup
    {
        private readonly ServiceCategoryService _serviceCategoryService;

        public AddServiceCategoryPopup()
        {
            InitializeComponent();

            // L?y ServiceCategoryService t? ServiceHelper
            _serviceCategoryService = Helpers.ServiceHelper.GetService<ServiceCategoryService>();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameEntry.Text) ||
                string.IsNullOrWhiteSpace(priceEntry.Text) ||
                string.IsNullOrWhiteSpace(unitEntry.Text))
            {
                await Shell.Current.DisplayAlert("L?i", "Vui l�ng nh?p ??y ?? th�ng tin.", "OK");
                return;
            }

            if (!float.TryParse(priceEntry.Text.Trim(), out float price))
            {
                await Shell.Current.DisplayAlert("L?i", "Gi� kh�ng h?p l?.", "OK");
                return;
            }

            var service = new ServiceCategory
            {
                Name = nameEntry.Text.Trim(),
                Price = price,
                Unit = unitEntry.Text.Trim()
            };

            try
            {
                await _serviceCategoryService.AddServiceCategoryAsync(service);
                await Shell.Current.DisplayAlert("Th�nh c�ng", "?� th�m d?ch v? m?i.", "OK");
                Close(true);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("L?i", $"Kh�ng th? th�m d?ch v?: {ex.Message}", "OK");
            }
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(null);
        }
    }
}
