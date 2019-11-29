using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using Draeger.CrmConnector.CrmOnline;
using Draeger.Testautomation.CredentialsManagerCore.Bases;
using Infoman.Xrm.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Serilog;
using static Draeger.Dynamics365.Testautomation.Common.Enums.Global;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager
{

    public abstract class EntityManagerComponent
    {
        public static List<string> IgnoredAttributesList = new List<string>()
        {
            // ReSharper disable once StringLiteralTypo
            "traversedpath",
            // ReSharper disable once StringLiteralTypo
            "ownerid",
            // ReSharper disable once StringLiteralTypo
            "closeprobability"
        };

        public static string FieldValueEmail = "dummy@draeger.com";
        public string DefaultPrefix = "DTA-";
        public static int FieldValueDefaultInt = 23456;
        public EntityReference TransactionCurrency;
        public EntityReference Pricelevel;
        public string AttributeRef;
        public Entity EntityInfoRef;
        public int OptionSetRef;
        public string AttributeName;
        public object AttributeValue;
        public char CharToReplace;
        public ReplaceType ReplaceType;
        public string OwnerEmail = "";
        public string InformationDescription = "Creating record (for detailed attributes, see log.level debug): ";
        protected SystemUser Owner;
        //protected object createLock =  new object();
        //protected object cloneLock = new object();

        protected EntityManagerComponent()
        {

        }

        protected EntityManagerComponent(ILogger logger)
        {
            Logger = logger;
        }

        public virtual ILogger Logger { get; }

        protected static readonly object GetRecordLock = new object();


        public abstract Entity CreateEntityRecord<T>(object caller, T entity) where T : Entity;
        public abstract bool DeleteEntityRecord(string entityName, Guid guid);
        public abstract Entity UpdateEntityRecord<T>(object caller, T entity, Guid guid) where T : Entity;
        public abstract Entity GetEntityRecord(object caller, string entityLogicalName, Guid guid);
        public abstract Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes);

    }


    public class BaseComponent : EntityManagerComponent
    {
        public BaseComponent(ILogger logger) : base(logger)
        {
        }

        public BaseComponent()
        {
        }


        public override Entity GetEntityRecord(object caller, string entityLogicalName, Guid guid)
        {
            ServiceContext context = AdminConnection.Instance.GetContext();
            return context.Retrieve(entityLogicalName, guid, new ColumnSet(true));

        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {

            ServiceContext context = AdminConnection.Instance.GetContext();

            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entity.LogicalName
            };


            RetrieveEntityResponse retrieveEntityResponse =
                (RetrieveEntityResponse)context.Execute(retrieveEntityRequest);

            EntityMetadata entityMetadata = retrieveEntityResponse.EntityMetadata;



            foreach (var attribute in entityMetadata.Attributes.Where(a =>
                a.IsValidForCreate.Value
                && !a.IsPrimaryId.Value))
            {
                try
                {
                    // Skip if the attribute has a value
                    if (entity.Attributes.Contains(attribute.LogicalName))
                    {
                        Logger.Debug("{LogicalName} : {@Value}", attribute.LogicalName,
                            entity.Attributes[attribute.LogicalName]);
                        continue;
                    }

                    // Attributes to skip
                    if (IgnoredAttributesList.Any(s => attribute.LogicalName.ToLower().Contains(s.ToLower())))
                        continue;

                    //IsRequiredForForm equals IsSystemRequired ??
                    //if (!attribute.IsRequiredForForm.Value)
                    //    continue;

                    // Skip non required attributes
                    var requiredLevel = attribute.RequiredLevel.Value;
                    if (requiredLevel == AttributeRequiredLevel.None)
                        continue;

                    var attrValueString = this.DefaultPrefix + attribute.LogicalName;
                    int timezoneCode =
                        context.TimeZoneDefinitionSet.First().TimeZoneCode ??
                        default(int); //110 = (GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna; 110

                    if (attribute is StringAttributeMetadata attr)
                    {
                        if (attrValueString.Length > attr.MaxLength)
                            attrValueString = attrValueString.Remove(attr.MaxLength ?? default(int));
                        entity.Attributes[attribute.LogicalName] = attribute.LogicalName.Contains("mail")
                            ? FieldValueEmail
                            : attrValueString;
                        Logger.Debug($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                        continue;
                    }

                    if (attribute is BigIntAttributeMetadata attrBI)
                    {
                        var tempIntValue = (long)FieldValueDefaultInt;
                        if (FieldValueDefaultInt > attrBI.MaxValue)
                            tempIntValue = attrBI.MaxValue ?? default(long);
                        else if (FieldValueDefaultInt < attrBI.MinValue)
                            tempIntValue = attrBI.MinValue ?? default(long);
                        entity.Attributes[attrBI.LogicalName] =
                            attrBI.LogicalName.Contains("offset") || attrBI.LogicalName.Contains("timezone")
                                ? timezoneCode
                                : tempIntValue;
                        Logger.Debug($"{attrBI.LogicalName} : {entity.Attributes[attrBI.LogicalName]}");
                        continue;
                    }

                    if (attribute is IntegerAttributeMetadata attrI)
                    {
                        var tempIntValue = FieldValueDefaultInt;
                        if (FieldValueDefaultInt > attrI.MaxValue)
                            tempIntValue = attrI.MaxValue ?? default(int);
                        else if (FieldValueDefaultInt < attrI.MinValue)
                            tempIntValue = attrI.MinValue ?? default(int);
                        entity.Attributes[attribute.LogicalName] =
                            attribute.LogicalName.Contains("offset") || attribute.LogicalName.Contains("timezone")
                                ? timezoneCode
                                : tempIntValue;

                        Logger.Debug($"{attrI.LogicalName} : {entity.Attributes[attrI.LogicalName]}");
                        continue;
                    }

                    if (attribute is DateTimeAttributeMetadata)
                    {
                        entity.Attributes[attribute.LogicalName] = DateTime.Now;
                        Logger.Debug($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                        continue;
                    }

                    if (attribute is StatusAttributeMetadata)
                    {
                        var statuscodeAttribute = attribute as StatusAttributeMetadata;
                        var statuscode = statuscodeAttribute.OptionSet.Options.First().Value ?? default(int);
                        entity.Attributes[attribute.LogicalName] = new OptionSetValue(statuscode);
                        Logger.Debug($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                        continue;
                    }

                    if (attribute is BooleanAttributeMetadata)
                    {
                        entity.Attributes[attribute.LogicalName] = false;
                        Logger.Debug($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                        continue;
                    }

                    if (attribute is MoneyAttributeMetadata)
                    {
                        Money money = new Money(100);
                        entity.Attributes[attribute.LogicalName] = money;
                        Logger.Debug($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                        continue;

                    }

                    if (attribute is PicklistAttributeMetadata)
                    {
                        var picklistAttribute = attribute as PicklistAttributeMetadata;
                        var option = picklistAttribute.OptionSet.Options.First().Value ?? default(int);
                        var optionSetValue = new OptionSetValue(option);

                        entity.Attributes[attribute.LogicalName] = optionSetValue;
                        Logger.Debug($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                        continue;
                    }

                    if (attribute is StateAttributeMetadata)
                    {
                        var statecodeAttribute = attribute as StateAttributeMetadata;
                        var statecode = statecodeAttribute.OptionSet.Options.First().Value ?? default(int);
                        entity.Attributes[attribute.LogicalName] = new OptionSetValue(statecode);
                        Logger.Debug($"{attribute.LogicalName} : {entity.Attributes[attribute.LogicalName]}");
                        continue;
                    }

                    if (attribute is LookupAttributeMetadata)
                    {
                        var lookupAttribute = attribute as LookupAttributeMetadata;
                        var entityName = lookupAttribute.Targets.First();


                        var entities = (from qR in context.CreateQuery(entityName)
                                        select qR);

                        EntityReference eR;
                        if (entityName == "transactioncurrency")
                        {
                            if (this.Pricelevel != null)
                            {
                                PriceLevel level = (PriceLevel)context.Retrieve(
                                    this.Pricelevel.LogicalName,
                                    this.Pricelevel.Id,
                                    new ColumnSet(true));
                                eR = level.TransactionCurrencyId;
                                this.TransactionCurrency = eR;
                            }
                            else
                            {
                                eR = this.TransactionCurrency == null
                                    ? new EntityReference(entityName, entities.First().Id)
                                    : this.TransactionCurrency;
                                this.TransactionCurrency = eR;
                            }
                        }
                        else if (entityName == "pricelevel")
                        {
                            if (this.TransactionCurrency != null)
                            {
                                eR = new EntityReference(entityName, entities.First(x =>
                                    x.Attributes["transactioncurrencyid"]
                                        .Equals(this.TransactionCurrency.Id)).Id);
                                this.Pricelevel = eR;
                            }
                            else
                            {
                                eR = this.Pricelevel == null
                                    ? new EntityReference(entityName, entities.First().Id)
                                    : this.Pricelevel;
                                this.Pricelevel = eR;
                            }
                        }
                        else
                            eR = new EntityReference(entityName, entities.First().Id);



                        entity.Attributes[attribute.LogicalName] = eR;
                        Logger.Debug($"{attribute.LogicalName} : {entities.First().Id}");
                        continue;
                    }

                    Logger.Warning($"Missing attribute {attribute.LogicalName} with type {attribute.GetType()}");

                }
                catch (Exception e)
                {
                    Logger.Error($"{attribute.LogicalName} : {e.Message}");
                }

            }

            var create = context.Create(entity);
            var result = GetEntityRecord(this, entity.LogicalName, create);

            Logger.Information("{informationDescription} {Entity} with name {defaultPrefix} and Guid {EntityId}",
                InformationDescription, entity.LogicalName, this.DefaultPrefix, create.ToString());
            return result;

        }


        public override Entity UpdateEntityRecord<T>(object caller, T entity, Guid guid)
        {
            throw new NotImplementedException();
        }


        public override bool DeleteEntityRecord(string entityName, Guid guid)
        {

            ServiceContext context = AdminConnection.Instance.GetContext();
            try
            {
                context.Delete(entityName, guid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return true;


        }

        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {

            ServiceContext context = AdminConnection.Instance.GetContext();

            var query = new QueryExpression
            {
                EntityName = entityName,
                ColumnSet = new ColumnSet(true)
            };

            foreach (var attr in attributes)
            {
                query.Criteria.AddCondition(attr.Key, ConditionOperator.Equal, attr.Value);
            }

            var entities = context.RetrieveMultiple(query);
            var entityToClone = entities.Entities.First();
            return entityToClone.CloneEntity();


        }
    }

    public abstract class EntityManagerDecorator : EntityManagerComponent
    {
        protected EntityManagerComponent _entityManagerComponent;
        public new string DefaultPrefix
        {
            get
            {
                if (_entityManagerComponent is EntityManagerDecorator deco)
                    return deco.DefaultPrefix;
                return _entityManagerComponent.DefaultPrefix;
            }
            set
            {
                if (_entityManagerComponent is EntityManagerDecorator deco)
                    deco.DefaultPrefix = value;
                else
                    _entityManagerComponent.DefaultPrefix = value;
            }
        }

        public override ILogger Logger { get => _entityManagerComponent.Logger; }

        public EntityManagerDecorator(EntityManagerComponent entityManagerComponent)
        {
            this._entityManagerComponent = entityManagerComponent;
        }

        public void SetComponent(EntityManagerComponent entityManagerComponent)
        {
            this._entityManagerComponent = entityManagerComponent;
        }

        public override bool DeleteEntityRecord(string entityName, Guid guid)
        {
            if (this._entityManagerComponent != null)
            {
                return this._entityManagerComponent.DeleteEntityRecord(entityName, guid);
            }
            else
            {
                return false;
            }
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {
            //lock (CreateRecordLock)
            {
                if (this._entityManagerComponent != null)
                {
                    return this._entityManagerComponent.CreateEntityRecord(caller, entity);
                }
                else
                {
                    return new Entity();
                }
            }
        }


    }

    public class EntityManager
    {

    }

    public class AdminConnection : Singleton<AdminConnection>, IDisposable
    {
        private Microsoft.Xrm.Sdk.Client.OrganizationServiceProxy proxy = null;
        private readonly object _proxyLock = new object();

        public Microsoft.Xrm.Sdk.Client.OrganizationServiceProxy OpenConnection()
        {
            if (proxy != null) return proxy;

            var connectionString = ConfigurationManager.ConnectionStrings["AdminConnection"]?.ConnectionString;

            lock (_proxyLock)
            {
                try
                {
                    // var connectionProvider = new CrmConnectionProvider();
                    // proxy = connectionProvider.ConnectWithCredentials(connectionString);
                    //
                    // proxy.EnableProxyTypes();

                    int tries = 10;

                    CrmServiceClient serviceClient =null;
                    for (int i = 0; i < tries; i++)
                    {
                        try
                        {

                            serviceClient = new CrmServiceClient(connectionString);
                            proxy = serviceClient.OrganizationServiceProxy;
                            var error = serviceClient.LastCrmError;
                            Console.WriteLine(error);
                            if (proxy != null)
                                break;
                            Console.WriteLine($"Could not establish connection. Retry in 500ms {i}/{tries}");
                            Thread.Sleep(500);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    proxy.EnableProxyTypes();
                    proxy.Timeout = new TimeSpan(1, 0, 0);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return proxy;
        }

        public ServiceContext GetContext()
        {
            lock (_proxyLock)
            {
                if (proxy == null) OpenConnection();
            }
            return new ServiceContext(proxy, null);
        }

        public void Dispose()
        {
            lock (_proxyLock)
            {
                proxy?.Dispose();
            }
        }
    }

    public static class EntityExtentions
    {
        public static Entity CloneEntity(this Entity inputEntity)
        {
            var copyEntity = inputEntity;
            copyEntity.Id = Guid.NewGuid();
            return copyEntity;
        }
    }
}

