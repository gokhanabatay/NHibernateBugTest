using NHibernate;
using NHibernate.Exceptions;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHibernateBugTest
{
    [TestFixture]
    public class FilterConditionTest
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
        public void Saves_MultiTenantUserSessions_Success()
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
                            UserCode = "gokhanabatay"
                        };

                        session.Save(userSession);

                        userSession = new UserSession()
                        {
                            Claims = "My Claims",
                            ExpireDateTime = DateTime.Now.AddDays(1),
                            MbrId = 2,
                            OpenDate = DateTime.Now,
                            LocalIpAddress = "192.168.1.1",
                            RemoteIpAddress = "127.0.0.1",
                            LocalPort = 80 + i.ToString(),
                            RemotePort = 80 + i.ToString(),
                            DeviceId = "None",
                            UserCode = "fredericDelaporte"
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

                    Assert.That(session.Query<UserSession>().ToList().Count == 20);
                }
            }
        }

        [TestCase(1), Order(2)]
        [TestCase(2)]
        public void Retrieve_MultiTenantUserSessions_Success(short mbrId)
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

                    Assert.That(session.Query<UserSession>().ToList().Count == 10);
                }
            }
        }

        [TestCase(1), Order(3)]
        public void InsertDmlStyle_Tenant1sUserSessions_Fail(short mbrId)
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
                    var query = session.CreateQuery(
               $@"INSERT INTO {typeof(UserSession)}(MbrId, UserCode, OpenDate, ExpireDateTime,
                RemoteIpAddress, RemotePort, LocalIpAddress, LocalPort, Claims)
                SELECT E.MbrId, E.UserCode, E.OpenDate, E.ExpireDateTime,
                E.RemoteIpAddress, E.RemotePort, E.LocalIpAddress, E.LocalPort, E.Claims from {typeof(UserSession)} AS E");

                    Assert.Throws<GenericADOException>(() => query.ExecuteUpdate());

                }
            }
        }

        [TestCase(1), Order(3)]
        public void NullableDateTimeCustomUserType_OrderBy_Success(short mbrId)
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
                    Assert.DoesNotThrow(() => session.Query<UserSession>()
                            .OrderBy(x => x.OpenDate).ToList());
                }
            }
        }

        [TestCase(1), Order(4)]
        public void NullableDateTimeCustomUserType_OrderBy_Fails(short mbrId)
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
                    Assert.Throws<GenericADOException>(() => session.Query<UserSession>()
                            .OrderBy(x => x.OpenDate == null ? null : x.OpenDate)
                            .ToList());
                }
            }
        }
    }
}
