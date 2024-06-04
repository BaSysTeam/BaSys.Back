namespace BaSys.FileStorage.S3.Models;

public class S3ConnectionSettings
{
    public string? AccessKeyId { get; set; }
    public string? SecretAccessKey { get; set; }
    public string? ServiceUrl { get; set; }
    public string? BucketName { get; set; }
    
    public S3ConnectionSettings()
    {
    }

    public S3ConnectionSettings(string connectionString)
    {
        Parse(connectionString);
    }
    
    private void Parse(string connectionString)
    {
        var separators = new[] { ';' };
        var parts = connectionString.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var equalsInd = part.IndexOf('=');

            if (equalsInd == -1)
            {
                continue;
            }

            var settingsName = part.Substring(0, equalsInd - 1).Trim();
            var settingsValue = part.Substring(equalsInd + 1).Trim();

            SetValue(settingsName, settingsValue);
        }
    }
    
    private void SetValue(string name, string value)
    {
        if (name.Equals("AccessKeyId", StringComparison.OrdinalIgnoreCase))
            AccessKeyId = value;
        else if (name.Equals("SecretAccessKey", StringComparison.OrdinalIgnoreCase))
            SecretAccessKey = value;
        else if (name.Equals("ServiceUrl", StringComparison.OrdinalIgnoreCase))
            ServiceUrl = value;
        else if (name.Equals("BucketName", StringComparison.OrdinalIgnoreCase))
            BucketName = value;
    }
}