using System;
namespace Persistence
{
    public class InvoiceDetail
    {
        public uint InvoiceDetailID { get; set; }
        public Book Book_Info { get; set; }
        public uint ItemQuantity { get; set; }
        public uint Amount { get; set; }
    }
}