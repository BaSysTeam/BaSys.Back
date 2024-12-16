using BaSys.Logging.InMemory;
using Jint;
using Jint.Native;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaSys.Core.Services.RecordsBuilder
{
    public sealed class JintExpressionEvaluator
    {

        private readonly Engine _engine;
        private readonly InMemoryLogger _logger;

        public JintExpressionEvaluator(InMemoryLogger logger, int timeout = 0)
        {

            // Create a Jint Engine instance
            _engine = new Engine(cfg =>
            {
                cfg.AllowClr();

                if (timeout > 0)
                {
                    cfg.TimeoutInterval(TimeSpan.FromSeconds(timeout));
                }

            });
            _engine.Execute(jsFunction.isEmpty);
            _engine.Execute(jsFunction.isNotEmpty);
            _engine.Execute(jsFunction.iif);
            _engine.Execute(jsFunction.ifs);
            _engine.Execute(jsFunction.dateTimeNow);
            _engine.Execute(jsFunction.dateDifference);
            _engine.Execute(jsFunction.dateExtensions);

            if (_logger != null)
            {
                _engine.SetValue("_log", _logger);
            }

            _logger = logger;

        }

        public void SetValue(string name, object value)
        {
            if (value is DateTime dateTimeValue)
            {
                _engine.SetValue(name, ConvertToJsDate(dateTimeValue));
            }
            else if (value is IDictionary<string, object?> dictionary)
            {
                var convertResult = ConvertDatesInDictionary(dictionary);
                _engine.SetValue(name, convertResult);

            }
            else
            {
                _engine.SetValue(name, value);
            }
            
        }

        public object? Evaluate(string expression)
        {
            object? value = null;
            try
            {
                var evalResult = _engine.Evaluate(expression);
                value = Cast(evalResult);
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError("Cannot eval expression: {0}. Message: {1}.", expression, ex.Message);
                }

            }
            return value;

        }

        public T Evaluate<T>(string expression)
        {

            var value = Evaluate(expression);

            if (value is T castValue)
            {
                return castValue;
            }
            else
            {
                return default(T);
            }

        }

        private JsValue ConvertToJsDate(DateTime dateTime)
        {
            var unixTimeMilliseconds = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            // Create JS Date via JINT.
            return _engine.Evaluate($"new Date({unixTimeMilliseconds})");
        }

        private IDictionary<string, object?> ConvertDatesInDictionary(IDictionary<string, object?> dictionary)
        {
            var convertResult = new Dictionary<string, object?>();
            foreach(var kvp in dictionary)
            {
                if (kvp.Value is DateTime dataTimeValue)
                {
                    convertResult.Add(kvp.Key, ConvertToJsDate(dataTimeValue));
                }
                else if (kvp.Value is IDictionary<string, object?> dictValue)
                {
                    convertResult.Add(kvp.Key, ConvertDatesInDictionary(dictValue));
                }
                else
                {
                    convertResult.Add(kvp.Key, kvp.Value);
                }
            }
            return convertResult;
        }

        private object? Cast(JsValue evalResult)
        {
            object? value = null;

            if (evalResult.IsPrimitive())
            {
                if (evalResult.IsNumber())
                {
                    value = (decimal)evalResult.AsNumber();
                }
                else if (evalResult.IsDate())
                {
                    value = evalResult.AsDate();
                }
                else if (evalResult.IsBoolean())
                {
                    value = evalResult.AsBoolean();
                }
                else if (evalResult.IsString())
                {
                    value = evalResult.AsString();
                }
            }
            else
            {
                value = evalResult.ToObject();
            }

            return value;
        }
    }
}
