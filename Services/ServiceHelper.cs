using System;
using Microsoft.Extensions.DependencyInjection;

namespace BTL_QLHD.Helpers
{
    public static class ServiceHelper
    {
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider chưa được khởi tạo.");

            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
