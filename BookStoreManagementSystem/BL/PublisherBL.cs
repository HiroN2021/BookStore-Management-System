using System;
using System.Collections.Generic;
using Persistence;
using DAL;

namespace BL
{
    public class PublisherBL
    {
        private PublisherDAL publisherDAL = new PublisherDAL();

        public List<Publisher> FindPublisherByPatternMatching(string pattern, out Exception ex)
        {
            return publisherDAL.FindPublisherByPatternMatching(pattern, out ex);
        }
    }
}
