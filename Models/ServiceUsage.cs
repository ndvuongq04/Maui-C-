using SQLite;

namespace BTL_QLHD.Models
{
    [Table("service_usage")]
    public class ServiceUsage
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("invoice_id")]
        public int InvoiceId { get; set; } // FK đến Invoice

        [Column("service_id")]
        public int ServiceId { get; set; } // FK đến ServiceCategory

        [Column("year")]
        public int Year { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("usage_value")]
        public int UsageValue { get; set; }
    }
}
