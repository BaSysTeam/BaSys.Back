using BaSys.FluentQueries.Abstractions;
using BaSys.Metadata.Models;
using System.Data;

namespace BaSys.Host.DAL.Abstractions
{
    public interface ISelectItemsProvider
    {
        IQuery? LastQuery { get; }

        Task<IEnumerable<SelectItem>> GetCollectionAsync(IDbTransaction? transaction);
        Task<SelectItem?> GetItemAsync<T>(T uid, IDbTransaction? transaction);
        Task<SelectItem?> GetItemAsync(string uid, IDbTransaction? transaction);
    }
}
