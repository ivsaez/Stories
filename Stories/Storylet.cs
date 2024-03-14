using Agents;
using Contexting;
using Identification;
using Rolling;
using Worlding;

namespace Stories
{
    public class Storylet : Identifiable, IStorylet
    {
        public const uint LowestPriority = 1;
        public const uint ItemsAndMappedsPriority = LowestPriority;
        public const uint EverythingInvolvedPriority = 2;
        public const uint AgentsPriority = 3;

        private readonly Descriptor descriptor;
        private readonly PotencialUser potencialUser;
        private readonly RoleScope roleScope;
        private readonly Timing timing;
        private readonly uint overridePriority;

        public EnvironmentPreconditions EnvironmentPreconditions { get; }

        public Preconditions ParticularPreconditions { get; }

        public Interaction Interaction { get; private set; }

        internal Storylet(
            string id,
            Descriptor descriptor,
            EnvironmentPreconditions environmentPreconditions,
            Preconditions particularPreconditions,
            Interaction rootInteraction,
            Timing timing = Timing.Repeteable,
            PotencialUser potencialUser = PotencialUser.Any,
            RoleScope roleScope = RoleScope.Any,
            uint overridePriority = 0)
            : base(id)
        {
            this.descriptor = descriptor;
            this.potencialUser = potencialUser;
            this.roleScope = roleScope;
            this.timing = timing;
            this.overridePriority = overridePriority;

            EnvironmentPreconditions = environmentPreconditions;
            ParticularPreconditions = particularPreconditions;
            Interaction = rootInteraction;
        }

        public uint Priority
        {
            get
            {
                if(overridePriority > 0)
                    return overridePriority;

                if (roleScope == RoleScope.Items || roleScope == RoleScope.Mappeds)
                    return ItemsAndMappedsPriority;

                if (roleScope == RoleScope.Any)
                    return EverythingInvolvedPriority;

                if (roleScope == RoleScope.Agents || roleScope == RoleScope.AwakenAgents)
                    return AgentsPriority;

                return LowestPriority;
            }
        }

        public ISet<string> InvolvedRoles => descriptor.Names;

        public IStory Execute(World world, Roles roles, Historic historic)
        {
            if (!descriptor.IsMatched(roles))
                throw new ArgumentException($"Error executing {this}: roles don't match.");

            historic.Add(getSnapshot(roles));

            return new Story(Interaction, world, roles);
        }

        public bool MatchesPotentialUser(IAgent agent) =>
            potencialUser == PotencialUser.Any
            || (potencialUser == PotencialUser.Human && agent.Actioner == Actioner.Human)
            || (potencialUser == PotencialUser.Machine && agent.Actioner == Actioner.IA);

        public bool MetsHistoricGlobalConditions(Historic historic)
        {
            if(timing == Timing.Repeteable)
                return true;

            if (timing == Timing.GlobalSingle)
                return !historic.HasGloballyHappened(Id);

            return true;
        }

        public bool MetsHistoricRolledConditions(Historic historic, Roles roles)
        {
            if (timing == Timing.Single)
                return !historic.HasHappened(getSnapshot(roles));

            return true;
        }

        public Permutations CalculatePermutations(IAgent main, Context<IWorldAgent, IWorldItem, IWorldMapped> context)
        {
            var identifiables = roleScope == RoleScope.Items
                ? context.Items
                : roleScope == RoleScope.Agents
                    ? context.Others
                    : roleScope == RoleScope.AwakenAgents
                        ? context.Others
                            .OfType<IAgent>()
                            .Where(a => a.Status.Machine.CurrentState == Status.Conscious)
                            .OfType<IIdentifiable>()
                            .ToHashSet()
                        : roleScope == RoleScope.Mappeds
                            ? context.Destinations
                            : context.All;

            return descriptor.GetPermutations(main, identifiables);
        }

        public override string ToString() => Id;

        private Snapshot getSnapshot(Roles roles) =>
            new Snapshot(Id, roles.Matchers.Select(matcher => matcher.Id));
    }
}
