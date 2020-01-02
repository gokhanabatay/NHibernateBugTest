using NHibernate;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHibernateBugTest
{
    [TestFixture]
    public class TxnIssuerTest
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
                    for (int i = 0; i < 10; i++)
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

                    Assert.That( count == 10);
                }
            }
        }

        [Test, Order(2)]
        public void UpdateRetrieve_TxnIssuer_Fails()
        {
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var listOfGuids = session.Query<TxnIssuer>().Select(x => x.Guid).ToList();

                    //Below query updates MrcDailyMoved and MrcDailyMovedDate with success
                    session.Query<TxnIssuer>()
                               .Where(x => listOfGuids.Contains(x.Guid))
                               .Update(x => new TxnIssuer
                               {
                                   MrcDailyMoved = "Y",
                                   MrcDailyMovedDate = DateTime.Now
                               });

                    Assert.That(session.Query<TxnIssuer>()
                        .Where(x => x.MrcDailyMoved == "Y")
                        .ToList().Count == 10);

                    //This Update Statement Creates wrong Sql, I think expression cache does not include Update fields to statement so above and below linq creates or use the same statement.
                    //Below query updates MrcDailyMoved and MrcDailyMovedDate instead of CycleMoved,CycleMovedDate
                    session.Query<TxnIssuer>()
                              .Where(x => listOfGuids.Contains(x.Guid))
                              .Update(x => new TxnIssuer
                              {
                                  CycleMoved = "Y",
                                  CycleMovedDate = DateTime.Now
                              });

                    Assert.That(session.Query<TxnIssuer>()
                        .Where(x => x.CycleMoved == "Y")
                        .ToList().Count == 10);
                }
            }
        }
    }
}
