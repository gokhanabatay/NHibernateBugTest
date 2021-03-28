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
    public class TxnIssuerDmlStyleUpdateTest
    {
        [SetUp]
        public void SetUpBase()
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
                            MrcDailyMoved = "N",
                            MbrId = Session.CurrentSession.MbrId
                        };

                        session.Save(txnIssuer);
                    }
                    transaction.Commit();
                }
            }
        }

        //Try rerun sometimes gets succeeded
        [Test]
        public void MultiThread_FilteredEntity_DmlStyleUpdateFails()
        {

            int totalTask = 200;
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

            var pagedGuids = GroupBy(listOfGuids, 1);
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < totalTask; i++)
            {
                List<long> guids = pagedGuids[i];
                Task task = Task.Run(() => UpdateIssuerTxnAsync(guids));
                tasks.Add(task);

            }

            Task.WaitAll(tasks.ToArray());

            async Task UpdateIssuerTxnAsync(List<long> guids)
            {
                using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                       await session.Query<TxnIssuer>()
                                   .Where(x => guids.Contains(x.Guid))
                                   .UpdateAsync(x => new TxnIssuer
                                   {
                                       MrcDailyMoved = "Y",
                                       MrcDailyMovedDate = DateTime.Now
                                   }).ConfigureAwait(false);

                        await transaction.CommitAsync().ConfigureAwait(false);
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
