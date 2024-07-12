using BaSys.Metadata.Models;
using System;
using System.Data;

namespace BaSys.Metadata.Abstractions
{
    public interface IDataTypesIndex
    {
        DataType GetDataType(Guid uid);
        DataType GetDataTypeSafe(Guid uid);
        DbType GetDbType(Guid uid);
        DbType GetDbTypeSafe(Guid uid);
        bool IsDataType(Guid uid); 
    }
}