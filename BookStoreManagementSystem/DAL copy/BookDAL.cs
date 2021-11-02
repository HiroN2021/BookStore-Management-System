using System;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class BookDAL
    {
        public Book GetBookByID(uint bookID, out Exception ex)
        {
            Book book = null;
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT * FROM Books WHERE BookID = @bookID LIMIT 1";
                command.Parameters.AddWithValue("@bookID", bookID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        book = new Book();
                        book.BookID = bookID;
                        book.Title = reader["Title"].ToString();
                        book.Publisher = reader["Publisher"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("PublicationYear")))
                            book.PublicationYear = reader.GetInt32("PublicationYear");
                        book.Author = reader["Author"].ToString();
                        book.ISBN = reader["ISBN"].ToString();
                        book.Language = reader["Language"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("Cost")))
                            book.Cost = reader.GetInt32("Cost");
                        book.ActualCopies = Int32.Parse(reader["ActualCopies"].ToString());
                        book.CurrentCopies = Int32.Parse(reader["CurrentCopies"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return book;
        }
    }
}
