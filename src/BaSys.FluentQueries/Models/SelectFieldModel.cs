using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class SelectFieldModel
    {
        public string TableName { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;

        public SelectFieldModel()
        {
            
        }

        public SelectFieldModel(string tableName, string fieldName, string alias)
        {
            TableName = tableName;
            FieldName = fieldName;
            Alias = alias;
        }
    }
}
