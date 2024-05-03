using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;
using BaSys.Metadata.Validators;
using BaSys.Translation;
using Humanizer;
using System.Data;

namespace BaSys.Constructor.Services
{
    public class MetadataTreeNodesService : IMetadataTreeNodesService, IDisposable
    {
        private readonly IMainConnectionFactory _connectionFactory;
        private readonly ILoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly IDbConnection _connection;
        private readonly MetadataTreeNodesProvider _nodesProvider;
        private bool _disposed;


        public MetadataTreeNodesService(IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            ILoggerService logger)
        {
            _connectionFactory = connectionFactory;
            _providerFactory = providerFactory;
            _logger = logger;

            _connection = _connectionFactory.CreateConnection();
            _providerFactory.SetUp(_connection);
            _nodesProvider = _providerFactory.Create<MetadataTreeNodesProvider>();
        }

        public async Task<ResultWrapper<int>> DeleteAsync(MetadataTreeNodeDto dto)
        {
            var result = new ResultWrapper<int>();

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (!dto.IsGroup)
                    {
                        var metadataKindUid = new Guid();
                        var metadataObjectUid = new Guid();

                        if (!dto.MetadataKindUid.HasValue || !dto.MetadataObjectUid.HasValue)
                        {
                            transaction.Rollback();
                            result.Error(-1, "Cannot delete item.");

                            return result;
                        }
                        else
                        {
                            metadataKindUid = dto.MetadataKindUid.Value;
                            metadataObjectUid = dto.MetadataObjectUid.Value;
                        }

                        var metadataKindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
                        var metadataKindSettings = await metadataKindProvider.GetSettingsAsync(metadataKindUid, transaction);
                        if (metadataKindSettings == null)
                        {
                            result.Error(-1, DictMain.CannotFindItem, $"Uid: {metadataKindUid}");
                            transaction.Rollback();
                            return result;
                        }

                        var metaObjectStorableProvider = _providerFactory.CreateMetaObjectStorableProvider(metadataKindSettings.Name);
                        await metaObjectStorableProvider.DeleteAsync(metadataObjectUid, transaction);
                    }

                    var deleteResult = await _nodesProvider.DeleteAsync(dto.Key, transaction);
                    result.Success(deleteResult);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, "Cannot delete item.", ex.Message);

                    _logger.Write($"Cannot delete metadata item.", Common.Enums.EventTypeLevels.Error, EventTypeFactory.MetadataDelete);
                }
            }

            return result;
        }

        public async Task<ResultWrapper<List<MetadataTreeNodeDto>>> GetChildrenAsync(Guid uid)
        {
            var result = new ResultWrapper<List<MetadataTreeNodeDto>>();

            try
            {
                var collection = await _nodesProvider.GetChildrenAsync(uid, null);
                var children = collection
                    .Select(x => new MetadataTreeNodeDto(x))
                    .ToList();

                foreach (var child in children)
                {
                    var hasChildren = await _nodesProvider.HasChildrenAsync(child.Key, null);
                    child.Leaf = !hasChildren;
                }

                result.Success(children);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Get children nodes error.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<List<MetadataTreeNodeDto>>> GetStandardNodesAsync()
        {
            var result = new ResultWrapper<List<MetadataTreeNodeDto>>();

            try
            {
                var collection = await _nodesProvider.GetStandardNodesAsync(null);

                var standardNodes = collection
                    .Select(x => new MetadataTreeNodeDto(x))
                    .ToList();

                foreach (var standardNode in standardNodes)
                {
                    var hasChildren = await _nodesProvider.HasChildrenAsync(standardNode.Key, null);
                    standardNode.Leaf = !hasChildren;
                }

                standardNodes = MakeTree(standardNodes);

                result.Success(standardNodes);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Get standard nodes error.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> InsertAsync(MetadataTreeNodeDto dto)
        {
            var result = new ResultWrapper<int>();

            if (string.IsNullOrEmpty(dto.Label))
            {
                result.Error(-1, "Cannot create item. Label is empty.");
                return result;
            }

            try
            {
                var model = dto.ToModel();

                var insertResult = await _nodesProvider.InsertAsync(model, null);

                result.Success(insertResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotCreateItem, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> InsertMetaObjectAsync(CreateMetaObjectDto dto)
        {
            var result = new ResultWrapper<int>();

            var validator = new CreateMetaObjectDtoValidator();
            var validationResult = validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                result.Error(-1, $"{DictMain.CannotCreateItem}.{validationResult}");
                return result;
            }

            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                var metadataKindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
                var metadataKindSettings = await metadataKindProvider.GetSettingsAsync(dto.MetaObjectKindUid, transaction);

                if (metadataKindSettings == null)
                {
                    result.Error(-1, DictMain.CannotFindItem, $"Uid: {dto.MetaObjectKindUid}");
                    transaction.Rollback();
                    return result;
                }

                var metaObjectStorableProvider = _providerFactory.CreateMetaObjectStorableProvider(metadataKindSettings.Name);
                var newMetaObjectSettings = new MetaObjectStorableSettings()
                {
                    MetaObjectKindUid = metadataKindSettings.Uid,
                    Name = dto.Name,
                    Title = dto.Title,
                    Memo = dto.Memo,
                };
                var newTreeNode = new MetadataTreeNode()
                {
                    ParentUid = dto.ParentUid,
                    Title = $"{metadataKindSettings.Title}.{dto.Title}",
                    MetadataKindUid = metadataKindSettings.Uid,
                    MetaObjectKindName = metadataKindSettings.Name,
                    MetaObjectName = newMetaObjectSettings.Name,
                    IconClass = metadataKindSettings.IconClass,
                };

                try
                {
                    var insertedCount = await metaObjectStorableProvider.InsertSettingsAsync(newMetaObjectSettings, transaction);
                    var savedMetaObject = await metaObjectStorableProvider.GetItemByNameAsync(newMetaObjectSettings.Name, transaction);

                    newTreeNode.MetadataObjectUid = savedMetaObject.Uid;
                    await _nodesProvider.InsertAsync(newTreeNode, transaction);

                    _logger.Write($"Metadata item created.", Common.Enums.EventTypeLevels.Info, EventTypeFactory.MetadataCreate);

                    transaction.Commit();
                    result.Success(insertedCount);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, DictMain.CannotCreateItem, ex.Message);
                    _logger.Write($"Cannot create metadata item.", Common.Enums.EventTypeLevels.Error, EventTypeFactory.MetadataCreate);
                }
            }

            return result;
        }

        private List<MetadataTreeNodeDto> MakeTree(List<MetadataTreeNodeDto> source, Guid? parentKey = null)
        {
            var result = new List<MetadataTreeNodeDto>();
            var parents = source.Where(x => x.ParentKey == parentKey);

            foreach (var parent in parents)
            {
                parent.Children = MakeTree(source, parent.Key);
                result.Add(parent);
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
