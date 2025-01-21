using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using System.Data;
using System.Text;

namespace BaSys.Core.Services
{
    public class WorkflowsService: IWorkflowsService
    {
        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly ILoggerService _logger;

        private IDbConnection _connection;
        private MetaWorkflowsProvider _provider;

        public WorkflowsService(ISystemObjectProviderFactory providerFactory, ILoggerService logger)
        {
            _providerFactory = providerFactory;

            _logger = logger;
        }

        public void SetUp(IDbConnection connection)
        {
            _connection = connection;

            _providerFactory.SetUp(_connection);
            _provider = _providerFactory.Create<MetaWorkflowsProvider>();
        }

        public async Task<ResultWrapper<string>> StartAsync(string name)
        {
            var result = new ResultWrapper<string>();

            result.Success(Guid.NewGuid().ToString());

            return result;
        }
    }
}
