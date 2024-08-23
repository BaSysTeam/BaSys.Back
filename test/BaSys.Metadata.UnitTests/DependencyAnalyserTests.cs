using BaSys.Metadata.Helpers;
using BaSys.Metadata.Models;
using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaSys.Metadata.UnitTests.Helpers;

namespace BaSys.Metadata.UnitTests
{
    [TestFixture]
    public class DependencyAnalyserTests
    {
        [Test]
        public void Analyse_RowPriceCalc_Dependencies()
        {
            var settings = ExampleBuilder.BuildProductOperation();
            var analyser = new DependencyAnalyser();
            analyser.Analyse(settings);

            var tableProducts = settings.DetailTables.FirstOrDefault(x => x.Name.Equals("products", StringComparison.OrdinalIgnoreCase));
            var amountColumn = tableProducts?.GetColumn("amount");
            var quantityColumn = tableProducts?.GetColumn("quantity");
            var priceColumn = tableProducts?.GetColumn("price");

            Assert.That(quantityColumn?.Dependencies.Count(), Is.EqualTo(1));
            Assert.That(quantityColumn.Dependencies[0].FieldUid, Is.EqualTo(amountColumn?.Uid));

            Assert.That(priceColumn?.Dependencies.Count(), Is.EqualTo(1));


        }

        [Test]
        public void Analyse_DiscountInHeader_Dependencies()
        {
            var settings = ExampleBuilder.BuildDiscountInHeaderExample();
            var analyser = new DependencyAnalyser();
            analyser.Analyse(settings);

            var discountColumn = settings.Header.GetColumn("discount");

            Assert.That(discountColumn.Dependencies.Count(), Is.EqualTo(1));

            var dependecy = discountColumn?.Dependencies[0];
            Assert.That(dependecy.Kind, Is.EqualTo(DependencyKinds.RowField));
        }

        [Test]
        public void ExtractArguments_TwoArgsExpression_ListOfArguments()
        {
            var analyser = new DependencyAnalyser();
            var arguments = analyser.ExtractArguments("$r.quantity * $r.price");

            Assert.That(arguments.Count(), Is.EqualTo(2));
            Assert.That(arguments[0], Is.EqualTo("$r.quantity"));
            Assert.That(arguments[1], Is.EqualTo("$r.price"));

        }

        [TestCase("$r.quantity", "$r", "quantity")]
        [TestCase("$h.rate", "$h", "rate")]
        public void ParseArgumentExpression_Expression_ParseResult(string expression, string prefixCheck, string nameCheck)
        {
            var analyser = new DependencyAnalyser();
            var (prefix, name) = analyser.ParseArgumentExpression(expression);

            Assert.That(prefix, Is.EqualTo(prefixCheck));
            Assert.That(name, Is.EqualTo(nameCheck));

        }

    }
}
