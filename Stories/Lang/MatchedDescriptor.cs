using Agents;
using Identification;
using Instanciation;
using Items;
using Languager;
using Languager.Extensions;
using Mapping;
using Rolling;
using RollingLang;
using Saver;

namespace Stories.Lang
{
    public abstract class MatchedDescriptor<T> : Descriptor<T>
        where T : class, IIdentifiable
    {
        protected MatchedDescriptor(T elem)
            : base(elem)
        {
        }

        public override string Name => elem.Id;

        public override string Description => elem.Id;

        public string MatchedDescription<A, I, M>(
            Roles roles,
            Existents<A, I, M> existents)
            where A : IAgent, ISavable, ICloneable
            where I : IItem, ISavable, ICloneable
            where M : IMapped, ISavable, ICloneable
        {
            var rolesDescriptor = new RolesDescriptor(roles);
            var map = rolesDescriptor.Map(existents);

            var description = $"{elem.Id}_description".trans();

            foreach (var (role, name) in map)
                description = description.Replace("{" + role + "}", name);

            return description;
        }
    }
}
