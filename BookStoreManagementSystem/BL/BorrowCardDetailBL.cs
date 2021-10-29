// using System;
// using System.Collections.Generic;
// using Persistence;
// using DAL;

// namespace BL
// {
//     public class BorrowCardDetailBL
//     {
//         private BorrowCardDetailDAL bookDAL = new BorrowCardDetailDAL();

//         public List<BorrowCardDetail> GetActiveBorrowCardDetailsByLibraryCardID(uint libraryCardID, out List<DateTime> borrowFromDateList, out List<DateTime> dueDateList, out Exception ex)
//         {
//             var borrowCardDetails = bookDAL.GetActiveBorrowCardDetailsByLibraryCardID(libraryCardID, out borrowFromDateList, out dueDateList, out ex);
//             if (ex != null)
//                 throw ex;
//             return borrowCardDetails;
//         }
//     }
// }
