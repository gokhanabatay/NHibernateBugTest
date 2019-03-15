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
    public class StaticFilterConditionTest
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
        public void Saves_Cycle_Success()
        {
            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        TxnCycle cycle = new TxnCycle()
                        {
                            Status = 1,
                            Description = "DESCRIPTION"
                        };

                        session.Save(cycle);
                    }
                    transaction.Commit();
                }
            }

            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {

                    Assert.That(session.Query<TxnCycle>().ToList().Count == 10);
                }
            }
        }

        [TestCase(1), Order(3)]
        public void UpdateDmlStyle_TxnCycle_Fail(short mbrId)
        {
            CurrentSession.MbrId = mbrId;
            //Sets Current Session
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    string[] descriptions = new string[] { "DESCRIPTION" };
                    

                    Assert.Throws<Exception>(() => session.Query<TxnCycle>()
                    .Where(x => descriptions.Contains(x.Description))
                    .Update(x => new TxnCycle() { Description = "DESCRIPTION_UPDATED" }));

                }
            }
        }
    }
}
