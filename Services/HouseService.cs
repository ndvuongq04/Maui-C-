using BTL_QLHD.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL_QLHD.Services
{
    public class HouseService
    {
        private readonly SQLiteAsyncConnection _connection;

        public HouseService(SQLiteAsyncConnection connection)
        {
            _connection = connection;
            // Không cần tạo bảng nữa, DataService đã tạo rồi
        }

        public async Task<List<House>> GetHousesAsync()
        {
            return await _connection.Table<House>().ToListAsync();
        }

        public Task<House> GetHouseAsync(int id)
        {
            return _connection.Table<House>()
                              .Where(h => h.Id == id)
                              .FirstOrDefaultAsync();
        }

        public Task<int> AddHouseAsync(House house)
        {
            return _connection.InsertAsync(house);
        }

        public Task<int> UpdateHouseAsync(House house)
        {
            return _connection.UpdateAsync(house);
        }

        public Task<int> DeleteHouseAsync(House house)
        {
            return _connection.DeleteAsync(house);
        }
    }
}
