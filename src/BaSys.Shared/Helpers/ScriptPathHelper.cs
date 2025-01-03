using System.IO;

namespace BaSys.Common.Helpers;

public class ScriptPathHelper
{
    //public static string GetScriptPath(string appName)
    //{
    //    var searchPattern = $"app.*.js";
    //    var jsPath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\app\\{appName}\\js";

    //    var files = Directory.GetFiles(jsPath, searchPattern);
    //    if (files.Length != 1)
    //        return string.Empty;

    //    var fullPath = $"{jsPath}\\{Path.GetFileName(files[0])}"; 
    //    var path = fullPath.Replace($"{Directory.GetCurrentDirectory()}\\wwwroot", "");
        
    //    return path.Replace("\\", "/");
    //}

    public static string GetScriptPath(string appName)
    {
        var searchPattern = "app.*.js";
        var jsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "app", appName, "js");

        if (!Directory.Exists(jsPath))
        {
            throw new DirectoryNotFoundException($"The directory '{jsPath}' does not exist.");
        }

        var files = Directory.GetFiles(jsPath, searchPattern);
        if (files.Length != 1)
        {
            return string.Empty;
        }

        var fullPath = Path.Combine(jsPath, Path.GetFileName(files[0]));
        var relativePath = fullPath.Replace($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}wwwroot{Path.DirectorySeparatorChar}", "");

        return relativePath.Replace(Path.DirectorySeparatorChar, '/');
    }

}