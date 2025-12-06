using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookstoreApp;

[Table("LagerSaldo")]
public class Stock
{
    [Column("ButikID")]                         // Store foreign key
    public int StoreId { get; set; }

    [Column("ISBN13")]                          // Book foreign key
    public string BookId { get; set; } = "";

    [Column("Antal")]                           // Number of copies in stock
    public int Quantity { get; set; }

    public Store? Store { get; set; }           // Navigation: Stock → Store
    public Book? Book { get; set; }             // Navigation: Stock → Book
}
