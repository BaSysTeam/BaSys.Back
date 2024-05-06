using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.TableManagers;
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
                var metadataNodeUid = new Guid("60738680-DAFD-42C0-8923-585FC7985176");
                var systemNodeUid = new Guid("AE28B333-3F36-4FEC-A276-92FCCC9B435C");

                var standardNodes = new List<MetadataTreeNodeDto>
                {
                    new MetadataTreeNodeDto
                    {
                        IsGroup = true,
                        IsStandard = true,
                        Label = "Metadata",
                        Key = metadataNodeUid,
                        Icon = "pi pi-folder",
                        Leaf = !await _nodesProvider.HasChildrenAsync(metadataNodeUid, null)
                    },
                    new MetadataTreeNodeDto
                    {
                        IsGroup = true,
                        IsStandard = true,
                        Label = "System",
                        Key = systemNodeUid,
                        Icon = "pi pi-folder",
                        Children = new List<MetadataTreeNodeDto>
                        {
                            new MetadataTreeNodeDto
                            {
                                IsGroup = false,
                                IsStandard = true,
                                Label = "DataTypes",
                                Key = new Guid("416C4B6C-48F7-426C-AA5A-774717C9984E"),
                                ParentKey = systemNodeUid,
                                Leaf = true
                            },
                            new MetadataTreeNodeDto
                            {
                                IsGroup = false,
                                IsStandard = true,
                                Label = "MetadataKinds",
                                Key = new Guid("CB930422-E50A-4C14-942F-B45DF8C23DE0"),
                                ParentKey = systemNodeUid,
                                Leaf = true
                            }
                        }
                    }
                };

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
                var kindSettings = await metadataKindProvider.GetSettingsAsync(dto.MetaObjectKindUid, transaction);

                if (kindSettings == null)
                {
                    result.Error(-1, DictMain.CannotFindItem, $"Uid: {dto.MetaObjectKindUid}");
                    transaction.Rollback();
                    return result;
                }

                var metaObjectStorableProvider = _providerFactory.CreateMetaObjectStorableProvider(kindSettings.Name);
                var newMetaObjectSettings = new MetaObjectStorableSettings(kindSettings)
                {
                    Name = dto.Name,
                    Title = dto.Title,
                    Memo = dto.Memo,
                };
                var newTreeNode = new MetadataTreeNode()
                {
                    ParentUid = dto.ParentUid,
                    Title = $"{kindSettings.Title}.{dto.Title}",
                    MetadataKindUid = kindSettings.Uid,
                    MetaObjectKindName = kindSettings.Name,
                    MetaObjectName = newMetaObjectSettings.Name,
                    IconClass = kindSettings.IconClass,
                };

                var dataObjectManager = new DataObjectManager(_connection, kindSettings, newMetaObjectSettings, new PrimitiveDataTypes());

                try
                {
                    var insertedCount = await metaObjectStorableProvider.InsertSettingsAsync(newMetaObjectSettings, transaction);
                    var savedMetaObject = await metaObjectStorableProvider.GetItemByNameAsync(newMetaObjectSettings.Name, transaction);

                    newTreeNode.MetadataObjectUid = savedMetaObject.Uid;
                    await _nodesProvider.InsertAsync(newTreeNode, transaction);
                    await dataObjectManager.CreateTableAsync(transaction);

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
