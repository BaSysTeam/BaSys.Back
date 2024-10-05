using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Core.Abstractions;
using BaSys.Core.Services;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.DAL.TableManagers;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using BaSys.Metadata.Validators;
using BaSys.Translation;
using System.Data;

namespace BaSys.Constructor.Services
{
    public class MetadataTreeNodesService : IMetadataTreeNodesService, IDisposable
    {
        private readonly IMainConnectionFactory _connectionFactory;
        private readonly ILoggerService _logger;
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly IDataTypesService _dataTypesService;

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

            _dataTypesService = new DataTypesService(providerFactory);
            _dataTypesService.SetUp(_connection);

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
                        var metaObjectStorableSettings = await metaObjectStorableProvider.GetSettingsItemAsync(metadataObjectUid, transaction);
                        if (metaObjectStorableSettings == null)
                        {
                            result.Error(-1, DictMain.CannotFindItem, $"Uid: {metadataKindUid}");
                            transaction.Rollback();
                            return result;
                        }

                        await metaObjectStorableProvider.DeleteAsync(metadataObjectUid, transaction);

                        var dataTypesIndex = await _dataTypesService.GetIndexAsync(null);
                        var dataObjectManager = new DataObjectManager(_connection, metadataKindSettings, metaObjectStorableSettings, dataTypesIndex);
                        await dataObjectManager.DropTableAsync(transaction);
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
                    .OrderBy(x => x.Label)
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
                var metadataNodeUid = MetadataGroupDefaults.MetadataGroup.Uid;
                var systemNodeUid = MetadataGroupDefaults.SystemGroup.Uid;

                var standardNodes = new List<MetadataTreeNodeDto>
                {
                    new MetadataTreeNodeDto
                    {
                        IsGroup = true,
                        IsStandard = true,
                        Label = MetadataGroupDefaults.MetadataGroup.Title,
                        Key = metadataNodeUid,
                        Icon = "pi pi-folder",
                        Leaf = !await _nodesProvider.HasChildrenAsync(metadataNodeUid, null)
                    },
                    new MetadataTreeNodeDto
                    {
                        IsGroup = true,
                        IsStandard = true,
                        Label = MetadataGroupDefaults.SystemGroup.Title,
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

                var insertedUid = await _nodesProvider.InsertAsync(model, null);

                result.Success(1);
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotCreateItem, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateAsync(MetadataTreeNodeDto dto)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var model = dto.ToModel();
                var updateResult = await _nodesProvider.UpdateAsync(model, null);

                result.Success(updateResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotUpdateItem, ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> InsertMetaObjectAsync(CreateMetaObjectDto dto)
        {
            var result = new ResultWrapper<int>();

            //var validator = new createmetaobjectdtovalidator();
            //var validationresult = validator.validate(dto);
            //if (!validationresult.isvalid)
            //{
            //    result.error(-1, $"{dictmain.cannotcreateitem}.{validationresult}");
            //    return result;
            //}

            //_connection.open();
            //using (idbtransaction transaction = _connection.begintransaction())
            //{
            //    var metadatakindprovider = _providerfactory.create<metaobjectkindsprovider>();
            //    var kindsettings = await metadatakindprovider.getsettingsasync(dto.metaobjectkinduid, transaction);

            //    if (kindsettings == null)
            //    {
            //        result.error(-1, dictmain.cannotfinditem, $"uid: {dto.metaobjectkinduid}");
            //        transaction.rollback();
            //        return result;
            //    }

            //    var metaobjectstorableprovider = _providerfactory.createmetaobjectstorableprovider(kindsettings.name);
            //    var newmetaobjectsettings = new metaobjectstorablesettings(kindsettings)
            //    {
            //        name = dto.name,
            //        title = dto.title,
            //        memo = dto.memo,
            //    };
            //    var newtreenode = new metadatatreenode()
            //    {
            //        parentuid = dto.parentuid,
            //        title = $"{kindsettings.title}.{dto.title}",
            //        metadatakinduid = kindsettings.uid,
            //        metaobjectkindname = kindsettings.name,
            //        metaobjectname = newmetaobjectsettings.name,
            //        iconclass = kindsettings.iconclass,
            //    };

            //    var datatypesindex = await _datatypesservice.getindexasync();
            //    var dataobjectmanager = new dataobjectmanager(_connection, kindsettings, newmetaobjectsettings, datatypesindex);

            //    try
            //    {
            //        var insertedcount = await metaobjectstorableprovider.insertsettingsasync(newmetaobjectsettings, transaction);
            //        var savedmetaobject = await metaobjectstorableprovider.getitembynameasync(newmetaobjectsettings.name, transaction);

            //        newtreenode.metadataobjectuid = savedmetaobject.uid;
            //        await _nodesprovider.insertasync(newtreenode, transaction);
            //        await dataobjectmanager.createtableasync(transaction);

            //        _logger.write($"metadata item created.", common.enums.eventtypelevels.info, eventtypefactory.metadatacreate);

            //        transaction.commit();
            //        result.success(insertedcount);

            //    }
            //    catch (exception ex)
            //    {
            //        transaction.rollback();
            //        result.error(-1, dictmain.cannotcreateitem, ex.message);
            //        _logger.write($"cannot create metadata item.", common.enums.eventtypelevels.error, eventtypefactory.metadatacreate);
            //    }
            //}

            return result;
        }

        public async Task<ResultWrapper<List<MetadataTreeNodeDto>>> GetGroupsAsync()
        {
            var result = new ResultWrapper<List<MetadataTreeNodeDto>>();

            try
            {
                var collection = await _nodesProvider.GetCollectionAsync(null);
                var groups = collection
                    .Where(x => x.IsGroup)
                    .Select(s => new MetadataTreeNodeDto(s))
                    .OrderBy(x => x.Label)
                    .ToList();

                var metadataNode = new MetadataTreeNodeDto
                {
                    IsGroup = true,
                    IsStandard = true,
                    Label = MetadataGroupDefaults.MetadataGroup.Title,
                    Key = MetadataGroupDefaults.MetadataGroup.Uid,
                    Icon = "pi pi-folder",
                };

                groups.Add(metadataNode);

                var treeNodes = MakeTree(groups);
                result.Success(treeNodes);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Get nodes groups error.", ex.Message);
            }

            return result;
        }

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

        private List<MetadataTreeNodeDto> MakeTree(List<MetadataTreeNodeDto> source, Guid? parentKey = null)
        {
            var result = new List<MetadataTreeNodeDto>();
            var parents = source.Where(x => x.ParentKey == parentKey);

            foreach (var parent in parents)
            {
                parent.Children = MakeTree(source, parent.Key);
                if (!parent.Children.Any())
                    parent.Leaf = true;

                result.Add(parent);
            }

            return result;
        }
    }
}
