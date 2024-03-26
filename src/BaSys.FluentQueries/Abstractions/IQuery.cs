using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Abstractions
{
    public interface IQuery
    {
        string Text { get; set; }

        IEnumerable<IQueryParameter> Parameters { get; }

        void AddParameters(IEnumerable<IQueryParameter> parameters);
    }
}
