using BaSys.Common.Enums;
using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.Logging.InMemory;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.App.Services
{
    public sealed class DataObjectsRecordsBuilder
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly MetaObjectKindSettings _kindSettings;
        private readonly MetaObjectStorableSettings _settings;
        private readonly DataObject _dataObject;

        private readonly InMemoryLogger _logger;

        public DataObjectsRecordsBuilder(IDbConnection connection, 
            IDbTransaction transaction, 
            MetaObjectKindSettings kindSettings, 
            MetaObjectStorableSettings settings, 
            DataObject dataObject, 
            EventTypeLevels logLevel)
        {
            _connection = connection;
            _transaction = transaction;
            _kindSettings = kindSettings;
            _settings = settings;
            _dataObject = dataObject;

            _logger = new InMemoryLogger(logLevel);
        }

        public ResultWrapper<int> Build()
        {
            var result = new ResultWrapper<int>();

            if (!_kindSettings.CanCreateRecords)
            {
                return result;
            }

            return result;
        }
    }
}
