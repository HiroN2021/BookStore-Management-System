using System;
using Xunit;
using Persistence;
using DAL;

namespace DALTest
{
    public class BookDALTest
    {
        private BookDAL bookDAL = new BookDAL();
        [Theory]
        [InlineData(1, true)]
        [InlineData(60, true)]
        [InlineData(61, false)]
        public void GetBookByID(uint bookID, bool resultExpectNotNull)
        {
            Book book = bookDAL.GetBookByID(bookID, out Exception ex);
            bool expected = false;
            if (resultExpectNotNull && book != null)
                expected = true;
            if ((!resultExpectNotNull) && book == null)
                expected = true;
            Assert.True(ex == null && expected);
        }
    }
}
