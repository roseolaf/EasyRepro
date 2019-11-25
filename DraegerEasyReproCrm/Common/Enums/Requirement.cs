using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draeger.Dynamics365.Testautomation.Common.Enums
{
    public static class Requirement
    {
        public enum Status
        {
            None = 0,
            Required1 = 1,
            Required2 = 2,
            Recommended = 3
        }

        public enum Availability    
        {
            NotAvailable,
            Available
        }
    }
}
