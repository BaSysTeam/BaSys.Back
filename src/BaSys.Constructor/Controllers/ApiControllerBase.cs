using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BaSys.Constructor.Controllers
{
   
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IBaSysConnectionFactory _connectionFactory;
        protected readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        protected IDbConnection _connection;
        protected string _dbName  = string.Empty;
        private bool _disposed;
      

        public ApiControllerBase(IBaSysConnectionFactory connectionFactory, IDbInfoRecordsProvider dbInfoRecordsProvider)
        {
            _connectionFactory = connectionFactory;
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
        }

        [NonAction]
        public void CreateConnection()
        {
            var dbName = GetDbName();

            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);
            if (dbInfoRecord == null)
                throw new ArgumentNullException($"Cannog get DbInfoRecord for DB {dbName}");

            _connection = _connectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind);
        }

        [NonAction]
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                _disposed = true;
            }
        }

        private string GetDbName()
        {
            var dbName = User.Claims.FirstOrDefault(x => x.Type == GlobalConstants.DbNameClaim)?.Value ?? string.Empty;
            return dbName;
        }
    }
}
