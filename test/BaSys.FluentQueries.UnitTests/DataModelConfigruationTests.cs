using BaSys.FluentQueries.UnitTests.Helpers;
using BaSys.Host.DAL.ModelConfigurations;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.UnitTests
{
    [TestFixture]
    public class DataModelConfigruationTests
    {
        [Test]
        public void TableName_InitFromEntity_TableName()
        {
            var config = new DefaultDataConfiguration();

            Console.WriteLine(config.ToString());

            Assert.AreEqual("MetadataGroup", config.TableName);
        }

        [Test]
        public void TableName_SetInConfiguration_TableName()
        {
            var config = new MetadataGroupConfiguration();

            Console.WriteLine(config.ToString());

            Assert.AreEqual("sys_metadata_group", config.TableName);
        }

        [Test]
        public void PrimaryKey_SetInConfiguration_Exists()
        {
            var config = new MetadataGroupConfiguration();

            Console.WriteLine(config.ToString());
            var column = config.Column("uid");

            Assert.NotNull(column);
            Assert.IsTrue(column.PrimaryKey);

        }

        [Test]
        public void Columns_SetInConfiguration_Check()
        {
            var config = new MetadataGroupConfiguration();

            Console.WriteLine(config.ToString());
            var titleColumn = config.Column("title");

            Assert.NotNull(titleColumn);
            Assert.AreEqual(100, titleColumn.StringLength);
            Assert.IsTrue(titleColumn.Required);

        }

        [Test]
        public void EmptyObject_NoExceptions()
        {
            var config = new EmptyObjectConfiguration();

            Assert.NotNull(config);
        }
    }
}
