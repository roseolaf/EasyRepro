using System.Collections.Generic;
using Draeger.Dynamics365.Testautomation.DTO;

namespace Draeger.Dynamics365.Testautomation.Common
{
    public class TestAutomationContext
    {
        public int TestCaseWorkItem { get; set; }

        public List<AcceptanceCriteria> AcceptanceCriteria { get; set; }

        public List<CrmField> Fields { get; set; }

        public Dictionary<string, object> Objects { get; set; }

        public TestAutomationContext()
        {
            AcceptanceCriteria = new List<AcceptanceCriteria>();
            Fields = new List<CrmField>();
        }
    }
}