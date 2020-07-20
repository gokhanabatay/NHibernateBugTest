using FluentNHibernate.Mapping;
using NHibernate;

namespace DbConnectionTest.Session
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
