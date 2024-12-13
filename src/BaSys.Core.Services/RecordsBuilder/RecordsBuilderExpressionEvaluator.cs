using BaSys.DAL.Models.App;
using BaSys.Logging.InMemory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Services.RecordsBuilder
{
    public sealed class RecordsBuilderExpressionEvaluator
    {
        private readonly JintExpressionEvaluator _jintEvaluator;
        private readonly InMemoryLogger _logger;

        public RecordsBuilderExpressionEvaluator(InMemoryLogger logger)
        {
            _logger = logger;
            _jintEvaluator = new JintExpressionEvaluator(_logger);
        }

        public Dictionary<string, object?> BuildContext(Dictionary<string, object?> header, DataObjectDetailsTableRow row)
        {
            var context = new Dictionary<string, object?>();
            context.Add("header", header);
            if (row != null)
            {
                context.Add("row", row.Fields);
            }

            return context;

        }

        public bool EvaluateCondition(
            string expression,
            Dictionary<string, object?> header,
            DataObjectDetailsTableRow row)
        {
            var result = false;
            var context = BuildContext(header, row);

            var expressionPrepared = PrepareExpression(expression);

            _jintEvaluator.SetValue("context", context);
            result = _jintEvaluator.Evaluate<bool>(expressionPrepared);

            _logger.LogDebug("Condition {0} evaluated. Result {1}.", expression, result);

            return result;
        }

        public object? EvaluateExpression(string expression, 
            DbType dbType, 
            Dictionary<string, object?> header, 
            DataObjectDetailsTableRow row)
        {
            object? result = null;
            var context = BuildContext(header, row);

            var expressionPrepared = PrepareExpression(expression);

            _jintEvaluator.SetValue("context", context);
            switch (dbType)
            {
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                case DbType.Xml:
                case DbType.AnsiString:
                    result = _jintEvaluator.Evaluate<string>(expressionPrepared);
                    break;
                case DbType.Byte:
                    result = _jintEvaluator.Evaluate<byte>(expressionPrepared);
                    break;
                case DbType.Boolean:
                    result = _jintEvaluator.Evaluate<bool>(expressionPrepared);
                    break;
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Date:
                case DbType.DateTime:
                    result = _jintEvaluator.Evaluate<DateTime>(expressionPrepared);
                    break;
                case DbType.Decimal:
                    result = _jintEvaluator.Evaluate<Decimal>(expressionPrepared);
                    break;
                case DbType.Double:
                    result = _jintEvaluator.Evaluate<Double>(expressionPrepared);
                    break;
                case DbType.Guid:
                    result = _jintEvaluator.Evaluate<Guid>(expressionPrepared);
                    break;
                case DbType.Int16:
                    result = _jintEvaluator.Evaluate<Int16>(expressionPrepared);
                    break;
                case DbType.Int32:
                    result = _jintEvaluator.Evaluate<int>(expressionPrepared);
                    break;
                case DbType.Int64:
                    result = _jintEvaluator.Evaluate<long>(expressionPrepared);
                    break;
                case DbType.SByte:
                    result = _jintEvaluator.Evaluate<SByte>(expressionPrepared);
                    break;
                case DbType.String:
                    result = _jintEvaluator.Evaluate<string>(expressionPrepared);
                    break;
                case DbType.UInt16:
                    result = _jintEvaluator.Evaluate<UInt16>(expressionPrepared);
                    break;
                case DbType.UInt32:
                    result = _jintEvaluator.Evaluate<UInt32>(expressionPrepared);
                    break;
                case DbType.UInt64:
                    result = _jintEvaluator.Evaluate<UInt64>(expressionPrepared);
                    break;
                case DbType.Time:
                case DbType.Single:
                case DbType.Currency:
                case DbType.Binary:
                case DbType.Object:
                case DbType.VarNumeric:
                    throw new NotImplementedException($"Evaluation expression for type {dbType} is not supported.");
                default:
                    throw new NotImplementedException($"Evaluation expression for type {dbType} is not supported.");

            }


            _logger.LogDebug("Expression {0} evaluated. Result {1}.", expression, result?.ToString() ?? "null");

            return result;
        }

        private string PrepareExpression(string expression)
        {
            var expressionPrepared = expression.Replace("$h.", "context.header.", StringComparison.InvariantCultureIgnoreCase)
              .Replace("$r.", "context.row.", StringComparison.OrdinalIgnoreCase);

            return expressionPrepared;
        }
    }
}
