using FluentNHibernate.Mapping;
using NHibernateBugTest.Session;
using System;

namespace NHibernateBugTest.Entity
{
    public class TxnIssuer
    {
        public virtual long Guid { get; set; }
        public virtual short MbrId { get; set; }
        public virtual DateTime MrcDailyMovedDate { get; set; } = SqlNumberDate.DefaultDate;
        public virtual string MrcDailyMoved { get; set; } = "N";
        public virtual DateTime CycleMovedDate { get; set; } = SqlNumberDate.DefaultDate;
        public virtual string CycleMoved { get; set; } = "N";
    }

    public class TxnIssuerMap : ClassMap<TxnIssuer>
    {
        public TxnIssuerMap()
        {
            Table("ISSUER");
            Id(x => x.Guid).Column("GUID").GeneratedBy.Increment();
            Map(x => x.MbrId).Column("MBR_ID");
            Map(x => x.MrcDailyMoved).Column("MRC_DAILY_MOVED").Not.Nullable().Not.Update().Length(1);
            Map(x => x.MrcDailyMovedDate).Column("MRC_DAILY_MOVED_DATE").CustomType<SqlNumberDate>().Precision(8);
            Map(x => x.CycleMoved).Column("CYCLE_MOVED").Not.Nullable().Not.Update().Length(1);
            Map(x => x.CycleMovedDate).Column("CYCLE_MOVED_DATE").CustomType<SqlNumberDate>().Precision(8);
            ApplyFilter<MemberConditionFilter>();
        }
    }
}
