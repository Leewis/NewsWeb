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

                return mappedNewsModel;
            }
            return null;
        }
    }
}