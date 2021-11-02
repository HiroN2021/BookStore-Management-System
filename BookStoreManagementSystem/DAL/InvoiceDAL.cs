using System;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class InvoiceDAL
    {
        public bool AddToDataBase(in Invoice invoice, out Exception ex)
        {
            ex = null;
            bool status = false;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlTransaction myTrans = null;
            MySqlCommand command = null;
            Customer customer = invoice.Customer_Info;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                command = connection.CreateCommand();
                command.Transaction = myTrans;
                // LOCK Table
                command.CommandText = @"LOCK TABLES customers WRITE , invoices WRITE , invoices_details WRITE, books WRITE;";
                command.ExecuteNonQuery();
                // Insert Customer
                if (customer.CustomerID != 0)
                {
                    command.CommandText = "UPDATE customers\r\nSET FirstName = @firstName,\r\n    LastName  = @lastName\r\nWHERE CustomerID = @customerID;";
                    command.Parameters.AddWithValue("@customerID", customer.CustomerID);
                    command.Parameters.AddWithValue("@firstName", customer.FirstName);
                    command.Parameters.AddWithValue("@lastName", customer.LastName);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
                else
                {
                    command.CommandText = "INSERT INTO customers(FirstName, LastName, Phone)\r\nVALUES (@firstName, @lastName, @phone);";
                    // command.CommandText = @"INSERT INTO customers (FirstName, LastName, ContactTitle, Gender, BirthDate, Address, City, Phone, Fax, Email, Note)
                    // VALUES (@firstName, @lastName, @contactTitle, @gender, @birthDate, @address, @city, @phone, @fax, @email, @note);";
                    command.Parameters.AddWithValue("@firstName", customer.FirstName);
                    command.Parameters.AddWithValue("@lastName", customer.LastName);
                    // command.Parameters.AddWithValue("@contactTitle", customer.ContactTitle);
                    // command.Parameters.AddWithValue("@gender", (int)customer.Gender);
                    // command.Parameters.AddWithValue("@birthDate", customer.BirthDate != null ? customer.BirthDate.Value.ToString("yyyy-MM-dd") : null);
                    // command.Parameters.AddWithValue("@address", customer.Address);
                    // command.Parameters.AddWithValue("@city", customer.City);
                    command.Parameters.AddWithValue("@phone", customer.Phone);
                    // command.Parameters.AddWithValue("@fax", customer.Fax);
                    // command.Parameters.AddWithValue("@email", customer.Email);
                    // command.Parameters.AddWithValue("@note", customer.Note);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    // Get Customer ID
                    customer.CustomerID = (uint)command.LastInsertedId;
                }
                // Insert Invoice
                command.CommandText = @"INSERT INTO invoices (CustomerID, EmployeeID, CreatedTime, Description)
                VALUES (@customerID, @employeeID, @createdTime, @invoiceDescription);";
                // command.CommandText = @"INSERT INTO invoices (EmployeeID, CreatedTime, Description)
                // VALUES (@employeeID, @createdTime, @invoiceDescription);";
                command.Parameters.AddWithValue("@customerID", customer.CustomerID);
                command.Parameters.AddWithValue("@employeeID", invoice.Employee_Info.EmployeeID);
                command.Parameters.AddWithValue("@createdTime", invoice.CreatedTime);
                command.Parameters.AddWithValue("@invoiceDescription", invoice.Descripton);
                command.ExecuteNonQuery();
                // Get Invoice ID
                invoice.InvoiceID = (uint)command.LastInsertedId;
                command.Parameters.Clear();
                // Insert Invoice Details
                command.CommandText = @"INSERT INTO invoices_details(InvoiceID, BookID, ItemQuantity, Amount)
VALUES (@invoiceID, @bookID, @itemQuantity, @amount);";
                command.Parameters.Add("@invoiceID", MySqlDbType.UInt32);
                command.Parameters.Add("@bookID", MySqlDbType.UInt32);
                command.Parameters.Add("@itemQuantity", MySqlDbType.UInt32);
                command.Parameters.Add("@amount", MySqlDbType.UInt32);
                for (int i = 0; i < invoice.InvoiceDetails.Count; i++)
                {
                    var orderDetail = invoice.InvoiceDetails[i];
                    command.Parameters["@invoiceID"].Value = invoice.InvoiceID;
                    command.Parameters["@bookID"].Value = orderDetail.Book_Info.BookID;
                    command.Parameters["@itemQuantity"].Value = orderDetail.ItemQuantity;
                    command.Parameters["@amount"].Value = orderDetail.Amount;
                    command.ExecuteNonQuery();
                }
                command.Parameters.Clear();
                // Update Books
                command.CommandText = @"UPDATE books
SET Quantity = Quantity - @itemQuantity, Status = @bookStatus
WHERE BookID = @bookID;";
                command.Parameters.Add("@itemQuantity", MySqlDbType.UInt32);
                command.Parameters.Add("@bookID", MySqlDbType.UInt32);
                command.Parameters.AddWithValue("@bookStatus", 1);
                for (int i = 0; i < invoice.InvoiceDetails.Count; i++)
                {
                    var orderDetail = invoice.InvoiceDetails[i];
                    command.Parameters["@bookID"].Value = orderDetail.Book_Info.BookID;
                    command.Parameters["@itemQuantity"].Value = orderDetail.ItemQuantity;
                    command.Parameters["@bookStatus"].Value = ((int)orderDetail.Book_Info.Status);
                    command.ExecuteNonQuery();
                }
                myTrans.Commit();
                status = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                ex = e;
                try
                {
                    command.Transaction.Rollback();
                }
                catch (MySqlException em)
                {
                    Console.WriteLine(em.ToString());
                    if (command.Transaction.Connection != null)
                    {
                        Console.WriteLine("\nAn exception of type " + em.GetType() +
                        " was encountered while attempting to roll back the transaction.");
                    }
                }
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return status;
        }

    }
}
