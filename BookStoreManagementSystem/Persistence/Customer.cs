using System;

namespace Persistence
{
    public enum PersonGender
    {
        male,
        female,
        unknow
    }
    public class Customer
    {
        public uint CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactTitle { get; set; }
        public PersonGender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
    }
}