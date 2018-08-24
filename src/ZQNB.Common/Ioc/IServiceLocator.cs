using System;
using System.Collections.Generic;

namespace ZQNB.Common.Ioc
{
    /// <summary>
    /// 程序通用的Service Locator
    /// 参看注释中的使用例子
    /// </summary>
    public interface IServiceLocator : IServiceProvider
    {
        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType);

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        object GetInstance(Type serviceType, string key);

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        IEnumerable<object> GetAllInstances(Type serviceType);

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        TService GetInstance<TService>();

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        TService GetInstance<TService>(string key);

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/> currently
        /// registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService"/>.</returns>
        IEnumerable<TService> GetAllInstances<TService>();

        bool IsRegistered(Type type);
    }

    #region 使用例子

    //集中使用一个ServiceLocator，然后网站的各个部分，对ServiceLocator依赖
    //这样的好处是应用的Ioc跟具体使用的依赖注入工具无关，后期可以无缝替换(只需要在CoreServiceConfig更改IServiceLocator的适配实现即可)
    //public static class CoreServiceConfig
    //{
    //    public static void SetupCoreDependencyResolver()
    //    {
    //        var kernel = new StandardKernel();
    //        var ninjectServiceLocator = new NinjectServiceLocator(kernel);
    //        kernel.Bind<IServiceLocator>().To<NinjectServiceLocator>().InSingletonScope(); //adapter of castle or unity or ...

    //        //kernal
    //        //ninject service locator
    //        //core service provider
    //        Singleton<IKernel>.Instance = kernel;
    //        Singleton<IServiceLocator>.Instance = ninjectServiceLocator;
    //        CoreServiceProvider.Current = ninjectServiceLocator;
    //    }
    //}

    //mvc dependency resolver
    //public class NbDependencyResolver : System.Web.Mvc.IDependencyResolver
    //{
    //    private readonly IServiceLocator _serviceLocator = Singleton<IServiceLocator>.Instance;

    //    public object GetService(Type serviceType)
    //    {
    //        return _serviceLocator.GetInstance(serviceType);
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType)
    //    {
    //        return _serviceLocator.GetAllInstances(serviceType);
    //    }
    //}

    //webapi dependency resolver
    //public class NbDependencyResolverForWebApi : IDependencyResolver
    //{
    //    private readonly IServiceLocator _serviceLocator = Singleton<IServiceLocator>.Instance;

    //    public object GetService(Type serviceType)
    //    {
    //        return _serviceLocator.GetInstance(serviceType);
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType)
    //    {
    //        return _serviceLocator.GetAllInstances(serviceType);
    //    }

    //    public IDependencyScope BeginScope()
    //    {
    //        return this;
    //    }

    //    public void Dispose()
    //    {
    //        // noop
    //    }
    //}

    //signalr dependency resolver
    //public class SignalRNinjectDependencyResolver : Microsoft.AspNet.SignalR.DefaultDependencyResolver
    //{

    //    private readonly IServiceLocator _serviceLocator = Singleton<IServiceLocator>.Instance;

    //    public override object GetService(Type serviceType)
    //    {

    //        return _serviceLocator.GetInstance(serviceType) ?? base.GetService(serviceType);
    //    }

    //    public override IEnumerable<object> GetServices(Type serviceType)
    //    {
    //        return _serviceLocator.GetAllInstances(serviceType).Concat(base.GetServices(serviceType));
    //    }
    //}

    #endregion

}
