using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models.PublishedContent;

namespace NewsWeb.Models
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

            foreach (var c in Children)
            {
                //children.Add(c.To<T>());
                children.Add((T)c);
            }

            return children;
        }

        public int ParentId { get; set; }
    }
}