using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace Aio.Umbraco.Common.ContentModels
{
    public class CategoryModel : BaseModel
    {
        #region Category

        public string ParentCategoryName { get; set; }
        public CategoryModel ParentCategory { get; set; }

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

        public IList<NewsModel> ChildrenNews { get; set; }

        public CategoryModel()
        {
            ChildrenNews = new List<NewsModel>();
            ChildrenCategories = new List<CategoryModel>();
        }

        #endregion
    }
}