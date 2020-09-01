using Aio.Umbraco.Common.Extensions;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Aio.Umbraco.Common.ContentModels
{
    public class Settings : BaseModel
    {
        #region Site Settings
        public string HomeUrl { get; set; }
        //public Media SiteLogoUrl { get; set; }
        public IPublishedContent SiteLogoUrl { get; set; }
        public Media LogoUrl { get; set; }
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

        #region Get In Touch
        public string GetInTouchTitle { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Cellphone { get; set; }
        public string EmailAddress { get; set; }
        #endregion

        #region Useful Links
        //public List<UsefulLink> UsefulLinks { get; set; }
        #endregion

        #region ContactUs
        //public ContactUs ContactUs { get; set; }
        #endregion

        public Settings()
        {
            //SiteLogoUrl = new Media();
        }
    }
}
