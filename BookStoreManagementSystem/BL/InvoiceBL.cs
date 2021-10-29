using System;
using Persistence;
using DAL;

namespace BL
{
    public class InvoiceBL
    {
        private InvoiceDAL invoiceDAL = new InvoiceDAL();

        public bool AddToDataBase(in Invoice invoice, out Exception ex)
        {
            bool status = invoiceDAL.AddToDataBase(in invoice, out ex);
            // if (ex != null)
            //     throw ex;
            return status;
        }
    }
}
