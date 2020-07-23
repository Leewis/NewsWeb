using NewsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

namespace NewsWeb.Services
{
    public class SettingsService
    {
        Settings siteSettings;

        public Settings SiteSettings
        {
            get
            {
                if (siteSettings == null)
                {
                   
                }
                return siteSettings;
            }
        }
    }
}