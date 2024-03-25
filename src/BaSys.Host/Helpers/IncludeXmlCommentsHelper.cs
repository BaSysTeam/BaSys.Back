using BaSys.Admin.Infrastructure;
using BaSys.SuperAdmin.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace BaSys.Host.Helpers
{
    public static class IncludeXmlCommentsHelper
    {
        public static void IncludeXmlComments(SwaggerGenOptions options)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var executingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var baseDirectoryArr = baseDirectory.Split(executingAssemblyName);

            if (baseDirectoryArr.Length != 2)
                return;

            var assemblyNameList = new List<string>
            {
                executingAssemblyName ?? "",
                Assembly.GetAssembly(typeof(SuperAdminExtension))?.GetName().Name ?? "",
                Assembly.GetAssembly(typeof(AdminExtension))?.GetName().Name ?? "",
            };

            foreach (var assemblyName in assemblyNameList)
            {
                if (string.IsNullOrEmpty(assemblyName))
                    continue;

                var assemblyBaseDirectory = baseDirectoryArr[0] + assemblyName + baseDirectoryArr[1];
                var xmlFileName = $"{assemblyName}.xml";
                var fullPath = Path.Combine(assemblyBaseDirectory, xmlFileName);

                if (File.Exists(fullPath))
                    options.IncludeXmlComments(fullPath);
            }
        }
    }
}
