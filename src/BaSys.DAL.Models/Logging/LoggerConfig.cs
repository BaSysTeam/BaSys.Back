using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.Logging
{
    public class LoggerConfig
    {
        public Guid Uid { get; set; }
        public bool IsEnabled { get; set; }
        public LoggerTypes? LoggerType { get; set; }
        public EventTypeLevels MinimumLogLevel { get; set; }
        public string? ConnectionString { get; set; }
        public AutoClearInterval AutoClearInterval { get; set; } = AutoClearInterval.Month;
    }
}