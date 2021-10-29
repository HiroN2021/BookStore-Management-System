using System;
using System.Collections.Generic;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class AuthorDAL
    {
        public List<Author> FindAuthorByPatternMatching(string pattern, out Exception ex)
        {
            List<Author> authors = new List<Author>();
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT a.*\r\nFROM authors a\r\n         LEFT JOIN books b ON a.AuthorID = b.AuthorID\r\nWHERE a.AuthorName LIKE @pattern\r\nGROUP BY a.AuthorID\r\nORDER BY COUNT(b.BookID) DESC;";
                command.Parameters.AddWithValue("@pattern", "%" + pattern + "%");
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Author author = new Author();
                        author.AuthorID = Convert.ToUInt32(reader["AuthorID"]);
                        author.AuthorName = reader["AuthorName"].ToString();
                        authors.Add(author);
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
                connection?.Dispose();
            }
            return authors;
        }
    }
}
