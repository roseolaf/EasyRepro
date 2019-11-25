using System;

namespace Draeger.Dynamics365.Testautomation.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AcceptanceCriteriaAttribute : Attribute
    {
        public int WorkItem { get; set; }
        public string Description { get; set; }
        public int Step { get; set; }
        public int Sort { get; set; }
    }
}