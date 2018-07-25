using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Testing;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Configuration = NHibernate.Cfg.Configuration;

namespace ZQNB.Common.NHExtensions
{
    public static class NHibernateHelper
    {
        /// <summary>
        /// 配置Nh
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="rebulidSchema"></param>
        /// <param name="mapperAssemblies">要从中查找的FluentNH的类映射的程序集</param>
        /// <param name="sessionFactoryKey">SessionFactory名，标识不同的SessionFactory, 默认名为Empty</param>
        /// <param name="conventions"></param>
        public static void InitSessionFactory(string connName, bool rebulidSchema, IList<Assembly> mapperAssemblies, string sessionFactoryKey = "", params IHibernateMappingConvention[] conventions)
        {
            if (string.IsNullOrWhiteSpace(connName))
            {
                throw new ArgumentException("必须指定connName");
            }

            string connStr = ConfigurationManager.ConnectionStrings[connName].ConnectionString;
            var dbConfig = MsSqlConfiguration.MsSql2005.ConnectionString(connStr);

            Action<MappingConfiguration> mappings = m =>
            {
                if (conventions != null && conventions.Length > 0)
                {
                    foreach (var hibernateMappingConvention in conventions)
                    {
                        var convention = hibernateMappingConvention;
                        m.FluentMappings.Conventions.Setup(x => x.Add(convention));
                    }
                }
                //解决同类名映射问题
                m.FluentMappings.Conventions.Setup(x => x.Add(AutoImport.Never()));
                if (mapperAssemblies != null && mapperAssemblies.Count > 0)
                {
                    //需要在站架子统一注册的mapper
                    foreach (var mapperAssembly in mapperAssemblies)
                    {
                        m.FluentMappings.AddFromAssembly(mapperAssembly);
                    }
                }
            };

            InitSessionFactory(dbConfig, mappings, rebulidSchema, sessionFactoryKey);
        }

        /// <summary>
        /// 全局统一SessionFactory初始化，一般在Application_Start方法中调用。
        /// 一次调用初始化一个SessionFactory
        /// </summary>
        /// <param name="dbCfg">NH数据库配置</param>
        /// <param name="mappings">NH映射配置</param>
        /// <param name="buildSchema">是否自动创建或重新创建数据库表，默认不创建</param>
        /// <param name="sessionFactoryKey">SessionFactory名，标识不同的SessionFactory, 默认名为Empty</param>
        /// <param name="script"></param>
        /// <param name="export"></param>
        /// <param name="justDrop"></param>
        public static void InitSessionFactory(IPersistenceConfigurer dbCfg,
            Action<MappingConfiguration> mappings,
            bool buildSchema = false,
            string sessionFactoryKey = "", bool script = true, bool export = true, bool justDrop = false)
        {
            if (_sessionFactories.ContainsKey(sessionFactoryKey))
                throw new ArgumentException("Duplicate Session Factory name");

            var cfg = Fluently.Configure()
                .Database(dbCfg)
                .Mappings(mappings)
                .BuildConfiguration();

            if (buildSchema)
                BuildSchema(cfg, script, export, justDrop);

            _sessionFactories[sessionFactoryKey] = cfg.BuildSessionFactory();
        }

        /// <summary>
        /// 通过初始化时使用的SessionFactory名，用相应的SessionFactory来创建NH的ISession，使用者负责ISession的释放
        /// </summary>
        /// <param name="sessionFactoryKey">初始化SessionFactory时用的Key, 默认为空串</param>
        /// <returns>NH的ISession</returns>
        public static ISession OpenSession(string sessionFactoryKey = "")
        {
            return _sessionFactories[sessionFactoryKey].OpenSession();
        }

        /// <summary>
        /// 用key获取SessionFactory
        /// </summary>
        /// <param name="sessionFactoryKey">初始化SessionFactory时用的Key, 默认为空串</param>
        /// <returns></returns>
        public static ISessionFactory SessionFactory(string sessionFactoryKey = "")
        {
            if (_sessionFactories.ContainsKey(sessionFactoryKey))
            {
                return _sessionFactories[sessionFactoryKey];
            }
            return null;
        }

        #region helpers

        private static void BuildSchema(Configuration configuration, bool script = true, bool export = true, bool justDrop = false)
        {
            new SchemaExport(configuration).Execute(script, export, justDrop);
        }

        private static readonly Dictionary<string, ISessionFactory> _sessionFactories = new Dictionary<string, ISessionFactory>();

        #endregion
    }
}