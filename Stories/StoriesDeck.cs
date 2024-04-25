using Agents;
using Contexting;
using Instanciation;
using Items;
using Mapping;
using Outputer.Choicing;
using Rolling;
using Saver;
using Stories.Lang;
using Worlding;

namespace Stories
{
    public class StoriesDeck
    {
        private readonly ISet<IStorylet> storylets;

        public ISet<IStorylet> All => storylets;

        public StoriesDeck() 
        {
            storylets = new HashSet<IStorylet>();
        }

        public void IncorporateVault(StoriesVault vault)
        {
            foreach (var storylet in vault.All)
                storylets.Add(storylet);
        }

        public DeckSelection GetValidStories(World world, Historic historic) 
        {
            var selectedStorylets = new HashSet<IStorylet>();

            foreach (var storylet in storylets
                .Where(s => s.MetsHistoricGlobalConditions(historic))
                .Where(s => s.EnvironmentPreconditions(world)))
            {
                selectedStorylets.Add(storylet);
            }

            return new DeckSelection(selectedStorylets);
        }
    }

    public class DeckSelection
    {
        private readonly ISet<IStorylet> storylets;

        public ISet<IStorylet> All => storylets;

        public DeckSelection(ISet<IStorylet> storylets)
        {
            this.storylets = storylets;
        }

        public RolledStories GetValidStories(
            IWorldAgent main, 
            World world,
            Historic historic)
        {
            var selectedStorylets = new RolledStories();

            var context = new Context<IWorldAgent, IWorldItem, IWorldMapped>(main, world.Existents);

            foreach (var storylet in storylets
                .Where(s => s.MatchesPotentialUser(main)))
            {
                var permutations = storylet.CalculatePermutations(main, context);

                foreach (var roles in permutations.Roles)
                {
                    if (storylet.MetsHistoricRolledConditions(historic, roles) 
                        && storylet.ParticularPreconditions(world, roles, historic))
                        selectedStorylets.Add(storylet, roles);
                }
            }

            return selectedStorylets;
        }
    }

    public class RolledStories
    {
        private readonly List<RolledStorylet> rolledStorylets;

        public bool IsEmpty => !rolledStorylets.Any();

        public RolledStories()
        {
            rolledStorylets = new List<RolledStorylet>();
        }

        public void Add(IStorylet storylet, Roles roles)
        {
            rolledStorylets.Add(new RolledStorylet(storylet, roles));
        }

        public Choices Choices<A, I, M>(
            Existents<A, I, M> existents)
            where A : IAgent, ISavable, ICloneable
            where I : IItem, ISavable, ICloneable
            where M : IMapped, ISavable, ICloneable =>
            rolledStorylets.Aggregate(
                new Choices(),
                (choices, rolledStorylet) =>
                    choices.With(
                        rolledStorylet.MatchedDescription(existents),
                        () => rolledStorylet, 
                        rolledStorylet.Storylet.Priority));
    }

    public class RolledStorylet
    {
        public RolledStorylet(IStorylet storylet, Roles roles)
        {
            Storylet = storylet;
            Roles = roles;
        }

        public IStorylet Storylet { get; }
        public Roles Roles { get; }

        public string MatchedDescription<A, I, M>(
            Existents<A, I, M> existents)
            where A : IAgent, ISavable, ICloneable
            where I : IItem, ISavable, ICloneable
            where M : IMapped, ISavable, ICloneable =>
                new StoryletDescriptor(Storylet).MatchedDescription(Roles, existents);

        public IStory Execute(World world, Historic historic) => 
            Storylet.Execute(world, Roles, historic);
    }
}
