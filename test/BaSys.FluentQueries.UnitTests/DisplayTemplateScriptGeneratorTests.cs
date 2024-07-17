using BaSys.FluentQueries.Enums;
using BaSys.Metadata.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.UnitTests
{
    [TestFixture]
    public class DisplayTemplateScriptGeneratorTests
    {
        [TestCase(SqlDialectKinds.MsSql)]
        [TestCase(SqlDialectKinds.PgSql)]
        public void OneField_Build_SqlExpression(SqlDialectKinds dialectKind)
        {
            var builder = new DisplayTemplateScriptGenerator(dialectKind);
            var sqExpression = builder.Build("title");


            string checkText = string.Empty;
            if (dialectKind == SqlDialectKinds.MsSql)
            {
                checkText = "[title]";
            }
            else if (dialectKind == SqlDialectKinds.PgSql)
            {

                checkText = "\"title\"";
            }

            Assert.That(sqExpression, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql)]
        [TestCase(SqlDialectKinds.PgSql)]
        public void OneFieldTemplate_Build_SqlExpression(SqlDialectKinds dialectKind)
        {
            var builder = new DisplayTemplateScriptGenerator(dialectKind);
            var sqExpression = builder.Build("{{title}}");


            string checkText = string.Empty;
            if (dialectKind == SqlDialectKinds.MsSql)
            {
                checkText = "[title]";
            }
            else if (dialectKind == SqlDialectKinds.PgSql)
            {

                checkText = "\"title\"";
            }

            Assert.That(sqExpression, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql)]
        [TestCase(SqlDialectKinds.PgSql)]
        public void OneFieldWithAlias_Build_SqlExpression(SqlDialectKinds dialectKind)
        {
            var builder = new DisplayTemplateScriptGenerator(dialectKind);
            var sqExpression = builder.Build("title", null, "text");

            string checkText = string.Empty;
            if (dialectKind == SqlDialectKinds.MsSql)
            {
                checkText = "[title] AS text";
            }
            else if (dialectKind == SqlDialectKinds.PgSql)
            {

                checkText = "\"title\" AS text";
            }

            Assert.That(sqExpression, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "DisplayCodeAndTitleMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "DisplayCodeAndTitlePgSql")]
        public void Template_Build_SqlExpression(SqlDialectKinds dialectKind, string checkKey)
        {
            var builder = new DisplayTemplateScriptGenerator(dialectKind);
            var sqExpression = builder.Build("{{code}} - {{title}}", "tableName", "text");

            Console.WriteLine(sqExpression);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(sqExpression, Is.EqualTo(checkText));
        }
    }
}

