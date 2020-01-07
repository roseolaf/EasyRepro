using System;
using System.Collections.Generic;
using Serilog.Events;

namespace TaADOLog.Logger.Sink
{
    public class ListSinkInfo<T>
    {
        public LogEvent LogEvent { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Step { get; set; }
        public string State { get; set; }
        public string Url { get; set; }
        public string EntityUrl { get; set; }
        public object Multimedia { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public T TestContext { get; set; }
        public IReadOnlyDictionary<string, LogEventPropertyValue> Properties { get; set; }
    }
}