﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.App
{
    public sealed class DataObjectDetailTableRow
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
                    else if (value is int intValue)
                    {
                        return intValue;
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
    }
}