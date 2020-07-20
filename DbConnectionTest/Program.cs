using DbConnectionTest.Session;
using NHibernate;
using System;

namespace DbConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connection Test!");
            try
            {
                using (ISession session = SessionProvider.ISessionFactory.OpenSession())
                {
                    Console.WriteLine("Session Opended!");
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        Console.WriteLine("Transaction Begined!");
                        //just init connection and ISessionFactory
                        transaction.Commit();
                        Console.WriteLine("Commit Executed!");
                    }
                }
                Console.WriteLine("Close Session!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            Console.ReadLine();
        }
    }
}
