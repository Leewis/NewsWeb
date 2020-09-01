using Aio.Umbraco.Common.ContentModels;
using System;
using System.Collections.Generic;
//using Umbraco.Core.Mapping;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Zone.UmbracoMapper.V8;
//using Zone.UmbracoMapper;

namespace Aio.Umbraco.Common.Extensions
{
    public static class PublishedContentExtensions
    {
        public static T To<T>(this IPublishedContent content) where T: BaseModel, new()
        {
            return content.To(typeof(T)) as T;
        }

        public static BaseModel To(this IPublishedContent content, Type targetType)
        {
            if (content == null)
                return null;

            var model = Activator.CreateInstance(targetType) as BaseModel;
            if (model == null) return null;

            UmbracoMapper mapper = new UmbracoMapper();         

            mapper.Map(content, model);

            model.PublishedContent = content;
            model.Children = content.Children;

            return model;
        }
    }
}
