using NewsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace NewsWeb.Services
{
    public class SettingsService : ISettingsService
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

        public SettingsService()
        {

        }

        public SettingsService(UmbracoHelper umbraco)
        {
            _umbraco = umbraco;
        }
        public SettingsService(UmbracoMapper mapper) => _mapper = mapper;

    }
}