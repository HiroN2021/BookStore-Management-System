using System;
using System.Collections.Generic;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class OrderDAL
    {
        public bool AddToDataBase(in Order order, out Exception ex)
        {
            ex = null;
            bool status = false;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlTransaction myTrans = null;
            MySqlCommand command = null;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                command = connection.CreateCommand();
                command.Transaction = myTrans;
                // Insert Order
                command.CommandText = "INSERT INTO BorrowCards" +
                "(LibraryCardID, BorrowFromDate, DueDate, LibrarianID, Status, NumberOfBookToReturn)" +
                " VALUES" +
                "(@libraryCardID, @borrowFromDate, @dueDate, @librarianID, @status, @numberOfBookToReturn)";
                command.Parameters.AddWithValue("@libraryCardID", order.LibraryCardInfo.LibaryCardID);
                command.Parameters.AddWithValue("@borrowFromDate", order.BorrowFromDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dueDate", order.BorrowToDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@librarianID", order.LibrarianInfo.ID);
                command.Parameters.AddWithValue("@status", order.Status);
                command.Parameters.AddWithValue("@numberOfBookToReturn", order.BorrowCardDetails.Count);
                command.ExecuteNonQuery();
                uint borrowCardID = (uint)command.LastInsertedId;
                // Insert BorrowCardDetails
                command.CommandText = "INSERT INTO BorrowCardDetails" +
                "(BorrowCardID, BookID)" +
                " VALUES" +
                "(@borrowCardID, @bookID)";
                // Console.WriteLine(order.BorrowCardDetails[0].BookInfo.BookID);
                command.Parameters.AddWithValue("@borrowCardID", borrowCardID);
                command.Parameters.AddWithValue("@bookID", order.BorrowCardDetails[0].BookInfo.BookID);
                command.ExecuteNonQuery();
                for (int i = 1; i < order.BorrowCardDetails.Count; i++)
                {
                    command.Parameters["@borrowCardID"].Value = borrowCardID;
                    command.Parameters["@bookID"].Value = order.BorrowCardDetails[i].BookInfo.BookID;
                    command.ExecuteNonQuery();
                }
                // Insert Books
                command.CommandText = "UPDATE Books SET CurrentCopies = CurrentCopies - 1 WHERE BookID = @bookID";
                command.Parameters["@bookID"].Value = order.BorrowCardDetails[0].BookInfo.BookID;
                command.ExecuteNonQuery();
                for (int i = 1; i < order.BorrowCardDetails.Count; i++)
                {
                    command.Parameters["@bookID"].Value = order.BorrowCardDetails[i].BookInfo.BookID;
                    command.ExecuteNonQuery();
                }
                // Change LibraryCard Status
                command.CommandText = "update LibraryCards set Status=@libraryCardstatus where LibraryCardID=@libraryCardID";
                command.Parameters.AddWithValue("@libraryCardstatus", (int)order.LibraryCardInfo.Status);
                command.ExecuteNonQuery();

                myTrans.Commit();
                status = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
        public bool ReturnBook(uint borrowCardID, uint bookID, out Exception ex)
        {
            ex = null;
            bool status = false;
            MySqlConnection connection = DbHelper.GetConnection();
            MySqlTransaction myTrans = null;
            MySqlCommand command = null;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                command = connection.CreateCommand();
                command.Transaction = myTrans;
                // Update BorrowCardDetails
                command.CommandText = "UPDATE borrowcarddetails SET ReturnDate = curdate() WHERE BorrowCardID = @borrowCardID";
                command.Parameters.AddWithValue("@borrowCardID", borrowCardID);
                command.ExecuteNonQuery();
                // Update Book
                command.CommandText = "UPDATE books SET CurrentCopies = CurrentCopies + 1 WHERE BookID = @bookID";
                command.Parameters.AddWithValue("@bookID", bookID);
                command.ExecuteNonQuery();

                myTrans.Commit();
                status = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
        // public List<Order> GetAllActiveBorrowCardsByLibraryCardID(in uint libraryCardID, out Exception ex)
        // {
        //     ex = null;
        //     var borrowCardList = new List<Order>();
        //     MySqlConnection connection = DbHelper.GetConnection();
        //     try
        //     {
        //         MySqlCommand command = connection.CreateCommand();
        //         command.CommandType = System.Data.CommandType.Text;
        //         // Get All BorrowCards
        //         command.CommandText = "SELECT BorrowCardID, LibraryCardID, BorrowFromDate, BorrowToDate, LibrarianID, NumberOfBookToReturn FROM BorrowCards WHERE LibraryCardID = @libraryCardID AND Status = @status";
        //         command.Parameters.AddWithValue("@libraryCardID", libraryCardID);
        //         command.Parameters.AddWithValue("@status", BorrowCardStatus.Borrowing);
        //         var reader = command.ExecuteReader();
        //         if (reader.Read())
        //         {
        //             var order = new Order();
        //             order.ID = Int32.Parse(reader["BorrowCardID"].ToString());
        //             order.BorrowFromDate = DateTime.Parse(reader["BorrowFromDate"].ToString());
        //             order.BorrowToDate = DateTime.Parse(reader["BorrowToDate"].ToString());
        //             order.LibrarianInfo = new Employee() { ID = Int32.Parse(reader["LibrarianID"].ToString()) };
        //             order.Status = BorrowCardStatus.Borrowing;
        //             borrowCardList.Add(order);
        //         }
        //         reader.Dispose();
        //         // Get all BorrowCards' Detail associated with each Order in the list
        //         command.Parameters.Add("@borrowCardID", MySqlDbType.UInt32);
        //         for (int i = 0; i < borrowCardList.Count; i++)
        //         {
        //             command.CommandText = "SELECT BorrowCardID, BookID, ReturnDate, Fine FROM BorrowCardDetails WHERE BorrowCardID = @borrowCardID";
        //             command.Parameters["@borrowCardID"].Value = borrowCardList[i].ID;
        //             reader = command.ExecuteReader();
        //             if (reader.Read())
        //             {
        //                 BorrowCardDetail borrowCardDetail = new BorrowCardDetail();
        //                 borrowCardDetail.
        //             }
        //             reader.Dispose();
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         ex = e;
        //     }
        //     finally
        //     {
        //         connection?.Dispose();
        //     }
        //     return borrowCardList;
        // }
    }
}
