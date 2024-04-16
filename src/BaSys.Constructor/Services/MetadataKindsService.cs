using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Metadata.Models;
using BaSys.SuperAdmin.DAL.Abstractions;
using System.Data;
using System.Xml.Linq;

namespace BaSys.Admin.Services
{
    public sealed class MetadataKindsService: IMetadataKindsService
    {
        private IDbConnection _connection;
        private MetadataKindProvider _provider;

        public MetadataKindsService()
        {
              
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;
            _provider = new MetadataKindProvider(_connection);
        }

        public async Task<ResultWrapper<int>> InsertSettingsAsync(MetadataKindSettings settings, IDbTransaction? transaction = null)
        {
            var result = new ResultWrapper<int>();
            try
            {
                var insertedCount = await _provider.InsertSettingsAsync(settings, transaction);
                result.Success(insertedCount);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot create item", $"Message: {ex.Message}, Query: {_provider.LastQuery}") ;
            }

            return result;
        }
    }
}
