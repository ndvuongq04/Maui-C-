using BTL_QLHD.Services;
using BTL_QLHD.View;
using BTL_QLHD.View.HousePages;
using BTL_QLHD.View.InvoicePages;

//using BTL_QLHD.View.HousePages.HousePage;
using BTL_QLHD.View.ServicePages;

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SQLite;
using System.IO;

namespace BTL_QLHD
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Đăng ký SQLiteAsyncConnection singleton
            builder.Services.AddSingleton(sp =>
                new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DB_QLHD.db")));

            // Đăng ký DataService singleton (khởi tạo bảng 1 lần)
            builder.Services.AddSingleton<DataService>();

            // Đăng ký các service singleton, inject connection
            builder.Services.AddSingleton<HouseService>();
            builder.Services.AddSingleton<ServiceCategoryService>();
            builder.Services.AddSingleton<InvoiceService>();
            builder.Services.AddSingleton<ServiceUsageService>();




            // Đăng ký các trang 
            builder.Services.AddTransient<HousePage>();
            builder.Services.AddTransient<AddHousePopup>();
            builder.Services.AddTransient<UpdateHousePopup>();

            builder.Services.AddTransient<ServiceCategoryPage>();
            builder.Services.AddTransient<AddServiceCategoryPopup>();
            builder.Services.AddTransient<UpdateServiceCategoryPopup>();
            builder.Services.AddTransient<DeleteServiceCategoryPopup>();


            builder.Services.AddTransient<InvoicePage>();
            builder.Services.AddTransient<AddInvoicePopup>();
            builder.Services.AddTransient<ViewDetailInvoicePopup>();
            builder.Services.AddTransient<UpdateInvoicePopup>();










            // Đăng ký ViewModel
            builder.Services.AddTransient<BTL_QLHD.ViewModels.HousePages.HousePageViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.HousePages.AddHousePopupViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.HousePages.UpdateHousePopupViewModel>();

            builder.Services.AddTransient<BTL_QLHD.ViewModels.ServiceCategoryPages.ServiceCategoryPageViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.ServiceCategoryPages.AddServiceCategoryPopupViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.ServiceCategoryPages.UpdateServiceCategoryPopupViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.ServiceCategoryPages.DeleteServiceCategoryPopupViewModel>();

            builder.Services.AddTransient<BTL_QLHD.ViewModels.InvoicePages.InvoicePageViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.InvoicePages.AddInvoicePopupViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.InvoicePages.ViewDetailInvoicePopupViewModel>();
            builder.Services.AddTransient<BTL_QLHD.ViewModels.InvoicePages.UpdateInvoicePopupViewModel>();


            // Thêm CommunityToolkit modal
            builder.UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Khởi tạo ServiceHelper với ServiceProvider
            BTL_QLHD.Helpers.ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
