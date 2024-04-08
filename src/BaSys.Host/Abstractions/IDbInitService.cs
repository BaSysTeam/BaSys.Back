using System.Data;

namespace BaSys.Host.Abstractions
{
    /// <summary>
    /// Provides a service for creating tables and fill neccessary data when db created.
    /// </summary>
    public interface IDbInitService
    {
        /// <summary>
        /// Set db connection for service. Must be called before Execute.
        /// </summary>
        /// <param name="connection"></param>
        void  SetUp(IDbConnection connection);
        /// <summary>
        /// Execute initialization logic.
        /// </summary>
        /// <returns></returns>
        Task ExecuteAsync();
        /// <summary>
        /// Check tables initially data.
        /// </summary>
        /// <returns></returns>
        Task CheckTablesAsync();
    }
}
