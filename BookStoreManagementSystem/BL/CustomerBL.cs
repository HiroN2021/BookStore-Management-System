using System;
using Persistence;
using DAL;

namespace BL
{
    public class CustomerBL
    {
        private CustomerDAL customerDAL = new CustomerDAL();

        public Customer GetCustomerByPhone(string phone, out Exception ex)
        {
            return customerDAL.GetCustomerByPhone(phone, out ex);
        }
    }
}
