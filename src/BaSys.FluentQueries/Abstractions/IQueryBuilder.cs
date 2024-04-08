using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Abstractions
{
    public interface IQueryBuilder
    {
        IQuery Build();
    }
}
