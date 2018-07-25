using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using ZQNB.Common;
using ZQNB.Common.Extensions;
using ZQNB.Common.NHExtensions;

namespace NhibernateDemo.Relations.SingleRelationTrees
{
    public class SingleRelationOrgDemo
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
                    from c in session.Query<SingleRelationOrg>()
                    let pName =
                    (
                        from p in session.Query<SingleRelationOrg>()
                        ////where c.Parent != null && c.Parent.Id == p.Id
                        where c.Parent.Id == p.Id
                        select p.Name

                        //sql => 
                        //         where  (singlerela1_.Id is not null) 
                        //               and singlerela0_.ParentId = singlerela2_.Id) as col_4_0_,
                    ).SingleOrDefault()
                    //select new { OrgId = c.Id, OrgName = c.Name, ParentOrgId =  c.Parent.Id, ParentOrgName = pName }; //System.NullReferenceException: 未将对象引用设置到对象的实例。
                    select new { OrgId = c.Id, OrgName = c.Name, ParentOrgId = c.Parent == null ? (Guid?)null : c.Parent.Id, ParentOrgName = pName };

                var list = query.ToList();

                #region Sql
                
                //select singlerela0_.Id                                  as col_0_0_,
                //       singlerela0_.Name                                as col_1_0_,
                //       singlerela1_.Id                                  as col_2_0_,
                //       singlerela0_.ParentId                            as col_3_0_,
                //       (select singlerela2_.Name
                //        from   Lib_Relations_SingleRelationOrg singlerela2_
                //        where  singlerela0_.ParentId = singlerela2_.Id) as col_4_0_,
                //       singlerela1_.Id                                  as Id1_,
                //       singlerela1_.Name                                as Name1_,
                //       singlerela1_.SortNum                             as SortNum1_,
                //       singlerela1_.ParentId                            as ParentId1_
                //from   Lib_Relations_SingleRelationOrg singlerela0_
                //       left outer join Lib_Relations_SingleRelationOrg singlerela1_
                //         on singlerela0_.ParentId = singlerela1_.Id

                //select singlerela0_.Id                                      as col_0_0_,
                //       singlerela0_.Name                                    as col_1_0_,
                //       singlerela1_.Id                                      as col_2_0_,
                //       singlerela0_.ParentId                                as col_3_0_,
                //       (select singlerela2_.Name
                //        from   Lib_Relations_SingleRelationOrg singlerela2_
                //        where  (singlerela1_.Id is not null)
                //               and singlerela0_.ParentId = singlerela2_.Id) as col_4_0_,
                //       singlerela1_.Id                                      as Id1_,
                //       singlerela1_.Name                                    as Name1_,
                //       singlerela1_.SortNum                                 as SortNum1_,
                //       singlerela1_.ParentId                                as ParentId1_
                //from   Lib_Relations_SingleRelationOrg singlerela0_
                //       left outer join Lib_Relations_SingleRelationOrg singlerela1_
                //         on singlerela0_.ParentId = singlerela1_.Id

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
                var orgCount = session.Query<SingleRelationOrg>().Count();
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
            var org1 = new SingleRelationOrg() { Id = entityIds.Dequeue(), Name = "A", SortNum = 0, Parent = null };
            session.Save(org1, org1.Id);
            for (int i = 1; i <= 2; i++)
            {
                var org1Child = new SingleRelationOrg() { Id = entityIds.Dequeue(), Name = "A." + i.ToString("00"), SortNum = 0, Parent = org1 };
                session.Save(org1Child, org1Child.Id);
                for (int j = 1; j <= 3; j++)
                {
                    var org1ChildChild = new SingleRelationOrg() { Id = entityIds.Dequeue(), Name = org1Child.Name + "." + i.ToString("00"), SortNum = 0, Parent = org1Child };
                    session.Save(org1ChildChild, org1ChildChild.Id);
                }
            }
        }
    }
}
