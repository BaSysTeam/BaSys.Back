using BaSys.FluentQueries.Abstractions;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.Abstractions
{
    public interface ISystemObjectProvider<T> where T : class
    {
        IQuery? LastQuery { get; }

        Task<int> DeleteAsync(Guid uid, IDbTransaction transaction);
        Task<IEnumerable<T>> GetCollectionAsync(IDbTransaction transaction);
        Task<T> GetItemAsync(Guid uid, IDbTransaction transaction);
        Task<Guid> InsertAsync(T item, IDbTransaction transaction);
        Task<int> UpdateAsync(T item, IDbTransaction transaction);
    }
}
