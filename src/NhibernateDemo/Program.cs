using System;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NhibernateDemo.Relations.SimpleTrees;
using ZQNB.Common.Db;
using ZQNB.Common.NHExtensions;
using ZQNB.Common.Serialize;

namespace NhibernateDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var utHelper = SqlUtHelper.Instance;
            InitDbScheme(utHelper, true);

            SimpleTreeDemo.Run(utHelper);

            ShowSplit("Demo Done");
            Console.Read();
        }

        static void InitDbScheme(IUtHelper utHelper, bool recreateDb = false)
        {
            JsonHelper.JsonSerialize = new NhJsonSerialize();

            NHibernateProfiler.Initialize();

            //FluentNhibernateConvertions.Conventions.Add(new Nb_ForeignKeyConstraintNameConvention());
            //DbSettingHelper.ShowSql = true;
            //DbSettingHelper.ConnString = @"Data Source=.;AttachDbFilename=|DataDirectory|\App_Data\DemoDb.mdf;Integrated Security=True;Connect Timeout=30";

            var dbName = "DemoDb";
            var connString = string.Format(@"Data Source=.;Initial Catalog={0};Persist Security Info=True;User ID=sa;Password=zqnb_123", dbName);
            DbSettingHelper.ConnString = connString;

            var sqlScriptHelper = new SqlScriptHelper();
            if (recreateDb)
            {
                sqlScriptHelper.ReCreateDbIfExist(connString, dbName);
            }
            else
            {
                sqlScriptHelper.CreateDbIfNotExist(connString, dbName);
            }

            utHelper.SetUpNHibernate(recreateDb, typeof(Program));
            ShowSplit("Setup Done!");
            Console.WriteLine();
        }
        static void ShowSplit(string title)
        {
            Console.WriteLine();
            Console.WriteLine("========== {0} ==========", title);
        }
    }
}
