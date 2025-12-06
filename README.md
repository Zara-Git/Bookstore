📚 BookstoreApp

A simple C# console application using Entity Framework Core (Database First).
The project connects to the BokhandelDB database and allows the user to manage data for:

Authors

Books

Stores

Stock (inventory per store)

✔️ What the application does 

Loads the SQL Server database using AppDbContext

Lists books, authors, stores, and stock

Adds new books, authors, stores, and stock items

Updates existing records

Deletes records

Uses EF Core models:

Author

Book

Store

Stock

Uses navigation properties to handle relationships
(one author → many books, one store → many stock entries)

✔️ Technologies

C# .NET

Entity Framework Core

SQL Server Database First

Console Application