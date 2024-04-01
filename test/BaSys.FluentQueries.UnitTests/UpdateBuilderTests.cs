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
    public class UpdateBuilderTests
    {
        [Test]
        public void UpdateBulder_ExampleQuery_Query()
        {
            var builder = UpdateBuilder.Make().Table("sys_metadata_groups")
                .Set("ParentUid")
                .Set("Title")
                .Set("Memo")
                .Set("IsStandard")
                .WhereAnd("Uid = @uid");

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
    }
}
