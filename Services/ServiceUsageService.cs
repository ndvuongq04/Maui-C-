using BTL_QLHD.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL_QLHD.Services
{
    public class ServiceUsageService
    {
        private readonly SQLiteAsyncConnection _connection;

        public ServiceUsageService(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }


        // Lấy danh sách usage theo invoiceId
        public async Task<List<ServiceUsage>> GetServiceUsagesByInvoiceAsync(int invoiceId)
        {
            return await _connection.Table<ServiceUsage>()
                .Where(su => su.InvoiceId == invoiceId)
                .ToListAsync();
        }

        // Lấy usage theo Id
        public Task<ServiceUsage> GetServiceUsageAsync(int id)
        {
            return _connection.Table<ServiceUsage>()
                .Where(su => su.Id == id)
                .FirstOrDefaultAsync();
        }

        // Thêm mới usage
        public async Task<int> AddServiceUsageAsync(ServiceUsage usage)
        {
            return await _connection.InsertAsync(usage);
        }

        // Cập nhật usage
        public async Task<int> UpdateServiceUsageAsync(ServiceUsage usage)
        {
            return await _connection.UpdateAsync(usage);
        }

        // Xoá usage theo Id
        public async Task<int> DeleteServiceUsageAsync(int id)
        {
            var usage = await GetServiceUsageAsync(id);
            if (usage != null)
            {
                return await _connection.DeleteAsync(usage);
            }
            return 0;
        }



        // Xoá tất cả usage theo invoiceId
        public async Task<int> DeleteServiceUsagesByInvoiceAsync(int invoiceId)
        {
            var usages = await _connection.Table<ServiceUsage>()
                .Where(su => su.InvoiceId == invoiceId)
                .ToListAsync();

            foreach (var usage in usages)
            {
                await _connection.DeleteAsync(usage);
            }

            return usages.Count;
        }

        // Kiểm tra usage đã tồn tại chưa theo invoiceId và serviceId
        public async Task<bool> ExistsUsageAsync(int invoiceId, int serviceId)
        {
            var count = await _connection.Table<ServiceUsage>()
                .Where(su => su.InvoiceId == invoiceId && su.ServiceId == serviceId)
                .CountAsync();
            return count > 0;
        }
    }
}
