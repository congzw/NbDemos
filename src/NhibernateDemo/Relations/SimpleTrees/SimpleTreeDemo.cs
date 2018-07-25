using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using ZQNB.Common;
using ZQNB.Common.Extensions;
using ZQNB.Common.NHExtensions;

namespace NhibernateDemo.Relations.SimpleTrees
{
    public class SimpleTreeDemo
    {
        public static void Run(IUtHelper utHelper)
        {
            FeedIfEmpty(utHelper);
            SubQuery_Org(utHelper);
            //结论： 
            //采用简单（或单向）建模、子查询模式可以很好的处理左连接的计数统计、树连接查询的场景
        }

        private static void SubQuery_Org(IUtHelper utHelper)
        {
            using (var session = utHelper.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var query =
                    from c in session.Query<SimpleOrg>()
                    let pName =
                    (
                        from p in session.Query<SimpleOrg>()
                        where c.ParentId == p.Id
                        select p.Name
                    ).SingleOrDefault()
                    select new { OrgId = c.Id, OrgName = c.Name, ParentOrgId = c.ParentId, ParentOrgName = pName };

                var list = query.ToList();

                #region Sql

                //select simpleorg0_.Id                                 as col_0_0_,
                //       simpleorg0_.Name                               as col_1_0_,
                //       simpleorg0_.ParentId                           as col_2_0_,
                //       (select simpleorg1_.Name
                //        from   Lib_Relations_SimpleOrg simpleorg1_
                //        where  simpleorg0_.ParentId = simpleorg1_.Id) as col_3_0_
                //from   Lib_Relations_SimpleOrg simpleorg0_

                #endregion

                Console.WriteLine(list.Count);
                Console.WriteLine(list.ToJson());

                #region Sql

                #endregion

                tx.Commit();
            }
        }

        private static void FeedIfEmpty(IUtHelper utHelper)
        {
            using (var session = utHelper.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                var orgCount = session.Query<SimpleOrg>().Count();
                if (orgCount == 0)
                {
                    Console.WriteLine("SEED!");
                    Seed(session);
                }
                tx.Commit();
            }
        }

        private static void Seed(ISession session)
        {
            var entityIds = GuidHelper.CreateMockGuidQueue(100);
            //Prepare Data
            var org1 = new SimpleOrg() { Id = entityIds.Dequeue(), Name = "A", SortNum = 0, ParentId = null };
            session.Save(org1, org1.Id);
            for (int i = 1; i <= 2; i++)
            {
                var org1Child = new SimpleOrg() { Id = entityIds.Dequeue(), Name = "A." + i.ToString("00"), SortNum = 0, ParentId = org1.Id };
                session.Save(org1Child, org1Child.Id);
                for (int j = 1; j <= 3; j++)
                {
                    var org1ChildChild = new SimpleOrg() { Id = entityIds.Dequeue(), Name = org1Child.Name + "." + i.ToString("00"), SortNum = 0, ParentId = org1Child.Id };
                    session.Save(org1ChildChild, org1ChildChild.Id);
                }
            }
        }
    }
}
