using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.IntegrationTests.Models
{
    internal class ExampleRecordIntKey
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;   
        public decimal Amount { get; set; }
    }
}
