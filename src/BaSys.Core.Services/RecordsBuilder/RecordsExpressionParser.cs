namespace BaSys.Core.Services.RecordsBuilder
{
    public sealed class RecordsExpressionParser
    {
        public RecordsExpressionParseResut Parse(string expression)
        {
            var parseResult = new RecordsExpressionParseResut();

            if (string.IsNullOrWhiteSpace(expression))
            {
                parseResult.IsError = true;
                return parseResult;
            }

            var tmp = expression.Trim();

            if (tmp.StartsWith("$h.", StringComparison.OrdinalIgnoreCase))
            {
                parseResult.IsHeader = true;

                var name = tmp.Substring(3);

                if (IsValidName(name))
                {
                    parseResult.Name = name;
                }
                else
                {
                    parseResult.IsFormula = true;
                    parseResult.IsHeader = false;
                }

            }
            else if (tmp.StartsWith("$r.", StringComparison.OrdinalIgnoreCase))
            {
                var name = tmp.Substring(3);


                if (IsValidName(name))
                {
                    parseResult.Name = name;
                }
                else
                {
                    parseResult.IsFormula = true;
                }
            }
            else
            {
                parseResult.IsFormula = true;
            }

            return parseResult;
        }

        private bool IsValidName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            foreach (char c in input)
            {
                if (!(char.IsLetterOrDigit(c) || c == '_'))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
