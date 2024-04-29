using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;
using BaSys.Metadata.Validators;
using Humanizer;
using System.Data;

namespace BaSys.Constructor.Services
{
    public class MetadataTreeNodesService : IMetadataTreeNodesService
    {
        private readonly IMainConnectionFactory _connectionFactory;

        public MetadataTreeNodesService(IMainConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ResultWrapper<int>> DeleteAsync(Guid uid)
        {
            var result = new ResultWrapper<int>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var provider = new MetadataTreeNodesProvider(connection);
                var deleteResult = await provider.DeleteAsync(uid, null);

                result.Success(deleteResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Cannot delete item.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<List<MetadataTreeNodeDto>>> GetChildrenAsync(Guid uid)
        {
            var result = new ResultWrapper<List<MetadataTreeNodeDto>>();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var provider = new MetadataTreeNodesProvider(connection);
                var collection = await provider.GetChildrenAsync(uid, null);
                var children = collection
                    .Select(x => new MetadataTreeNodeDto(x))
                    .ToList();

                foreach (var child in children)
                {
                    var hasChildren = await provider.HasChildrenAsync(child.Key, null);
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
                using var connection = _connectionFactory.CreateConnection();
                var provider = new MetadataTreeNodesProvider(connection);
                var collection = await provider.GetStandardNodesAsync(null);

                var standardNodes = collection
                    .Select(x => new MetadataTreeNodeDto(x))
                    .ToList();
                
                foreach (var standardNode in standardNodes)
                {
                    var hasChildren = await provider.HasChildrenAsync(standardNode.Key, null);
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

                using var connection = _connectionFactory.CreateConnection();
                var provider = new MetadataTreeNodesProvider(connection);
                var insertResult = await provider.InsertAsync(model, null);

                result.Success(insertResult);
            }
            catch (Exception ex)
            {
                result.Error(-1, "Cannot create item.", ex.Message);
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
                result.Error(-1, $"Cannot create item.{validationResult}");
                return result;
            }

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                var metadataKindProvider = new MetadataKindsProvider(connection);
                var metadataKindSettings = await metadataKindProvider.GetSettingsAsync(dto.MetadataKindUid, transaction);

                if (metadataKindSettings == null)
                {
                    result.Error(-1, $"Cannot find item", $"Uid: {dto.MetadataKindUid}");
                    transaction.Rollback();
                    return result;
                }

                var metaObjectStorableProvider = new MetaObjectStorableProvider(connection, metadataKindSettings.NamePlural);
                var newMetaObjectSettings = new MetaObjectStorableSettings()
                {
                    MetaObjectKindUid = metadataKindSettings.Uid,
                    Name = dto.Name,
                    Title = dto.Title,
                    Memo = dto.Memo,
                };
              

                try
                {
                    var insertedCount = await metaObjectStorableProvider.InsertSettingsAsync(newMetaObjectSettings, transaction);
                    transaction.Commit();
                    result.Success(insertedCount);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, "Cannot create item.", ex.Message);
                }
            }

            return result;
        }

        private List<MetadataTreeNodeDto> MakeTree(List<MetadataTreeNodeDto> source, Guid? parentKey = null)
        {
            var result = new List<MetadataTreeNodeDto>();
            var parents = source.Where(x => x.ParentKey == parentKey);
            
            foreach ( var parent in parents)
            {
                parent.Children = MakeTree(source, parent.Key);
                result.Add(parent);
            }

            return result;
        }
    }
}
