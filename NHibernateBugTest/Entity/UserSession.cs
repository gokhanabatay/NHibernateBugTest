using System;

namespace NHibernateBugTest.Entity
{
    public class UserSession 
    {
        public virtual long Guid { get; set; }

        public virtual short MbrId { get; set; }

        public virtual string UserCode { get; set; }

        public virtual DateTime? OpenDate { get; set; }
        public virtual DateTime? ExpireDateTime { get; set; }

        public virtual string RemoteIpAddress { get; set; }
        public virtual string RemotePort { get; set; }
        public virtual string LocalIpAddress { get; set; }
        public virtual string LocalPort { get; set; }
        public virtual string DeviceId { get; set; }
        public virtual string Claims { get; set; }
    }
}
