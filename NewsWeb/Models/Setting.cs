using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace NewsWeb.Models
{
    public class Setting: BaseModel
    {
        #region Site Settings
        public string HomeUrl { get; set; }
        public string SiteLogoUrl { get; set; }
        public IPublishedContent RootSiteContentNode { get; set; }
        #endregion

        #region Social
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string GoogleUrl { get; set; }
        public string PinterestUrl { get; set; }
        public string VimeoUrl { get; set; }
        public string LinkedinUrl { get; set; }
        #endregion

        #region Meta Tags
        public string DescriptionSuffix { get; set; }

        #endregion
    }
}