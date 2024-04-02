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
    public class DeleteBuilderTests
    {
        [TestCase(SqlDialectKinds.MsSql)]
        [TestCase(SqlDialectKinds.PgSql)]
        public void DeleteBuilder_ByUid_Query(SqlDialectKinds dialectKinds)
        {
            var builder = DeleteBuilder.Make()
                .Table("sys_metadata_group")
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
