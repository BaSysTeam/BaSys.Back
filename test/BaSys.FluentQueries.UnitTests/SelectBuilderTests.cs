using BaSys.FluentQueries.Enums;
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
            Assert.AreEqual(1, query.Parameters.Count());
        }
    }
}
