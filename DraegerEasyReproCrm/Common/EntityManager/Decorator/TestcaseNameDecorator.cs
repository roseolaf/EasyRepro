using Draeger.Dynamics365.Testautomation.Common.Helper;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator
{
    public class TestcaseNameDecorator : EntityManagerDecorator
    {
        public TestcaseNameDecorator(EntityManagerComponent entityManagerComponent) : base(entityManagerComponent)
        {
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {
            GenerateTestcaseName(caller);

            return base.CreateEntityRecord(caller, entity);
        }


        public override bool DeleteEntityRecord(string entity, Guid guid)
        {
            return base.DeleteEntityRecord(entity, guid);
        }

        public override Entity UpdateEntityRecord<T>(object caller, T entity, Guid guid)
        {
            throw new NotImplementedException();
        }

        public string GenerateTestcaseName(object caller)
        {
            var randomName = Generator.GenerateRandomName();
          
            this.DefaultPrefix = $"{((TestBase)caller).TestContext.Properties["TestCaseId"]}_{randomName}_";
            InformationDescription += $"Auto generated prefix: {DefaultPrefix}\\n";
            return this.DefaultPrefix;
        }

        public override Entity GetEntityRecord(object caller, string entityLogicalName, Guid guid)
        {
            throw new NotImplementedException();
        }

        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }
    }
}
