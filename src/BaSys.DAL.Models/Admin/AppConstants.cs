using BaSys.Common.Abstractions;

namespace BaSys.DAL.Models.Admin
{
    public sealed class AppConstants: ISystemObject
    {
        public Guid Uid { get; set; }
        public Guid DataBaseUid { get; set; }
        public string ApplicationTitle { get; set; } = string.Empty;

        public void BeforeSave()
        {
            if (Uid == Guid.Empty)
                Uid = Guid.NewGuid();
        }
    }
}
