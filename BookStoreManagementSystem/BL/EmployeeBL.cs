using System;
using Persistence;
using DAL;

namespace BL
{
    public class EmployeeBL
    {
        private EmployeeDAL employeeDAL = new EmployeeDAL();

        public Employee GetEmployee(string userName, string password, out Exception ex)
        {
            return employeeDAL.GetEmployee(userName, password, out ex);
        }
    }
}
