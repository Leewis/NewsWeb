using NewsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Examine;
using Examine;
using Umbraco.Web;
using Examine.Search;
using Umbraco.Web.PublishedCache;
using Umbraco.Core.Mapping;
using System.Text.RegularExpressions;

namespace NewsWeb.Services
{
    public interface ISearchHelper
    {
        NewsSearchModel Search(NewsSearchModel newsSearch);
    }
    public class SearchHelper : ISearchHelper
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IExamineManager _examineManager;

        public SearchHelper(IUmbracoContextFactory umbracoContextFactory, IExamineManager examineManager)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _examineManager = examineManager;
        }

        public NewsSearchModel Search(NewsSearchModel newsSearch)
        {
            IExamineManager examineManager = ExamineManager.Instance;

            if (!examineManager.TryGetIndex(Constants.UmbracoIndexes.ExternalIndexName, out IIndex index))
                throw new InvalidOperationException($"No index found by name {Constants.UmbracoIndexes.ExternalIndexName}");

            var searcher = index.GetSearcher();
            var criteria = searcher.CreateQuery(IndexTypes.Content, BooleanOperation.And);
            var examineQuery = criteria.NodeTypeAlias("news");
            var filter = new[] { "title", "shortDescription", "fullDescription", "source", "sourceUrl", "author" };


            if (!string.IsNullOrWhiteSpace(newsSearch.Tag))
            {
                var categoryQuery = searcher.CreateQuery("content").NodeTypeAlias("category").And().Field("nodeName", newsSearch.Tag).Execute();
                if (categoryQuery.Any())
                {
                    using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
                    {
                        foreach (var typeCategory in categoryQuery)
                        {
                            if (int.TryParse(typeCategory.Id, out int catId))
                            {
                                IPublishedContentCache contentHelper = umbracoContextReference.UmbracoContext.Content;
                                var category = contentHelper.GetById(catId);
                                examineQuery.And().Field("path", category.Path.ToString().MultipleCharacterWildcard());
                            }

                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(newsSearch.Keywords))
            {
                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = newsSearch.Keywords.Normalize(System.Text.NormalizationForm.FormD);
                var tempSearch = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                examineQuery.And().GroupedOr(filter, tempSearch);
            }



            int pageIndex = newsSearch.CurrentPage - 1;
            int pageSize = newsSearch.ItemsPerPage;
            ISearchResults searchResult = examineQuery.Execute(maxResults: pageSize * (pageIndex + 1));
            IEnumerable<ISearchResult> pagedResults = searchResult.Skip(pageIndex * pageSize);
            int totalResults = Convert.ToInt32(searchResult.TotalItemCount);
            newsSearch.TotalItems = totalResults;
            newsSearch.TotalPages = (totalResults + newsSearch.ItemsPerPage - 1) / newsSearch.ItemsPerPage;

            if (pagedResults != null && pagedResults.Count() > 0)
                newsSearch.News = GetNewsFromSearch(pagedResults);

            return newsSearch;
        }


        private List<NewsModel> GetNewsFromSearch(IEnumerable<ISearchResult> pagedResults)
        {
            List<NewsModel> newsDetailsPages = new List<NewsModel>();
            using (UmbracoContextReference umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
            {
                foreach (ISearchResult result in pagedResults)
                {
                    if (int.TryParse(result.Id, out int nodeId))
                    {
                        IPublishedContentCache contentHelper = umbracoContextReference.UmbracoContext.Content;
                        var newsDetailsPage = contentHelper.GetById(nodeId);
                        UmbracoMapper mapper = new UmbracoMapper(new MapDefinitionCollection(new List<IMapDefinition>()));

                        INewsService service = new NewsService(mapper);

                        var news = service.GetNewsModel(newsDetailsPage);

                        if ( newsDetailsPage != null)
                        {
                            newsDetailsPages.Add(news);
                        }
                    }
                }
            }

            return newsDetailsPages;
        }
    }
}
