using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;



namespace BookstoreApp;

internal class Program
{
    private static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("  📚 WELCOME TO BOOKSTORE 📚  ");
            Console.WriteLine("===============================");
            Console.WriteLine("1. List all books");
            Console.WriteLine("2. Add a new book");
            Console.WriteLine("3. Update a book");
            Console.WriteLine("4. Delete a book");
            Console.WriteLine("===============================");
            Console.WriteLine("5. List all authors");
            Console.WriteLine("6. Add a new author");
            Console.WriteLine("7. Update an author");
            Console.WriteLine("8. Delete an author");
            Console.WriteLine("===============================");
            Console.WriteLine("0. Exit");
            Console.Write("Your choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ListBooks(); break;
                case "2": AddBook(); break;
                case "3": UpdateBook(); break;
                case "4": DeleteBook(); break;
                case "5": ListAuthors(); break;
                case "6": AddAuthor(); break;
                case "7": UpdateAuthor(); break;
                case "8": DeleteAuthor(); break;
                case "0": running = false; break;
                default:
                    Console.WriteLine("\nInvalid choice.");
                    Console.WriteLine("Press any key to return to the main menu...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    // ============================================================
    //                           BOOKS
    // ============================================================

    // ----- BOOKS: READ (List all books) -----
    private static void ListBooks()
    {
        using var db = new AppDbContext();

        var books = db.Books
            .Include(b => b.Author)
            .OrderBy(b => b.Title)
            .ToList();

        Console.WriteLine("\n--- All Books ---");
        if (!books.Any())
        {
            Console.WriteLine("No books yet.");
        }
        else
        {
            foreach (var b in books)
            {
                var authorName = b.Author != null
                    ? $"{b.Author.FirstName} {b.Author.LastName}"
                    : "Unknown author";

                Console.WriteLine(
                    $"{b.ISBN13}: {b.Title} by {authorName} – {b.Language} – {b.Price:0.00} kr – {b.ReleaseDate:yyyy-MM-dd}");
            }
        }

        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    // ----- BOOKS: CREATE (Add new book) -----
    private static void AddBook()
    {
        using var db = new AppDbContext();

        Console.WriteLine("\n--- Add a New Book ---");

        Console.Write("ISBN13 (13 chars): ");
        var isbn = (Console.ReadLine() ?? "").Trim();

        Console.Write("Title: ");
        var title = Console.ReadLine() ?? "";

        Console.Write("Language: ");
        var lang = Console.ReadLine() ?? "";

        decimal price;
        while (true)
        {
            Console.Write("Price (e.g. 199.50): ");
            var input = Console.ReadLine();
            if (decimal.TryParse(input, out price) && price >= 0) break;
            Console.WriteLine("Invalid price. Try again.");
        }

        DateTime releaseDate;
        while (true)
        {
            Console.Write("Release date (YYYY-MM-DD): ");
            var input = Console.ReadLine();
            if (DateTime.TryParse(input, out releaseDate)) break;
            Console.WriteLine("Invalid date. Try again.");
        }

        var authors = db.Authors
      .AsEnumerable()
      .GroupBy(a => new { a.FirstName, a.LastName })    // group duplicates
      .Select(g => g.First())                           // keep only one
      .OrderBy(a => a.LastName)
      .ThenBy(a => a.FirstName)
      .ToList();


        Author chosenAuthor;

        if (!authors.Any())
        {
            Console.WriteLine("\nNo authors yet. You must create one.");
            chosenAuthor = CreateAuthorInline(db);
        }
        else
        {
            Console.WriteLine("\nExisting authors:");
            foreach (var a in authors)
            {
                Console.WriteLine($"{a.Id}: {a.FirstName} {a.LastName}");
            }

            Console.Write("Enter author ID (or 0 to create a new author): ");
            int authorId;
            while (!int.TryParse(Console.ReadLine(), out authorId))
            {
                Console.Write("Invalid number. Try again: ");
            }

            if (authorId == 0)
            {
                chosenAuthor = CreateAuthorInline(db);
            }
            else
            {
                chosenAuthor = db.Authors.FirstOrDefault(a => a.Id == authorId)
                               ?? CreateAuthorInline(db);
            }
        }

        var book = new Book
        {
            ISBN13 = isbn,
            Title = title,
            Language = lang,
            Price = price,
            ReleaseDate = releaseDate,
            AuthorId = chosenAuthor.Id
        };

        db.Books.Add(book);
        db.SaveChanges();

        Console.WriteLine($"\nBook '{book.Title}' by '{chosenAuthor.FirstName} {chosenAuthor.LastName}' was added.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    // ----- BOOKS: UPDATE (Edit existing book) -----
    private static void UpdateBook()
    {
        using var db = new AppDbContext();

        Console.Write("\nEnter the ISBN13 of the book you want to update: ");
        var isbn = (Console.ReadLine() ?? "").Trim();

        var book = db.Books
            .Include(b => b.Author)
            .FirstOrDefault(b => b.ISBN13 == isbn);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nCurrent title: {book.Title}");
        Console.Write("New title (leave empty to keep): ");
        var newTitle = Console.ReadLine();

        Console.WriteLine($"Current language: {book.Language}");
        Console.Write("New language (leave empty to keep): ");
        var langInput = Console.ReadLine();

        Console.WriteLine($"Current price: {book.Price:0.00} kr");
        Console.Write("New price (leave empty to keep): ");
        var priceInput = Console.ReadLine();

        Console.WriteLine($"Current release date: {book.ReleaseDate:yyyy-MM-dd}");
        Console.Write("New release date (leave empty to keep): ");
        var dateInput = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(newTitle))
            book.Title = newTitle;

        if (!string.IsNullOrWhiteSpace(langInput))
            book.Language = langInput;

        if (!string.IsNullOrWhiteSpace(priceInput)
            && decimal.TryParse(priceInput, out decimal price) && price >= 0)
            book.Price = price;

        if (!string.IsNullOrWhiteSpace(dateInput)
            && DateTime.TryParse(dateInput, out DateTime date))
            book.ReleaseDate = date;

        db.SaveChanges();

        Console.WriteLine("\n✓ Book updated successfully.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    // ----- BOOKS: DELETE (Remove book if not in stock) -----
    private static void DeleteBook()
    {
        using var db = new AppDbContext();

        Console.WriteLine("\n--- Delete Book ---");
        Console.Write("Enter the ISBN13 of the book you want to delete: ");
        var isbn = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(isbn))
        {
            Console.WriteLine("No ISBN entered.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        var book = db.Books.FirstOrDefault(b => b.ISBN13 == isbn);
        if (book == null)
        {
            Console.WriteLine("Book not found.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        var stockRows = db.Stock
            .Where(s => s.BookId == isbn)
            .ToList();

        if (stockRows.Any())
        {
            Console.WriteLine("\nThis book is still in stock in these stores:");
            foreach (var s in stockRows)
            {
                var store = db.Stores.FirstOrDefault(st => st.Id == s.StoreId);
                var storeName = store != null
                    ? $"{store.Name} ({store.City})"
                    : $"StoreId {s.StoreId}";
                Console.WriteLine($"- {storeName}: {s.Quantity} pcs");
            }

            Console.WriteLine(
                "\nYou must remove this book from all stores (LagerSaldo) before you can delete the title.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nAre you sure you want to delete '{book.Title}'? (y/n)");
        var ans = Console.ReadLine();

        if (ans?.ToLower() != "y")
        {
            Console.WriteLine("Cancelled.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        db.Books.Remove(book);
        db.SaveChanges();

        Console.WriteLine("\nBook deleted successfully.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    // ============================================================
    //                          AUTHORS
    // ============================================================

    // ----- AUTHORS: READ (List all authors) -----
    private static void ListAuthors()
    {
        using var db = new AppDbContext();

        // remove duplicates with same FirstName + LastName (UI only)
        var authors = db.Authors
            .AsEnumerable()                                 // go to memory
            .GroupBy(a => new { a.FirstName, a.LastName })  // group duplicates
            .Select(g => g.First())                         // keep one from each group
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .ToList();

        Console.WriteLine("\n--- All Authors ---");
        if (!authors.Any())
        {
            Console.WriteLine("No authors yet.");
        }
        else
        {
            foreach (var a in authors)
            {
                var bookCount = db.Books.Count(b => b.AuthorId == a.Id);
                Console.WriteLine($"{a.Id}: {a.FirstName} {a.LastName}   Books: {bookCount}");
            }
        }

        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void AddAuthor()
    {
        using var db = new AppDbContext();

        Console.WriteLine("\n--- Add a New Author ---");
        Console.Write("First name: ");
        var first = Console.ReadLine() ?? "";

        Console.Write("Last name: ");
        var last = Console.ReadLine() ?? "";

        var existing = db.Authors
            .FirstOrDefault(a => a.FirstName == first && a.LastName == last);

        if (existing != null)
        {
            Console.WriteLine(
                $"\nAuthor '{existing.FirstName} {existing.LastName}' already exists (ID: {existing.Id}).");
            Console.WriteLine("No new author was created. Press any key...");
            Console.ReadKey();
            return;
        }

        var author = new Author
        {
            FirstName = first,
            LastName = last
        };

        db.Authors.Add(author);
        db.SaveChanges();

        Console.WriteLine($"Author '{author.FirstName} {author.LastName}' added with ID {author.Id}.");
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }

    private static void UpdateAuthor()
    {
        using var db = new AppDbContext();

        Console.Write("\nEnter the ID of the author you want to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        var author = db.Authors.FirstOrDefault(a => a.Id == id);
        if (author == null)
        {
            Console.WriteLine("Author not found.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Current first name: {author.FirstName}");
        Console.Write("New first name (leave empty to keep): ");
        var newFirst = Console.ReadLine();

        Console.WriteLine($"Current last name: {author.LastName}");
        Console.Write("New last name (leave empty to keep): ");
        var newLast = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(newFirst))
            author.FirstName = newFirst;

        if (!string.IsNullOrWhiteSpace(newLast))
            author.LastName = newLast;

        db.SaveChanges();

        Console.WriteLine("\n✓ Author updated successfully.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void DeleteAuthor()
    {
        using var db = new AppDbContext();

        Console.Write("\nEnter the ID of the author you want to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        var author = db.Authors.FirstOrDefault(a => a.Id == id);
        if (author == null)
        {
            Console.WriteLine("Author not found.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        bool hasBooks = db.Books.Any(b => b.AuthorId == author.Id);
        if (hasBooks)
        {
            Console.WriteLine("This author still has books. Remove or reassign the books first.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nAre you sure you want to delete '{author.FirstName} {author.LastName}'? (y/n)");
        var ans = Console.ReadLine();
        if (ans?.ToLower() != "y")
        {
            Console.WriteLine("Cancelled.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        db.Authors.Remove(author);
        db.SaveChanges();

        Console.WriteLine("\nAuthor deleted successfully.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    private static Author CreateAuthorInline(AppDbContext db)
    {
        Console.Write("\nNew author first name: ");
        var first = Console.ReadLine() ?? "";

        Console.Write("New author last name: ");
        var last = Console.ReadLine() ?? "";

        var existing = db.Authors
            .FirstOrDefault(a => a.FirstName == first && a.LastName == last);

        if (existing != null)
        {
            Console.WriteLine(
                $"\nAuthor '{existing.FirstName} {existing.LastName}' already exists (ID: {existing.Id}).");
            Console.WriteLine("Using existing author. Press any key to continue...");
            Console.ReadKey();
            return existing;
        }

        var author = new Author
        {
            FirstName = first,
            LastName = last
        };

        db.Authors.Add(author);
        db.SaveChanges();

        Console.WriteLine($"\nAuthor '{author.FirstName} {author.LastName}' created with ID {author.Id}.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        return author;
    }
}
