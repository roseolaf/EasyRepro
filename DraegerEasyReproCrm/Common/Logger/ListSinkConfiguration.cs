using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public static class ListSinkConfiguration
    {
        const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}  {Exception}";

        public static LoggerConfiguration ListOutput(
            this LoggerSinkConfiguration sinkConfiguration,
            List<ListSinkInfo> sinkList,
            TestContext testContext,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            LoggingLevelSwitch levelSwitch = null,
            string outputTemplate = DefaultOutputTemplate)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

            return sinkConfiguration.Sink(new ListSink(formatter, sinkList, testContext), restrictedToMinimumLevel, levelSwitch);
        }

    }
}
