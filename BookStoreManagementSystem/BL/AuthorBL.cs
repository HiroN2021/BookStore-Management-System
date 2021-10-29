using System;
using System.Collections.Generic;
using Persistence;
using DAL;

namespace BL
{
    public class AuthorBL
    {
        private AuthorDAL authorDAL = new AuthorDAL();

        public List<Author> FindAuthorByPatternMatching(string pattern, out Exception ex)
        {
            return authorDAL.FindAuthorByPatternMatching(pattern, out ex);
        }
    }
}
