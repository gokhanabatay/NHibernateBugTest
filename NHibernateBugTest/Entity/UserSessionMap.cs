using FluentNHibernate.Mapping;
using NHibernateBugTest.Session;

namespace NHibernateBugTest.Entity
{
    public class UserSessionMap : ClassMap<UserSession>
    {
        public UserSessionMap()
        {
            Table("USER_SESSION");
            Id(x => x.Guid).Column("GUID").GeneratedBy.Sequence("DefaultSequence");
            Map(x => x.MbrId).Column("MBR_ID").Precision(4).Not.Update();
            Map(x => x.UserCode).Column("USER_CODE").Not.Nullable().Length(50);
            Map(x => x.OpenDate).Column("OPEN_DATE").CustomType<SqlNumberDate>().Precision(8);
            Map(x => x.ExpireDateTime).Column("EXPIRE_DATE_TIME").CustomType<SqlNumberDateTime>().Precision(17);
            Map(x => x.IsOpen).Column("IS_OPEN").CustomType<SqlBoolean>().Precision(8);
            Map(x => x.RemoteIpAddress).Column("REMOTE_IP_ADDRESS").Length(39);
            Map(x => x.RemotePort).Column("REMOTE_PORT").Length(5);
            Map(x => x.LocalIpAddress).Column("LOCAL_IP_ADDRESS").Length(39);
            Map(x => x.LocalPort).Column("LOCAL_PORT").Length(5);
            Map(x => x.DeviceId).Column("DEVICE_ID").Length(40);
            Map(x => x.Claims).Column("CLAIMS").Length(4000);
            References(x => x.User).Column("USER_CODE").PropertyRef(x=>x.UserCode).ReadOnly();

            ApplyFilter<MemberConditionFilter>();
        }
    }
}
