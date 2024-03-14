using Agents;
using Instanciation;
using Items;
using Languager;
using Mapping;
using Rolling;
using Saver;

namespace Stories.Lang
{
    public class StoryletDescriptor : Descriptor<IStorylet>
    {
        public StoryletDescriptor(IStorylet elem) : base(elem)
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
            var interactionDescriptor = new InteractionDescriptor(elem.Interaction);
            return interactionDescriptor.MatchedDescription(roles, existents);
        }
    }
}
