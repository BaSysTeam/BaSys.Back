using MessagePack;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class MetaObjectTableColumn : IEquatable<MetaObjectTableColumn>
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        //public Guid DataTypeUid { get; set; }
        //public int StringLength { get; set; }
        //public int NumberDigits { get; set; }
        //public bool PrimaryKey { get; set; }
        //public bool Required { get; set; }
        //public bool Unique { get; set; }
        //public string DefaultValue { get; set; } = String.Empty;

        public string Formula { get; set; } = string.Empty;
        public bool IsStandard { get; set; }

        public MetaObjectTableColumnDataSettings DataSettings { get; set; } = new MetaObjectTableColumnDataSettings();
        public MetaObjectTableColumnRenderSettings RenderSettings { get; set; } = new MetaObjectTableColumnRenderSettings();
        public List<DependencyInfo> Dependencies { get; set; } = new List<DependencyInfo>();

        [SerializationConstructor]
        public MetaObjectTableColumn()
        {
            
        }

        public MetaObjectTableColumn(Guid dataTypeUid, bool primaryKey = true)
        {
            DataSettings.DataTypeUid = dataTypeUid;
            DataSettings.PrimaryKey = primaryKey;
        }

        public bool Equals(MetaObjectTableColumn other)
        {
            if (other == null) return false;

            return Uid == other.Uid &&
                   DataSettings.Equals(other.DataSettings);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MetaObjectTableColumn);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Uid, DataSettings.GetHashCode());
        }

        public MetaObjectTableColumn Clone()
        {
            var clone = new MetaObjectTableColumn();
            clone.Uid = Uid;
            clone.Title = Title;
            clone.Name = Name;
            clone.IsStandard = IsStandard;

            clone.Formula = Formula;

            clone.RenderSettings = RenderSettings.Clone();
            clone.DataSettings = DataSettings.Clone();

            foreach (var sourceDependency in Dependencies)
            {
                clone.Dependencies.Add(sourceDependency.Clone());
            }

            return clone;
        }

        public void ClearDependecies()
        {
            Dependencies.Clear();
        }

        public override string ToString()
        {
            return $"{Title}/{Name}";
        }
      
    }
}
