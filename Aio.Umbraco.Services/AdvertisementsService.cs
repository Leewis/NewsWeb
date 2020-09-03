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
using Umbraco.Core;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Models;

namespace Aio.Umbraco.Services
{
    public class AdvertisementsService : BaseService, IAdvertisementsService
    {
        public List<Advertising> GetByType(string typeName)
        {
            var advItems = new List<Advertising>();
            var advItemLowPriority = new List<Advertising>();
            var advItemHightPriority = new List<Advertising>();

            var currentNode = CurrentPublishedContent;

            IPublishedContent advertisements = currentNode.Children.First(c => c.ContentType.Alias == DocumentTypes.contentFolder && string.Equals(c.Name, typeName, StringComparison.InvariantCultureIgnoreCase));
            if (advertisements == null) return null;

            if (advertisements != null)
            {
                var advs = advertisements.Value<IEnumerable<IPublishedElement>>("AdvertisingData");
                foreach (var content in advs)
                {
                    if (content != null && isValidDate(content.Value<DateTime>("ExpiredDate"), content.Value<DateTime>("PublishedDate")))
                    {
                        Advertising adv = new Advertising();
                        MappingAdvertising(adv, content);
                        if (content.Value<bool>("priority") == true)
                        {
                            advItemHightPriority.Add(adv);
                        }
                        else
                        {
                            advItemLowPriority.Add(adv);
                        }
                    }
                }
                advItems.AddRange(advItemHightPriority);
                advItems.AddRange(advItemLowPriority);
            }
            return advItems;
        }
        private bool isValidDate(DateTime expiredDate, DateTime publishedDate)
        {
            DateTime _CurDate = DateTime.Now;
            if (_CurDate <= expiredDate)
            {
                if (_CurDate >= publishedDate)
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }

        private void MappingAdvertising(Advertising adv, IPublishedElement content)
        {
            adv.Title = content.Value("Title") as string;
            adv.PublishDate = content.Value<DateTime>("PublishedDate");
            adv.ExpiredDate = content.Value<DateTime>("ExpiredDate");
            adv.Link = content.Value<string>("LinkToSite");
            adv.Price = content.Value<Double>("price");
            adv.Priority = content.Value<bool>("priority");
            adv.IconUrl = content.Value<IPublishedContent>("siteImage").Url;
        }
    }
}
