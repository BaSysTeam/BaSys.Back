using BaSys.App.Abstractions;
using BaSys.App.Models.DataObjectRecordsDialog;
using BaSys.Common.Infrastructure;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Translation;
using System.Data;
using System.Security.AccessControl;

namespace BaSys.App.Services
{
    public sealed class DataObjectRecordsService: IDataObjectRecordsService, IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly MetaObjectKindsProvider _kindProvider;
        private bool _disposed;

        public DataObjectRecordsService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _providerFactory = providerFactory;
            _providerFactory.SetUp(_connection);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
        }

        public async Task<ResultWrapper<DataObjectRecordsDialogViewModel>> GetModelAsync(string kind, string name, string uid)
        {
            var result = new ResultWrapper<DataObjectRecordsDialogViewModel>();

            try
            {
               result = await ExecuteGetModelAsync(kind, name, uid);
            }
            catch(Exception ex)
            {
                result.Error(-1, $"Cannot get model. Message: {ex.Message}.", ex.StackTrace );
            }

            return result;
        }

        private async Task<ResultWrapper<DataObjectRecordsDialogViewModel>> ExecuteGetModelAsync(string kindName, string objectName, string uid)
        {
            var result = new ResultWrapper<DataObjectRecordsDialogViewModel>();

            var allKinds = await _kindProvider.GetCollectionAsync(null);
            var kind = allKinds.FirstOrDefault(x => x.Name.Equals(kindName, StringComparison.InvariantCultureIgnoreCase));

            if (kind == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind}: {kindName}");
                return result;
            }

            var objectKindSettings = kind.ToSettings();

            var recordsDestinationKind = allKinds.FirstOrDefault(x => x.Uid == objectKindSettings.RecordsSettings.StorageMetaObjectKindUid);

            if (recordsDestinationKind == null)
            {
                result.Error(-1, $"Cannot find records meta object kind by uid: {objectKindSettings.RecordsSettings.StorageMetaObjectKindUid}");
                return result;
            }

            var metaObjectProvider = new MetaObjectStorableProvider(_connection, objectKindSettings.Name);
            var metaObject = await metaObjectProvider.GetItemByNameAsync(objectName, null);

            if (metaObject == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObject}: {kindName}.{objectName}");
                return result;
            }

            var metaObjectSettings = metaObject.ToSettings();

            var recordsMetaObjectProvider = new MetaObjectStorableProvider(_connection, recordsDestinationKind.Name);
            

            var model = new DataObjectRecordsDialogViewModel();

            foreach(var recordsSettingsItem in metaObjectSettings.RecordsSettings)
            {
                var destinationMetaObject = await recordsMetaObjectProvider.GetItemAsync(recordsSettingsItem.DestinationMetaObjectUid, null);

                if (destinationMetaObject == null)
                {
                    continue;
                }

                var tab = new DataObjectRecordsDialogTab()
                {
                    Uid = destinationMetaObject.Uid,
                    Title = destinationMetaObject.Title
                };

                model.Tabs.Add(tab);
            }

            result.Success(model);

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
