using System;
using Xunit;
using Persistence;
using DAL;

namespace DALTest
{
    public class EmployeeDALTest
    {
        private EmployeeDAL employeeDAL = new EmployeeDAL();
        [Theory]
        [InlineData("cashier1", "test", true)]
        [InlineData("cashier2", "pf12group04", true)]
        [InlineData("cashier1", "lolasjdlkj", false)]
        [InlineData("cashier2", "pf12askjdlsjdl", false)]
        public void GetEmployee(string userName, string password, bool employeeExpectNotNull)
        {
            Employee employee = employeeDAL.GetEmployee(userName, password, out Exception ex);
            bool expected = false;
            if (employeeExpectNotNull && employee != null)
                expected = true;
            else if ((!employeeExpectNotNull) && employee == null)
                expected = true;
            Assert.True(ex == null && expected);
        }
    }
}
