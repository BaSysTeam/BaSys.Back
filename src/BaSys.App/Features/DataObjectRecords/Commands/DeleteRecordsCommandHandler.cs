using BaSys.App.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.Core.Services.RecordsBuilder;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Translation;
using System.Data;

namespace BaSys.App.Features.DataObjectRecords.Commands
{
    public sealed class DeleteRecordsCommandHandler : IDeleteRecordsCommandHandler, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly MetaObjectKindsProvider _kindProvider;
        private readonly IDataTypesService _dataTypesService;
        private readonly IMetadataService _metadataService;
        private bool _disposed;

        public DeleteRecordsCommandHandler(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory, 
            IMetadataService metadataService)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();

            _dataTypesService = new DataTypesService(_providerFactory);
            _dataTypesService.SetUp(_connection);

            _metadataService = metadataService;
            _metadataService.SetUp(_providerFactory);
        }

        public async Task<ResultWrapper<bool>> ExecuteAsync(DeleteRecordsCommand command)
        {
            var result = new ResultWrapper<bool>();

            try
            {
                result = await ExecuteCommandAsync(command);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot execute command: {nameof(command)}. Message: {ex.Message}.", ex.StackTrace);
            }

            return result;
        }

        private async Task<ResultWrapper<bool>> ExecuteCommandAsync(DeleteRecordsCommand command)
        {
            var result = new ResultWrapper<bool>();

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                var kindSettings = await _metadataService.GetKindSettingsByNameAsync(command.KindName, transaction);
                if (kindSettings == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {command.KindName}");
                    return result;
                }

                var allMetaObjects = await _metadataService.GetAllMetaObjectsAsync(transaction);

                var metaObjectSettings = await _metadataService.GetMetaObjectSettingsByNameAsync(command.KindName, command.ObjectName, transaction);
                if (metaObjectSettings == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"{DictMain.CannotFindMetaObject}: {command.KindName}.{command.ObjectName}");
                    return result;
                }

                var createRecordsColumn = metaObjectSettings.Header.GetColumn(kindSettings.RecordsSettings.SourceCreateRecordsColumnUid);
                if (createRecordsColumn == null)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot find column: {command.KindName}.{command.ObjectName}.{kindSettings.RecordsSettings.SourceCreateRecordsColumnUid}");
                    return result;
                }

                var dataTypesIndex = await _dataTypesService.GetIndexAsync(transaction);
                var provider = new DataObjectProvider(_connection, kindSettings, metaObjectSettings, dataTypesIndex);

                var mockObject = new DataObject(metaObjectSettings, dataTypesIndex);
                mockObject.SetPrimaryKey(command.ObjectUid);

                // Delete records.
                var recordsBuilder = new DataObjectsRecordsBuilder(_connection,
                    transaction,
                    kindSettings,
                    metaObjectSettings,
                    mockObject,
                    _kindProvider,
                    dataTypesIndex,
                    Common.Enums.EventTypeLevels.Trace);
                var buildResult = await recordsBuilder.DeleteAsync();

                // Set flag CreateRecords false.
                await provider.UpdateFieldAsync(mockObject, createRecordsColumn.Name, false, transaction);

                transaction.Commit();
                result.Success(true);

            }

            return result;
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
