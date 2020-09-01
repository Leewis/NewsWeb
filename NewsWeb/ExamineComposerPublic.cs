using Aio.Umbraco.Services.Interfaces;
using Aio.Umbraco.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web.Search;

namespace NewsWeb
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ExamineComposerPublic : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<ExamineComponent>();
            composition.Register<ISearchHelper, SearchHelper>(Lifetime.Singleton);
            composition.Register<ISettingsService, SettingsService>(Lifetime.Singleton);
        }
    }
}