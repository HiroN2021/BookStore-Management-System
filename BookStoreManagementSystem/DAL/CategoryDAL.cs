using System;
using System.Collections.Generic;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class CategoryDAL
    {
        public List<Category> FindCategories(string pattern, out Exception ex)
        {
            List<Category> categories = new List<Category>();
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES categories c READ, categories_books cb READ, books b READ;";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT c.*\r\nFROM categories c\r\n         LEFT JOIN categories_books cb ON c.CategoryID = cb.CategoryID\r\n         LEFT JOIN books b ON b.BookID = cb.BookID\r\nWHERE c.CategoryName LIKE @pattern\r\nGROUP BY c.CategoryID\r\nORDER BY COUNT(b.BookID) DESC;";
                command.Parameters.AddWithValue("@pattern", "%" + pattern + "%");
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Category category = new Category();
                        category.CategoryID = Convert.ToUInt32(reader["CategoryID"]);
                        category.CategoryName = reader["CategoryName"].ToString();
                        categories.Add(category);
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
            return categories;
        }
    }
}
