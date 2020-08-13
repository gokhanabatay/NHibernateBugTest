using NHibernate;
using NHibernate.Linq;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
                    User user = new User()
                    {
                        UserCode = "gokhanabatay",
                        IsOpen = true,
                        UserName = "G�khan Abatay"
                    };
                    session.Save(user);

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
        public void Get_DateCustomType_NullableDateValueEquals_IncorrectCast()
        {
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Assert.That(session.Query<UserSession>()
                        .Where(x => x.OpenDate.Value == DateTime.Now)
                        .ToList().Count == 10);
                }
            }
        }

        public class LetUsageFailsDto
        {
            public string Claims { get; set; }
        }

        [Test, Order(2)]
        public void Get_DateCustomType_NullableDateLetUsage_FailsWithEqualsOperator()
        {
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var items = (from userSession in session.Query<UserSession>()
                                 where userSession.OpenDate.Equals(DateTime.Now)
                                 let IsSessionOpen = userSession.IsOpen == true && userSession.ExpireDateTime.Value <= DateTime.Now
                                 select new LetUsageFailsDto() 
                                 {                                 
                                     Claims = IsSessionOpen ? userSession.Claims : string.Empty
                                 }
                                 )
                        .ToList();

                        Assert.That(items.Count == 10);
                }
            }
        }

        [Test, Order(2)]
        public void Get_DateCustomType_NullableDateValueEqualsV2_IncorrectCast()
        {
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Assert.That(session.Query<UserSession>()
                        .Where(x => x.OpenDate.Value.Equals(DateTime.Now))
                        .ToList().Count == 10);
                }
            }
        }

        [Test, Order(3)]
        public void Get_DateCustomType_SuccessDateEquals()
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

        public class NullableBooleanResult
        {
            public bool? IsOpen { get; set; }
        
        }

        [Test, Order(4)]
        public void Get_BooleanCustomType_Fails_IncorrectCast()
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
                        .Select(x=>new NullableBooleanResult()
                        {
                            IsOpen = x.User.IsOpen
                        })
                        .ToList().Count == 10);
                }
            }
        }
    }
}
