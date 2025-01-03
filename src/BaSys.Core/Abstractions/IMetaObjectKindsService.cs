﻿using BaSys.Common.Infrastructure;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Core.Abstractions;

public interface IMetaObjectKindsService
{
    void SetUp(IDbConnection connection);
    Task<ResultWrapper<MetaObjectKindSettings>> GetSettingsItemAsync(Guid uid);
    Task<ResultWrapper<MetaObjectKindSettings>> GetSettingsItemByNameAsync(string name);
    Task<ResultWrapper<IEnumerable<MetaObjectKind>>> GetCollectionAsync();
    Task<ResultWrapper<IEnumerable<MetaObjectKindSettings>>> GetSettingsCollection();
    Task<ResultWrapper<MetaObjectKindSettings>> InsertSettingsAsync(MetaObjectKindSettings settings);
    Task<ResultWrapper<int>> InsertStandardItemsAsync();
    Task<ResultWrapper<int>> UpdateSettingsAsync(MetaObjectKindSettings settings);
    Task<ResultWrapper<int>> DeleteAsync(Guid uid);
}