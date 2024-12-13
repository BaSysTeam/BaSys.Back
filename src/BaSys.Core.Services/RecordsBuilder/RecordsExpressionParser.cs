namespace BaSys.Core.Services.RecordsBuilder
{
    public sealed class RecordsExpressionParser
    {
        public RecordsExpressionParseResut Parse(string expression)
        {
            var parseResult = new RecordsExpressionParseResut();
            parseResult.Expression = expression;

            if (string.IsNullOrWhiteSpace(expression))
            {
               return parseResult;
            }

            var tmp = expression.Trim();

            if (tmp.StartsWith("$h.", StringComparison.OrdinalIgnoreCase))
            {

                var name = tmp.Substring(3);

                if (IsValidName(name))
                {
                    parseResult.Kind = RecordsExpressionKinds.Header;
                    parseResult.Name = name;
                }
                else
                {
                    parseResult.Kind = RecordsExpressionKinds.Formula;
                }

            }
            else if (tmp.StartsWith("$r.", StringComparison.OrdinalIgnoreCase))
            {
                var name = tmp.Substring(3);

                if (IsValidName(name))
                {
                    parseResult.Name = name;
                    parseResult.Kind = RecordsExpressionKinds.Row;
                }
                else
                {
                    parseResult.Kind = RecordsExpressionKinds.Formula;
                }
            }
            else
            {
                parseResult.Kind = RecordsExpressionKinds.Formula;
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
