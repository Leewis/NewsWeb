using Aio.Umbraco.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using umbraco.NodeFactory;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Mapping;

namespace Aio.Umbraco.Services
{
    public abstract class BaseService : IBaseService
    {
        protected virtual IPublishedContent CurrentPublishedContent
        {
            get
            {
                var currentNode = DependencyResolver.Current.GetService<IUmbracoContextFactory>().EnsureUmbracoContext().UmbracoContext.Content.GetById(1088); //Root Content Node

                return currentNode;
            }
        }
    }
}
