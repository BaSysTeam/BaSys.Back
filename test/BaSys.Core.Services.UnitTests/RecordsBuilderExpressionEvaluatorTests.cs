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
        private InMemoryLogger _logger;
        private RecordsBuilderExpressionEvaluator _evaluator;
        private Dictionary<string, object?> _header;
        private DataObjectDetailsTableRow _row;

        [SetUp]
        public void SetUp()
        {
            _logger = new InMemoryLogger(Common.Enums.EventTypeLevels.Error);
            _evaluator = new RecordsBuilderExpressionEvaluator(_logger);

            _header = new Dictionary<string, object?>
            {
                { "date", new DateTime(2024, 12, 16, 12, 23, 14) }
            };
            _row = new DataObjectDetailsTableRow();
            _row.Fields.Add("amount", 1000);

        }


        [TestCase("$r.amount > 100", true)]
        [TestCase("$r.amount > 5000", false)]
        [TestCase("$r.unknown > 100", false)]
        public void EvaluateCondition_ReturnBoolValue(string expression, bool checkValue)
        {
         
            var result = _evaluator.EvaluateCondition(expression, _header, _row);
            Assert.That(result, Is.EqualTo(checkValue));

        }

        [TestCase("$r.amount + 1.5", 1001.5)]
        public void EvaluateExpression_DecimalColumn_ReturnDecimalValue(string expression, decimal checkValue)
        {
            var result = _evaluator.EvaluateExpression(expression, DbType.Decimal, _header, _row);
            Assert.That(result, Is.EqualTo(checkValue));
        }

        [Test]
        public void EvaluateDateExpression_ReturnDateValue()
        {
            var result = _evaluator.EvaluateExpression($"$h.date.endDay()", DbType.DateTime, _header, _row);

            Assert.That(result, Is.EqualTo(new DateTime(2024, 12, 16, 23, 59, 59, 999)));
        }
    }
}
