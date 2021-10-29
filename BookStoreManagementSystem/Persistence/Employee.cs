using System;

namespace Persistence
{
    public class Employee
    {
        public uint EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public PersonGender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}