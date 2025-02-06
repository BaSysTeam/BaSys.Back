using BaSys.Common.Helpers;
using BaSys.Workflows.DTO;
using System.Data;
using System.Text.Json;

namespace BaSys.Workflows.Infrastructure
{
    public sealed class WorkflowParametersParser
    {
        public static Dictionary<string, object?> Parse(IEnumerable<WorkflowParameterDto> input)
        {
            var parameters = new Dictionary<string, object?>();

            foreach (var parameterDto in input)
            {
                if (string.IsNullOrWhiteSpace(parameterDto.Name))
                {
                    throw new ArgumentException($"WorkflowParametersParser. Empty parameter name in workflow parameters.");
                }

                var value = ParseParameter(parameterDto);
                parameters.Add(parameterDto.Name, value);
            }

            return parameters;
        }

        private static object? ParseParameter(WorkflowParameterDto parameterDto)
        {
            object? value = null;

            switch (parameterDto.DataType)
            {
                case "string":
                    value = parameterDto.Value;
                    break;

                case "number":
                    value = ValueParser.Parse(parameterDto.Value, DbType.Decimal);
                    break;

                case "boolean":
                    value = ValueParser.Parse(parameterDto.Value, DbType.Boolean);
                    break;

                case "date":
                    value = ValueParser.Parse(parameterDto.Value, DbType.DateTime);
                    break;

                case "integer":
                    value = ValueParser.Parse(parameterDto.Value, DbType.Int32);
                    break;

                case "long":
                    value = ValueParser.Parse(parameterDto.Value, DbType.Int64);
                    break;

                case "object":

                    var parameters = JsonSerializer.Deserialize<List<WorkflowParameterDto>>(parameterDto.Value, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (parameters != null)
                    {
                        value = Parse(parameters);
                    }

                    break;

                default:
                    throw new ArgumentException($"WorflowParametersParser. Unknown data type: {parameterDto.DataType}.");
            }

            return value;
        }
    }
}
