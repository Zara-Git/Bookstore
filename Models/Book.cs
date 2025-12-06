using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreApp;

[Table("Böcker")]
public class Book
{
    [Key]                                    
    [Column("ISBN13")]
    public string ISBN13 { get; set; } = "";

    [Column("Titel")]                           
    public string Title { get; set; } = "";

    [Column("Språk")]                          
    public string Language { get; set; } = "";

    [Column("Pris")]                            
    public decimal Price { get; set; }

    [Column("Utgivningsdatum")]              
    public DateTime ReleaseDate { get; set; }

    [Column("FörfattareID")]                    
    public int AuthorId { get; set; }

    public Author? Author { get; set; }         // Navigation property to Author
}
