using BaSys.Common.Abstractions;
using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.DTO.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models;
using BaSys.Translation;

namespace BaSys.Core.Features.DataObjects.Queries
{
    public sealed class DataObjectRegistratorRouteQueryHandler : DataObjectQueryHandlerBase<DataObjectRegistratorRouteQuery, DataObjectRouteDto>,
        IDataObjectRegistratorRouteQueryHandler
    {
        public DataObjectRegistratorRouteQueryHandler(
            IMainConnectionFactory connectionFactory,
            ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataReader,
            ILoggerService logger) : base(connectionFactory, providerFactory, metadataReader, logger)
        {
        }

        protected override async Task<ResultWrapper<DataObjectRouteDto>> ExecuteQueryAsync(DataObjectRegistratorRouteQuery query)
        {
            var result = new ResultWrapper<DataObjectRouteDto>();

            var recordsKindSettings = await _metadataReader.GetKindSettingsByNameAsync(query.Kind, null);
            if (recordsKindSettings == null)
            {
                result.Error(-1, $"{DictMain.CannotFindMetaObjectKind} {query.Kind}");
                return result;
            }

            var allKinds = await _metadataReader.GetAllKindsAsync(null);

            // Find meta object kind which crates records.
            MetaObjectKindSettings? registratorKindSettings = null;
            foreach (var currentKind in allKinds)
            {
                var currentSettings = currentKind.ToSettings();

                if (currentSettings.CanCreateRecords
                    && currentSettings.RecordsSettings.StorageMetaObjectKindUid == recordsKindSettings.Uid)
                {
                    registratorKindSettings = currentSettings;
                    break;
                }
            }

            if (registratorKindSettings == null)
            {
                result.Error(-1, $"Cannot find records creator kind for {query.Kind}");
                return result;
            }

            // Find current records list meta object.
            var recordsMetaObject = await _metadataReader.GetMetaObjectByNameAsync(query.Kind, query.Name, null);
            if (recordsMetaObject == null)
            {
                result.Error(-1, $"Cannot find meta object {query.Kind}.{query.Name}");
                return result;
            }

            var recordsMetaObjectSettings = recordsMetaObject.ToSettings();

            // Get columns from settings to get route data.
            var columnKind = GetColumn(recordsMetaObjectSettings, 
                registratorKindSettings.RecordsSettings.StorageKindColumnUid, 
                result);
            if (columnKind == null) return result;

            var columnMetaObject = GetColumn(recordsMetaObjectSettings, 
                registratorKindSettings.RecordsSettings.StorageMetaObjectColumnUid, 
                result);
            if (columnMetaObject == null) return result;

            var columnObject = GetColumn(recordsMetaObjectSettings, 
                registratorKindSettings.RecordsSettings.StorageObjectColumnUid, 
                result);
            if (columnObject == null) return result;

            // Get data and build route dto.
            var registratorKindUidStr = GetColumnValue(query, columnKind, result);
            if (string.IsNullOrWhiteSpace(registratorKindUidStr)) return result;

            var registratorMetaObjectUidStr = GetColumnValue(query, columnMetaObject, result);
            if (string.IsNullOrWhiteSpace(registratorMetaObjectUidStr)) return result;

            var registratorUid = GetColumnValue(query, columnObject, result);
            if (string.IsNullOrWhiteSpace(registratorUid)) return result;

            var routeDto = new DataObjectRouteDto { Uid = registratorUid };

            if (Guid.TryParse(registratorKindUidStr, out var registratorKindUid))
            {
                var registratorKind = await _metadataReader.GetKindAsync(registratorKindUid, null);
                routeDto.Kind = registratorKind?.Name;
            }

            if (Guid.TryParse(registratorMetaObjectUidStr, out var registratorMetaObjectUid))
            {
                var registrator = await _metadataReader.GetMetaObjectAsync(registratorMetaObjectUid, null);
                routeDto.Name = registrator?.Name;
            }

            if (string.IsNullOrWhiteSpace(routeDto.Kind))
            {
                result.Error(-1, $"Cannot get meta object kind {registratorKindUidStr}");
                return result;
            }

            if (string.IsNullOrWhiteSpace(routeDto.Name))
            {
                result.Error(-1, $"Cannot get meta object  {registratorMetaObjectUidStr}");
                return result;
            }

            result.Success(routeDto);
            return result;
        }

        private string? GetColumnValue(DataObjectRegistratorRouteQuery query,
            MetaObjectTableColumn column,
            IResultWrapper result)
        {
            var value = string.Empty;
            if (query.Data.ContainsKey(column.Name))
            {
                value = query.Data[column.Name]?.ToString();
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                result.Error(-1, $"Cannot get column.{column.Name} value");
            }

            return value;
        }

        private MetaObjectTableColumn? GetColumn(MetaObjectStorableSettings recordsMetaObjectSettings, 
            Guid columnUid, 
            IResultWrapper result)
        {
            var column = recordsMetaObjectSettings.Header.GetColumn(columnUid);
            if (column == null)
            {
                result.Error(-1, $"Cannot find records column {columnUid}");
            }

            return column;
        }
    }
}
