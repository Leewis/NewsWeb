using NewsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsWeb.Services
{
    public interface ISettingsService
    {
        Setting SiteSetting { get; }
    }
}