using Aio.Umbraco.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Aio.Umbraco.Common.ContentModels
{
    public class Advertising : BaseModel
    {
        public string Link { get; set; }
        public string IconUrl { get; set; }
        public bool Priority { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Iframe { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public Advertising() : base()
        {
        }   
    }
}
