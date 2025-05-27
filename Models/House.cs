using SQLite;


namespace BTL_QLHD.Models
{
    [Table("houses")]
    public class House
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("house_number")]
        public string HouseNumber { get; set; }

        [Column("owner_name")]
        public string OwnerName { get; set; }

        [Column("owner_phone")]
        public string OwnerPhone { get; set; }

        [Column("address")]
        public string Address { get; set; }



    }
}
