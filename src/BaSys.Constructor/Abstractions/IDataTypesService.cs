using BaSys.Metadata.Models;

namespace BaSys.Constructor.Abstractions;

public interface IDataTypesService
{
    Task<List<DataType>> GetAllDataTypes();
}