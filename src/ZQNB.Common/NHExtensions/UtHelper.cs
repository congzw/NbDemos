using System;
using System.Collections.Generic;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using ZQNB.Common.Db;

namespace ZQNB.Common.NHExtensions
{
    public class DbSettingHelper
    {
        public static string ConnString = @"Data Source=.;Initial Catalog=NbV3.1DbTest;Persist Security Info=True;User ID=sa;Password=zqnb_123";
        public static bool ShowSql = false;
    }

    public class FluentNhibernateConvertions
    {
        public static IList<IConvention> Conventions = new List<IConvention>();
    }

    public interface IUtHelper
    {
        string GetConnStr();
        IPersistenceConfigurer GetConfigurer(string connStr);
        void ReCreateDbIfExist(string dbserver, string dbname, string dbuser, string dbpassword);
        void CreateDbIfNotExist(string dbserver, string dbname, string dbuser, string dbpassword);
        void SetUpNHibernate(bool rebulidSchema, params Type[] mapperTypes);
        ISession OpenSession();

        void ClearData();
        void InitData();
    }

    /// <summary>
    /// SqlServerDb
    /// </summary>
    public class SqlUtHelper : IUtHelper
    {
        public string GetConnStr()
        {
            return DbSettingHelper.ConnString;
        }

        public virtual IPersistenceConfigurer GetConfigurer(string connStr)
        {
            var dbCfg = MsSqlConfiguration.MsSql2005.ConnectionString(connStr);
            if (DbSettingHelper.ShowSql)
            {
                dbCfg.ShowSql();
            }
            return dbCfg;
        }
        
        public virtual void ReCreateDbIfExist(string dbserver, string dbname, string dbuser, string dbpassword)
        {
            SqlScriptHelper helper = new SqlScriptHelper();
            helper.ReCreateDbIfExist(dbserver, dbname, dbuser, dbpassword);
        }

        public virtual void CreateDbIfNotExist(string dbserver, string dbname, string dbuser, string dbpassword)
        {
            SqlScriptHelper helper = new SqlScriptHelper();
            helper.CreateDbIfNotExist(dbserver, dbname, dbuser, dbpassword);
        }

        public virtual void SetUpNHibernate(bool rebulidSchema, params Type[] mapperTypes)
        {
            //ISessionFactory sessionFactory = NbSessionFactoryProvider.CurrentNbSessionFactory("app").SessionFactory();
            ISessionFactory sessionFactory = NHibernateHelper.SessionFactory();
            if (sessionFactory == null)
            {
                string connStr = GetConnStr();
                var dbCfg = GetConfigurer(connStr);
                NHibernateHelper.InitSessionFactory(dbCfg,
                    m =>
                    {
                        var conventions = FluentNhibernateConvertions.Conventions;
                        if (conventions != null && conventions.Count > 0)
                        {
                            foreach (var hibernateMappingConvention in conventions)
                            {
                                var convention = hibernateMappingConvention;
                                m.FluentMappings.Conventions.Setup(x => x.Add(convention));
                            }
                        }
                        //解决同类名映射问题
                        m.FluentMappings.Conventions.Setup(x => x.Add(AutoImport.Never()));
                        foreach (var mapperType in mapperTypes)
                        {
                            m.FluentMappings.AddFromAssembly(mapperType.Assembly);
                        }
                    },
                    rebulidSchema, "", true, true, false);
            }
        }

        public virtual ISession OpenSession()
        {
            var session = NHibernateHelper.OpenSession();
            return session;
        }

        public virtual void ClearData()
        {
            //child class todo..
        }

        public virtual void InitData()
        {
            //child class todo..
        }

        private static IUtHelper _utHelper = new SqlUtHelper();
        public static IUtHelper Instance
        {
            get { return _utHelper; }
            set { _utHelper = value; }
        }
    }

    /// <summary>
    /// SqLiteDb
    /// </summary>
    public class SqLiteUtHelper : SqlUtHelper
    {
        public override void ReCreateDbIfExist(string dbserver, string dbname, string dbuser, string dbpassword)
        {
            throw new NotSupportedException("SqLiteUtHelper无须支持此方法！");
        }

        static SqLiteUtHelper()
        {
            DbSettingHelper.ConnString = @"Data Source=|DataDirectory|\SqLiteUtDb.sdf";
        }

        public override IPersistenceConfigurer GetConfigurer(string connStr)
        {
            var dbCfg = SQLiteConfiguration.Standard.InMemory().ConnectionString(connStr);
            return dbCfg;
        }

        private static IUtHelper _utHelper = new SqLiteUtHelper();
        public new static IUtHelper Instance
        {
            get { return _utHelper; }
            set { _utHelper = value; }
        }
    }
}
