using SQLite;

namespace BTL_QLHD.Models
{
    [Table("service_category")]
    public class ServiceCategory
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("price")]
        public float Price { get; set; }

        [Column("unit")]
        public string Unit { get; set; }

    }
}
