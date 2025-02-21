using BaSys.SuperAdmin.DAL.Models;

namespace BaSys.Host.Abstractions
{
    public interface IAppSettingsReader
    {
        Task<AppRecord> GetSettingsAsync();
    }
}
