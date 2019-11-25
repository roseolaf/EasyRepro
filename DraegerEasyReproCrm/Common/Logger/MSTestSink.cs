using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public class MSTestSink : ILogEventSink
    {
        private readonly MessageTemplateTextFormatter _formatter;
        private TestContext _testContext;

        public MSTestSink(MessageTemplateTextFormatter formatter, TestContext testContext)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            _formatter = formatter;
            _testContext = testContext;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            var writer = new StringWriter();
            _formatter.Format(logEvent, writer);

            _testContext.WriteLine(writer.ToString());
        }

    }
}
