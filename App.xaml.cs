namespace BTL_QLHD
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            // Đảm bảo ServiceProvider đã sẵn sàng trước khi khởi tạo database
            Microsoft.Maui.Controls.Application.Current?.Dispatcher.Dispatch(InitializeDatabase);
        }

        private async void InitializeDatabase()
        {
            var dataService = Helpers.ServiceHelper.GetService<Services.DataService>();
            if (dataService != null)
                await dataService.InitializeAsync();
        }
    }
}
