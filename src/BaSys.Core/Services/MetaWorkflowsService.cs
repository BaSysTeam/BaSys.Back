using BaSys.Common.Infrastructure;
using BaSys.Core.Abstractions;
using BaSys.DTO.Constructor;
using BaSys.DTO.Metadata;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Metadata.Defaults;
using BaSys.Translation;
using System.Data;

namespace BaSys.Core.Services
{
    public sealed class MetaWorkflowsService: IMetaWorkflowsService
    {

        private readonly ISystemObjectProviderFactory _providerFactory;
        private readonly ILoggerService _logger;

        private IDbConnection _connection;
        private MetaWorkflowsProvider _provider;

        public MetaWorkflowsService(ISystemObjectProviderFactory providerFactory, ILoggerService logger)
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

        public async Task<ResultWrapper<MetaObjectListDto>> GetListAsync()
        {
            var result = new ResultWrapper<MetaObjectListDto>();

            try
            {
                var collection = await _provider.GetCollectionAsync(null, null);
                var listDto = new MetaObjectListDto
                {
                    Title = DictMain.Workflow,
                    MetaObjectKindUid = MetaObjectKindDefaults.Workflow.Uid.ToString(),
                    Items = collection.Select(x => new MetaObjectDto(x)).ToList()
                };

                result.Success(listDto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get data: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<WorkflowSettingsDto>> GetSettingsItemAsync(string objectName)
        {
            var result = new ResultWrapper<WorkflowSettingsDto>();

            try
            {
                var item = await _provider.GetItemByNameAsync(objectName, null);
                if (item == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindItem}: {objectName}");
                    return result;
                }

                var dto = new WorkflowSettingsDto(item.ToSettings());

                result.Success(dto);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get data: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> CreateAsync(WorkflowSettingsDto input)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var settingsDto = input.Parse();

                var settings = settingsDto.ToModel();
                var insertedCount = await _provider.InsertSettingsAsync(settings, null);
                result.Success(insertedCount, DictMain.ItemSaved);
            }
            catch(Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotCreateItem}: {ex.Message}", ex.StackTrace);

            }

            return result;
        }

        public async Task<ResultWrapper<int>> UpdateSettingsItemAsync(WorkflowSettingsDto input)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var settingsDto = input.Parse();

                var settings = settingsDto.ToModel();
                var savedSettings = await _provider.GetSettingsItemAsync(settings.Uid, null);
                if (savedSettings == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindItem} Menu.{settings.Name}: {settings.Uid}");
                }

                savedSettings.CopyFrom(settings);
                var insertedCount = await _provider.UpdateSettingsAsync(savedSettings, null);
                result.Success(insertedCount, DictMain.ItemSaved);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotUpdateItem}: {ex.Message}", ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> DeleteAsync(string objectName)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var item = await _provider.GetItemByNameAsync(objectName, null);
                if (item == null)
                {
                    result.Error(-1, $"{DictMain.CannotFindItem} Workflow.{objectName}");
                    return result;
                }

                var deletedCount = await _provider.DeleteAsync(item.Uid, null);
                result.Success(deletedCount, DictMain.ItemDeleted);
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteItem} Workflow.{objectName}: {ex.Message}", ex.StackTrace);
            }


            return result;
        }

     
    }
}
