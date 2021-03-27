using FluentNHibernate.Mapping;
using NHibernate;

namespace WebApplication.Session
{
    public class MemberConditionFilter : FilterDefinition
    {
        public const string FilterName = "MemberConditionFilter";

        public MemberConditionFilter()
        {
            WithName(FilterName).AddParameter("MBR_ID", NHibernateUtil.Int16)
                                .WithCondition("MBR_ID = :MBR_ID");
        }
    }
}
