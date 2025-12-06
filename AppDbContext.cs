using Microsoft.EntityFrameworkCore;

namespace BookstoreApp;

public class AppDbContext : DbContext
{
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Store> Stores { get; set; } = null!;
    public DbSet<Stock> Stock { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB; Initial Catalog=BokhandelDB; Integrated Security=True; TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------------- Book ↔ Author ----------------
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)              // One book has one author
            .WithMany(a => a.Books)             // One author has many books
            .HasForeignKey(b => b.AuthorId);    // FK: AuthorId

        // ---------------- Stock (Composite key) ----------------
        modelBuilder.Entity<Stock>()
            .HasKey(s => new { s.StoreId, s.BookId });  // Composite PK

        // Stock → Store relationship
        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Store)
            .WithMany(st => st.StockItems)
            .HasForeignKey(s => s.StoreId);

        // Stock → Book relationship (key is ISBN13)
        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Book)
            .WithMany()
            .HasForeignKey(s => s.BookId)        // FK to ISBN13
            .HasPrincipalKey(b => b.ISBN13);     // Book PK = ISBN13
    }
}
