using BaSys.Logging.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Workflows
{
    public sealed class BaSysWorkflowData
    {
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public List<InMemoryLogMessage> Log { get; set; } = new List<InMemoryLogMessage>();

    }
}
