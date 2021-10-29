using System;
using Persistence;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class LibraryCardDAL
    {
        public LibraryCard GetLibraryCardByID(uint libraryCardID, out Exception ex, bool mustBeActive = true)
        {
            LibraryCard libraryCard = null;
            ex = null;

            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                if (mustBeActive)
                    command.CommandText = "SELECT l.LibraryCardID, r.ReaderName, r.Class, r.Gender, r.Phone, r.Email, l.ExpiryDate FROM librarycards l INNER JOIN readers r ON l.ReaderID = r.ReaderID WHERE l.LibraryCardID = @libraryCardID AND l.Status = @status LiMIT 1";
                else
                    command.CommandText = "SELECT l.LibraryCardID, r.ReaderName, r.Class, r.Gender, r.Phone, r.Email, l.ExpiryDate FROM librarycards l INNER JOIN readers r ON l.ReaderID = r.ReaderID WHERE l.LibraryCardID = @libraryCardID LiMIT 1";
                command.Parameters.AddWithValue("@libraryCardID", libraryCardID);
                command.Parameters.AddWithValue("@status", LibraryCardStatus.Active);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    libraryCard = new LibraryCard();
                    libraryCard.LibaryCardID = libraryCardID;
                    Reader rd = libraryCard.ReaderInfo = new Reader();
                    libraryCard.ExpiryDate = DateTime.Parse(reader["ExpiryDate"].ToString());
                    // libraryCard.Status = Enum.Parse<LibraryCardStatus>(reader["Status"].ToString());
                    libraryCard.Status = LibraryCardStatus.Active;
                    rd.Name = reader["ReaderName"].ToString();
                    rd.ReaderClass = reader["Class"].ToString();
                    rd.Gender = reader["Gender"].ToString();
                    rd.Phone = reader["Phone"].ToString();
                    rd.Email = reader["Email"].ToString();
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
            return libraryCard;
        }
        public bool UpdateLibraryCard(in LibraryCard libraryCard, out Exception ex)
        {
            ex = null;
            bool status = false;
            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "UPDATE LibraryCards SET IssuedAt = @issuedAt , ExpiryDate = @expiryDate , Status = @status WHERE LibraryCardID = @libraryCardID";
                command.Parameters.AddWithValue("@issuedAt", libraryCard.IssuedAt);
                command.Parameters.AddWithValue("@expiryDate", libraryCard.ExpiryDate);
                command.Parameters.AddWithValue("@status", libraryCard.Status);
                command.Parameters.AddWithValue("@libraryCardID", libraryCard.LibaryCardID);
                if (command.ExecuteNonQuery() == 1)
                    status = true;
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                connection?.Dispose();
            }
            return status;
        }
        public int GetNumberOfBookBorrowed(uint libraryCardID, out Exception ex)
        {
            ex = null;
            int count = 0;
            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"SELECT COUNT(*) FROM librarycards l JOIN borrowcards b ON l.LibraryCardID = b.LibraryCardID JOIN borrowcarddetails b2 ON b.BorrowCardID = b2.BorrowCardID JOIN books b3 ON b3.BookID = b2.BookID WHERE l.LibraryCardID = @libraryCardID AND b2.ReturnDate IS NULL";
                command.Parameters.AddWithValue("@libraryCardID", libraryCardID);
                count = Int32.Parse(command.ExecuteScalar().ToString());
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                connection?.Dispose();
            }
            return count;
        }
        public int GetNumberOfBookOverDue(uint libraryCardID, out Exception ex)
        {
            ex = null;
            int count = 0;
            MySqlConnection connection = DbHelper.GetConnection();
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = @"SELECT COUNT(*) FROM librarycards l JOIN borrowcards b ON l.LibraryCardID = b.LibraryCardID JOIN borrowcarddetails b2 ON b.BorrowCardID = b2.BorrowCardID JOIN books b3 ON b3.BookID = b2.BookID WHERE l.LibraryCardID = @libraryCardID AND b2.ReturnDate IS NULL AND b.DueDate < CURDATE()";
                command.Parameters.AddWithValue("@libraryCardID", libraryCardID);
                count = Int32.Parse(command.ExecuteScalar().ToString());
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                connection?.Dispose();
            }
            return count;
        }
    }
}
