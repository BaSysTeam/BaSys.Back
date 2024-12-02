using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.InMemory
{
    public sealed class InMemoryLogMessage
    {
        public DateTime Period { get; set; }
        public EventTypeLevels Level { get; set; }
        public string Text { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Period:dd.MM.yyyy hh:mm:ss.fff} [{Level}] {Text}";
        }
    }
}
