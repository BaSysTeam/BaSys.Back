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
        public void Analyse_TotalInHeader_Dependencies()
        {
            var settings = ExampleBuilder.BuildProductOperation();
            var analyser = new DependencyAnalyser();
            analyser.Analyse(settings);

            var totalColumn = settings.Header.GetColumn("total");
            var tableProducts = settings.GetTable("products");
            var amountColumn = tableProducts?.GetColumn("amount");
          

            Assert.That(amountColumn?.Dependencies.Count(), Is.EqualTo(1));
            Assert.That(amountColumn?.Dependencies[0].FieldUid, Is.EqualTo(totalColumn?.Uid));

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

        [Test]
        public void ExtractArguments_TableArgsExpression_ListOfArguments()
        {
            var analyser = new DependencyAnalyser();
            var arguments = analyser.ExtractArguments("$t.products.sum(\"amount\") * 2");

            Assert.That(arguments.Count(), Is.EqualTo(1));
            Assert.That(arguments[0], Is.EqualTo("$t.products.sum(\"amount\")"));

        }

        [TestCase("$r.quantity", "$r", "quantity")]
        [TestCase("$h.rate", "$h", "rate")]
        [TestCase("$t.products.sum(\"amount\")", "$t", "products.sum(\"amount\")")]
        public void ParseArgumentExpression_Expression_ParseResult(string expression, string prefixCheck, string nameCheck)
        {
            var analyser = new DependencyAnalyser();
            var (prefix, name) = analyser.ParseArgumentExpression(expression);

            Assert.That(prefix, Is.EqualTo(prefixCheck));
            Assert.That(name, Is.EqualTo(nameCheck));

        }

        [TestCase("products.sum(\"amount\")", "products", "amount")]
        public void ParseTableExpression_Expression_ParseResult(string expression, string tableNameCheck, string columnNameCheck)
        {
            var analyser = new DependencyAnalyser();
            var (tableName, columnName) = analyser.ParseTableExpression(expression);

            Assert.That(tableName, Is.EqualTo(tableNameCheck));
            Assert.That(columnName, Is.EqualTo(columnNameCheck));
        }

    }
}
