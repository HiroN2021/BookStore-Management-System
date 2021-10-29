using System;
using System.Collections.Generic;
using Persistence;
using DAL;

namespace BL
{
    public class CategoryBL
    {
        private CategoryDAL categoryDAL = new CategoryDAL();

        public List<Category> FindCategories(string pattern, out Exception ex)
        {
            return categoryDAL.FindCategories(pattern, out ex);
        }
    }
}
