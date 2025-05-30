using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTL_QLHD.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL_QLHD.Services
{
    public class InvoiceService
    {
        private readonly SQLiteAsyncConnection _connection;

        // Add a public property to expose the connection
        public SQLiteAsyncConnection Connection => _connection;

        public InvoiceService(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        // Lấy tất cả hóa đơn
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            return await _connection.Table<Invoice>().ToListAsync();
        }

        // Lấy hóa đơn theo Id
        public Task<Invoice> GetInvoiceAsync(int id)
        {
            return _connection.Table<Invoice>()
                .Where(inv => inv.Id == id)
                .FirstOrDefaultAsync();
        }

        // Lấy hóa đơn theo houseId, year, month
        public async Task<Invoice> GetInvoiceByHouseAndTimeAsync(int houseId, int year, int month)
        {
            return await _connection.Table<Invoice>()
                .Where(inv => inv.HouseId == houseId && inv.Year == year && inv.Month == month)
                .FirstOrDefaultAsync();
        }

        // Thêm mới hóa đơn
        public async Task<int> AddInvoiceAsync(Invoice invoice)
        {
            return await _connection.InsertAsync(invoice);
        }

        // Cập nhật hóa đơn
        public async Task<int> UpdateInvoiceAsync(Invoice invoice)
        {
            return await _connection.UpdateAsync(invoice);
        }

        // Xoá hóa đơn theo Id
        public async Task<int> DeleteInvoiceAsync(int id)
        {
            var invoice = await GetInvoiceAsync(id);
            if (invoice != null)
            {
                return await _connection.DeleteAsync(invoice);
            }
            return 0;
        }

        // Kiểm tra hóa đơn đã tồn tại chưa
        public async Task<bool> ExistsInvoiceAsync(int houseId, int year, int month)
        {
            var count = await _connection.Table<Invoice>()
                .Where(inv => inv.HouseId == houseId && inv.Year == year && inv.Month == month)
                .CountAsync();
            return count > 0;
        }
    }
}
