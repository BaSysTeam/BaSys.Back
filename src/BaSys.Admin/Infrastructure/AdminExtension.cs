﻿using BaSys.Admin.Abstractions;
using BaSys.Admin.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace BaSys.Admin.Infrastructure
{
    public static class AdminExtension
    {
        public static IServiceCollection AddAdmin(this IServiceCollection services)
        {
            // add controllers
            services.AddControllers()
                .PartManager
                .ApplicationParts
                .Add(new AssemblyPart(typeof(AdminExtension).Assembly));

            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IAppConstantsService, AppConstantsService>();
            services.AddTransient<ILoggerConfigService, LoggerConfigService>();
            services.AddTransient<IFileStorageConfigService, FileStorageConfigService>();
            services.AddTransient<IUserGroupService, UserGroupService>();
            services.AddTransient<IWorkflowsBoardService, WorkflowsBoardService>();

            return services;
        }
    }
}
