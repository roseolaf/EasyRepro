using System;

namespace Draeger.Dynamics365.Testautomation.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCaseAttribute : Attribute
    {
        public int WorkItem { get; set; }

        public string Title { get; set; }
    }
}