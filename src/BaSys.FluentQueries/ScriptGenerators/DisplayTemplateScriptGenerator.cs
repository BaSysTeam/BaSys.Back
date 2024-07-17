using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.ScriptGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Helpers
{
    public sealed class DisplayTemplateScriptGenerator : ScriptGeneratorBase
    {
        public DisplayTemplateScriptGenerator(SqlDialectKinds dialectKind) : base(dialectKind)
        {

        }

        public string Build(string template, string tableName = null, string alias = null)
        {
            var templateParts = Parse(template);

            if (templateParts.Count == 0)
                return string.Empty;


            _sb.Clear();

            if (templateParts.Count == 1)
            {
                _sb.Append(ColumnExpression(templateParts[0].Item2, tableName));
            }
            else
            {
                _sb.Append("CONCAT(");
                var n = 1;
                foreach (var templatePart in templateParts)
                {
                    if (n > 1)
                    {
                        _sb.Append(", ");
                    }

                    if (templatePart.Item1)
                    {
                        var columnExpression = ColumnExpression(templatePart.Item2, tableName);

                        _sb.Append(columnExpression);
                    }
                    else
                    {
                        _sb.Append("'");
                        _sb.Append(templatePart.Item2);
                        _sb.Append("'");
                    }

                    n++;
                }

                _sb.Append(")");
            }


            if (!string.IsNullOrEmpty(alias))
            {
                _sb.Append($" AS {alias}");
            }


            return _sb.ToString();
        }

        private string ColumnExpression(string expression, string tableName)
        {
            string columnExpression;
            if (!string.IsNullOrEmpty(tableName))
            {
                columnExpression = $"{_wrapperOpen}{tableName}{_wrapperClose}.{_wrapperOpen}{expression}{_wrapperClose}";
            }
            else
            {
                columnExpression = $"{_wrapperOpen}{expression}{_wrapperClose}";
            }

            return columnExpression;
        }

        private IList<Tuple<bool, string>> Parse(string template)
        {
            var result = new List<Tuple<bool, string>>();

            bool insidePlaceholder = false;
            StringBuilder currentPlaceholder = new StringBuilder();

            for (int i = 0; i < template.Length; i++)
            {
                if (template[i] == '{')
                {
                    if (i + 1 < template.Length && template[i + 1] == '{')
                    {
                        if (insidePlaceholder)
                        {
                            throw new InvalidOperationException("Nested placeholders are not supported.");
                        }
                        insidePlaceholder = true;

                        if (currentPlaceholder.Length > 0)
                        {
                            var currentSegment = new Tuple<bool, string>(false, currentPlaceholder.ToString());
                            result.Add(currentSegment);
                        }

                        // start new segment.
                        currentPlaceholder.Clear();
                        i++; // Skip the next '{' character
                    }
                    else
                    {
                        currentPlaceholder.Append(template[i]);
                    }
                }
                else if (template[i] == '}')
                {
                    if (i + 1 < template.Length && template[i + 1] == '}')
                    {
                        if (!insidePlaceholder)
                        {
                            throw new InvalidOperationException("Unexpected closing brace '}'.");
                        }
                        insidePlaceholder = false;
                        i++; // Skip the next '}' character

                        // Finish of template segment.
                        if (currentPlaceholder.Length > 0)
                        {
                            var currentSegment = new Tuple<bool, string>(true, currentPlaceholder.ToString());
                            result.Add(currentSegment);

                            currentPlaceholder.Clear();
                        }

                    }
                    else
                    {
                        currentPlaceholder.Append(template[i]);
                    }
                }
                else
                {
                    currentPlaceholder.Append(template[i]);
                }
            }


            if (insidePlaceholder)
            {
                throw new InvalidOperationException("Unclosed placeholder found.");
            }


            if (currentPlaceholder.Length > 0)
            {
                var currentSegment = new Tuple<bool, string>(false, currentPlaceholder.ToString());
                result.Add(currentSegment);
            }

            return result;
        }
    }
}
