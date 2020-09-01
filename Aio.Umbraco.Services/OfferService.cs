using Aio.Umbraco.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AffiliateMarketing.Data.Entities;
using AffiliateMarketing.Data.Common;
using System.Web;
using System.Linq.Dynamic;
using Aio.Umbraco.Common.ContentModels;

namespace Aio.Umbraco.Services
{
    public class OfferService : IOfferService
    {
        #region "Consts"
        const string RndCatIds = "RndCatIds1";
        #endregion

        #region Category
        public List<Category> GetCategories(Guid? parentId = null, params string[] excludeItems)
        {
            var query = DataUtils.CreateRepository<Category>().GetAll();
            var prodRepo = DataUtils.CreateRepository<Product>();
            if (excludeItems != null && excludeItems.Length > 0)
            {
                var pred = new StringBuilder();
                var AND = " AND ";
                for (var index = 0; index < excludeItems.Length; index++)
                {
                    pred.AppendFormat("FriendlyUrlName != @{0}{1}", index, AND);
                }

                pred.Length -= AND.Length;

                query = query.Where(pred.ToString(), excludeItems);
            }
            var rootCategories = query.Where(c => c.ParentCategoryId == parentId).OrderBy("Column").ToList();
            return rootCategories;
        }

        public Category GetRootCategory(Guid categoryId)
        {
            var query = DataUtils.CreateRepository<Category>();
            var category = query.GetById(categoryId);
            while (category != null && category.ParentCategoryId != null)
            {
                category = query.GetById(category.ParentCategoryId);
            }
            return category;
        }

        public Category GetCategory(Guid id)
        {
            var query = DataUtils.CreateRepository<Category>();
            var category = query.GetById(id);
            return category;
        }

        public Category GetCategory(string slug, string parentSlug = null)
        {
            var query = DataUtils.CreateRepository<Category>();
            var category = query.GetAll().Where(c => c.FriendlyUrlName == slug);
            if (!string.IsNullOrEmpty(parentSlug))
            {
                var parentIds = category.Select(c => c.ParentCategoryId);
                var parentCategory = query.GetAll().Where(c => parentIds.Contains(c.Id) && c.FriendlyUrlName == parentSlug);
                if (parentCategory.Any())
                {
                    return category.FirstOrDefault(c => c.ParentCategoryId == parentCategory.FirstOrDefault().Id);
                }
            }
            return category.FirstOrDefault();
        }

        public Category GetRootCategory(string slug, string parentSlug = null)
        {
            var query = DataUtils.CreateRepository<Category>();
            var category = query.GetAll().Where(c => c.FriendlyUrlName == slug);
            if (!string.IsNullOrEmpty(parentSlug))
            {
                var parentIds = category.Select(c => c.ParentCategoryId);
                var parentCategory = query.GetAll().Where(c => parentIds.Contains(c.Id) && c.FriendlyUrlName == parentSlug);
                if (parentCategory.Any())
                {
                    return category.FirstOrDefault(c => c.ParentCategoryId == parentCategory.FirstOrDefault().Id);
                }
            }
            return category.FirstOrDefault();
        }

        public bool HasProduct(Guid id)
        {
            var repo = DataUtils.CreateRepository<Product>();
            return repo.GetAll().Where(p => p.CategoryIdPath.Contains(id.ToString())).Count() > 0;
        }
        #endregion

        #region Product
        public Product GetProductBySlugName(string slugName)
        {
            var repo = DataUtils.CreateRepository<Product>();
            return repo.GetAll().FirstOrDefault(p => p.FriendlyUrlName == slugName);
        }

        public List<Product> GetProducts(int pageIndex, int pageSize, out int totalPage, Guid? categoryId = null, string searchFor = null, 
            int? numberOfRandomCategories = null, int? expirationOfRandomCategories = null, 
            string sortBy = "Discount", FilterByInfor filterByInfor = null)
        {
            var repo = DataUtils.CreateRepository<Product>();
            var catRepo = DataUtils.CreateRepository<Category>();
            var query = repo.GetAll();
            if (categoryId != null)
            {
                var category = catRepo.GetById(categoryId.Value);
                if(category != null)
                {
                    var id = category.Id.ToString();
                    query = query.Where(p => p.CategoryIdPath.Contains(id));
                }
            }
            if (!string.IsNullOrEmpty(searchFor))
            {
                var slug = DataUtils.GenerateFriendlyUrl(searchFor);
                query = query.Where(p => p.Name.Contains(searchFor) || p.FriendlyUrlName.Contains(slug));
            }

            if(filterByInfor != null)
            {
                if(filterByInfor.FromPrice != null && filterByInfor.ToPrice != null)
                {
                    query = query.Where("FinalPrice >= @0 AND FinalPrice <= @1", filterByInfor.FromPrice, filterByInfor.ToPrice);
                }

                if (filterByInfor.FromDiscount != null && filterByInfor.ToDiscount != null)
                {
                    query = query.Where("Discount >= @0 AND Discount <= @1", filterByInfor.FromDiscount, filterByInfor.ToDiscount);
                }
            }

            if(numberOfRandomCategories != null && numberOfRandomCategories.Value > 0)
            {
                var rndCatIdsCookie = HttpContext.Current.Request.Cookies[RndCatIds];
                var catIdsArr = rndCatIdsCookie != null && !string.IsNullOrEmpty(rndCatIdsCookie.Value) ? 
                    rndCatIdsCookie.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : null;

                List<Guid> rndCatIds = catIdsArr != null ? catIdsArr.Select(i => new Guid(i)).ToList() : new List<Guid>();
                if(rndCatIds == null || rndCatIds.Count == 0)
                {
                    var rnd = new Random();
                    rndCatIds = GetCategories(null, "khuyen-mai").OrderBy(x => rnd.Next()).Take(numberOfRandomCategories.Value).Select(c => c.Id).ToList();
                    rndCatIdsCookie = new HttpCookie(RndCatIds);
                    var value = String.Join("|", rndCatIds.Select(g => g.ToString()));
                    rndCatIdsCookie.Value = value;
                    rndCatIdsCookie.Expires = DateTime.Now.AddMinutes(expirationOfRandomCategories != null ? expirationOfRandomCategories.Value : 30);
                    HttpContext.Current.Response.Cookies.Add(rndCatIdsCookie);
                }

                StringBuilder queryBuilder = new StringBuilder();
                var or = " OR ";
                for(var index = 0; index < rndCatIds.Count; index++)
                {
                    queryBuilder.AppendFormat("CategoryIdPath.Contains (@{0}){1}", index, or);
                }
                if(queryBuilder.Length > 0)
                {
                    queryBuilder.Length = queryBuilder.Length - or.Length;
                    var idArr = rndCatIds.Select(id => id.ToString()).ToArray();
                    query = query.Where(queryBuilder.ToString(), idArr);
                }
                
            }

            var count = query.Count();
            totalPage = count / pageSize;
            if (count % pageSize > 0) totalPage++;

            var orderBy = sortBy + " descending";
            query = query.OrderBy(orderBy);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public List<Product> GetRelatedProducts(string slug, int pageSize)
        {
            var repo = DataUtils.CreateRepository<Product>();
            var product = repo.FirstOrDefault(p => p.FriendlyUrlName == slug);
            if (product == null) return new List<Product>();

            var relatedProducts = repo.GetAll()
                .Where(p => p.CategoryId == product.CategoryId && p.FriendlyUrlName != slug)
                .Take(pageSize).ToList();
            return relatedProducts;
        }

        public void UpdateProduct(Product product)
        {
            var repo = DataUtils.CreateRepository<Product>();
            repo.Update(product);
        }
        
        public void DeleteProduct(Product product)
        {
            var repo = DataUtils.CreateRepository<Product>();
            repo.Delete(product);
        }

        public Tuple<decimal?, decimal?> GetDiscountRange()
        {
            var repo = DataUtils.CreateRepository<Product>();
            var from = repo.GetAll().OrderBy(p => p.Discount).FirstOrDefault();
            decimal? fromValue = from != null ? from.Discount : 0;

            var to = repo.GetAll().OrderByDescending(p => p.Discount).FirstOrDefault();
            decimal? toValue = to != null ? to.Discount : 0;
            return new Tuple<decimal?, decimal?>(fromValue, toValue);
        }

        public Tuple<decimal?, decimal?> GetFinalPriceRange()
        {
            var repo = DataUtils.CreateRepository<Product>();
            var from = repo.GetAll().OrderBy(p => p.FinalPrice).FirstOrDefault();
            decimal? fromValue = from != null ? from.FinalPrice : 0;

            var to = repo.GetAll().OrderByDescending(p => p.FinalPrice).FirstOrDefault();
            decimal? toValue = to != null ? to.FinalPrice : 0;
            return new Tuple<decimal?, decimal?>(fromValue, toValue);
        }
        #endregion

        #region Others
        public Website GetWebsite(Guid id)
        {
            var repo = DataUtils.CreateRepository<Website>();
            return repo.GetById(id);
        }
        #endregion
    }
}
