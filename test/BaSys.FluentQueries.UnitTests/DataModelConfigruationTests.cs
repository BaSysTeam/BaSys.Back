﻿using BaSys.FluentQueries.UnitTests.Helpers;
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

            Assert.That(config.TableName, Is.EqualTo("MetaObjectKind"));
        }

        [Test]
        public void TableName_SetInConfiguration_TableName()
        {
            var config = new MetaObjectKindConfiguration();

            Console.WriteLine(config.ToString());

            Assert.That(config.TableName, Is.EqualTo("sys_meta_object_kinds"));
        }

        [Test]
        public void PrimaryKey_SetInConfiguration_Exists()
        {
            var config = new MetaObjectKindConfiguration();

            Console.WriteLine(config.ToString());
            var column = config.Column("uid");

            Assert.NotNull(column);
            Assert.IsTrue(column.PrimaryKey);

        }

        [Test]
        public void Columns_SetInConfiguration_Check()
        {
            var config = new MetaObjectKindConfiguration();

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
