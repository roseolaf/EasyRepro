using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoman.Xrm.Services;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator
{
    public class CloseIncidentDecorator : EntityManagerDecorator
    {
        public CloseIncidentDecorator(EntityManagerComponent entityManagerComponent) : base(entityManagerComponent)
        {
        }

        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {
            var retVal = base.CreateEntityRecord(caller, entity);

            var incidentResolution = new IncidentResolution
            {
                Subject = this.DefaultPrefix,
                IncidentId = new EntityReference(retVal.LogicalName, retVal.Id)
            };

            // Close the incident with the resolution.
            var closeIncidentRequest = new CloseIncidentRequest
            {
                IncidentResolution = incidentResolution,
                Status = new OptionSetValue((int)IncidentStatusCode.Resolved_ProblemSolved_2)
            };

            ServiceContext context = AdminConnection.Instance.GetContext();
            context.Execute(closeIncidentRequest);
              
            return retVal;
        }

        public override Entity GetEntityRecord(object caller, string entityLogicalName, Guid guid)
        {
            throw new NotImplementedException();
        }

        public override Entity UpdateEntityRecord<T>(object caller, T entity, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
