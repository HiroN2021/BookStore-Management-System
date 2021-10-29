// using System;
// using Xunit;
// using Persistence;
// using DAL;

// namespace DALTest
// {
//     public class LibrarianDALTest
//     {
//         private Employee employee = new Employee();
//         private EmployeeDAL dal = new EmployeeDAL();
//         [Fact]
//         public void LoginTest1()
//         {
//             employee.UserName = "hieunm114010120014";
//             employee.UserPassword = "PF12VTCAcademy";
//             int expected = 1;
//             int result = dal.Login(ref employee, out string message);
//             Assert.True(result == expected);
//         }

//         [Theory]
//         [InlineData("hieunm114010120014", "PF12VTCAcademy", 1)]
//         [InlineData("TestUser", "PF12VTCAcademy", 0)]
//         [InlineData("hieunm114010120014", "TestPassword", 0)]
//         [InlineData("TestUser", "TestPassword", 0)]
//         public void LoginTest2(string userName, string pass, int expected)
//         {
//             employee.UserName = userName;
//             employee.UserPassword = pass;
//             int result = dal.Login(ref employee, out string message);
//             Assert.True(result == expected);
//         }
//     }
// }
