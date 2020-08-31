using NewsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace NewsWeb.Services
{
    public class NewsService : INewsService
    {
        Setting _siteSetting;
        private readonly UmbracoHelper _umbraco;
        private readonly UmbracoMapper _mapper;

        public Setting SiteSetting
        {
            get
            {
                if (_siteSetting == null)
                {
                    //var rootNode = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext.Content.GetAtRoot();

                    _siteSetting = new Setting();
                    var setting = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext.Content.GetById(1118);
                    if (setting != null)
                    {
                        _mapper.Define<IPublishedContent, Setting>(
                            (src, ctx) => new Setting(),
                            (src, dst, ctx) =>
                            {
                                // Run base mappers
                                //_siteSetting = ctx.Map<IPublishedContent, Setting>(src, dst);

                                // Custom maps
                                dst.Id = src.Id;
                                dst.PublishedContent = src;
                                dst.CreatedDate = src.CreateDate;
                                dst.SiteLogoUrl = src.Value<IPublishedContent>("siteLogoUrl") != null ? src.Value<IPublishedContent>("siteLogoUrl").Url : string.Empty;

                                dst.FacebookUrl = src.GetProperty("facebookUrl").GetValue().ToString();
                                dst.GoogleUrl = src.GetProperty("googleUrl") != null && src.GetProperty("googleUrl").HasValue() ? src.GetProperty("googleUrl").GetValue().ToString() : "";

                                dst.HomeUrl = src.GetProperty("homeUrl") != null && src.GetProperty("homeUrl").HasValue() ? src.GetProperty("homeUrl").GetValue().ToString() : "";

                                //TwitterUrl
                                dst.TwitterUrl = src.GetProperty("twitterUrl") != null && src.GetProperty("twitterUrl").HasValue() ? src.GetProperty("twitterUrl").GetValue().ToString() : "";

                                //PinterestUrl
                                dst.PinterestUrl = src.GetProperty("tinterestUrl") != null && src.GetProperty("tinterestUrl").HasValue() ? src.GetProperty("tinterestUrl").GetValue().ToString() : "";

                                //VimeoUrl
                                dst.VimeoUrl = src.GetProperty("vimeoUrl") != null && src.GetProperty("vimeoUrl").HasValue() ? src.GetProperty("vimeoUrl").GetValue().ToString() : "";

                                //LinkedinUrl
                                dst.LinkedinUrl = src.GetProperty("linkedinUrl") != null && src.GetProperty("linkedinUrl").HasValue() ? src.GetProperty("linkedinUrl").GetValue().ToString() : "";

                                //DescriptionSuffix
                                dst.DescriptionSuffix = src.GetProperty("descriptionSuffix") != null && src.GetProperty("descriptionSuffix").HasValue() ? src.GetProperty("descriptionSuffix").GetValue().ToString() : "";
                            }
                        );

                        _mapper.Map(setting, _siteSetting);
                    }
                }
                return _siteSetting;
            }
        }

        public NewsService()
        {

        }

        public NewsService(UmbracoHelper umbraco)
        {
            _umbraco = umbraco;
        }
        public NewsService(UmbracoMapper mapper) => _mapper = mapper;

        public NewsModel GetNewsModel(IPublishedContent newsDetail, NewsModel mappedNewsModel)
        {
            if (newsDetail != null)
            {
                _mapper.Define<IPublishedContent, NewsModel>(
                    (src, ctx) => new NewsModel(),
                    (src, dst, ctx) =>
                    {
                        // Run base mappers
                        //_siteSetting = ctx.Map<IPublishedContent, Setting>(src, dst);

                        // Custom maps
                        dst.Id = src.Id;
                        dst.PublishedContent = src;
                        dst.CreatedDate = src.CreateDate;
                        dst.Source = src.Value<IPublishedContent>("source") != null ? src.Value<IPublishedContent>("source").Url : string.Empty;

                        dst.SourceUrl = src.Value<IPublishedContent>("sourceUrl") != null ? src.Value<IPublishedContent>("sourceUrl").Url : string.Empty;
                        dst.Author = src.GetProperty("author") != null && src.GetProperty("author").HasValue() ? src.GetProperty("author").GetValue().ToString() : "";

                        dst.Title = src.GetProperty("title") != null && src.GetProperty("title").HasValue() ? src.GetProperty("title").GetValue().ToString() : "";

                        //ShortDescription
                        dst.ShortDescription = src.GetProperty("shortDescription") != null && src.GetProperty("shortDescription").HasValue() ? src.GetProperty("shortDescription").GetValue().ToString() : "";

                        //FullDescription
                        dst.FullDescription = src.GetProperty("fullDescription") != null && src.GetProperty("fullDescription").HasValue() ? src.GetProperty("fullDescription").GetValue().ToString() : "";

                        //Picture
                        dst.Picture = src.GetProperty("picture") != null && src.GetProperty("picture").HasValue() ? src.GetProperty("picture").GetValue().ToString() : "";

                        //PostedDateTime
                        dst.PostedDateTime = src.GetProperty("postedDateTime") != null && src.GetProperty("postedDateTime").HasValue() ? src.GetProperty("postedDateTime").GetValue().ToString() : "";

                        //DescriptionSuffix
                        dst.DescriptionSuffix = src.GetProperty("descriptionSuffix") != null && src.GetProperty("descriptionSuffix").HasValue() ? src.GetProperty("descriptionSuffix").GetValue().ToString() : "";

                        //PostedTime
                        dst.PostedTime = src.GetProperty("postedTime") != null && src.GetProperty("postedTime").HasValue() ? src.GetProperty("postedTime").GetValue().ToString() : "";

                        //Category
                        dst.Category = src.GetProperty("category") != null && src.GetProperty("category").HasValue() ? src.GetProperty("category").GetValue().ToString() : "";

                        dst.CategoryPublishedContent = src.GetProperty("category") as IPublishedContent;
                    }
                );

                _mapper.Map(newsDetail, mappedNewsModel);

            }

            var news = newsDetail.Value("relatedNews") as List<Umbraco.Web.Models.Link>;

            if (news != null && news.Any())
            {
                IList<NewsModel> relatedNews = new List<NewsModel>();
                var context = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext;
                foreach (var ne in news)
                {
                    var ipublicContent = GetContent(ne.Udi);

                    var related = new NewsModel();
                    _mapper.Map(ipublicContent, related);

                    mappedNewsModel.RelatedNews.Add(related);
                }
            }

            return mappedNewsModel;
        }

        public IList<NewsModel> GetRelatedNews(IPublishedContent newsDetail)
        {
            IList<NewsModel> relatedNews = new List<NewsModel>();

            var news = newsDetail.Value("relatedNews") as List<Umbraco.Web.Models.Link>;
            var context = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext;
            foreach (var ne in news)
            {
                relatedNews.Add(new NewsModel()
                {
                    Title = ne.Name,
                    Url = ne.Url,
                    Picture = ne.Target
                });
            }

            return relatedNews;
        }

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
                _mapper.Define<IPublishedContent, NewsModel>(
                    (src, ctx) => new NewsModel(),
                    (src, dst, ctx) =>
                    {
                        // Run base mappers
                        //_siteSetting = ctx.Map<IPublishedContent, Setting>(src, dst);

                        // Custom maps
                        dst.Id = src.Id;
                        dst.PublishedContent = src;
                        dst.CreatedDate = src.CreateDate;
                        dst.Source = src.GetProperty("source") != null && src.GetProperty("source").HasValue() ? src.GetProperty("source").GetValue().ToString() : string.Empty;

                        dst.SourceUrl = src.GetProperty("sourceUrl") != null && src.GetProperty("sourceUrl").HasValue() ? src.GetProperty("sourceUrl").GetValue().ToString() : string.Empty;
                        dst.Author = src.GetProperty("author") != null && src.GetProperty("author").HasValue() ? src.GetProperty("author").GetValue().ToString() : "";

                        dst.Title = src.GetProperty("title") != null && src.GetProperty("title").HasValue() ? src.GetProperty("title").GetValue().ToString() : "";

                        //ShortDescription
                        dst.ShortDescription = src.GetProperty("shortDescription") != null && src.GetProperty("shortDescription").HasValue() ? src.GetProperty("shortDescription").GetValue().ToString() : "";

                        //FullDescription
                        dst.FullDescription = src.GetProperty("fullDescription") != null && src.GetProperty("fullDescription").HasValue() ? src.GetProperty("fullDescription").GetValue().ToString() : "";

                        //Picture
                        dst.Picture = src.GetProperty("picture") != null && src.GetProperty("picture").HasValue() ? src.GetProperty("picture").GetValue().ToString() : "";

                        //PostedDateTime
                        dst.PostedDateTime = src.GetProperty("postedDateTime") != null && src.GetProperty("postedDateTime").HasValue() ? src.GetProperty("postedDateTime").GetValue().ToString() : "";

                        //DescriptionSuffix
                        dst.DescriptionSuffix = src.GetProperty("descriptionSuffix") != null && src.GetProperty("descriptionSuffix").HasValue() ? src.GetProperty("descriptionSuffix").GetValue().ToString() : "";

                        //PostedTime
                        dst.PostedTime = src.GetProperty("postedTime") != null && src.GetProperty("postedTime").HasValue() ? src.GetProperty("postedTime").GetValue().ToString() : "";

                        //Category
                        dst.Category = src.GetProperty("category") != null && src.GetProperty("category").HasValue() ? src.Value<IPublishedContent>("category").Value("Title").ToString() : "";

                        dst.CategoryPublishedContent = src.Value<IPublishedContent>("category") as IPublishedContent;

                        dst.Url = src.Url;
                    }
                );

                var mappedNewsModel = new NewsModel();
                _mapper.Map(newsDetail, mappedNewsModel);

                var news = newsDetail.Value("relatedNews") as List<Umbraco.Web.Models.Link>;

                if (news != null && news.Any())
                {
                    IList<NewsModel> relatedNews = new List<NewsModel>();
                    mappedNewsModel.RelatedNews = new List<NewsModel>();
                    var context = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext;
                    foreach (var ne in news)
                    {
                        var ipublicContent = GetContent(ne.Udi);

                        var related = new NewsModel();
                        related.RelatedNews = new List<NewsModel>();
                        _mapper.Map(ipublicContent, related);

                        mappedNewsModel.RelatedNews.Add(related);
                    }
                }

                return mappedNewsModel;
            }
            return null;
        }
    }
}