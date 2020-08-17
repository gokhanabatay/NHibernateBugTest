using FluentNHibernate.Mapping;
using System.Collections.Generic;

namespace NHibernateBugTest.Entity
{
    public class Customer
    {
        public virtual long Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<Address> Addresses { get; set; }
    }

    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Table("CUSTOMER");
            Id(x => x.Guid).Column("GUID").GeneratedBy.Sequence("DefaultSequence");
            Map(x => x.Name).Column("NAME").Length(500);
            HasMany(x=>x.Addresses).KeyColumn("CUSTOMER_GUID").Cascade.AllDeleteOrphan();
        }
    }

    public class Address
    {
        public virtual long Guid { get; set; }
        public virtual string City { get; set; }
        public virtual string AddressText { get; set; }

        public virtual short? AddressType { get; set; }

        public virtual Customer Customer { get; set; }
    }

    public class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Table("ADDRESS");
            Id(x => x.Guid).Column("GUID").GeneratedBy.Sequence("DefaultSequence");
            Map(x => x.City).Column("CITY").Length(500);
            Map(x => x.AddressText).Column("ADDRESS_TEXT").Length(500);
            Map(x => x.AddressType).Column("ADDRESS_TYPE").Precision(2);
            References(x => x.Customer).Column("CUSTOMER_GUID");
        }
    }
}
