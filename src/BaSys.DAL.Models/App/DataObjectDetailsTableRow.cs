using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.App
{
    public sealed class DataObjectDetailsTableRow
    {
        public long Id
        {
            get
            {
                if (Fields.TryGetValue("id", out var value))
                {
                    if (value is long longValue)
                    {
                        return longValue;
                    }

                }

                return 0;
            }
            set
            {
                Fields["id"] = value;
            }

        }

        public object ObjectUid
        {
            get
            {
                if (Fields.TryGetValue("object_uid", out var objectUid))
                {
                    return objectUid;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                Fields["object_uid"] = value;
            }
        }

        public int RowNumber
        {
            get
            {

                if (Fields.TryGetValue("row_nubmer", out var value))
                {
                    if (value is int intValue)
                    {
                        return intValue;
                    }
                }

                return 0;

            }
            set
            {
                Fields["row_number"] = value;
            }
        }

        public Dictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();

        public DataObjectDetailsTableRow()
        {
            
        }

        public DataObjectDetailsTableRow(IDictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                Fields.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
