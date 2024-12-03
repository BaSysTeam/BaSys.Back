namespace BaSys.Core.Services.RecordsBuilder
{
    public sealed class RecordsExpressionParseResut
    {
        public string Name { get; set; } = string.Empty;
        public bool IsHeader { get; set; }
        public bool IsFormula { get; set; }
        public bool IsError { get; set; }
    }
}
