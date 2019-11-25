using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics365.UIAutomation.Api.UCI
{
    public static class DwReference
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
        }
        public static class Case
        {
            public static string CustomerRaisedComplaint = "dw_customerraisedcomplaint";
            public static string PatientInvolvement = "dw_patientinvolvement";
            public static string ProductMalfunction = "dw_productmalfunction";
            public static string DeviceTested = "dw_devicetested";
            public static string DescriptionOfEvent = "dw_descriptionofevent";
            public static string DateTimeOfEvent = "dw_datetimeofevent";
            public static string InitialReporterRelationship = "dw_initialreporterrelationship";
            public static string InjuryReported = "dw_injuryreported";
            public static string GeneratedAlarms = "dw_generatedalarms";
            public static string MaterialAvailability = "dw_materialavailability";
            public static string OccurenceOfEvent = "dw_occurenceofevent";
            public static string PatientPersionInjured ="dw_patientpersoninjured";
            public static string UserfacilityCareport ="dw_userfacilitycareport";

        }
    }
}
