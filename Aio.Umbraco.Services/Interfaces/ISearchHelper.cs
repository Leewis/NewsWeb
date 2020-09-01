using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Examine;
using Examine;
using Umbraco.Web;
using Examine.Search;
using Umbraco.Web.PublishedCache;
using Umbraco.Core.Mapping;
using System.Text.RegularExpressions;
using Aio.Umbraco.Common.ContentModels;

namespace Aio.Umbraco.Services.Interfaces
{
    public interface ISearchHelper
    {
        NewsSearchModel Search(NewsSearchModel newsSearch);    
    }
}
