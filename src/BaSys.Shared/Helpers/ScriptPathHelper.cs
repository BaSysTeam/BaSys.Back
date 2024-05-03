using System.IO;

namespace BaSys.Common.Helpers;

public class ScriptPathHelper
{
    public static string GetScriptPath(string appName)
    {
        var searchPattern = $"{appName}.*.js";
        var jsPath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\app\\{appName}\\js";

        var files = Directory.GetFiles(jsPath, searchPattern);
        if (files.Length != 1)
            return string.Empty;

        var fullPath = $"{jsPath}\\{Path.GetFileName(files[0])}"; 
        var path = fullPath.Replace($"{Directory.GetCurrentDirectory()}\\wwwroot", "");
        
        return path.Replace("\\", "/");
    }
}