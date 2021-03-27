using NHibernate;

namespace WebApplication.Session
{
    public class ContextInterceptor : EmptyInterceptor
    {
        public override void SetSession(ISession session)
        {
            session.EnableFilter(MemberConditionFilter.FilterName)
                   .SetParameter("MBR_ID", CurrentSession.MbrId);
        }
    }
}
