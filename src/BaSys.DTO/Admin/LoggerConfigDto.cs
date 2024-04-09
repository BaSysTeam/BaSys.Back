using BaSys.Common.Enums;
using BaSys.DAL.Models.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Admin
{
    public sealed class LoggerConfigDto
    {
        public string Uid { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public LoggerTypes? LoggerType { get; set; }
        public EventTypeLevels MinimumLogLevel { get; set; }
        public string? ConnectionString { get; set; }
        public AutoClearInterval AutoClearInterval { get; set; } = AutoClearInterval.Month;

        public LoggerConfigDto()
        {
            
        }

        public LoggerConfigDto(LoggerConfig model)
        {
            Uid = model.Uid.ToString();
            IsEnabled = model.IsEnabled;
            LoggerType = model.LoggerType;
            MinimumLogLevel = model.MinimumLogLevel;
            ConnectionString = model.ConnectionString;
            AutoClearInterval = model.AutoClearInterval;
        }

        public LoggerConfig ToModel()
        {
            var model = new LoggerConfig
            {
                AutoClearInterval = AutoClearInterval,
                ConnectionString = ConnectionString,
                IsEnabled = IsEnabled,
                LoggerType = LoggerType,
                MinimumLogLevel = MinimumLogLevel
            };

            if (Guid.TryParse(Uid, out var uid))
                model.Uid = uid;

            return model;
        }
    }
}
