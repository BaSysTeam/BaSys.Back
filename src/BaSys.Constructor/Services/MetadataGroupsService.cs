using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Models;
using BaSys.Translation;
using Humanizer;
using System.Data;

namespace BaSys.Constructor.Services
{
    public class MetadataGroupsService : IMetadataGroupsService
    {
        private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
        private readonly IBaSysConnectionFactory _baSysConnectionFactory;

        public MetadataGroupsService(
            IDbInfoRecordsProvider dbInfoRecordsProvider,
            IBaSysConnectionFactory baSysConnectionFactory)
        {
            _dbInfoRecordsProvider = dbInfoRecordsProvider;
            _baSysConnectionFactory = baSysConnectionFactory;
        }

        public async Task<int> DeleteAsync(Guid uid, string dbName)
        {
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);
            using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);
                return await provider.DeleteAsync(uid, null);
            }
        }

        public async Task<List<MetadataGroupDto>> GetChildrenAsync(Guid parentUid, string dbName)
        {
            var result = new List<MetadataGroupDto>();
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);
                var collection = await provider.GetCollectionAsync(null);
                return collection
                    .Where(x => x.ParentUid == parentUid)
                    .Select(s => new MetadataGroupDto(s))
                    .ToList();
            }
        }

        public async Task<bool> HasChildrenAsync(Guid parentUid, string dbName)
        {
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);
            using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var provider = new MetadataGroupProvider(connection);
                return await provider.HasChildrenAsync(parentUid, null);
            }
        }

        public async Task<int> InsertAsync(MetadataGroupDto dto, string dbName)
        {
            var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);

            using (IDbConnection connection = _baSysConnectionFactory.CreateConnection(dbInfoRecord.ConnectionString, dbInfoRecord.DbKind))
            {
                var metadataGroup = new MetadataGroup(dto);
                var provider = new MetadataGroupProvider(connection);
                return await provider.InsertAsync(metadataGroup, null);
            }
        }
    }
}
