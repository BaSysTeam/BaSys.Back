using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public interface ITableManager
    {
        string TableName { get; }
        Task<int> CreateTableAsync(IDbTransaction transaction = null);
        Task<int> DropTableAsync(IDbTransaction transaction = null);
        Task<bool> TableExistsAsync(IDbTransaction transaction = null);
    }
}
