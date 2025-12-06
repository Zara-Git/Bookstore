using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreApp;

[Table("Författare")]
public class Author
{
    [Key]                               // Only primary key
    [Column("ID")]
    public int Id { get; set; }

    [Column("Förnamn")]                 // Map to Swedish column
    public string FirstName { get; set; } = "";

    [Column("Efternamn")]               // Map to Swedish column
    public string LastName { get; set; } = "";

    [NotMapped]                          // Not stored in DB
    public string Name => $"{FirstName} {LastName}";

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
