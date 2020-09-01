using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aio.Umbraco.Common.ContentModels
{
    public class BasePage : BaseModel
    {
        public virtual string MetaKeywords { get; set; }
        public virtual string MetaDescription { get; set; }
        public virtual string MetaPageTitle { get; set; }
    }
}
