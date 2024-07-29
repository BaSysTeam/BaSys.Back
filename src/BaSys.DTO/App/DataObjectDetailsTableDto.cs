using BaSys.DAL.Models.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.App
{
    public sealed class DataObjectDetailsTableDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid Uid { get; set; }

        public List<Dictionary<string, object>> Rows { get; set; } = new List<Dictionary<string, object>>();

        public DataObjectDetailsTableDto()
        {

        }

        public DataObjectDetailsTableDto(DataObjectDetailsTable detailsTable)
        {
            Name = detailsTable.Name;
            Uid = detailsTable.Uid;

            foreach (var row in detailsTable.Rows)
            {
                Rows.Add(row.Fields);
            }
        }
    }
}
