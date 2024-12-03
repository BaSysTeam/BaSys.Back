namespace BaSys.Core.Services.RecordsBuilder
{
    public sealed class RecordsExpressionParseResut
    {
        public string Expression { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public RecordsExpressionKinds Kind { get; set; }
     
    }
}
