using Aio.Umbraco.Common.ContentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aio.Umbraco.Services.Interfaces
{
    public interface ISettingsService : IBaseService
    {
        Settings SiteSettings { get; }
    }
}
