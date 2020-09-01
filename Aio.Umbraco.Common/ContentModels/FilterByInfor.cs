using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aio.Umbraco.Common.ContentModels
{
    public class FilterByInfor
    {
        public decimal? FromPrice { get; set; }
        public decimal? FromDiscount { get; set; }
        public decimal? ToPrice { get; set; }
        public decimal? ToDiscount { get; set; }
    }
}
