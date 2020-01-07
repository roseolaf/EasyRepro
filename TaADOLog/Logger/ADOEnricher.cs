using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaADOLog.Logger
{
    public class ADOEnricher: ILogEventEnricher
    {
        TestContext _testContext;

        private readonly List<LogEventProperty> logEventProperties;
            //Application,
            //Interface,
            //MethodName,
            //ClassName,
            //Status,
            //GUID,
            //TestCaseInfo;

        public ADOEnricher(TestContext testContext)
        {
            _testContext = testContext;
            logEventProperties = new List<LogEventProperty>();
            logEventProperties.Add(new LogEventProperty("Application", new ScalarValue("Dynamics 365")));
            logEventProperties.Add(new LogEventProperty("Interface", new ScalarValue("Unified Client Interface")));
            logEventProperties.Add(new LogEventProperty("MethodName", new ScalarValue(_testContext.TestName)));
            logEventProperties.Add(new LogEventProperty("ClassName", new ScalarValue(_testContext.FullyQualifiedTestClassName)));

            //Failed = 0,       Der Test wurde ausgeführt, aber es gab Probleme. Möglicherweise liegen Ausnahmen oder Assertionsfehler vor.
            //Inconclusive = 1, Der Test wurde abgeschlossen, es lässt sich aber nicht sagen, ob er bestanden wurde oder fehlerhaft war. Kann für abgebrochene Tests verwendet werden.
            //Passed = 2,       Der Test wurde ohne Probleme ausgeführt.
            //InProgress = 3,   Der Test wird zurzeit ausgeführt.
            //Error = 4,        Systemfehler beim Versuch, einen Test auszuführen.
            //Timeout = 5,      Timeout des Tests.
            //Aborted = 6,      Der Test wurde vom Benutzer abgebrochen.
            //Unknown = 7,NotRunnable = 8   Der Test weist einen unbekannten Zustand auf.
            logEventProperties.Add(new LogEventProperty("Status", new ScalarValue(_testContext.CurrentTestOutcome)));
            //unique testcase guid TODO: Testsuite guid
            logEventProperties.Add(new LogEventProperty("GUID", new ScalarValue(Guid.NewGuid().ToString())));

            //contains info from azure devops TODO: Currently running with PAT, we need an AppRegistration
            logEventProperties.Add(new LogEventProperty("TestCaseInfo", new DictionaryValue(new TestCaseInfo().GetTestCaseEnrichInfo(int.Parse(GetProperty("TestCaseId"))))));
        } 
        public ADOEnricher(IDictionary<string, object> testContext)
        {

            logEventProperties = new List<LogEventProperty>();
            //Application = new LogEventProperty("Application", new ScalarValue("Dynamics 365"));
            //Interface = new LogEventProperty("Interface", new ScalarValue("Unified Client Interface"));
            //MethodName = new LogEventProperty("MethodName", new ScalarValue(_testContext.TestName));
            //ClassName = new LogEventProperty("ClassName", new ScalarValue(_testContext.FullyQualifiedTestClassName));

            ////Failed = 0,       Der Test wurde ausgeführt, aber es gab Probleme. Möglicherweise liegen Ausnahmen oder Assertionsfehler vor.
            ////Inconclusive = 1, Der Test wurde abgeschlossen, es lässt sich aber nicht sagen, ob er bestanden wurde oder fehlerhaft war. Kann für abgebrochene Tests verwendet werden.
            ////Passed = 2,       Der Test wurde ohne Probleme ausgeführt.
            ////InProgress = 3,   Der Test wird zurzeit ausgeführt.
            ////Error = 4,        Systemfehler beim Versuch, einen Test auszuführen.
            ////Timeout = 5,      Timeout des Tests.
            ////Aborted = 6,      Der Test wurde vom Benutzer abgebrochen.
            ////Unknown = 7,NotRunnable = 8   Der Test weist einen unbekannten Zustand auf.
            //Status = new LogEventProperty("Status", new ScalarValue(_testContext.CurrentTestOutcome));
            ////unique testcase guid TODO: Testsuite guid
            logEventProperties.Add(new LogEventProperty("GUID", new ScalarValue(Guid.NewGuid().ToString())));

            //contains info from azure devops TODO: Currently running with PAT, we need an AppRegistration
            logEventProperties.Add(new LogEventProperty("TestCaseInfo", new DictionaryValue(new TestCaseInfo().GetTestCaseEnrichInfo(int.Parse(testContext["TestCaseId"].ToString())))));
        }

        // Catches missing properties
        string GetProperty(string property)
        {
            if (_testContext.Properties.Contains(property))
                return _testContext.Properties[property].ToString();
            return "";
        }

        // Default set of information which should be shown on every log line
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            foreach (var lEP in logEventProperties)
            {
                logEvent.AddOrUpdateProperty(lEP);
            }
            //logEvent.AddPropertyIfAbsent(this.GUID);
            //logEvent.AddPropertyIfAbsent(this.Application);
            //logEvent.AddPropertyIfAbsent(this.Interface);
            //logEvent.AddPropertyIfAbsent(this.ClassName);
            //logEvent.AddPropertyIfAbsent(this.MethodName);
            //logEvent.AddOrUpdateProperty(this.Status);
            //logEvent.AddPropertyIfAbsent(this.TestCaseInfo);

        }

    }
}