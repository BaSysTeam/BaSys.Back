using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BaSys.Metadata.Helpers
{
    public class DependencyAnalyser
    {

        public void Analyse(MetaObjectStorableSettings settings)
        {
            settings.ClearDependencies();

            // Header.
            var headerColumnsDict = settings.Header.Columns.ToDictionary(x => x.Name, x => x);
            foreach (var column in settings.Header.ColumnsWithFormulas)
            {
                var arguments = ExtractArguments(column.Formula);
                foreach (var arg in arguments)
                {
                    var (prefix, name) = ParseArgumentExpression(arg);
                    if (prefix.Equals("$h", StringComparison.OrdinalIgnoreCase))
                    {
                        if (headerColumnsDict.TryGetValue(name, out var dependentColumn))
                        {
                            var dependencyInfo = new DependencyInfo()
                            {
                                Kind = Common.Enums.DependencyKinds.HeaderField,
                                FieldUid = column.Uid,
                                TableUid = settings.Header.Uid,
                            };

                            dependentColumn.Dependencies.Add(dependencyInfo);

                        }
                    }
                    else if(prefix.Equals("$t", StringComparison.OrdinalIgnoreCase))
                    {
                        // Table total expressions.
                        var (tableName, columnName) = ParseTableExpression(name);

                        var tableSettings = settings.DetailTables.FirstOrDefault(x=>x.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
                        if (tableSettings == null)
                        {
                            continue;
                        }

                        var dependentColumn = tableSettings.GetColumn(columnName);

                        if (dependentColumn != null)
                        {
                            var dependencyInfo = new DependencyInfo()
                            {
                                Kind = Common.Enums.DependencyKinds.HeaderField,
                                FieldUid = column.Uid,
                                TableUid = settings.Header.Uid,
                            };
                            dependentColumn.Dependencies.Add(dependencyInfo);
                        }
                    }

                }
            }

            // Details tables.
            foreach (var table in settings.DetailTables)
            {
                var columnsDict = table.Columns.ToDictionary(x => x.Name, x => x);
                foreach (var column in table.ColumnsWithFormulas)
                {
                    var arguments = ExtractArguments(column.Formula);
                    foreach (var arg in arguments)
                    {
                        var (prefix, name) = ParseArgumentExpression(arg);
                        if (prefix.Equals("$r", StringComparison.OrdinalIgnoreCase))
                        {

                            if (columnsDict.TryGetValue(name, out var dependentColumn))
                            {
                                var dependencyInfo = new DependencyInfo()
                                {
                                    Kind = Common.Enums.DependencyKinds.RowField,
                                    FieldUid = column.Uid,
                                    TableUid = table.Uid,
                                };

                                dependentColumn.Dependencies.Add(dependencyInfo);

                            }

                        }
                        else if (prefix.Equals("$h", StringComparison.OrdinalIgnoreCase))
                        {
                            if (headerColumnsDict.TryGetValue(name, out var dependentColumn))
                            {
                                var dependencyInfo = new DependencyInfo()
                                {
                                    Kind = Common.Enums.DependencyKinds.RowField,
                                    FieldUid = column.Uid,
                                    TableUid = table.Uid,
                                };

                                dependentColumn.Dependencies.Add(dependencyInfo);

                            }
                        }

                    }
                }
            }
        }

        public List<string> ExtractArguments(string formula)
        {
            if (string.IsNullOrEmpty(formula))
                return new List<string>();

            var regex = new Regex(@"\$[hrt]\.[a-zA-Z_]\w*(\.\w+)*(\(.*?\))?");
            var matches = regex.Matches(formula);
            var arguments = new HashSet<string>();

            foreach (Match match in matches)
            {
                arguments.Add(match.Value);
            }

            return arguments.ToList();
        }

        public Tuple<string, string> ParseArgumentExpression(string argument)
        {
            var prefix = string.Empty;
            var name = string.Empty;
            var ind = argument.IndexOf('.');
            if (ind > -1)
            {
                prefix = argument.Substring(0, ind);
                name = argument.Substring(ind + 1);
            }
            else
            {
                name = argument;
            }

            return new Tuple<string, string>(prefix, name);

        }

        public Tuple<string, string> ParseTableExpression(string expression)
        {
            var tableName = string.Empty;
            var columnName = string.Empty;

            var ind = expression.IndexOf('.');
            if (ind > -1)
            {
                tableName = expression.Substring(0, ind);
            }
            columnName = ExtractNameFromExpression(expression);

            return new Tuple<string, string>(tableName, columnName);
        }

        public static string ExtractNameFromExpression(string expression)
        {
            // Regular expression to match any function name followed by a string inside quotes
            //string pattern = @"\.\w+\(\s*""([^""]+)""\s*\)";
            string pattern = @"(\w+)\.\w+\(\s*['""]([^'""]+)['""]\s*\)";

            Match match = Regex.Match(expression, pattern);

            if (match.Success)
            {
                return match.Groups[2].Value;
            }

            return null;
        }
    }
}
