﻿using Microsoft.AspNetCore.Mvc.ApplicationParts;

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

            return services;
        }
    }
}