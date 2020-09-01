using AffiliateMarketing.Data.Entities;
using Aio.Umbraco.Common.ContentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aio.Umbraco.Services.Interfaces
{
    public interface IOfferService : IBaseService
    {
        #region Category
        List<Category> GetCategories(Guid? parentId = null, params string[] excludeItems);
        Category GetRootCategory(Guid categoryId);
        Category GetCategory(Guid id);
        Category GetCategory(string slug, string parentSlug = null);
        Category GetRootCategory(string slug, string parentSlug = null);
        bool HasProduct(Guid id);
        #endregion

        #region Product
        List<Product> GetProducts(int pageIndex, int pageSize, out int totalPage, Guid? categoryId = null, string searchFor = null, 
            int? numberOfRandomCategories = null, int? expirationOfRandomCategories = null, 
            string sortBy = "Discount", FilterByInfor filterByInfor = null);
        Product GetProductBySlugName(string slug);
        List<Product> GetRelatedProducts(string slug, int pageSize);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);

        Tuple<decimal?, decimal?> GetDiscountRange();
        Tuple<decimal?, decimal?> GetFinalPriceRange();
        #endregion

        #region Website
        Website GetWebsite(Guid id);
        #endregion
    }
}
