using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aio.Umbraco.Common.ContentModels
{
    public class Result<T> where T : BaseModel
    {
        public T Model { get; set; }
        public bool HasError { get { return Error != null; } }
        public Exception Error { get; set; }
    }
    public class PagedResult<T> where T : class
    {
        public string Html { get; set; }
        public int TotalPages { get; set; }
    }
}
