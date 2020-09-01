using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aio.Umbraco.Common.ContentModels
{
    public class UserComment : BaseModel
    {
        public string Comment { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PageUrl { get; set; }
    }
}
