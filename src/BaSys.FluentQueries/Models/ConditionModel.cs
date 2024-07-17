using BaSys.FluentQueries.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public sealed class ConditionModel
    {
        public string LeftTable { get; set; } = string.Empty;
        public string LeftField { get; set; } = string.Empty;
        public ComparisionOperators ComparisionOperator { get; set; }
        public string RightTable { get; set; } = string.Empty;
        public string RightField { get; set; } = string.Empty;
        public LogicalOperators LogicalOperator { get; set; }
    }
}
