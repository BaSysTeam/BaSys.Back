using BaSys.Metadata.Models;

namespace BaSys.Core.Abstractions;

public interface IDataTypesService
{
    Task<List<DataType>> GetAllDataTypes();
}