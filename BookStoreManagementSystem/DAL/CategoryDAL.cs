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
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT c.*\r\nFROM categories c\r\n         LEFT JOIN categories_books cb ON c.CategoryID = cb.CategoryID\r\n         LEFT JOIN books b ON b.BookID = cb.BookID\r\nWHERE c.CategoryName LIKE @pattern\r\nGROUP BY c.CategoryName\r\nORDER BY COUNT(b.BookID) DESC;";
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
                connection?.Dispose();
            }
            return categories;
        }
    }
}
