using BaSys.Core.Features.RecordsBuilder;
using BaSys.Logging.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Services.UnitTests
{
    [TestFixture]
    public class JintExpressionEvaluatorTests
    {
        private Dictionary<string, object> _context;
        private InMemoryLogger _logger;
        private JintExpressionEvaluator _evaluator;

        [SetUp]
        public void SetUp()
        {
            _context = new Dictionary<string, object>();
            _logger = new InMemoryLogger(Common.Enums.EventTypeLevels.Trace);
            _evaluator = new JintExpressionEvaluator(_logger);

        }

        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase("", true)]
        [TestCase("test", false)]
        public void Evaluate_IsEmptyFunction_ReturnBoolValue(object argument, bool check)
        {
           
            _context.Add("amount", argument);
            _evaluator.SetValue("context", _context);

            var result = _evaluator.Evaluate<bool>(@"isEmpty(context.amount)");

            Assert.That(result, Is.EqualTo(check));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase("", false)]
        [TestCase("test", true)]
        public void Evaluate_IsNotEmptyFunction_ReturnBoolValue(object argument, bool check)
        {
            _context.Add("amount", argument);
            _evaluator.SetValue("context", _context);

            var result = _evaluator.Evaluate<bool>(@"isNotEmpty(context.amount)");

            Assert.That(result, Is.EqualTo(check));
        }

        [Test]
        public void Evaluate_dateTimeNow_ReturnDateTimeValue()
        {
          
            var result = _evaluator.Evaluate<DateTime>("dateTimeNow()");
            var now = DateTime.Now;

            Assert.That(result.Year, Is.EqualTo(now.Year));
            Assert.That(result.Month, Is.EqualTo(now.Month));
            Assert.That(result.Day, Is.EqualTo(now.Day));

        }

        [TestCase("2024-12-12", "2024-12-14", "day", 2)]
        public void Evaluate_DateDifference_ReturnNumber(string startDateStr, string endDateStr, string kind, int check)
        {
            DateTime.TryParse(startDateStr, out var startDate);
            DateTime.TryParse(endDateStr, out var endDate);

            _evaluator.SetValue("startDate", startDate);
            _evaluator.SetValue("endDate", endDate); 
            _evaluator.SetValue("kind", kind);

            var result = _evaluator.Evaluate<int>(@"dateDifference(startDate, endDate, kind)");

            Assert.Pass();

        }

        [Test]
        public void Evaluate_EndDate_ReturnDate()
        {
            var date = new DateTime(2024, 12, 16, 10, 23, 45);
            var endDay = new DateTime(2024, 12, 16, 23, 59, 59, 999);

            _evaluator.SetValue("date", date);

            var result = _evaluator.Evaluate<DateTime>(@"date.endDay()");

            Assert.That(result, Is.EqualTo(endDay));

        }

        [Test]
        public void TestDateWrapper()
        {
            _context.Add("date", DateTime.Now);

            _evaluator.SetValue("date", DateTime.Now);
            var result = _evaluator.Evaluate(@"date.endDay().toISOString();");
            Console.WriteLine(result);

            var endDate = _evaluator.Evaluate<DateTime>(@"date.endDay();");
            Console.WriteLine(endDate);

            Assert.Pass();
        }
    }
}
