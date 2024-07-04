using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Core.Abstractions;

public interface IDataTypesService
{
    void SetUp(IDbConnection connection);
    Task<List<DataType>> GetAllDataTypes();
}