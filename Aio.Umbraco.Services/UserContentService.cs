using Aio.Umbraco.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aio.Umbraco.Common.ContentModels;
using Umbraco.Core.Models;

namespace Aio.Umbraco.Services
{
    public class UserContentService : BaseService, IUserContentService
    {
        public Result<T> Create<T>(T model) where T : BaseModel
        {
            return SaveAndPublishData(() => {
                return UmbracoServices.ContentService.CreateContent(
                    model.Name, model.ParentId, model.DocumentType, 0);
            }, model);
        }

        public Result<T> Update<T>(T model) where T : BaseModel
        {
            return SaveAndPublishData(() => {
                return UmbracoServices.ContentService.GetById(model.Id);
            }, model);            
        }

        private Result<T> SaveAndPublishData<T>(Func<IContent> func, T model) where T : BaseModel
        {
            var result = new Result<T>();
            try
            {
                var content = func();
                PopulateContentFields(content, model);
                UmbracoServices.ContentService.SaveAndPublishWithStatus(content);
                result.Model = model;
            }
            catch (Exception error)
            {
                result.Error = error;
            }
            return result;
        }

        private void PopulateContentFields<T>(IContent content, T model) where T: BaseModel
        {
            content.Name = model.Name;
            var properties = typeof(T).GetProperties();
            foreach(var prop in content.ContentType.CompositionPropertyTypes)
            {
                var property = properties.FirstOrDefault(p => string.Equals(p.Name, prop.Alias, StringComparison.InvariantCultureIgnoreCase));
                if (property == null || !property.CanRead) continue;

                var value = property.GetValue(model);
                content.SetValue(prop.Alias, value);
            }
        }
    }
}
