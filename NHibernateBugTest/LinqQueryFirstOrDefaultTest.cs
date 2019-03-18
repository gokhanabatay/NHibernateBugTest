using NHibernate;
using NHibernate.Exceptions;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernateBugTest
{
    [TestFixture]
    public class LinqQueryFirstOrDefaultTest
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
        public void Saves_CustomerAndAddresses_Success()
        {
            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Customer customer = new Customer()
                        {
                            Name = "ABATAY" + i,
                            Addresses = new List<Address>()
                            {
                             new Address()
                             {
                                 City = "NewYork",
                                 AddressText = "NewYork etc. Street No:"+i
                             },
                             new Address()
                             {
                                 City = "Istanbul",
                                 AddressText = "Istanbul etc. Street No:"+i+10
                             }
                            }
                        };
                        session.Save(customer);
                    }
                    transaction.Commit();
                }
            }

            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Assert.That(session.Query<Customer>().ToList().Count == 10);
                }
            }
        }

        [TestCase(1), Order(2)]
        [TestCase(2)]
        public void Retrieve_CustomerAndAddresses_Success(short mbrId)
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
                    Assert.That(session.Query<Customer>().ToList().Count == 10);
                    Assert.That(session.Query<Customer>().SelectMany(x=>x.Addresses).Count() == 20);
                }
            }
        }

        [TestCase(1), Order(4)]
        public void Retrieve_CustomerAndSelectedAddresses_Fail(short mbrId)
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
                    Assert.Throws<GenericADOException>(() => session.Query<Customer>()
                        .Select(x => new
                        {
                            x.Guid,
                            x.Name,
                            City = x.Addresses.Where(y => y.City == "NewYork").Select(y => y.City).FirstOrDefault()
                        }).ToList());
                }
            }
        }
    }
}
