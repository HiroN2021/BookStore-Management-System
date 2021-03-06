using System;
using System.Collections.Generic;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class PublisherDAL
    {
        public List<Publisher> FindPublisherByPatternMatching(string pattern, out Exception ex)
        {
            List<Publisher> publishers = new List<Publisher>();
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES publishers p READ, books b READ;";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT p.*\r\nFROM publishers p\r\n         LEFT JOIN books b ON p.PublisherID = p.PublisherID\r\nWHERE p.PublisherName LIKE @pattern\r\nGROUP BY p.PublisherID\r\nORDER BY COUNT(b.BookID) DESC;";
                command.Parameters.AddWithValue("@pattern", "%" + pattern + "%");
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Publisher publisher = new Publisher();
                        publisher.PublisherID = Convert.ToUInt32(reader["PublisherID"]);
                        publisher.PublisherName = reader["PublisherName"].ToString();
                        publishers.Add(publisher);
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
            return publishers;
        }
    }
}
