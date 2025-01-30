using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Logging.InMemory
{
    public sealed class InMemoryLogger
    {
        private List<InMemoryLogMessage> _messages = new List<InMemoryLogMessage>();
        private EventTypeLevels _minimumLevel = EventTypeLevels.Trace;

        public IEnumerable<InMemoryLogMessage> Messages => _messages;

        public InMemoryLogger(EventTypeLevels level)
        {
            _minimumLevel = level;
        }

        public InMemoryLogger(EventTypeLevels level, List<InMemoryLogMessage> messages)
        {
            _minimumLevel = level;
            _messages = messages;
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public void SetLevel(EventTypeLevels level) { 
            _minimumLevel = level;
        }

        public void Log(EventTypeLevels level, string messageText)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            _messages.Add(new InMemoryLogMessage() { Level = level, Text = messageText });
        }

        public void Log(EventTypeLevels level, string messageTemplate, params object[] args)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            var messageText = string.Format(messageTemplate, args);

            _messages.Add(new InMemoryLogMessage() { Level = level, Text = messageText });
        }

        public void LogTrace(string messageTemplate, params object[] args)
        {
            Log(EventTypeLevels.Trace, messageTemplate, args);
        }

        public void LogDebug(string messageTemplate, params object[] args)
        {
            Log(EventTypeLevels.Debug, messageTemplate, args);
        }

        public void LogInfo(string messageTemplate, params object[] args)
        {
            Log(EventTypeLevels.Info, messageTemplate, args);
        }

        public void LogWarning(string messageTemplate, params object[] args)
        {
            Log(EventTypeLevels.Warning, messageTemplate, args);
        }

        public void LogError(string messageTemplate, params object[] args)
        {
            Log(EventTypeLevels.Error, messageTemplate, args);
        }

        public void LogCritical(string messageTemplate, params object[] args)
        {
            Log(EventTypeLevels.Critical, messageTemplate, args);
        }

        private bool IsEnabled(EventTypeLevels level)
        {
            return level >= _minimumLevel;
        }
    }
}
