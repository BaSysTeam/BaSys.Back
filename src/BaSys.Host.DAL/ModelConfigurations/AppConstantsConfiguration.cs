using BaSys.DAL.Models.Admin;
using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Host.DAL.ModelConfigurations
{
    public sealed class AppConstantsConfiguration: DataModelConfiguration<AppConstants>
    {
        public AppConstantsConfiguration()
        {
            Table("sys_app_constants_records");

            Column("uid").IsPrimaryKey();
            Column("DataBaseUid").IsRequired();
            Column("ApplicationTitle").MaxLength(100).IsOptional();
        }
    }
}
