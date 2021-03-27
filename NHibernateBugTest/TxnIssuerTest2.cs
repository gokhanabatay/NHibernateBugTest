using NHibernate;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHibernateBugTest
{
    [TestFixture]
    public class TxnIssuerTest2
    {
        [SetUp]
        public void SetUpBase()
        {
            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //just init connection and ISessionFactory
                    transaction.Commit();
                }
            }
        }

        [Test, Order(1)]
        public void Saves_TxnIssuer_Success()
        {
            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        TxnIssuer txnIssuer = new TxnIssuer()
                        {
                            CycleMoved = "N",
                            MrcDailyMoved = "N"
                        };

                        session.Save(txnIssuer);
                    }
                    transaction.Commit();
                }
            }

            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var count = session.Query<TxnIssuer>().ToList().Count;

                    Assert.That( count > 0);
                }
            }
        }

        [Test, Order(2)]
        public void UpdateRetrieve_TxnIssuer_Success()
        {

            int totalTask = 100;
            List<long> listOfGuids;

            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    listOfGuids = session.Query<TxnIssuer>().Select(x => x.Guid).ToList();
                }
            }

            var abc = GroupBy(listOfGuids, 5);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < totalTask; i++)
            {
                List<long> guid = abc[i];
                int taskId = i + 1;
                Task task = Task.Run(() => UpdateIssuerTxnAsync(taskId, guid));
                tasks.Add(task);

            }

            Task.WaitAll(tasks.ToArray());

            async Task UpdateIssuerTxnAsync(int taskNumber, List<long> guids)
            {
                using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        //Below query updates MrcDailyMoved and MrcDailyMovedDate with success
                       var rowcount= session.Query<TxnIssuer>()
                                   .Where(x => guids.Contains(x.Guid))
                                   .Update(x => new TxnIssuer
                                   {
                                       MrcDailyMoved = "Y",
                                       MrcDailyMovedDate = DateTime.Now
                                   });

                        transaction.Commit();
                    }
                }
            }
        }

        public static List<T>[] GroupBy<T>(IEnumerable<T> items, int itemCount)
        {
            List<List<T>> pagedItems = new List<List<T>>();
            List<T> pageItem = new List<T>();
            foreach (var item in items)
            {
                if (pageItem.Count == itemCount)
                {
                    pagedItems.Add(pageItem);
                    pageItem = new List<T>();
                }

                pageItem.Add(item);
            }
            if (pageItem.Count > 0)
            {
                pagedItems.Add(pageItem);
            }

            return pagedItems.ToArray();
        }
    }
}
