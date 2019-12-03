using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Draeger.Dynamics365.Testautomation.Common.Helper;
using Microsoft.Xrm.Sdk;
using static Draeger.Dynamics365.Testautomation.Common.Enums.Global;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager.Decorator
{
    public class SpecificAttributeDecorator :EntityManagerDecorator
    {

     
        public SpecificAttributeDecorator(string attributeName, object attributeValue, ReplaceType replaceType, char charToReplace, EntityManagerComponent entityManagerComponent) : base(entityManagerComponent)
        {
            this.AttributeName = attributeName;
            this.AttributeValue = attributeValue;
            this.ReplaceType = replaceType;
            this.CharToReplace = charToReplace;
        }

        public override Entity CreateEntityRecord<T>(object caller, T entity)
        {
            switch (ReplaceType)
            {
                case ReplaceType.None:
                    break;
                case ReplaceType.Number:
                    Random random = new Random();
                    AttributeValue = Regex.Replace(AttributeValue.ToString(), CharToReplace.ToString(), (match) =>
                        string.Format("{0}", random.Next(0,9)));
                    break;
                case ReplaceType.Letter:
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                    random = new Random();
                    AttributeValue = Regex.Replace(AttributeValue.ToString(), CharToReplace.ToString(), (match) =>
                        string.Format("{0}", chars[random.Next(chars.Length)-1]));
                    break;
                default:
                    break;
            }

            entity.Attributes[AttributeName] = AttributeValue;
            InformationDescription += $"Specified attribute {AttributeName} with value {AttributeValue}. {Environment.NewLine}";
            return base.CreateEntityRecord(caller, entity);
        }
        public override Entity CloneEntityRecord(string entityName, params KeyValuePair<string, object>[] attributes)
        {
            throw new NotImplementedException();
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
