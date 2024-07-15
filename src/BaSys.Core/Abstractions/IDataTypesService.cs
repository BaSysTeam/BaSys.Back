using BaSys.Metadata.Models;
using BaSys.Metadata.Abstractions;
using System.Data;

namespace BaSys.Core.Abstractions;

public interface IDataTypesService
{
    void SetUp(IDbConnection connection);
    Task<List<DataType>> GetAllDataTypesAsync();
    Task<IDataTypesIndex> GetIndexAsync();
}