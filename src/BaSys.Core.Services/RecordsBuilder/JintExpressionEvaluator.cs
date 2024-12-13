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

            if (_logger != null)
            {
                _engine.SetValue("_log", _logger);
            }

            _logger = logger;

        }

        public void SetValue(string name, object value)
        {
            _engine.SetValue(name, value);
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
