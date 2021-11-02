using System;
using Persistence;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DAL
{
    public class BookDAL
    {
        public Book GetBookByID(uint bookID, out Exception ex)
        {
            Book book = null;
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES books b READ, publishers p READ, authors a READ, authors_books ab READ, languages l READ, languages_books lb READ, categories c READ, categories_books cb READ;";
                command.ExecuteNonQuery();
                
                command.CommandText = @"SELECT b.BookID,
       Title,
       Price,
       Format,
       ISBN13,
       ISBN10,
       PublicationDate,
       NumberOfPages,
       Dimensions,
       Description,
       Quantity,
       Status,
       p.PublisherID,
       PublisherName
FROM books b
         LEFT JOIN publishers p ON p.PublisherID = b.PublisherID
         WHERE b.BookID = @bookID
         LIMIT 1";
                command.Parameters.AddWithValue("@bookID", bookID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        book = new Book();
                        GetBookInfoFromMySqlDataReader(reader, book);
                    }
                }
                if (book == null)
                    return null;

                command.Parameters.Clear();
                command.CommandText = "SELECT  c.CategoryID, c.CategoryName\r\nFROM books b\r\n         JOIN categories_books cb ON b.BookID = cb.BookID\r\n         LEFT JOIN categories c ON c.CategoryID = cb.CategoryID\r\nWHERE b.BookID = @bookID";
                command.Parameters.AddWithValue("@bookID", book.BookID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book.AddCategories(reader.GetUInt32("CategoryID"), reader["CategoryName"].ToString());
                    }
                }
                command.CommandText = "SELECT  a.AuthorID, a.AuthorName\r\nFROM books b\r\n         JOIN authors_books ab ON b.BookID = ab.BookID\r\n         LEFT JOIN authors a ON a.AuthorID = ab.AuthorID\r\nWHERE b.BookID = @bookID";
                command.Parameters["@bookID"].Value = book.BookID;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book.AddAuthors(reader.GetUInt32("AuthorID"), reader["AuthorName"].ToString());
                    }
                }
                command.CommandText = "SELECT  l.LanguageID, l.LanguageName\r\nFROM books b\r\n         JOIN languages_books lb ON b.BookID = lb.BookID\r\n         LEFT JOIN languages l ON l.LanguageID = lb.LanguageID\r\nWHERE b.BookID = @bookID";
                command.Parameters["@bookID"].Value = book.BookID;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book.AddLanguages(reader.GetUInt32("LanguageID"), reader["LanguageName"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine(e);
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return book;
        }
        public Book GetBookByISBN(string isbn, out Exception ex)
        {
            Book book = null;
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES books b READ, publishers p READ, authors a READ, authors_books ab READ, languages l READ, languages_books lb READ, categories c READ, categories_books cb READ;";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT b.BookID,\r\n       Title,\r\n       Price,\r\n       Format,\r\n       ISBN13,\r\n       ISBN10,\r\n       PublicationDate,\r\n       NumberOfPages,\r\n       Dimensions,\r\n       Description,\r\n       Quantity,\r\n       Status,\r\n       p.PublisherID,\r\n       PublisherName\r\nFROM books b\r\n         LEFT JOIN publishers p ON p.PublisherID = b.PublisherID\r\n         WHERE b.ISBN10 = @isbn OR b.ISBN13 = @isbn\r\n         LIMIT 1";
                command.Parameters.AddWithValue("@isbn", isbn);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        book = new Book();
                        GetBookInfoFromMySqlDataReader(reader, book);
                    }
                }
                if (book == null)
                    return null;

                command.CommandText = "SELECT  c.CategoryID, c.CategoryName\r\nFROM books b\r\n         JOIN categories_books cb ON b.BookID = cb.BookID\r\n         LEFT JOIN categories c ON c.CategoryID = cb.CategoryID\r\nWHERE b.BookID = @bookID";
                command.Parameters.AddWithValue("@bookID", book.BookID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book.AddCategories(reader.GetUInt32("CategoryID"), reader["CategoryName"].ToString());
                    }
                }
                command.CommandText = "SELECT  a.AuthorID, a.AuthorName\r\nFROM books b\r\n         JOIN authors_books ab ON b.BookID = ab.BookID\r\n         LEFT JOIN authors a ON a.AuthorID = ab.AuthorID\r\nWHERE b.BookID = @bookID";
                command.Parameters["@bookID"].Value = book.BookID;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book.AddAuthors(reader.GetUInt32("AuthorID"), reader["AuthorName"].ToString());
                    }
                }
                command.CommandText = "SELECT  l.LanguageID, l.LanguageName\r\nFROM books b\r\n         JOIN languages_books lb ON b.BookID = lb.BookID\r\n         LEFT JOIN languages l ON l.LanguageID = lb.LanguageID\r\nWHERE b.BookID = @bookID";
                command.Parameters["@bookID"].Value = book.BookID;
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book.AddLanguages(reader.GetUInt32("LanguageID"), reader["LanguageName"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine(e);
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return book;
        }
        public List<Book> FindBooksByTitle(string pattern, out Exception ex)
        {
            List<Book> books = new List<Book>();
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES books b READ, publishers p READ, authors a READ, authors_books ab READ, languages l READ, languages_books lb READ, categories c READ, categories_books cb READ;";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT b.BookID,\r\n       Title,\r\n       Price,\r\n       Format,\r\n       ISBN13,\r\n       ISBN10,\r\n       PublicationDate,\r\n       NumberOfPages,\r\n       Dimensions,\r\n       Description,\r\n       Quantity,\r\n       Status,\r\n       p.PublisherID,\r\n       PublisherName\r\nFROM books b\r\n         LEFT JOIN publishers p ON p.PublisherID = b.PublisherID\r\n"
                                    + "WHERE b.Title LIKE @pattern;";
                command.Parameters.AddWithValue("@pattern", "%" + pattern + "%");
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Book book = new Book();
                        GetBookInfoFromMySqlDataReader(reader, book);
                        book.Status = Enum.Parse<BookStatus>(reader["Status"].ToString());
                        books.Add(book);
                    }
                }
                command.Parameters.Add("@bookID", MySqlDbType.UInt32);
                for (int i = 0; i < books.Count; i++)
                {
                    command.CommandText = "SELECT  c.CategoryID, c.CategoryName\r\nFROM books b\r\n         JOIN categories_books cb ON b.BookID = cb.BookID\r\n         LEFT JOIN categories c ON c.CategoryID = cb.CategoryID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = books[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books[i].AddCategories(reader.GetUInt32("CategoryID"), reader["CategoryName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  a.AuthorID, a.AuthorName\r\nFROM books b\r\n         JOIN authors_books ab ON b.BookID = ab.BookID\r\n         LEFT JOIN authors a ON a.AuthorID = ab.AuthorID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = books[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books[i].AddAuthors(reader.GetUInt32("AuthorID"), reader["AuthorName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  l.LanguageID, l.LanguageName\r\nFROM books b\r\n         JOIN languages_books lb ON b.BookID = lb.BookID\r\n         LEFT JOIN languages l ON l.LanguageID = lb.LanguageID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = books[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books[i].AddLanguages(reader.GetUInt32("LanguageID"), reader["LanguageName"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine(e);
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return books;
        }
        public List<Book> FindBooksByCategory(uint categoriesID, out Exception ex)
        {
            ex = null;
            List<Book> filteredBooks = new List<Book>();
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES books b READ, publishers p READ, authors a READ, authors_books ab READ, languages l READ, languages_books lb READ, categories c READ, categories_books cb READ;";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT b.BookID,\r\n       Title,\r\n       Price,\r\n       Format,\r\n       ISBN13,\r\n       ISBN10,\r\n       PublicationDate,\r\n       NumberOfPages,\r\n       Dimensions,\r\n       Description,\r\n       Quantity,\r\n       Status,\r\n       p.PublisherID,\r\n       PublisherName\r\nFROM books b\r\n         LEFT JOIN publishers p ON p.PublisherID = b.PublisherID\r\n         JOIN categories_books cb ON b.BookID = cb.BookID\r\n         JOIN categories c ON c.CategoryID = cb.CategoryID\r\n"
                    + "WHERE c.CategoryID = @categoryID";
                command.Parameters.AddWithValue("@categoryID", categoriesID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new Book();
                        GetBookInfoFromMySqlDataReader(reader, book);
                        filteredBooks.Add(book);
                    }
                }
                if (filteredBooks.Count == 0)
                    return null;
                command.Parameters.Add("@bookID", MySqlDbType.UInt32);
                for (int i = 0; i < filteredBooks.Count; i++)
                {
                    command.CommandText = "SELECT  c.CategoryID, c.CategoryName\r\nFROM books b\r\n         JOIN categories_books cb ON b.BookID = cb.BookID\r\n         LEFT JOIN categories c ON c.CategoryID = cb.CategoryID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddCategories(reader.GetUInt32("CategoryID"), reader["CategoryName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  a.AuthorID, a.AuthorName\r\nFROM books b\r\n         JOIN authors_books ab ON b.BookID = ab.BookID\r\n         LEFT JOIN authors a ON a.AuthorID = ab.AuthorID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddAuthors(reader.GetUInt32("AuthorID"), reader["AuthorName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  l.LanguageID, l.LanguageName\r\nFROM books b\r\n         JOIN languages_books lb ON b.BookID = lb.BookID\r\n         LEFT JOIN languages l ON l.LanguageID = lb.LanguageID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddLanguages(reader.GetUInt32("LanguageID"), reader["LanguageName"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine(e);
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return filteredBooks;
        }
        public List<Book> FindBooksByAuthor(uint authorID, out Exception ex)
        {
            ex = null;
            List<Book> filteredBooks = new List<Book>();
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES books b READ, publishers p READ, authors a READ, authors_books ab READ, languages l READ, languages_books lb READ, categories c READ, categories_books cb READ;";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT b.BookID,\r\n       Title,\r\n       Price,\r\n       Format,\r\n       ISBN13,\r\n       ISBN10,\r\n       PublicationDate,\r\n       NumberOfPages,\r\n       Dimensions,\r\n       Description,\r\n       Quantity,\r\n       Status,\r\n       p.PublisherID,\r\n       PublisherName\r\nFROM books b\r\n         LEFT JOIN publishers p ON p.PublisherID = b.PublisherID\r\n         JOIN authors_books ab ON b.BookID = ab.BookID\r\n         JOIN authors a ON a.AuthorID = ab.AuthorID\r\n"
                    + "WHERE a.AuthorID = @authorID";
                command.Parameters.AddWithValue("@authorID", authorID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new Book();
                        GetBookInfoFromMySqlDataReader(reader, book);
                        filteredBooks.Add(book);
                    }
                }
                if (filteredBooks.Count == 0)
                    return null;
                command.Parameters.Add("@bookID", MySqlDbType.UInt32);
                for (int i = 0; i < filteredBooks.Count; i++)
                {
                    command.CommandText = "SELECT  c.CategoryID, c.CategoryName\r\nFROM books b\r\n         JOIN categories_books cb ON b.BookID = cb.BookID\r\n         LEFT JOIN categories c ON c.CategoryID = cb.CategoryID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddCategories(reader.GetUInt32("CategoryID"), reader["CategoryName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  a.AuthorID, a.AuthorName\r\nFROM books b\r\n         JOIN authors_books ab ON b.BookID = ab.BookID\r\n         LEFT JOIN authors a ON a.AuthorID = ab.AuthorID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddAuthors(reader.GetUInt32("AuthorID"), reader["AuthorName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  l.LanguageID, l.LanguageName\r\nFROM books b\r\n         JOIN languages_books lb ON b.BookID = lb.BookID\r\n         LEFT JOIN languages l ON l.LanguageID = lb.LanguageID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddLanguages(reader.GetUInt32("LanguageID"), reader["LanguageName"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine(e);
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return filteredBooks;
        }
        public List<Book> FindBooksByPublisher(uint publisherID, out Exception ex)
        {
            ex = null;
            List<Book> filteredBooks = new List<Book>();
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES books b READ, publishers p READ, authors a READ, authors_books ab READ, languages l READ, languages_books lb READ, categories c READ, categories_books cb READ;";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT b.BookID,\r\n       Title,\r\n       Price,\r\n       Format,\r\n       ISBN13,\r\n       ISBN10,\r\n       PublicationDate,\r\n       NumberOfPages,\r\n       Dimensions,\r\n       Description,\r\n       Quantity,\r\n       Status,\r\n       p.PublisherID,\r\n       PublisherName\r\nFROM books b\r\n         JOIN publishers p ON p.PublisherID = b.PublisherID\r\n"
                    + "WHERE p.PublisherID = @publisherID";
                command.Parameters.AddWithValue("@publisherID", publisherID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new Book();
                        GetBookInfoFromMySqlDataReader(reader, book);
                        filteredBooks.Add(book);
                    }
                }
                if (filteredBooks.Count == 0)
                    return null;
                command.Parameters.Add("@bookID", MySqlDbType.UInt32);
                for (int i = 0; i < filteredBooks.Count; i++)
                {
                    command.CommandText = "SELECT  c.CategoryID, c.CategoryName\r\nFROM books b\r\n         JOIN categories_books cb ON b.BookID = cb.BookID\r\n         LEFT JOIN categories c ON c.CategoryID = cb.CategoryID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddCategories(reader.GetUInt32("CategoryID"), reader["CategoryName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  a.AuthorID, a.AuthorName\r\nFROM books b\r\n         JOIN authors_books ab ON b.BookID = ab.BookID\r\n         LEFT JOIN authors a ON a.AuthorID = ab.AuthorID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddAuthors(reader.GetUInt32("AuthorID"), reader["AuthorName"].ToString());
                        }
                    }
                    command.CommandText = "SELECT  l.LanguageID, l.LanguageName\r\nFROM books b\r\n         JOIN languages_books lb ON b.BookID = lb.BookID\r\n         LEFT JOIN languages l ON l.LanguageID = lb.LanguageID\r\nWHERE b.BookID = @bookID";
                    command.Parameters["@bookID"].Value = filteredBooks[i].BookID;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            filteredBooks[i].AddLanguages(reader.GetUInt32("LanguageID"), reader["LanguageName"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine(e);
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return filteredBooks;
        }
        void GetBookInfoFromMySqlDataReader(MySqlDataReader reader, Book book)
        {
            book.BookID = uint.Parse(reader["BookID"].ToString());
            if (!reader.IsDBNull(reader.GetOrdinal("Title")))
                book.Title = reader["Title"].ToString();
            if (!reader.IsDBNull(reader.GetOrdinal("Price")))
                book.Price = int.Parse(reader["Price"].ToString());
            book.Format = Enum.Parse<BookFormat>(reader["Format"].ToString());
            if (!reader.IsDBNull(reader.GetOrdinal("ISBN13")))
                book.ISBN13 = reader["ISBN13"].ToString();
            if (!reader.IsDBNull(reader.GetOrdinal("ISBN10")))
                book.ISBN10 = reader["ISBN10"].ToString();
            if (!reader.IsDBNull(reader.GetOrdinal("PublisherID")) && !reader.IsDBNull(reader.GetOrdinal("PublisherName")))
                book.Publisher = new Publisher(reader.GetUInt32("PublisherID"), reader["PublisherName"].ToString());
            if (!reader.IsDBNull(reader.GetOrdinal("PublicationDate")))
                book.PublicationDate = reader.GetDateTime("PublicationDate");
            if (!reader.IsDBNull(reader.GetOrdinal("NumberOfPages")))
                book.NumberOfPages = reader.GetUInt32("NumberOfPages");
            if (!reader.IsDBNull(reader.GetOrdinal("Dimensions")))
                book.Dimensions = reader["Dimensions"].ToString();
            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                book.Description = reader["Description"].ToString();
            if (!reader.IsDBNull(reader.GetOrdinal("Quantity")))
                book.Quantity = reader.GetInt32("Quantity");
            book.Status = Enum.Parse<BookStatus>(reader["Status"].ToString());
        }
    }
}
