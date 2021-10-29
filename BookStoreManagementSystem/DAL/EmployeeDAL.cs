using System;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class EmployeeDAL
    {
        public Employee GetEmployee(string userName, string password, out Exception ex)
        {
            Employee employee = null;
            ex = null;
            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT * FROM employees WHERE UserName = @userName AND Password = @password LIMIT 1";

                command.Parameters.AddWithValue("@userName", userName);
                command.Parameters.AddWithValue("@password", Md5Algorithms.CreateMD5(password));

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    employee = new Employee();
                    employee.EmployeeID = uint.Parse(reader["EmployeeID"].ToString());
                    employee.FirstName = reader["FirstName"].ToString();
                    employee.LastName = reader["LastName"].ToString();
                }
                reader.Dispose();
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                connection?.Dispose();
            }
            return employee;
        }
    }
}
