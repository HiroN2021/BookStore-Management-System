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
        // [InlineData("hieunm114010120014", "PF12VTCAcademy")]
        [InlineData("cashier1", "test")]
        // [InlineData("TestUser", "PF12VTCAcademy", 0)]
        // [InlineData("hieunm114010120014", "TestPassword", 0)]
        // [InlineData("TestUser", "TestPassword", 0)]
        public void GetEmployee(string userName, string password)
        {
            Employee result = employeeDAL.GetEmployee(userName, password, out Exception ex);
            // Assert.True(result != null);
            Assert.False(ex != null || result == null);
        }
    }
}
