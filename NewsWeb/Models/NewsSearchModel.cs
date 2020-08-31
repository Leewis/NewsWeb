using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsWeb.Models
{
    public class NewsSearchModel
    {
        public string Tag { get; set; }
        public string Keywords { get; set; }


        //return fields
        public List<NewsModel> News { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }

        public NewsSearchModel()
        {
            CurrentPage = 1;
        }
    }
}