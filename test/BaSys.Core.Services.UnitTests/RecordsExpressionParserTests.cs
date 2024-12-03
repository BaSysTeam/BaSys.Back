using BaSys.Core.Services.RecordsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Services.UnitTests
{
    [TestFixture]
    public class RecordsExpressionParserTests
    {
        [TestCase("","", false, false, true )]
        [TestCase("$h.store", "store", true, false, false)]
        [TestCase("$r.product", "product", false, false, false)]
        [TestCase("$r.product + 1", "", false, true, false)]
        [TestCase("$h.total", "total", true, false, false)]
        [TestCase("$h.total+1", "", false, true, false)]
        [TestCase("-$h.total", "", false, true, false)]
        public void Parse(string expression, 
            string checkName, 
            bool checkIsHeader, 
            bool checkIsFormula, 
            bool checkIsError)
        {
            var parser = new RecordsExpressionParser();
            var parseResult = parser.Parse(expression);

            var checkResult = new RecordsExpressionParseResut
            {
                Name = checkName,
                IsHeader = checkIsHeader,
                IsFormula = checkIsFormula,
                IsError = checkIsError
            };

            Assert.That(parseResult.Name, Is.EqualTo(checkResult.Name));
            Assert.That(parseResult.IsHeader, Is.EqualTo(checkResult.IsHeader));
            Assert.That(parseResult.IsFormula, Is.EqualTo(checkResult.IsFormula));
            Assert.That(parseResult.IsHeader, Is.EqualTo(checkResult.IsHeader));
            Assert.That(parseResult.IsHeader, Is.EqualTo(checkResult.IsHeader));

        }
    }
}
