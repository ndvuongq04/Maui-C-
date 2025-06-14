using BTL_QLHD.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL_QLHD.Services
{
    public class ServiceCategoryService
    {
        private readonly SQLiteAsyncConnection _connection;

        public ServiceCategoryService(SQLiteAsyncConnection connection)
        {
            _connection = connection;
            // Không cần tạo bảng nữa, DataService đã tạo rồi
        }

        public async Task<List<ServiceCategory>> GetServiceCategoriesAsync()
        {
            return await _connection.Table<ServiceCategory>().ToListAsync();
        }

        public Task<ServiceCategory> GetServiceCategoryAsync(int id)
        {
            return _connection.Table<ServiceCategory>()
                              .Where(sc => sc.Id == id)
                              .FirstOrDefaultAsync();
        }

        public Task<int> AddServiceCategoryAsync(ServiceCategory serviceCategory)
        {
            return _connection.InsertAsync(serviceCategory);
        }

        public Task<int> UpdateServiceCategoryAsync(ServiceCategory serviceCategory)
        {
            return _connection.UpdateAsync(serviceCategory);
        }

        public Task<int> DeleteServiceCategoryAsync(ServiceCategory serviceCategory)
        {
            return _connection.DeleteAsync(serviceCategory);
        }

        public Task<int> DeleteServiceCategoryByIdAsync(int id)
        {
            return _connection.DeleteAsync<ServiceCategory>(id);
        }
    }
}
