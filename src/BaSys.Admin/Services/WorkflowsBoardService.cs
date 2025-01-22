using BaSys.Admin.Abstractions;
using WorkflowCore.Interface;

namespace BaSys.Admin.Services
{
    public class WorkflowsBoardService: IWorkflowsBoardService
    {
        private readonly IPersistenceProvider _persistenceProvider;

        public WorkflowsBoardService(IWorkflowHost host, IPersistenceProvider provider)
        {
            _persistenceProvider = provider;
        }

        public async Task<IEnumerable<string?>> GetInfoAsync()
        {
            var result = await _persistenceProvider.GetRunnableInstances(DateTime.MinValue);

            return result;
        }
    }
}
