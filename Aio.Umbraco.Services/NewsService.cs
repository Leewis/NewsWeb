﻿using System;
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

        #region Helpers

        public string CalculatePostedDateTime(string dateTime)
        {
            string dateTimeStr = string.Empty;
            //ExactPostedHours
            var slipDateTime = dateTime.Split(' ');

            DateTime dateVal = Convert.ToDateTime(slipDateTime[0]);
            //DateTime dateVal = DateTime.ParseExact(slipDateTime[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var splipTime = slipDateTime[1].Split(':');

            DateTime postedDate = new DateTime(dateVal.Year, dateVal.Month, dateVal.Day, int.Parse(splipTime[0]) + 4, int.Parse(splipTime[1]), int.Parse(splipTime[2]), DateTimeKind.Local);

            var excatPostHours = DateTime.Now - postedDate;

            if (excatPostHours.Days > 0)
            {
                dateTimeStr += excatPostHours.Days + " ngày";
            }
            else if (excatPostHours.Hours > 0)
            {
                dateTimeStr += excatPostHours.Hours + " giờ";

                if (excatPostHours.Minutes > 0)
                {
                    dateTimeStr += excatPostHours.Hours + " giờ" + excatPostHours.Minutes + " phút";
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