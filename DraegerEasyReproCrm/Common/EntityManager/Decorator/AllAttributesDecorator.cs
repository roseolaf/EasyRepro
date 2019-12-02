using System;
using System.Collections.Generic;
using System.Linq;
using Infoman.Xrm.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator
{
    public class AllAttributesDecorator : EntityManagerDecorator
    {
        public AllAttributesDecorator(EntityManagerComponent entityManagerComponent) : base(entityManagerComponent)
        {
        }

        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {
            ServiceContext context = CrmConnection.Instance.GetContext();

            var retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = entity.LogicalName
            };

            var retrieveEntityResponse = (RetrieveEntityResponse)context.Execute(retrieveEntityRequest);

            var entityMetadata = retrieveEntityResponse.EntityMetadata;

            foreach (var attribute in entityMetadata.Attributes.Where(a =>
                a.IsValidForCreate.Value
                && !a.IsPrimaryId.Value))
                try
                {
                    if (IgnoredAttributesList.Any(s => attribute.LogicalName.ToLower().Contains(s.ToLower())))
                        continue;

                    if (entity.Attributes.Contains(attribute.LogicalName))
                    {
                        continue;
                    }


                    var attrValueString = this.DefaultPrefix + attribute.LogicalName;
                    var timezoneCode =
                        context.TimeZoneDefinitionSet.First().TimeZoneCode ??
                        default(int); //110 = (GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna; 110
                    if (attribute is StringAttributeMetadata)
                    {
                        var attr = attribute as StringAttributeMetadata;
                        if (attrValueString.Length > attr.MaxLength)
                            attrValueString = attrValueString.Remove(attr.MaxLength ?? default(int));
                        entity.Attributes[attribute.LogicalName] =
                            attribute.LogicalName.Contains("mail") ? FieldValueEmail : attrValueString;
                        continue;
                    }

                    if (attribute is BigIntAttributeMetadata)
                    {
                        var attr = attribute as BigIntAttributeMetadata;
                        var tempIntValue = (long)FieldValueDefaultInt;
                        if (FieldValueDefaultInt > attr.MaxValue)
                            tempIntValue = attr.MaxValue ?? default(long);
                        else if (FieldValueDefaultInt < attr.MinValue)
                            tempIntValue = attr.MinValue ?? default(long);
                        entity.Attributes[attr.LogicalName] =
                            attr.LogicalName.Contains("offset") || attr.LogicalName.Contains("timezone")
                                ? timezoneCode
                                : tempIntValue;
                        continue;
                    }

                    if (attribute is IntegerAttributeMetadata)
                    {
                        var attr = attribute as IntegerAttributeMetadata;
                        var tempIntValue = FieldValueDefaultInt;
                        if (FieldValueDefaultInt > attr.MaxValue)
                            tempIntValue = attr.MaxValue ?? default(int);
                        else if (FieldValueDefaultInt < attr.MinValue)
                            tempIntValue = attr.MinValue ?? default(int);
                        entity.Attributes[attribute.LogicalName] =
                            attribute.LogicalName.Contains("offset") || attribute.LogicalName.Contains("timezone")
                                ? timezoneCode
                                : tempIntValue;
                        continue;
                    }

                    if (attribute is DateTimeAttributeMetadata)
                    {
                        entity.Attributes[attribute.LogicalName] = DateTime.Now;
                        continue;
                    }

                    if (attribute is StatusAttributeMetadata)
                    {
                        entity.Attributes[attribute.LogicalName] = 0;
                        continue;
                    }

                    //if (attribute is BooleanAttributeMetadata)
                    //{
                    //    entity.Attributes[attribute.LogicalName] = false;
                    //    Console.WriteLine($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                    //    continue;
                    //}

                    if (attribute is MoneyAttributeMetadata)
                    {
                        var money = new Money(100);
                        entity.Attributes[attribute.LogicalName] = money;
                        continue;
                    }

                    if (attribute is PicklistAttributeMetadata)
                    {
                        var picklistAttribute = attribute as PicklistAttributeMetadata;
                        var option = picklistAttribute.OptionSet.Options.First().Value ?? default(int);
                        var optionSetValue = new OptionSetValue(option);

                        entity.Attributes[attribute.LogicalName] = optionSetValue;
                    }

                    //if (attribute is StateAttributeMetadata)
                    //{
                    //    var statecodeAttribute = attribute as StateAttributeMetadata;
                    //    var statecode = statecodeAttribute.OptionSet.Options.First().Value ?? default(int);
                    //    entity.Attributes[attribute.LogicalName] = new OptionSetValue(statecode);
                    //    Console.WriteLine($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                    //    continue;
                    //}

                    //if (attribute is LookupAttributeMetadata)
                    //{
                    //    var lookupAttribute = attribute as LookupAttributeMetadata;
                    //    var entityName = lookupAttribute.Targets.First();

                    //    var entities = (from qR in context.CreateQuery(entityName)
                    //                    select qR);

                    //    entity.Attributes[attribute.LogicalName] = new EntityReference(entityName, entities.First().Id);
                    //    Console.WriteLine($"{attribute.LogicalName} : {entities.First().Id}");
                    //    continue;
                    //}
                    //Console.WriteLine($"Missing attribute {attribute.LogicalName} with type {attribute.GetType()}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error:{attribute.LogicalName} : {e.Message}");
                }



            return base.CreateEntityRecord(caller, entity);
        }


        public override bool DeleteEntityRecord(string entityName, Guid guid)
        {
            return base.DeleteEntityRecord(entityName, guid);
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