using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreApp;

[Table("Butiker")]
public class Store
{
    [Key]
    [Column("ButikId")]
    public int Id { get; set; }

    [Column("Namn")]
    public string Name { get; set; } = "";

    [Column("Stad")]
    public string City { get; set; } = "";

    public ICollection<Stock> StockItems { get; set; } = new List<Stock>();
}
