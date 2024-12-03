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
        [TestCase("","", RecordsExpressionKinds.Error )]
        [TestCase("$h.store", "store", RecordsExpressionKinds.Header)]
        [TestCase("$r.product", "product", RecordsExpressionKinds.Row)]
        [TestCase("$r.product + 1", "", RecordsExpressionKinds.Formula)]
        [TestCase("$h.total", "total", RecordsExpressionKinds.Header)]
        [TestCase("$h.total+1", "", RecordsExpressionKinds.Formula)]
        [TestCase("-$h.total", "", RecordsExpressionKinds.Formula)]
        public void Parse(string expression, 
            string checkName, 
            RecordsExpressionKinds checkKind)
        {
            var parser = new RecordsExpressionParser();
            var parseResult = parser.Parse(expression);

            var checkResult = new RecordsExpressionParseResut
            {
                Name = checkName,
                Kind = checkKind,
            };

            Assert.That(parseResult.Name, Is.EqualTo(checkResult.Name));
            Assert.That(parseResult.Kind, Is.EqualTo(checkResult.Kind));
          

        }
    }
}
