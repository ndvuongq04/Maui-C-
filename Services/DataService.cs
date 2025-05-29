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
            await _connection.ExecuteAsync("PRAGMA foreign_keys = ON;");

            // Tạo bảng house và service_category trước (bảng cha)
            await _connection.CreateTableAsync<House>(); // Sẽ tạo bảng "houses"
            await _connection.CreateTableAsync<ServiceCategory>();

            // Tạo bảng invoice với FK đến house
            await _connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS invoice (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    house_id INTEGER,
                    month INTEGER,
                    year INTEGER,
                    total_amount DECIMAL,
                    created_date TEXT,
                    status TEXT,
                    note TEXT,
                    FOREIGN KEY(house_id) REFERENCES houses(id) ON DELETE CASCADE
                );
            ");

            // Tạo bảng service_usage với FK đến invoice, house, service_category
            await _connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS service_usage (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    invoice_id INTEGER,
                    service_id INTEGER,
                    year INTEGER,
                    month INTEGER,
                    usage_value INTEGER,
                    FOREIGN KEY(invoice_id) REFERENCES invoice(id) ON DELETE CASCADE,
                    FOREIGN KEY(service_id) REFERENCES service_category(id) ON DELETE CASCADE
                );
            ");
        }
    }
}
