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
                // Insert Customer
                command.CommandText = @"INSERT INTO customers (FirstName, LastName, ContactTitle, Gender, BirthDate, Address, City, Phone, Fax, Email, Note)
VALUES (@firstName, @lastName, @contactTitle, @gender, @birthDate, @address, @city, @phone, @fax, @email, @note);";
                command.Parameters.AddWithValue("@firstName", customer.FirstName); // ?? "NULL"
                command.Parameters.AddWithValue("@lastName", customer.LastName); // ?? "NULL"
                command.Parameters.AddWithValue("@contactTitle", customer.ContactTitle); // ?? "NULL"
                command.Parameters.AddWithValue("@gender", (int)customer.Gender);
                command.Parameters.AddWithValue("@birthDate", customer.BirthDate != null ? customer.BirthDate.Value.ToString("yyyy-MM-dd") : null);
                command.Parameters.AddWithValue("@address", customer.Address); // ?? "NULL"
                command.Parameters.AddWithValue("@city", customer.City); // ?? "NULL"
                command.Parameters.AddWithValue("@phone", customer.Phone); // ?? "NULL"
                command.Parameters.AddWithValue("@fax", customer.Fax); // ?? "NULL"
                command.Parameters.AddWithValue("@email", customer.Email); // ?? "NULL"
                command.Parameters.AddWithValue("@note", customer.Note); // ?? "NULL"
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                // Get Customer ID
                customer.CustomerID = (uint)command.LastInsertedId;
                // Insert Invoice
                command.CommandText = @"INSERT INTO invoices (CustomerID, EmployeeID, CreatedTime, Description)
VALUES (@customerID, @employeeID, @createdTime, @invoiceDescription);";
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
SET Quantity = Quantity - @itemQuantity
WHERE BookID = @bookID;";
                command.Parameters.Add("@itemQuantity", MySqlDbType.UInt32);
                command.Parameters.Add("@bookID", MySqlDbType.UInt32);
                for (int i = 0; i < invoice.InvoiceDetails.Count; i++)
                {
                    var orderDetail = invoice.InvoiceDetails[i];
                    command.Parameters["@bookID"].Value = orderDetail.Book_Info.BookID;
                    command.Parameters["@itemQuantity"].Value = orderDetail.ItemQuantity;
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
                connection?.Dispose();
            }
            return status;
        }

    }
}
