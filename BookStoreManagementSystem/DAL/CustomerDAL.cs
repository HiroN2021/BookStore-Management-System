using System;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class CustomerDAL
    {
        public Customer GetCustomerByPhone(string phone, out Exception ex)
        {
            Customer customer = null;
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlCommand command = null;
            try
            {
                connection.Open();
                command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                // LOCK TABLES
                command.CommandText = @"LOCK TABLES customers READ";
                command.ExecuteNonQuery();
                
                command.CommandText = "SELECT * FROM customers WHERE Phone = @phone LIMIT 1";

                command.Parameters.AddWithValue("@phone", phone);

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    customer = new Customer();
                    customer.CustomerID = uint.Parse(reader["CustomerID"].ToString());
                    customer.FirstName = reader["FirstName"].ToString();
                    customer.LastName = reader["LastName"].ToString();
                    customer.Phone = reader["Phone"].ToString();
                }
                reader.Dispose();
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
            return customer;
        }
    }
}
