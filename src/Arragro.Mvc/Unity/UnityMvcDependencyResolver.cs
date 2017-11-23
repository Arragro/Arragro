using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Unity;

namespace Arragro.Mvc.Unity
{
    public class UnityMvcDependencyResolver : IDependencyResolver, IDisposable
    {
        protected IUnityContainer container;
        protected IDependencyResolver resolver;

        public UnityMvcDependencyResolver(IUnityContainer container, IDependencyResolver resolver)
        {
            this.container = container;
            this.resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return this.container.Resolve(serviceType);
            }
            catch
            {
                return this.resolver.GetService(serviceType);
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this.container.ResolveAll(serviceType);
            }
            catch
            {
                return this.resolver.GetServices(serviceType);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool dispose)
        {
            container.Dispose();
        }
    }
}
