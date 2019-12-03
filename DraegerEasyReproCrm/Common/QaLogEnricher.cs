using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Dynamics365.Testautomation.Common
{
    class QaLogEnricher : ILogEventEnricher
    {
        TestContext _testContext;
        readonly LogEventProperty 
            Application, 
            Interface, 
            MethodName, 
            ClassName,
            Status,
            GUID,
            TestCaseInfo;

        public QaLogEnricher(TestContext testContext)
        {
            _testContext = testContext;
            Application = new LogEventProperty("Application", new ScalarValue("Dynamics 365"));
            Interface = new LogEventProperty("Interface", new ScalarValue("Unified Client Interface"));
            MethodName = new LogEventProperty("MethodName", new ScalarValue(_testContext.TestName));
            ClassName = new LogEventProperty("ClassName", new ScalarValue(_testContext.FullyQualifiedTestClassName));

            //Failed = 0,       Der Test wurde ausgeführt, aber es gab Probleme. Möglicherweise liegen Ausnahmen oder Assertionsfehler vor.
            //Inconclusive = 1, Der Test wurde abgeschlossen, es lässt sich aber nicht sagen, ob er bestanden wurde oder fehlerhaft war. Kann für abgebrochene Tests verwendet werden.
            //Passed = 2,       Der Test wurde ohne Probleme ausgeführt.
            //InProgress = 3,   Der Test wird zurzeit ausgeführt.
            //Error = 4,        Systemfehler beim Versuch, einen Test auszuführen.
            //Timeout = 5,      Timeout des Tests.
            //Aborted = 6,      Der Test wurde vom Benutzer abgebrochen.
            //Unknown = 7,NotRunnable = 8   Der Test weist einen unbekannten Zustand auf.
            Status = new LogEventProperty("Status", new ScalarValue(_testContext.CurrentTestOutcome));
            //unique testcase guid TODO: Testsuite guid
            GUID = new LogEventProperty("GUID", new ScalarValue(Guid.NewGuid().ToString()));
            
            //contains info from azure devops TODO: Currently running with PAT, we need an AppRegistration
            TestCaseInfo = new LogEventProperty("TestCaseInfo", new DictionaryValue(new DevOpsConnector().GetTestCaseEnrichInfo(int.Parse(GetProperty("TestCaseId")))));
        }

        // Catches missing properties
        string GetProperty(string property)
        {
            if(_testContext.Properties.Contains(property))
                return _testContext.Properties[property].ToString();
            return "";
        }

        // Default set of information which should be shown on every log line
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(this.GUID);
            logEvent.AddPropertyIfAbsent(this.Application);
            logEvent.AddPropertyIfAbsent(this.Interface);
            logEvent.AddPropertyIfAbsent(this.ClassName);
            logEvent.AddPropertyIfAbsent(this.MethodName);
            logEvent.AddOrUpdateProperty(this.Status);
            logEvent.AddPropertyIfAbsent(this.TestCaseInfo);

        }
    }
}
