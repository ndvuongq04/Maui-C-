using SQLite;
using System;

namespace BTL_QLHD.Models
{
    [Table("invoice")]
    public class Invoice
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("house_id")]
        public int HouseId { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("year")]
        public int Year { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("status")]
        public string Status { get; set; } = string.Empty; // Default value

        [Column("note")]
        public string Note { get; set; } = string.Empty; // Default value

        // Thêm property này để binding UI, không mapping với SQLite
        [Ignore]
        public House House { get; set; } = null!; // Suppress warning, set as nullable reference
    }
}
