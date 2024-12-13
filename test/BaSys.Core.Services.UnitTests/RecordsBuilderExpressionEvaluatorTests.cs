using BaSys.Core.Services.RecordsBuilder;
using BaSys.DAL.Models.App;
using BaSys.Logging.InMemory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Services.UnitTests
{
    [TestFixture]
    public class RecordsBuilderExpressionEvaluatorTests
    {
        [TestCase("$r.amount > 100", true)]
        [TestCase("$r.amount > 5000", false)]
        [TestCase("$r.unknown > 100", false)]
        public void EvaluateCondition_ReturnBoolValue(string expression, bool checkValue)
        {
            var logger = new InMemoryLogger(Common.Enums.EventTypeLevels.Error);
            var evaluator = new RecordsBuilderExpressionEvaluator(logger);

            var header = new Dictionary<string, object?>();
            var row = new DataObjectDetailsTableRow();
            row.Fields.Add("amount", 1000);
            var result = evaluator.EvaluateCondition(expression, header, row);

            Assert.That(result, Is.EqualTo(checkValue));

        }

        [TestCase("$r.amount + 1.5", 1001.5)]
        public void EvaluateExpression_DecimalColumn_ReturnDecimalValue(string expression, decimal checkValue)
        {
            var logger = new InMemoryLogger(Common.Enums.EventTypeLevels.Error);
            var evaluator = new RecordsBuilderExpressionEvaluator(logger);

            var header = new Dictionary<string, object?>();
            var row = new DataObjectDetailsTableRow();
            row.Fields.Add("amount", 1000);
            var result = evaluator.EvaluateExpression(expression, DbType.Decimal, header, row);

            Assert.That(result, Is.EqualTo(checkValue));
        }
    }
}
