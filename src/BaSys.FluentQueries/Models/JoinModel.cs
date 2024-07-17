using BaSys.FluentQueries.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public class JoinModel
    {
        private readonly List<ConditionModel> _joinConditions = new List<ConditionModel>();

        public string TableName { get; set; } = string.Empty;
        public JoinKinds JoinKind { get; set; }
        public IReadOnlyCollection<ConditionModel> JoinConditions => _joinConditions;

        public JoinModel()
        {

        }

        public JoinModel(JoinKinds joinKind, string tableName, IEnumerable<ConditionModel> joinConditions)
        {
            TableName = tableName;
            JoinKind = joinKind;

            _joinConditions.Clear();
            _joinConditions.AddRange(joinConditions);
        }

        public void AddCodition(ConditionModel condition)
        {
            _joinConditions.Add(condition);
        }

        public void ClearConditions()
        {
            _joinConditions.Clear();
        }
    }
}
