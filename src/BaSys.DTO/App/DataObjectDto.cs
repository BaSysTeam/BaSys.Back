using BaSys.DAL.Models.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.App
{
    public sealed class DataObjectDto
    {
        public Dictionary<string, object> Header { get; set; } = new Dictionary<string, object>();
        public List<DataObjectDetailsTableDto> DetailsTables { get; set; } = new List<DataObjectDetailsTableDto>();

        public DataObjectDto()
        {

        }

        public DataObjectDto(IDictionary<string, object> data)
        {
            foreach (var key in data.Keys)
            {
                Header[key] = data[key];
            }
        }

        public DataObjectDto(DataObject dataObject)
        {
            foreach (var kvp in dataObject.Header)
            {
                Header[kvp.Key] = dataObject.Header[kvp.Key];
            }
        }

        public DataObject ToObject()
        {
            var dataObject = new DataObject(Header);
            foreach (var sourceTable in DetailsTables)
            {
                var destinationTable = sourceTable.ToObject();
                dataObject.DetailTables.Add(destinationTable);
            }

            return dataObject;
        }
    }
}
