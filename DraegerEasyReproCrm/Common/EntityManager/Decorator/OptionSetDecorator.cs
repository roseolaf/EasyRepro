using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator
{
    class OptionSetDecorator :EntityManagerDecorator
    {

        public OptionSetDecorator(string attribute, int optionSet, EntityManagerComponent entityManagerComponent) : base(entityManagerComponent)
        {
            this.AttributeRef = attribute;
            this.OptionSetRef = optionSet;
        }

        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {            
            entity.Attributes[AttributeRef] = new OptionSetValue(OptionSetRef);

            InformationDescription += $"Set Option {AttributeRef} to {OptionSetRef} \\n";
            return base.CreateEntityRecord(caller, entity);
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
