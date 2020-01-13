using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.Extensions;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace TaADOLog.Logger.Sink
{
    public class ListSinkMSTest : ILogEventSink
    {
        private readonly MessageTemplateTextFormatter _formatter;
        private IList<ListSinkInfo<TestContext>> _sinkList;
        private TestContext _testContext;

        public ListSinkMSTest(MessageTemplateTextFormatter formatter,IList<ListSinkInfo<TestContext>> sinkList,  TestContext testContext)
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

            var logInfoLine = new ListSinkInfo<TestContext>
            {
                LogEvent = logEvent,
                DateTime = logTime,
                Step = step,
                State = _testContext.CurrentTestOutcome.ToString(),
                Message = message,
                Level = logEvent.Level.ToString(),
                Url = _testContext.Properties.Contains("WebClient") ? (_testContext.Properties["WebClient"] as WebClient).Browser.Driver.Url: "",
                Multimedia = (_testContext.Properties.Contains("WebClient") && (logEvent.Level >= LogEventLevel.Information || message.Contains(SerilogExtensions.VerboseScreenshot))) ?(new Screenshot()).SaveScreenshot(_testContext.Properties["WebClient"] as WebClient, _testContext) : "",
                Properties = logEvent.Properties,
                TestContext = _testContext
            };

            _sinkList.Add(logInfoLine);

            //var link = $"{uri.Scheme}://{uri.Authority}/main.aspx?appid={appId}&etn={entityName}&pagetype=entityrecord&id={id}";
            // Add URL to entity id
            if (_testContext.Properties.Contains("Scheme"))
            {
                foreach (var line in _sinkList)
                {
                    if (line.Properties.ContainsKey("EntityId"))
                    {
                        var entityId = line.Properties["EntityId"].ToString().Replace("\"", "");
                        var entity = line.Properties["Entity"].ToString().Replace("\"", "");
                        var url = $"{_testContext.Properties["Scheme"]}://{_testContext.Properties["Authority"]}/main.aspx?appid={_testContext.Properties["AppId"]}&etn={entity}&pagetype=entityrecord&id={entityId}";
                        line.EntityUrl = url;

                        var defaultPrefix = line.Properties["defaultPrefix"].ToString().Replace("\"", "");

                        line.Message = line.Message.Replace(defaultPrefix, $"<a href=\"{url}\">{defaultPrefix}</a>");
                    }
                }
            }
        }

        public static ILogEventSink For(IList<ListSinkInfo<TestContext>> sinkList,  TestContext testContext)
        {
            var formatter = new MessageTemplateTextFormatter(DefaultOutputTemplate, null);

            return new ListSinkMSTest(formatter, sinkList, testContext);
        }
        internal const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";


    }
}
