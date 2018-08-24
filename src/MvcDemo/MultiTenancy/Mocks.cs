using System;
using System.Collections.Generic;
using ZQNB.Common.Ioc;

namespace MvcDemo.MultiTenancy
{
    public class HostnameTenantService : IHostnameTenantService
    {
        IList<ITenant> ITenantService.GetAllTenants()
        {
            return (IList<ITenant>)getAllTenants();
        }

        IList<HostnameTenant> IHostnameTenantService.GetAllTenants()
        {
            return getAllTenants();
        }

        private IList<HostnameTenant> getAllTenants()
        {
            var mocks = new List<HostnameTenant>();
            mocks.Add(new HostnameTenant() { Alias = "A1", Hostnames = new List<string>() { "http://localhost:27495" } });
            mocks.Add(new HostnameTenant() { Alias = "A2", Hostnames = new List<string>() { "http://localhost:27496", "http://www.hao123.com" } });
            mocks.Add(new HostnameTenant() { Alias = "A3", Hostnames = new List<string>() { "http://localhost:10067" } });
            return mocks;
        }
    }
    
    internal class MockServiceLocator : IServiceLocator
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ITenantContextService))
            {
                return new TenantContextService(new HostnameTenantService());
            }
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType, string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public TService GetInstance<TService>()
        {
            var serviceType = typeof(TService);
            return (TService)GetService(serviceType);
        }

        public TService GetInstance<TService>(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Type type)
        {
            throw new NotImplementedException();
        }
    }

    internal class MockHelper
    {
        public static void SetupIoc()
        {
            CoreServiceProvider.CurrentFunc = () => new MockServiceLocator();
        }
    }
}
