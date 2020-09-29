using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Aio.Umbraco.Common.ContentModels;
using Aio.Umbraco.Services.Interfaces;
using Aio.Umbraco.Common.Extensions;
using Umbraco.Web.Models;
using System.Globalization;

namespace Aio.Umbraco.Services
{
    public class NewsService : BaseService, INewsService
    {
        Settings _siteSetting;
        private readonly UmbracoHelper _umbraco;
        private readonly UmbracoMapper _mapper;

        public NewsService()
        {
            _mapper = new UmbracoMapper(new MapDefinitionCollection(new List<IMapDefinition>()));
        }

        public NewsService(UmbracoHelper umbraco)
        {
            _umbraco = umbraco;
        }
        public NewsService(UmbracoMapper mapper) => _mapper = mapper;

        public IPublishedContent GetContent(Udi udi)
        {
            var context = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext;
            var udiAsString = udi.ToString();
            var guid = Guid.ParseExact(udiAsString.Substring(udiAsString.Length - 32), "N");
            //var cache = context.ContentCache.get
            var node = context.ContentCache.GetById(true, guid);
            return node;
        }

        public NewsModel GetNewsModel(IPublishedContent newsDetail)
        {
            if (newsDetail != null)
            {
                Aio.Umbraco.Common.ContentModels.NewsModel mappedNewsModel = new Aio.Umbraco.Common.ContentModels.NewsModel();
                mappedNewsModel = newsDetail.To<NewsModel>();
                mappedNewsModel.PostedTime = CalculatePostedDateTime(mappedNewsModel.PostedDateTime);
                mappedNewsModel.ShortDescription = String.IsNullOrEmpty(mappedNewsModel.ShortDescription) ? String.Empty : System.Net.WebUtility.HtmlDecode(mappedNewsModel.ShortDescription);

                var news = newsDetail.Value<IEnumerable<Link>>("relatedNews");

                if (news != null && news.Any())
                {
                    var context = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext;
                    foreach (var ne in news)
                    {
                        var ipublicContent = GetContent(ne.Udi);
                        mappedNewsModel.RelatedNewsModel.Add(ipublicContent.To<NewsModel>());
                    }
                }

                if (newsDetail.Parent != null)
                {
                    CategoryModel cat = newsDetail.Parent.To<CategoryModel>();
                    mappedNewsModel.Category = cat != null ? cat.Title : newsDetail.Parent.GetProperty("title").GetValue().ToString();
                    mappedNewsModel.CategoryUrl = cat != null ? cat.Url : newsDetail.Parent.GetProperty("url").GetValue().ToString();
                }

                return mappedNewsModel;
            }
            return null;
        }

        public List<NewsModel> GetTopicNews(IEnumerable<IPublishedContent> newestNews)
        {
            List<NewsModel> listItem = new List<NewsModel>();
            foreach (var i in newestNews)
            {
                listItem.Add(GetNewsModel(i));
            }
            return listItem;
        }

        public IPublishedContent GetNewsByCategoryAndName(string categoryName, string name)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var home = CurrentPublishedContent.FirstChild();

                var category = home.Descendants().Where(x => x.IsVisible() && x.ContentType.Alias == "category");

                foreach (var ca in category)
                {
                    if (ca.Name.Equals(categoryName, StringComparison.CurrentCultureIgnoreCase) || ca.GetProperty("title").GetValue().Equals(categoryName))
                    {
                        var newsChildren = ca.Descendants().Where(news => news.IsVisible() && news.ContentType.Alias == "news" && news.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                        return newsChildren.FirstOrDefault();
                    }
                }
            }

            return null;
        }

        public IList<IPublishedContent> GetNewsByCategoryId(int id)
        {
            var home = CurrentPublishedContent.FirstChild();

            var category = home.Descendants().Where(x => x.IsVisible() && x.ContentType.Alias == "category" && x.Id == id).FirstOrDefault();

            return category != null && category.Children.Any() ? category.Children.ToList() : null;
        }

        #region Helpers

        public string CalculatePostedDateTime(string dateTime)
        {
            string dateTimeStr = string.Empty;
            //ExactPostedHours

            //var slipDateTime = dateTime.Split(' ');
            ////DateTime dateVal = DateTime.ParseExact(slipDateTime[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //DateTime dateVal = Convert.ToDateTime(slipDateTime[0]);

            //var splipTime = slipDateTime[1].Split(':');

            //DateTime postedDate = new DateTime(dateVal.Year, dateVal.Month, dateVal.Day, (slipDateTime.Length >=3 && slipDateTime[2].Equals("PM", StringComparison.InvariantCultureIgnoreCase) ? int.Parse(splipTime[0]) + 12 : int.Parse(splipTime[0])), int.Parse(splipTime[1]), int.Parse(splipTime[2]), DateTimeKind.Local);
            DateTime postedDate = Convert.ToDateTime(dateTime);

            var excatPostHours = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")) - postedDate;

            if (excatPostHours.Days > 0)
            {
                dateTimeStr += excatPostHours.Days + " ngày";
            }
            else if (excatPostHours.Hours > 0)
            {
                if (excatPostHours.Minutes > 0)
                {
                    dateTimeStr += excatPostHours.Hours + " giờ" + excatPostHours.Minutes + " phút";
                }
                else
                {
                    dateTimeStr += excatPostHours.Hours + " giờ";
                }
            }
            else
                if (excatPostHours.Minutes > 0)
            {
                dateTimeStr += excatPostHours.Minutes + " phút";
            }

            return dateTimeStr;
        }

        #endregion
    }
}