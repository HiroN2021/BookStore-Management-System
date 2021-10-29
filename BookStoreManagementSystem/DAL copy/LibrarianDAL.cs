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
                command.CommandText = "select * from Librarians where UserName=@userName and UserPassword=@pass limit 1";

                command.Parameters.AddWithValue("@userName", userName);
                command.Parameters.AddWithValue("@pass", Md5Algorithms.CreateMD5(password));

                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    employee = new Employee();
                    employee.Name = reader["LibrarianName"].ToString();
                    employee.ID = Int32.Parse(reader["LibrarianID"].ToString());
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
