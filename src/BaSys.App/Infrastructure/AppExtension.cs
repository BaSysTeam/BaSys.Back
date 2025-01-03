﻿using BaSys.App.Abstractions;
using BaSys.App.Features.DataObjectRecords.Commands;
using BaSys.App.Features.DataObjectRecords.Queries;
using BaSys.App.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace BaSys.App.Infrastructure
{
    public static class AppExtension
    {
        public static IServiceCollection AddApp(this IServiceCollection services)
        {
            // add controllers
            services.AddControllers()
                .PartManager
                .ApplicationParts
                .Add(new AssemblyPart(typeof(AppExtension).Assembly));

            services.AddTransient<IDataObjectsService, DataObjectsService>();
            services.AddTransient<ISelectItemService, SelectItemsService>();
            services.AddTransient<IMenusService, MenusService>();
            services.AddTransient<IGetRecordsQueryHandler, GetRecordsQueryHandler>();
            services.AddTransient<IGetRecordsDialogModelQueryHandler, GetRecordsDialogModelQueryHandler>();
            services.AddTransient<ICreateRecordsCommandHandler, CreateRecordsCommandHandler>();
            services.AddTransient<IDeleteRecordsCommandHandler, DeleteRecordsCommandHandler>();

            return services;
        }
    }
}
