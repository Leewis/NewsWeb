using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace NewsWeb.Models
{
    public class CategoryModel : BaseModel
    {
        #region Category

        public string ParentCategory { get; set; }
        public IPublishedContent CategoryPublishedContent { get; set; }

        #endregion

        #region Data
        
        public string Title { get; set; }
        public bool TopMenu { get; set; }
        
        #endregion

        #region Meta Tags
        public string DescriptionSuffix { get; set; }

        #endregion

        #region Children Category

        public IList<CategoryModel> ChildrenCategories { get;set;}

        #endregion
    }
}