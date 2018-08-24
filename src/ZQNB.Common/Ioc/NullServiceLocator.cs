using System;
using System.Collections.Generic;
using System.Linq;

namespace ZQNB.Common.Ioc
{
    /// <summary>
    /// 默认的空实现
    /// </summary>
    public class NullServiceLocator : IServiceLocator
    {
        /// <summary>
        /// 获取指定类型的服务对象。
        /// </summary>
        /// <returns>
        /// <paramref name="serviceType"/> 类型的服务对象。 - 或 - 如果没有 <paramref name="serviceType"/> 类型的服务对象，则为 null。
        /// </returns>
        /// <param name="serviceType">一个对象，它指定要获取的服务对象的类型。</param>
        public object GetService(Type serviceType)
        {
            return null;
        }

        /// <summary>
        /// Get an instance of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType)
        {
            return null;
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is an error resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public object GetInstance(Type serviceType, string key)
        {
            return null;
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Get an instance of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public TService GetInstance<TService>()
        {
            return default(TService);
        }

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="key">Name the object was registered with.</param>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public TService GetInstance<TService>(string key)
        {
            return default(TService);
        }

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/> currently
        /// registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">if there is are errors resolving
        /// the service instance.</exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService"/>.</returns>
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return Enumerable.Empty<TService>();
        }

        #region IServiceLocator 成员


        public bool IsRegistered(Type type)
        {
            return true;
        }

        #endregion
    }
}