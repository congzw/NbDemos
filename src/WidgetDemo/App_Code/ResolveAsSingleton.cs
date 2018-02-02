using System;
using System.Threading;

namespace WidgetDemo
{
    public interface IResolveAsSingleton
    {
        TInterface Resolve<T, TInterface>() where T : TInterface, new();
    }
    public class ResolveAsSingleton : IResolveAsSingleton
    {
        #region for di extensions

        private static readonly Lazy<IResolveAsSingleton> Lazy = new Lazy<IResolveAsSingleton>(() => new ResolveAsSingleton());
        private static Func<IResolveAsSingleton> _resolve = () => Lazy.Value;
        public static Func<IResolveAsSingleton> Factory
        {
            get { return _resolve; }
            set { _resolve = value; }
        }

        #endregion

        //for simple use
        public static TInterface Resolve<T, TInterface>() where T : TInterface, new()
        {
            return Factory().Resolve<T, TInterface>();
        }

        TInterface IResolveAsSingleton.Resolve<T, TInterface>()
        {
            return AutoResolveAsSingletonHelper<T, TInterface>.Lazy.Value;
        }

        #region inner helper

        #endregion
    }
    internal sealed class AutoResolveAsSingletonHelper<T, TInterface> where T : TInterface, new()
    {
        public static Lazy<T> Lazy = new Lazy<T>(LazyThreadSafetyMode.ExecutionAndPublication);
    }
}
