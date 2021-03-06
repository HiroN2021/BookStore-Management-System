using System;
using Persistence;
using DAL;
using System.Collections.Generic;

namespace BL
{
    public class BookBL
    {
        private BookDAL bookDAL = new BookDAL();
        public Book GetBookByID(uint bookID, out Exception ex)
        {
            return bookDAL.GetBookByID(bookID, out ex);
        }
        public Book GetBookByISBN(string isbn, out Exception ex)
        {
            var book = bookDAL.GetBookByISBN(isbn, out ex);
            return book;
        }
        public List<Book> FindBooksByTitle(string pattern, out Exception ex)
        {
            return bookDAL.FindBooksByTitle(pattern, out ex);
        }
        public List<Book> FindBooksByCategory(uint categoryID, out Exception ex)
        {
            return bookDAL.FindBooksByCategory(categoryID, out ex);
        }
        public List<Book> FindBooksByAuthor(uint authorID, out Exception ex)
        {
            return bookDAL.FindBooksByAuthor(authorID, out ex);
        }
        public List<Book> FindBooksByPublisher(uint publisherID, out Exception ex)
        {
            return bookDAL.FindBooksByPublisher(publisherID, out ex);
        }
    }
}
