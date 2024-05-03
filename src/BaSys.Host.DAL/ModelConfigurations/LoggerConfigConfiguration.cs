using BaSys.DAL.Models.Logging;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class LoggerConfigConfiguration : DataModelConfiguration<LoggerConfig>
    {
        public LoggerConfigConfiguration()
        {
            Table("sys_logger_config");

            Column("uid").IsPrimaryKey();
            Column("IsEnabled");
            Column("LoggerType").IsOptional();
            Column("MinimumLogLevel");
            Column("ConnectionString").MaxLength(300).IsOptional();
            Column("AutoClearInterval");
            Column("IsSelected");
        }
    }
}
 