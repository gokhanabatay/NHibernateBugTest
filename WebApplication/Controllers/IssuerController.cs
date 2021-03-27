using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuerController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<long>> Get()
        {
            using (ISession session = Session.SessionProvider.ISessionFactory.OpenSession())
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

                    return session.Query<TxnIssuer>().Select(x => x.Guid).ToList();
                }
            }
        }

        // POST api/values
        [HttpPost]
        public int Post([FromBody] List<long> guids)
        {
            try
            {
                using (ISession session = Session.SessionProvider.ISessionFactory.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        var rowNumber = session.Query<TxnIssuer>()
                            .Where(x => guids.Contains(x.Guid))
                            .Update(x => new TxnIssuer
                            {
                                MrcDailyMoved = "Y",
                                MrcDailyMovedDate = DateTime.Now
                            });
                        transaction.Commit();
                        return rowNumber;
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex);
                throw;
            }
           
        }
    }
}
