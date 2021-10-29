using System;
using System.Collections.Generic;
namespace Persistence
{
    public class Invoice
    {
        public uint InvoiceID { get; set; }
        public Customer Customer_Info { get; set; }
        public Employee Employee_Info { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<InvoiceDetail> InvoiceDetails { get; set; }
        public string Descripton { get; set; }
    }
}