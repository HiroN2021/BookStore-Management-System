using System;
using System.Collections.Generic;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class BorrowCardDetailDAL
    {
        public List<BorrowCardDetail> GetActiveBorrowCardDetailsByLibraryCardID(uint libraryCardID, out List<DateTime> borrowFromDateList, out List<DateTime> dueDateList, out Exception ex)
        {
            List<BorrowCardDetail> borrowCardDetailList = new List<BorrowCardDetail>();
            borrowFromDateList = new List<DateTime>();
            dueDateList = new List<DateTime>();
            ex = null;

            BorrowCardDetail borrowCardDetail = null;
            Book book = null;

            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT b3.BookID, b2.BorrowCardID, b.BorrowFromDate, b.DueDate, b3.Title, b3.Publisher, b3.PublicationYear, b3.Author, b3.ISBN, b3.Language, b3.Cost, b3.ActualCopies, b3.CurrentCopies FROM librarycards l JOIN borrowcards b ON l.LibraryCardID = b.LibraryCardID JOIN borrowcarddetails b2 ON b.BorrowCardID = b2.BorrowCardID JOIN books b3 ON b3.BookID = b2.BookID WHERE l.LibraryCardID = @libraryCardID AND b2.ReturnDate IS NULL ORDER BY b.BorrowFromDate";
                command.Parameters.AddWithValue("@libraryCardID", libraryCardID);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        book = new Book();
                        book.BookID = UInt32.Parse(reader["BookID"].ToString());
                        book.Title = reader["Title"].ToString();
                        book.Publisher = reader["Publisher"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("PublicationYear")))
                            book.PublicationYear = reader.GetInt32("PublicationYear");
                        book.Author = reader["Author"].ToString();
                        book.ISBN = reader["ISBN"].ToString();
                        book.Language = reader["Language"].ToString();
                        if (!reader.IsDBNull(reader.GetOrdinal("Cost")))
                            book.Cost = reader.GetInt32("Cost");
                        book.ActualCopies = Int32.Parse(reader["ActualCopies"].ToString());
                        book.CurrentCopies = Int32.Parse(reader["CurrentCopies"].ToString());


                        borrowCardDetail = new BorrowCardDetail();
                        borrowCardDetail.BorrowCardID = UInt32.Parse(reader["BorrowCardID"].ToString());

                        borrowFromDateList.Add(DateTime.Parse(reader["BorrowFromDate"].ToString()));
                        dueDateList.Add(DateTime.Parse(reader["DueDate"].ToString()));

                        borrowCardDetail.BookInfo = book;
                        borrowCardDetailList.Add(borrowCardDetail);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ex = e;
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return borrowCardDetailList;
        }
        public int GetNumberOfBooksOverDue(uint libraryCardID, out Exception ex)
        {
            ex = null;
            int overdueCount = 0;

            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT COUNT(*) FROM librarycards l JOIN borrowcards b ON l.LibraryCardID = b.LibraryCardID JOIN borrowcarddetails b2 ON b.BorrowCardID = b2.BorrowCardID JOIN books b3 ON b3.BookID = b2.BookID WHERE l.LibraryCardID = @libraryCardID AND b2.ReturnDate IS NULL AND b.DueDate > curdate()";
                command.Parameters.AddWithValue("@libraryCardID", libraryCardID);
                overdueCount = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ex = e;
            }
            finally
            {
                command.CommandText = @"UNLOCK TABLES";
                command.ExecuteNonQuery();
                connection?.Dispose();
            }
            return overdueCount;
        }
    }
}
