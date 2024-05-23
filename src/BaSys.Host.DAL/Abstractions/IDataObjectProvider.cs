using BaSys.DAL.Models.App;
using BaSys.FluentQueries.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public interface IDataObjectProvider
    {
        IQuery? LastQuery { get; }

        Task<int> DeleteAsync<T>(T uid, IDbTransaction? transaction);
        Task<List<DataObject>> GetCollectionAsync(IDbTransaction? transaction);
        Task<DataObject?> GetItemAsync<T>(T uid, IDbTransaction? transaction);
        Task<int> InsertAsync(DataObject item, IDbTransaction? transaction);
        Task<int> UpdateAsync(DataObject item, IDbTransaction? transaction);
    }
}
