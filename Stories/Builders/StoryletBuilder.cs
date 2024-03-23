using Rolling;

namespace Stories.Builders
{
    public class StoryletBuilder
    {
        private readonly string id;
        private Descriptor descriptor;
        private EnvironmentPreconditions environmentPreconditions;
        private Preconditions preconditions;
        private Timing timing;
        private PotencialUser potencialUser;
        private RoleScope roleScope;
        private Interaction? interaction;
        private uint cost;
        private uint overridePriority;

        private StoryletBuilder(string id) 
        {
            this.id = id;
            descriptor = Descriptor.Empty;
            environmentPreconditions = (world) => true;
            preconditions = (world, roles, historic) => true;
            timing = Timing.Repeteable;
            potencialUser = PotencialUser.Any;
            roleScope = RoleScope.Any;
            cost = 1;
            overridePriority = 0;
        }

        public static StoryletBuilder Create(string id) 
        {
            return new StoryletBuilder(id);
        }

        public StoryletBuilder WithDescriptor(params string[] roles)
        {
            descriptor = Descriptor.New(roles);
            return this;
        }

        public StoryletBuilder WithEnvPreconditions(EnvironmentPreconditions environmentPreconditions)
        {
            this.environmentPreconditions = environmentPreconditions;
            return this;
        }

        public StoryletBuilder WithPreconditions(Preconditions preconditions)
        {
            this.preconditions = preconditions;
            return this;
        }

        public StoryletBuilder BeingRepeteable()
        {
            timing = Timing.Repeteable;
            return this;
        }

        public StoryletBuilder BeingSingle()
        {
            timing = Timing.Single;
            return this;
        }

        public StoryletBuilder BeingGlobalSingle()
        {
            timing = Timing.GlobalSingle;
            return this;
        }

        public StoryletBuilder ForAnyUser()
        {
            potencialUser = PotencialUser.Any;
            return this;
        }

        public StoryletBuilder ForHumans()
        {
            potencialUser = PotencialUser.Human;
            return this;
        }

        public StoryletBuilder ForMachines()
        {
            potencialUser = PotencialUser.Machine;
            return this;
        }

        public StoryletBuilder WithAgentsScope()
        {
            roleScope = RoleScope.Agents;
            return this;
        }

        public StoryletBuilder WithAwakenAgentsScope()
        {
            roleScope = RoleScope.AwakenAgents;
            return this;
        }

        public StoryletBuilder WithItemsScope()
        {
            roleScope = RoleScope.Items;
            return this;
        }

        public StoryletBuilder WithMappedsScope()
        {
            roleScope = RoleScope.Mappeds;
            return this;
        }

        public StoryletBuilder WithAnyScope()
        {
            roleScope = RoleScope.Any;
            return this;
        }

        public StoryletBuilder WithOverridePriority(uint priority)
        {
            overridePriority = priority;
            return this;
        }

        public StoryletBuilder WithCost(uint cost)
        {
            this.cost = cost;
            return this;
        }

        public StoryletBuilder WithoutCost() => WithCost(0);

        public InteractionBuilder WithInteraction(ExecutionFunction function)
        {
            var counter = new InteractionCounter(id);
            counter.Increase();

            return InteractionBuilder.Create(this, counter)
                .WithFunction(function);
        }

        public bool RoleInDescriptor(string role) =>
            descriptor.Has(role);

        internal void SetInteraction(Interaction interaction) =>
            this.interaction = interaction;

        public Storylet Finish()
        {
            if (interaction is null)
                throw new InvalidOperationException("Storylet must contain an interaction.");

            return new Storylet(
                id, 
                descriptor, 
                environmentPreconditions, 
                preconditions, 
                interaction, 
                timing, 
                potencialUser,
                roleScope,
                cost,
                overridePriority > 0 ? overridePriority : 0);
        }
    }
}
