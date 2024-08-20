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
                                    Kind = Common.Enums.DependencyKinds.HeaderField,
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

            var regex = new Regex(@"\$[rh]\.[a-zA-Z_]\w*");
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
            var ind = argument.LastIndexOf('.');
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
    }
}
