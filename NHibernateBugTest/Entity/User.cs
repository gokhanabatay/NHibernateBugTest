using System;

namespace NHibernateBugTest.Entity
{
    public class User
    {
        public virtual string UserCode { get; set; }

        public virtual bool IsOpen { get; set; }

        public virtual string UserName { get; set; }
    }
}
