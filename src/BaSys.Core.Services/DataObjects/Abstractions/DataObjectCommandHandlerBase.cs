using BaSys.Common.Infrastructure;
using BaSys.Core.Features.Abstractions;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Models.WorkflowModel;
using BaSys.Workflows.Abstractions;
using BaSys.Workflows.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Security.Claims;

namespace BaSys.Core.Features.DataObjects.Abstractions
{
    public abstract class DataObjectCommandHandlerBase<TCommand, TResult> : CommandHandlerBase<TCommand, TResult>, IDisposable
    {

        protected readonly ISystemObjectProviderFactory _providerFactory;
        protected readonly IMetadataReader _metadataReader;
        protected readonly ILoggerService _logger;
        protected readonly IServiceProvider _serviceProvider;


        protected readonly List<WorkflowTrigger> _workflowTriggers;
        protected readonly Dictionary<string, object?> _headerData;

        protected IDbConnection? _connection;
        protected MetaObjectKindsProvider? _kindProvider;
        protected WorkflowTriggersProvider? _triggersProvider;

        protected bool _disposed;

        protected DataObjectCommandHandlerBase(ISystemObjectProviderFactory providerFactory,
            IMetadataReader metadataReader,
            ILoggerService logger,
            IServiceProvider serviceProvider)
        {

            _providerFactory = providerFactory;
            _metadataReader = metadataReader;

            _logger = logger;
            _serviceProvider = serviceProvider;

            _workflowTriggers = new List<WorkflowTrigger>();
            _headerData = new Dictionary<string, object?>();

        }

        public override async Task<ResultWrapper<TResult>> ExecuteAsync(TCommand command)
        {
            var result = new ResultWrapper<TResult>();
            _connection.Open();
            using (IDbTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    result = await ExecuteCommandAsync(command, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result.Error(-1, $"Cannot execute command: {nameof(command)}. Message: {ex.Message}.", ex.StackTrace);
                }
            }

            if (result.IsOK)
            {
                await StartTirggersAsync();
            }

            return result;
        }

        protected void SetUpConnection(IDbConnection connection)
        {
            _connection = connection;

            _providerFactory.SetUp(_connection);
            _metadataReader.SetUp(_providerFactory);

            _kindProvider = _providerFactory.Create<MetaObjectKindsProvider>();
            _triggersProvider = _providerFactory.Create<WorkflowTriggersProvider>();

        }

        protected async Task GetActiveTriggersAsync(Guid uid, IDbTransaction? transaction)
        {
            var triggers = await _triggersProvider.GetActiveObjectTriggersAsync(uid, transaction);
            _workflowTriggers.Clear();
            _workflowTriggers.AddRange(triggers);
        }

        protected void FillHeaderData(DataObject? dataObject)
        {
            if (dataObject == null) return;

            _headerData.Clear();
            foreach(var kvp in dataObject.Header)
            {
                _headerData.Add(kvp.Key, kvp.Value);
            }
        }

        private async Task StartTirggersAsync()
        {
            if (!_workflowTriggers.Any())
            {
                return;
            }

            var startTriggerCommand = new WorkflowTriggersStartCommand();
            startTriggerCommand.Triggers.AddRange(_workflowTriggers);
            foreach(var kvp in _headerData)
            {
                startTriggerCommand.Parameters.Add(kvp.Key, kvp.Value);
            }

            var loggerConfigService = _serviceProvider.GetRequiredService<ILoggerConfigService>();
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var startTriggersHandler = _serviceProvider.GetRequiredService<IWorkflowTriggersStartCommandHandler>();

            var loggerConfig = await loggerConfigService.GetLoggerConfig();

            var user = httpContextAccessor.HttpContext?.User;
            var userUid = string.Empty;
            var userName = string.Empty;
            if (user != null)
            {
                userUid = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                userName = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            }

            startTriggerCommand.UserName = userName;
            startTriggerCommand.UserUid = userUid;
            startTriggerCommand.LoggerConfig = loggerConfig;

            _ = Task.Run(async () =>
            {
                try
                {
                    await startTriggersHandler.ExecuteAsync(startTriggerCommand);
                }
                catch (Exception ex)
                {
                    // Cannot start triggers.
                    Console.WriteLine($"Cannot start triggers: {ex}");
                }
            });

        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                        _connection.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
