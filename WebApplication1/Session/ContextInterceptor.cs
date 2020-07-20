using NHibernate;

namespace WebApplication1.Session
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
