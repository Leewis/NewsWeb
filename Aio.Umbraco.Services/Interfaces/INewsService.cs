using Aio.Umbraco.Common.ContentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace Aio.Umbraco.Services.Interfaces
{
    public interface INewsService
    {
        NewsModel GetNewsModel(IPublishedContent newsDetail);
        NewsModel[] GetCategory(IOrderedEnumerable<IPublishedContent> fillterData);
    }
}