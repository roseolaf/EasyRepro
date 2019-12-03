using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoman.Xrm.Services;
using Microsoft.Xrm.Sdk;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator
{
    class SoldToDecorator : EntityManagerDecorator
    {
        public SoldToDecorator(EntityManagerComponent entityManagerComponent) : base(entityManagerComponent)
        {
        }

        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {
            entity.Attributes["dw_accountgroup"] = new OptionSetValue((int)Account_AccountGroup_OptionSet.SoldTo);

            InformationDescription += $"Account group set to sold to.{Environment.NewLine} ";

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
