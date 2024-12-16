using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Core.Features.RecordsBuilder
{
    public sealed class JsDateWrapper
    {
        private readonly DateTime _dateTime;

        public JsDateWrapper(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public object ToJsDate()
        {
            return new DateTimeOffset(_dateTime).ToUnixTimeMilliseconds();
        }
    }
}
