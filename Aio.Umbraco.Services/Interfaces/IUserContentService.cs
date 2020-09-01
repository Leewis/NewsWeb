using Aio.Umbraco.Common.ContentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aio.Umbraco.Services.Interfaces
{
    public interface IUserContentService : IBaseService
    {
        Result<T> Create<T>(T model) where T : BaseModel;
        Result<T> Update<T>(T model) where T : BaseModel;
    }
}
