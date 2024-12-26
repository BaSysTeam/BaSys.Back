using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class ChangeColumnModel
    {
        public bool DataTypeChanged { get; set; }
        public bool RequiredChanged { get; set; }
        public bool UniqueChanged { get; set; }
        public TableColumn Column { get; set; } = new TableColumn();

        public ChangeColumnModel()
        {
            
        }

        public ChangeColumnModel(TableColumn column,
                                 bool dataTypeChanged,
                                 bool requiredChanged,
                                 bool uniqueChanged)
        {
            Column = column;
            DataTypeChanged = dataTypeChanged;
            RequiredChanged = requiredChanged;
            UniqueChanged = uniqueChanged;
        }

        public override string ToString()
        {
            return $"{Column}";
        }
    }
}
