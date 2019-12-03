using Infoman.Xrm.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator
{
    public class OwnerDecorator : EntityManagerDecorator
    {
        public OwnerDecorator(EntityManagerComponent entityManagerComponent) : base(entityManagerComponent)
        {
        }

        public OwnerDecorator(string ownerEmail, EntityManagerComponent entityManagerComponent) : base(
            entityManagerComponent)
        {
            this.OwnerEmail = ownerEmail;

        }

        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {
            GetOwner(caller);
            entity.Attributes["ownerid"] = new EntityReference(SystemUser.EntityLogicalName, Owner.Id);

            InformationDescription += $"Set owner to {OwnerEmail} \\n";
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

        private void GetOwner(object caller)
        {
            ServiceContext context = CrmConnection.Instance.GetContext();
            Owner = context.SystemUserSet.First(sysUser => sysUser.DomainName.Equals(OwnerEmail));                   
            
        }
    }
}
