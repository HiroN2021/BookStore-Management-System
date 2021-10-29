using System;
using Xunit;
using Persistence;
using DAL;

namespace DALTest
{
    public class EmployeeDALTest
    {
        private Exception e;
        private Employee employee = new Employee();
        private EmployeeDAL employeeDAL = new EmployeeDAL();
        // [Fact]
        // public void LoginTest1()
        // {
        //     employee.UserName = "hieunm114010120014";
        //     employee.UserPassword = "PF12VTCAcademy";
        //     int expected = 1;
        //     int result = employeeDAL.Login(ref employee, out string message);
        //     Assert.True(result == expected);
        // }

        [Theory]
        // [InlineData("hieunm114010120014", "PF12VTCAcademy")]
        [InlineData("cashier1", "test")]
        // [InlineData("TestUser", "PF12VTCAcademy", 0)]
        // [InlineData("hieunm114010120014", "TestPassword", 0)]
        // [InlineData("TestUser", "TestPassword", 0)]
        public void GetEmployee(string userName, string password)
        {
            // employee.UserName = userName;
            // employee.UserPassword = pass;
            Employee result = employeeDAL.GetEmployee(userName, password, out Exception ex);
            // Assert.True(result != null);
            Assert.False(result == null);
        }
    }
}
