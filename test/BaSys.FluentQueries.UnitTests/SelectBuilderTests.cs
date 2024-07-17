using BaSys.FluentQueries.Enums;
using BaSys.FluentQueries.Models;
using BaSys.FluentQueries.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.UnitTests
{
    [TestFixture]
    public class SelectBuilderTests
    {
        [Test]
        public void SelectBuilder_SelectAll_Query()
        {
            var builder = SelectBuilder.Make().Select("*").From("sys_metadata_group");

            var msQuery = builder.Query(Enums.SqlDialectKinds.MsSql);
            var pgQuery = builder.Query(Enums.SqlDialectKinds.PgSql);

            Console.WriteLine("MS SQL:");
            Console.WriteLine(msQuery.Text);

            Console.WriteLine("===================");
            Console.WriteLine("PG SQL:");
            Console.WriteLine(pgQuery.Text);

            Assert.IsNotNull(msQuery);
            Assert.IsNotNull(pgQuery);
        }

        [TestCase(SqlDialectKinds.MsSql)]
        [TestCase(SqlDialectKinds.PgSql)]
        public void SelectBuilder_SelectByUid_Query(SqlDialectKinds dialectKinds)
        {
            var builder = SelectBuilder.Make().Select("*")
                .From("sys_metadata_group")
                .WhereAnd("uid = @uid")
                .Parameter("uid", Guid.Parse("{2DEB97F4-4971-42CD-8519-5113FA3D1768}"), DbType.Guid);

            var query = builder.Query(dialectKinds);


            Console.WriteLine($"{dialectKinds}");
            Console.WriteLine(query);

            Assert.IsNotNull(query);
            Assert.That(query.Parameters.Count(), Is.EqualTo(1));
        }

        [TestCase(SqlDialectKinds.MsSql, "SelectTopMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "SelectTopPgSql")]
        public void SelectBuilder_SelectTop_Query(SqlDialectKinds dialectKinds, string checkKey)
        {
            var builder = SelectBuilder.Make()
                .Top(1)
                .Select("*")
                .From("my_table");

            var query = builder.Query(dialectKinds);

            Console.WriteLine($"{dialectKinds}");
            Console.WriteLine(query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "SelectTopOrderByMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "SelectTopOrderByPgSql")]
        public void SelectBuilder_SelectTopOrderBy_Query(SqlDialectKinds dialectKinds, string checkKey)
        {
            var builder = SelectBuilder.Make()
                .Top(1)
                .Select("*")
                .From("my_table")
                .OrderBy("name desc");

            var query = builder.Query(dialectKinds);

            Console.WriteLine($"{dialectKinds}");
            Console.WriteLine(query);

            var checkText = Texts.ResourceManager.GetString(checkKey);
            Assert.That(query.Text, Is.EqualTo(checkText));
        }

        [TestCase(SqlDialectKinds.MsSql, "SelectTopOrderByMsSql")]
        [TestCase(SqlDialectKinds.PgSql, "SelectTopOrderByPgSql")]
        public void SelectBuilder_Joins_Query(SqlDialectKinds dialectKinds, string checkKey)
        {

            var joinCondition = new ConditionModel()
            {
                LeftTable = "cat_product",
                LeftField = "group_id",
                ComparisionOperator = ComparisionOperators.Equal,
                RightTable = "cat_product_group",
                RightField = "id",

            };

            var conditions = new List<ConditionModel>
            {
                joinCondition
            };

            var builder = SelectBuilder.Make()
                .From("cat_product")
                .Select("cat_product.id as id")
                .Select("cat_product.title as title")
                .Select("cat_product.groupId as group_id")
                .Select("cat_product_group.title as group_display")
                .Join(JoinKinds.Left, "cat_product_group", conditions);


            var query = builder.Query(dialectKinds);

            Console.WriteLine($"{dialectKinds}");
            Console.WriteLine(query);

            Assert.Pass();
        }
    }
}
