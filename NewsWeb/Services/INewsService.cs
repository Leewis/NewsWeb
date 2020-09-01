using NewsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace NewsWeb.Services
{
    public interface INewsService
    {
        Setting SiteSetting { get; }
        IList<NewsModel> GetRelatedNews(IPublishedContent newsDetail);
        NewsModel GetNewsModel(IPublishedContent newsDetail);
        NewsModel[] GetCategory(IOrderedEnumerable<IPublishedContent> fillterData);
    }
}