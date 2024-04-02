using BaSys.FluentQueries.Abstractions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public class Query : IQuery
    {
        private List<IQueryParameter> _parameters = new List<IQueryParameter>();

        public string Text { get; set; }
        public IEnumerable<IQueryParameter> Parameters => _parameters;

        public DynamicParameters DynamicParameters
        {
            get
            {
                var dynamicParameters = new DynamicParameters();

                foreach (var p in _parameters)
                {
                    dynamicParameters.Add(p.Name, p.Value, p.DbType);
                }

                return dynamicParameters;
            }
        }

        public void AddParameters(IEnumerable<IQueryParameter> parameters)
        {
            _parameters.AddRange(parameters);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Query:");
            sb.AppendLine(Text);

            return sb.ToString();
        }
    }
}
