using BaSys.Host.DAL.ModelConfigurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.UnitTests
{
    [TestFixture]
    public class DataModelConfigurationTests
    {
        [Test]
        public void DataModelConfiguration_InitColumns_CheckEnumTypes()
        {
            var config = new LoggerConfigConfiguration();
            var loggerTypeColumn = config.Column("LoggerType");
            var logLevelColumn = config.Column("MinimumLogLevel");

            Assert.That(loggerTypeColumn.DbType, Is.EqualTo(DbType.Int32));
            Assert.That(logLevelColumn.DbType, Is.EqualTo(DbType.Int32));
        }
    }
}
