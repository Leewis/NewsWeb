using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Mapping;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Aio.Umbraco.Common.ContentModels;
using Aio.Umbraco.Services.Interfaces;
using Aio.Umbraco.Common.Extensions;
using Umbraco.Web.Models;

namespace Aio.Umbraco.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        Settings _siteSetting;
        private readonly UmbracoHelper _umbraco;
        private readonly UmbracoMapper _mapper;

        public CategoryService()
        {
            _mapper = new UmbracoMapper(new MapDefinitionCollection(new List<IMapDefinition>()));
        }

        public CategoryService(UmbracoHelper umbraco)
        {
            _umbraco = umbraco;
        }
        public CategoryService(UmbracoMapper mapper) => _mapper = mapper;

        public CategoryModel GetCategoryModel(IPublishedContent categoryModel)
        {
            if (categoryModel != null)
            {
                Aio.Umbraco.Common.ContentModels.CategoryModel mappedCategoryModel = new Aio.Umbraco.Common.ContentModels.CategoryModel();
                mappedCategoryModel = categoryModel.To<CategoryModel>();
                mappedCategoryModel.ChildrenCategories = new List<CategoryModel>();
                mappedCategoryModel.ChildrenNews = new List<NewsModel>();

                var categories = categoryModel.Children;

                if (categories != null && categories.Any())
                {
                    foreach (var caLev0 in categories)
                    {
                        if (caLev0.ContentType.Alias.Equals("category"))
                        {
                            var mappedCategoryModel0 = caLev0.To<CategoryModel>();
                            mappedCategoryModel0.ChildrenCategories = new List<CategoryModel>();

                            foreach (var caLev1 in caLev0.Children)
                            {                                
                                if (caLev1.ContentType.Alias.Equals("category"))
                                {
                                    var mappedCategoryModel1 = caLev1.To<CategoryModel>();
                                    mappedCategoryModel1.ChildrenCategories = new List<CategoryModel>();

                                    foreach (var caLev2 in caLev1.Children)
                                    {
                                        if (caLev2.ContentType.Alias.Equals("category"))
                                        {
                                            var mappedCategoryModel2 = caLev0.To<CategoryModel>();
                                            mappedCategoryModel1.ChildrenCategories.Add(mappedCategoryModel2);

                                            foreach (var caLev3 in caLev2.Children)
                                            {
                                                if (caLev3.ContentType.Alias.Equals("news"))
                                                {
                                                    var news = caLev3.To<NewsModel>();
                                                    if (mappedCategoryModel.ChildrenNews.Any() && mappedCategoryModel.ChildrenNews[0].PostedDateTime.CompareTo(news.PostedDateTime) < 0)
                                                    {
                                                        mappedCategoryModel.ChildrenNews.Insert(0, news);
                                                    }
                                                    else
                                                    {
                                                        mappedCategoryModel.ChildrenNews.Add(news);
                                                    }
                                                }
                                            }
                                        }
                                        else if (caLev2.ContentType.Alias.Equals("news"))
                                        {   
                                            var news = caLev2.To<NewsModel>();
                                            if (mappedCategoryModel.ChildrenNews.Any() && mappedCategoryModel.ChildrenNews[0].PostedDateTime.CompareTo(news.PostedDateTime) < 0)
                                            {
                                                mappedCategoryModel.ChildrenNews.Insert(0, news);
                                            }
                                            else
                                            {
                                                mappedCategoryModel.ChildrenNews.Add(news);
                                            }
                                        }
                                    }

                                    mappedCategoryModel0.ChildrenCategories.Add(mappedCategoryModel1);
                                }
                                else if (caLev1.ContentType.Alias.Equals("news"))
                                {
                                    var news = caLev1.To<NewsModel>();
                                    if (mappedCategoryModel.ChildrenNews.Any() && mappedCategoryModel.ChildrenNews[0].PostedDateTime.CompareTo(news.PostedDateTime) < 0)
                                    {
                                        mappedCategoryModel.ChildrenNews.Insert(0, news);
                                    }
                                    else
                                    {
                                        mappedCategoryModel.ChildrenNews.Add(news);
                                    }
                                }
                            }

                            mappedCategoryModel.ChildrenCategories.Add(mappedCategoryModel0);
                        }
                        else if (caLev0.ContentType.Alias.Equals("news"))
                        {
                            mappedCategoryModel.ChildrenNews.Add(caLev0.To<NewsModel>());
                        }
                    }
                }

                return mappedCategoryModel;
            }
            return null;
        }

        public IList<CategoryModel> GetAllCategoryModel()
        {
            IList<CategoryModel> categories = new List<CategoryModel>();

            var homeNode = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext.Content.GetById(1089);
            if (homeNode != null && homeNode.Children.Any())
            {
                foreach (var cat in homeNode.Children)
                {
                    categories.Add(GetCategoryModel(cat));
                }
            }

            return categories;
        }

        public IPublishedContent GetCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var home = CurrentPublishedContent.FirstChild();

                var category = home.Descendants().Where(x => x.IsVisible() && x.ContentType.Alias == "category");

                foreach (var ca in category)
                {
                    if (ca.Name.Equals(categoryName, StringComparison.CurrentCultureIgnoreCase) || ca.GetProperty("title").GetValue().Equals(categoryName))
                    {
                        return ca;
                    }
                }
            }

            return null;
        }
    }
}