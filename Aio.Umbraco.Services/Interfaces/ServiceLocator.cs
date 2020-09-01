using Autofac;
using System;

namespace Aio.Umbraco.Services.Interfaces
{
    public class ServiceLocator : IDisposable
    {
        #region "Singleton Stuff"
        private static ServiceLocator serviceLocator;
        private static readonly object lockObj = new object();
        private IContainer container;

        private ServiceLocator()
        {

        }

        private static ServiceLocator Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (serviceLocator == null)
                        serviceLocator = new ServiceLocator();
                }
                return serviceLocator;
            }
        }
        #endregion

        #region "Get Methods"
        public static T Resolve<T>()
        {
            var instance = Instance.container.Resolve<T>();
            return instance == null ? default(T) : instance;
        }

        public static object Resolve(Type type)
        {
            return Instance.container.Resolve(type);
        }
        #endregion

        #region "Container"
        public static void SetContainer(IContainer container)
        {
            Instance.container = container;
        }
        #endregion

        #region "IDisposable"
        public void Dispose()
        {
            serviceLocator = null;
        }
        #endregion
    }
}
