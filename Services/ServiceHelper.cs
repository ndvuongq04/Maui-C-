using System;
using Microsoft.Extensions.DependencyInjection;  // Cung cấp các extension methods để lấy service

namespace BTL_QLHD.Helpers
{
    // Khai báo class tĩnh
    public static class ServiceHelper
    {
        // Biến static lưu giữ IServiceProvider để sử dụng toàn cục
        private static IServiceProvider _serviceProvider;

        // Hàm khởi tạo service provider từ bên ngoài
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;  // Gán giá trị truyền vào cho biến _serviceProvider
        }

        // Hàm lấy service theo kiểu (Generic), dùng ở bất cứ đâu trong app
        public static T GetService<T>() where T : class
        {
            // Nếu chưa khởi tạo, thì ném ra lỗi
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider chưa được khởi tạo.");

            // Trả về service tương ứng đã đăng ký trong DI Container
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
