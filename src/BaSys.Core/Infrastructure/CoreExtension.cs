using BaSys.Core.Abstractions;
using BaSys.Core.Features.Abstractions;
using BaSys.Core.Features.DataObjects.Queries;
using BaSys.Core.Features.MetaObjects.Commands;
using BaSys.Core.Features.MetaObjects.Services;
using BaSys.Core.Services;
using BaSys.Workflows.Abstractions;
using BaSys.Workflows.Commands;
using BaSys.Workflows.Services;

namespace BaSys.Core.Infrastructure;

public static class CoreExtension
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient<IMetaObjectKindsService, MetaObjectKindsService>();
        services.AddTransient<IMetaObjectsService, MetaObjectsService>();
        services.AddTransient<IMetaMenusService, MetaMenusService>();
        services.AddTransient<IDataTypesService, DataTypesService>();
        services.AddTransient<IMetadataReader, MetadataReader>();
        services.AddTransient<IMetaObjectCreateCommandHandler, MetaObjectCreateCommandHandler>();
        services.AddTransient<IMetaObjectUpdateCommandHandler, MetaObjectUpdateCommandHandler>();
        services.AddTransient<IDataObjectRegistratorRouteQueryHandler, DataObjectRegistratorRouteQueryHandler>();

        // Workflows
        services.AddTransient<IMetaWorkflowsService, MetaWorkflowsService>();
        services.AddTransient<IWorkflowsService, WorkflowsService>();
        services.AddTransient<IWorkflowTerminateCommandHandler, WorkflowTerminateCommandHandler>();
        services.AddTransient<IWorkflowsScheduleService, WorkflowsScheduleService>();
        services.AddTransient<IWorkflowTriggersService, WorkflowTriggersService>();

        return services;
    }
}