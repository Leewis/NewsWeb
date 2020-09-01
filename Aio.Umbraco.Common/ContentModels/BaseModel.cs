using Aio.Umbraco.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Aio.Umbraco.Common.ContentModels
{
    public class BaseModel
    {
        public virtual int Id { get; set; }
        public virtual string DocumentType { get; set; }
        public virtual string Name { get; set; }

        public virtual string Path { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string Creator { get; set; }
        public virtual string Url { get; set; }
        public virtual IPublishedContent PublishedContent { get; set; }
        public virtual IEnumerable<IPublishedContent> Children { get; set; }
        public virtual IList<T> GetChildren<T>() where T : BaseModel, new()
        {
            var children = new List<T>();

            if (Children == null) return children;
            
            foreach(var c in Children)
            {
                children.Add(c.To<T>());
            }

            return children;
        }
        
        public int ParentId { get; set; }
    }
}
