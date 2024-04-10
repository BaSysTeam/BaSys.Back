using BaSys.Common;

namespace BaSys.Common
{
    public sealed class HostVersionService : IHostVersionService
    {
        private readonly string _hostVersion;

        public HostVersionService(string hostVersion)
        {
            _hostVersion = hostVersion;
        }

        public string GetVersion()
        {
            return _hostVersion;
        }
    }
}
