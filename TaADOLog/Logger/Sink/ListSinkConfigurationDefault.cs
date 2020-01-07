using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace TaADOLog.Logger.Sink
{
    public static class ListSinkConfigurationDefault
    {
        const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}  {Exception}";

        public static LoggerConfiguration ListOutput(
            this LoggerSinkConfiguration sinkConfiguration,
            List<ListSinkInfo<IDictionary<string,object>>> sinkList,
            IDictionary<string, object> testContext,
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

            return sinkConfiguration.Sink(new ListSinkDefault(formatter, sinkList, testContext), restrictedToMinimumLevel, levelSwitch);
        }

    }
}
