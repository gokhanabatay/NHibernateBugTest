using NHibernate;
using NHibernate.Linq;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHibernateBugTest
{
    [TestFixture]
    public class CustomTypeSelectFailsTest
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
        public void Saves_UserSession_Success()
        {
            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        UserSession userSession = new UserSession()
                        {
                            Claims = "My Claims",
                            ExpireDateTime = DateTime.Now.AddDays(1),
                            MbrId = 1,
                            OpenDate = DateTime.Now,
                            LocalIpAddress = "192.168.1.1",
                            RemoteIpAddress = "127.0.0.1",
                            LocalPort = 80 + i.ToString(),
                            RemotePort = 80 + i.ToString(),
                            DeviceId = "None",
                            UserCode = "gokhanabatay",
                            IsOpen = true
                        };

                        session.Save(userSession);
                    }
                    transaction.Commit();
                }
            }

            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var count = session.Query<UserSession>().Select(x=>x.Guid).ToList().Count;

                    Assert.That( count == 10);
                }
            }
        }

        [Test, Order(2)]
        public void Get_UserSession_FailsWithDateCustomType()
        {
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Assert.That(session.Query<UserSession>()
                        .Where(x => x.OpenDate == DateTime.Now)
                        .ToList().Count == 10);
                }
            }
        }

        [Test, Order(3)]
        public void Get_UserSession_FailsWithBooleanCustomType()
        {
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Assert.That(session.Query<UserSession>()
                        .Where(x => x.OpenDate == DateTime.Now)
                        .Select(x=>x.IsOpen)
                        .ToList().Count == 10);
                }
            }
        }
    }
}
