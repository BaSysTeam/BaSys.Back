using System;

namespace BaSys.Common.Abstractions
{
    public abstract class SystemObjectBase : ISystemObject
    {
        public virtual Guid Uid { get; set; }

        public virtual void BeforeSave()
        {
            if (Uid == Guid.Empty)
                Uid = Guid.NewGuid();
        }
    }
}
