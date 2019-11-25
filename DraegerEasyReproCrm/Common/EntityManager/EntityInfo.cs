using System;

namespace Draeger.Dynamics365.Testautomation.Common.EntityManager
{
    public class EntityInfo: Tuple<Guid, string, string>
    {
        public EntityInfo(Guid guid, string name, string entityName) : base(guid, name, entityName)
        {
        }

        public Guid Guid => Item1;

        public string Name => Item2;

        public string EntityName => Item3;
    }
}
