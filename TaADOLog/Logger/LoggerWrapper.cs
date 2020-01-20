//using Draeger.CrmConnector.CrmOnline;

using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace TaADOLog.Logger
{
    public class LoggerWrapper : ILogger
    {
        private Serilog.Core.Logger _logger;
        private int _step;

        public LoggerWrapper(Serilog.Core.Logger logger)
        {
            _logger = logger;
            _step = 1;
        }

        public void NextStep()
        {
            _step++;
        }

        public bool BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate, out IEnumerable<LogEventProperty> boundProperties)
        {
            return _logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
        }

        public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
        {
            return _logger.BindProperty(propertyName, value, destructureObjects, out property);
        }

        public void Debug(string messageTemplate)
        {
            _logger.Debug(messageTemplate);
        }

        public void Debug<T>(string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Debug(messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Debug(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Debug(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            _logger.Debug(messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            _logger.Debug(exception, messageTemplate);
        }

        public void Debug<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Debug(exception, messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Debug(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Debug(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Debug(exception, messageTemplate, propertyValues);
        }

        public void Error(string messageTemplate)
        {
            _logger.Error($"{_step}: {messageTemplate}");
        }

        public void Error<T>(string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Error($"{_step}: {messageTemplate}", propertyValue);
        }

        public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Error($"{_step}: {messageTemplate}", propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Error($"{_step}: {messageTemplate}", propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            _logger.Error($"{_step}: {messageTemplate}", propertyValues);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            _logger.Error(exception, $"{_step}: {messageTemplate}");
        }

        public void Error<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Error(exception, $"{_step}: {messageTemplate}", propertyValue);
        }

        public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Error(exception, $"{_step}: {messageTemplate}", propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Error(exception, $"{_step}: {messageTemplate}", propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Error(exception, $"{_step}: {messageTemplate}", propertyValues);
        }

        public void Fatal(string messageTemplate)
        {
            _logger.Fatal($"{_step}: {messageTemplate}");
        }

        public void Fatal<T>(string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Fatal($"{_step}: {messageTemplate}", propertyValue);
        }

        public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Fatal($"{_step}: {messageTemplate}", propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Fatal($"{_step}: {messageTemplate}", propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
            _logger.Fatal($"{_step}: {messageTemplate}", propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
            _logger.Fatal(exception, $"{_step}: {messageTemplate}");
        }

        public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Fatal(exception, $"{_step}: {messageTemplate}", propertyValue);
        }

        public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Fatal(exception, $"{_step}: {messageTemplate}", propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Fatal(exception, $"{_step}: {messageTemplate}", propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Fatal(exception, messageTemplate, propertyValues);
        }

        public ILogger ForContext(ILogEventEnricher enricher)
        {
            _logger = (Serilog.Core.Logger)_logger.ForContext(enricher);
            return this;  
        }

        public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
        {
            _logger = (Serilog.Core.Logger)_logger.ForContext(enrichers);
            return this; 
        }

        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
        {
            _logger = (Serilog.Core.Logger)_logger.ForContext(propertyName, value, destructureObjects);
            return this; 
        }

        public ILogger ForContext<TSource>()
        {
            _logger = (Serilog.Core.Logger)((ILogger)_logger).ForContext<TSource>();
            return this;
        }

        public ILogger ForContext(Type source)
        {
            _logger = (Serilog.Core.Logger)_logger.ForContext(source);
            return this;
        }

        public void Information(string messageTemplate)
        {
            _logger.Information($"Step{_step}: {messageTemplate}");
        }

        public void Information<T>(string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Information($"Step{_step}: {messageTemplate}", propertyValue);
        }

        public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Information($"Step{_step}: {messageTemplate}", propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Information($"Step{_step}: {messageTemplate}", propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            _logger.Information($"Step{_step}: {messageTemplate}", propertyValues);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            _logger.Information(exception, $"Step{_step}: {messageTemplate}");
        }

        public void Information<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Information(exception, $"Step{_step}: {messageTemplate}", propertyValue);
        }

        public void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Information(exception, $"Step{_step}: {messageTemplate}", propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Information(exception, $"Step{_step}: {messageTemplate}", propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Information(exception, $"Step{_step}: {messageTemplate}", propertyValues);
        }

        public bool IsEnabled(LogEventLevel level)
        {
            return _logger.IsEnabled(level);
        }

        public void Verbose(string messageTemplate)
        {
            _logger.Verbose(messageTemplate);
        }

        public void Verbose<T>(string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Verbose(messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Verbose(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Verbose(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            _logger.Verbose(messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            _logger.Verbose(exception, messageTemplate);
        }

        public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Verbose(exception, messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Verbose(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Verbose(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Verbose(exception, messageTemplate, propertyValues);
        }

        public void Warning(string messageTemplate)
        {
            _logger.Warning(messageTemplate);
        }

        public void Warning<T>(string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Warning(messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Warning(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            _logger.Warning(messageTemplate, propertyValues);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            _logger.Warning(exception, messageTemplate);
        }

        public void Warning<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Warning(exception, messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Warning(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Warning(exception, messageTemplate, propertyValues);
        }

        public void Write(LogEvent logEvent)
        {
            _logger.Write(logEvent);
        }

        public void Write(LogEventLevel level, string messageTemplate)
        {
            _logger.Write(level, messageTemplate);
        }

        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Write(level, messageTemplate, propertyValue);
        }

        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Write(level, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            _logger.Write(level, messageTemplate, propertyValues);
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
        {
            _logger.Write(level, exception, messageTemplate);
        }

        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            ((ILogger)_logger).Write(level, exception, messageTemplate, propertyValue);
        }

        public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            ((ILogger)_logger).Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            ((ILogger)_logger).Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Write(level, exception, messageTemplate, propertyValues);
        }
    }
}