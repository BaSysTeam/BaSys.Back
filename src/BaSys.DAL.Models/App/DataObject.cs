using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.App
{
    public sealed class DataObject
    {
        public Dictionary<string, object> Header { get; set; } = new Dictionary<string, object>();

        public DataObject()
        {
            
        }

        public DataObject(IDictionary<string, object> header)
        {
            foreach(var kvp in header)
            {
                Header.Add(kvp.Key, kvp.Value);
            }
        }

        public void SetValue(string key, object value)
        {

            var keyLower = key.ToLower();
            if (Header.ContainsKey(keyLower))
            {
                Header[keyLower] = value;
            }
            else
            {
                Header.Add(keyLower, value);
            }
        }

        public T? GetValue<T>(string key)
        {
            if (Header.TryGetValue(key.ToLower(), out var value))
            {
                if (value is T tmp)
                {
                    return tmp;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }
        }
    }
}
