using System;
using Persistence;
using DAL;

namespace BL
{
    public class EmployeeBL
    {
        private EmployeeDAL librarianDAL = new EmployeeDAL();

        public Employee GetEmployee(string userName, string password, out Exception ex)
        {
            return librarianDAL.GetEmployee(userName, password, out ex);
        }
    }
}
