using Aio.Umbraco.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aio.Umbraco.Common.ContentModels;
using Umbraco.Web;
using Aio.Umbraco.Common.Constants;
using Aio.Umbraco.Common.Extensions;
using Umbraco.Core.Models;
//using umbraco.NodeFactory;

namespace Aio.Umbraco.Services
{
    public class SettingsService : BaseService, ISettingsService
    {
        Settings siteSettings;

        public Settings SiteSettings
        {
            get
            {
                if (siteSettings == null)
                {
                    var currentNode = CurrentPublishedContent;

                    var webSiteNode = currentNode;//.AncestorOrSelf(DocumentTypes.Website);
                    if (webSiteNode == null) return null;                    

                    var settingsNode = webSiteNode.Children.First(c => c.ContentType.Alias == DocumentTypes.Settings);
                    if (settingsNode == null) return null;

                    siteSettings = settingsNode.To<Settings>();
                    siteSettings.RootSiteContentNode = webSiteNode;

                    var homeNode = webSiteNode.Children.FirstOrDefault(c => c.ContentType.Alias == DocumentTypes.HomePage);
                    if (homeNode != null)
                    {
                        siteSettings.HomeUrl = homeNode.Url;
                    }

                    siteSettings.LogoUrl = siteSettings.SiteLogoUrl.To<Aio.Umbraco.Common.ContentModels.Media>();

                    //var menuBarNode = settingsNode.Children.FirstOrDefault(c =>
                    //    string.Equals(c.ContentType.Alias, DocumentTypes.MenuBar, StringComparison.InvariantCultureIgnoreCase));
                    //siteSettings.MenuItems = new List<MenuItem>();
                    //if (menuBarNode != null)
                    //{
                    //    foreach (var c in menuBarNode.Children)
                    //    {
                    //        var menuItem = c.To<MenuItem>();
                    //        siteSettings.MenuItems.Add(menuItem);
                    //    }
                    //}

                    //var usefulTile = settingsNode.Children.FirstOrDefault(c =>
                    //       string.Equals(c.ContentType.Alias, DocumentTypes.UsefulLinks, StringComparison.InvariantCultureIgnoreCase));

                    //if (usefulTile != null)
                    //{
                    //    List<UsefulLink> usefulLinks = new List<UsefulLink>();
                    //    foreach (var c in usefulTile.Children)
                    //    {
                    //        var usefulLinkItem = c.To<UsefulLink>();
                    //        if (usefulLinkItem != null)
                    //            usefulLinks.Add(usefulLinkItem);
                    //    }

                    //    SiteSettings.UsefulLinks = usefulLinks;
                    //}

                    //var contactUs = settingsNode.Children.FirstOrDefault(c =>
                    //      string.Equals(c.ContentType.Alias, DocumentTypes.ContactUs, StringComparison.InvariantCultureIgnoreCase));

                    //if (contactUs != null)
                    //{
                    //    //SiteSettings.ContactUs = contactUs.To<ContactUs>();
                    //}
                }
                return siteSettings;
            }
        }
    }
}
