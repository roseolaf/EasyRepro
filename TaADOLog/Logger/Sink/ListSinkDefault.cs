using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.Extensions;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace TaADOLog.Logger.Sink
{
    public class ListSinkDefault : ILogEventSink
    {
        private readonly MessageTemplateTextFormatter _formatter;
        private IList<ListSinkInfo<IDictionary<string,object>>> _sinkList;
        private IDictionary<string, object> _testContext;

        public ListSinkDefault(MessageTemplateTextFormatter formatter,IList<ListSinkInfo<IDictionary<string, object>>> sinkList, IDictionary<string, object> testContext)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _sinkList = sinkList;
            _testContext = testContext;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }
            
            var msgWriter = new StringWriter();
            var msgformatter = new MessageTemplateTextFormatter("{Message}", null);
            msgformatter.Format(logEvent,msgWriter);

            var stepRegex = new Regex(@"Step\d+");
            var msgRegex = new Regex(@"Step\d+:");
            var step = stepRegex.Match(msgWriter.ToString()).Value;
            var message =  msgRegex.Replace(msgWriter.ToString(), "");

            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            var logTime = TimeZoneInfo.ConvertTime(logEvent.Timestamp, info).DateTime;

            var logInfoLine = new ListSinkInfo<IDictionary<string, object>>
            {
                LogEvent = logEvent,
                DateTime = logTime,
                Step = step,
                State = _testContext["State"].ToString(),
                Message = message,
                Level = logEvent.Level.ToString(),
                Url = _testContext.ContainsKey("Url") ? _testContext["Url"].ToString(): "",
                Multimedia = (_testContext.ContainsKey("Multimedia") && logEvent.Level >= LogEventLevel.Information) ?_testContext["Multmedia"].ToString() : "",
                Properties = logEvent.Properties,
                TestContext = _testContext
            };

            _sinkList.Add(logInfoLine);
            
        }

        public static ILogEventSink For(IList<ListSinkInfo<IDictionary<string, object>>> sinkList, IDictionary<string, object> testContext)
        {
            var formatter = new MessageTemplateTextFormatter(DefaultOutputTemplate, null);

            return new ListSinkDefault(formatter, sinkList, testContext);
        }
        internal const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";


    }
}
