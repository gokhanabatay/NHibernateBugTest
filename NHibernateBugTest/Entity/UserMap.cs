using FluentNHibernate.Mapping;
using NHibernateBugTest.Session;

namespace NHibernateBugTest.Entity
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("ALL_USER");
            Id(x => x.UserCode).Column("USER_CODE").GeneratedBy.Assigned();
            Map(x => x.UserName).Column("USER_NAME").Not.Nullable().Length(50);
            Map(x => x.IsOpen).Column("IS_OPEN").CustomType<SqlBoolean>().Precision(8);
        }
    }
}
