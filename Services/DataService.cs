using BTL_QLHD.Models;
using SQLite;
using System.Threading.Tasks;

namespace BTL_QLHD.Services
{
    public class DataService
    {
        private readonly SQLiteAsyncConnection _connection;

        public DataService(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task InitializeAsync()
        {
            await _connection.CreateTableAsync<House>();
            await _connection.CreateTableAsync<ServiceCategory>();
        }
    }
}
