using BaSys.Common.Infrastructure;
using BaSys.DTO.Constructor;
using System.Data;

namespace BaSys.Core.Abstractions
{
    public interface IWorkflowsService
    {
        void SetUp(IDbConnection connection);
        Task<ResultWrapper<string>> StartAsync(string name);
    }
}
