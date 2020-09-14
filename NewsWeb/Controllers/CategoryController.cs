using Aio.Umbraco.Common.ContentModels;
using Aio.Umbraco.Services.Interfaces;
//using Lucene.Net.Util;
using System;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;
using Umbraco.Core.Composing;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Web.Models;
using Umbraco.Core.PropertyEditors;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsWeb.Controllers
{
    public class CategoryApiController : UmbracoApiController
    {
        private readonly ICategoryService _iCategoryService;
        public CategoryApiController(ICategoryService iCategoryService)
        {
            _iCategoryService = iCategoryService;
        }

        public bool InsertCategory(CategoryModel cat)
        {
            //Check if this Category has existed or not
            var existedCategory = _iCategoryService.GetCategory(cat.Name);

            if (existedCategory == null)
            {
                IContent createdCategory = Services.ContentService.Create(cat.Name, cat.ParentId, "category");

                PopulateContentFields(createdCategory, cat);

                var catResult = Services.ContentService.Save(createdCategory);

                if (catResult.Success)
                    return true;
            }
            else
            {
                var content = Services.ContentService.GetById(existedCategory.Id);
                //Update News
                PopulateContentFields(content, cat);

                var newsResult = Services.ContentService.Save(content);

                if (newsResult.Success)
                    return true;
            }

            return false;
        }

        private void PopulateContentFields(IContent content, CategoryModel model)
        {
            content.Name = model.Name;
            content.SetValue("title", model.Title);           

            content.SetValue("metaKeywords", model.MetaKeywords);
            content.SetValue("metaPageTitle", model.MetaPageTitle);
            content.SetValue("metaDescription", model.MetaDescription);
        }       
    }
}