using BaSys.FluentQueries.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Abstractions
{
    public interface IDataModelConfiguration
    {
         string TableName { get; set; }
         IReadOnlyCollection<TableColumn> Columns { get;  }
    }
}
