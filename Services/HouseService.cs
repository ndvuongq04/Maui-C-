using BTL_QLHD.Models;           // Sử dụng các model đã định nghĩa, ví dụ class House
using SQLite;                    // Thư viện SQLite dùng cho kết nối cơ sở dữ liệu
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL_QLHD.Services
{
    // Lớp dịch vụ dùng để thao tác với bảng House trong cơ sở dữ liệu SQLite
    public class HouseService
    {
        private readonly SQLiteAsyncConnection _connection; // Kết nối đến database (dạng bất đồng bộ)

        // Constructor nhận vào kết nối SQLite
        public HouseService(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        // Lấy toàn bộ danh sách nhà (House) từ bảng House trong database
        public async Task<List<House>> GetHousesAsync()
        {
            return await _connection.Table<House>().ToListAsync(); // Truy vấn toàn bộ và chuyển sang danh sách
        }

        // Lấy một nhà cụ thể theo ID
        public Task<House> GetHouseAsync(int id)
        {
            return _connection.Table<House>()
                              .Where(h => h.Id == id)             // Lọc theo ID
                              .FirstOrDefaultAsync();             // Trả về phần tử đầu tiên nếu có
        }

        // Thêm một nhà mới vào bảng House
        public Task<int> AddHouseAsync(House house)
        {
            return _connection.InsertAsync(house);                // Thêm vào database
        }

        // Cập nhật thông tin một nhà đã có
        public Task<int> UpdateHouseAsync(House house)
        {
            return _connection.UpdateAsync(house);               // Cập nhật bản ghi
        }

        // Xóa một nhà khỏi bảng House
        public Task<int> DeleteHouseAsync(House house)
        {
            return _connection.DeleteAsync(house);               // Xóa bản ghi
        }
    }
}
