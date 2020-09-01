using Aio.Umbraco.Common.ContentModels;
using Aio.Umbraco.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Macros;
using Umbraco.Web.Mvc;

namespace NewsWeb.Controllers
{
    public class SearchController : SurfaceController
    {
        private readonly ISearchHelper _searchHelper;

        NewsSearchModel vm = new NewsSearchModel();

        public SearchController(ISearchHelper searchHelper)
        {
            _searchHelper = searchHelper;
        }


        public ActionResult Index(int pageSize)
        {
            string cat = string.IsNullOrEmpty(Request["cat"]) ? string.Empty : Request["cat"];
            string searchTerm = string.IsNullOrEmpty(Request["q"]) ? string.Empty : Request["q"];
            string page = string.IsNullOrEmpty(Request["page"]) ? "1" : Request["page"];

            if (!int.TryParse(page, out int pageNumber))
                pageNumber = 1;

            NewsSearchModel newsSearch = new NewsSearchModel();
            newsSearch.CurrentPage = pageNumber;
            newsSearch.ItemsPerPage = pageSize;
            newsSearch.Tag = cat;
            newsSearch.Keywords = searchTerm;
            vm = _searchHelper.Search(newsSearch);
            return PartialView("SearchResultPartial", vm);
        }
    }
}