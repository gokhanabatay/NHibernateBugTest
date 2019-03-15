using FluentNHibernate.Mapping;

namespace NHibernateBugTest.Entity
{
    public class TxnCycle
    {
        public virtual long Guid { get; set; }
        public virtual short Status { get; set; }
        public virtual string Description { get; set; }
    }

    public class TxnCycleMap : ClassMap<TxnCycle>
    {
        public TxnCycleMap()
        {
            Table("TXN_CYCLE");
            Id(x => x.Guid).Column("GUID").GeneratedBy.Sequence("DefaultSequence");
            Map(x => x.Status).Column("STATUS").Precision(1);
            Map(x => x.Description).Column("DESCRIPTION").Length(500);
            Where("STATUS = 1");
        }
    }
}
