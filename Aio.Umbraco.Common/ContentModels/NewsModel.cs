using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace Aio.Umbraco.Common.ContentModels
{
    public class NewsModel : BaseModel
    {
        #region Category
        public string Category { get; set; }
        public IPublishedContent CategoryPublishedContent { get; set; }

        #endregion

        #region Data
        public string Author { get; set; }
        public string Source { get; set; }
        public string SourceUrl { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Picture { get; set; }
        public string PostedDateTime { get; set; }
        public string PostedTime { get; set; }
        
        #endregion

        #region Meta Tags
        public string DescriptionSuffix { get; set; }

        #endregion

        #region Related News

        public IList<NewsModel> RelatedNewsModel {get;set;}

        #endregion

        public NewsModel()
        {
            RelatedNewsModel = new List<NewsModel>();
        }
    }
}