using AgentBody;
using Agents;
using Agents.Extensions;
using Identification;
using Logic;
using Rolling;
using Worlding;

namespace Stories
{
    public abstract class Predefined
    {
        public World World { get; }
        public Roles Roles { get; }
        public Historic Historic { get; }

        public Predefined(World world, Roles roles, Historic historic)
        {
            World = world;
            Roles = roles;
            Historic = historic;
        }

        public IWorldAgent Main =>
            Agent(Descriptor.MainRole);

        public IWorldMapped MainPlace
        {
            get
            {
                var main = Main;
                return World.Map.GetUbication(main);
            }
        }

        public IWorldAgent Agent(string role) =>
            Roles.Get<IWorldAgent>(role);

        public IWorldMapped Mapped(string role) =>
            Roles.Get<IWorldMapped>(role);

        public IWorldItem Item(string role) =>
            Roles.Get<IWorldItem>(role);

        public bool HasHappened(string story, params string[] roles) =>
            Historic.HasHappened(new Snapshot(story, roles.Select(r => Roles.Get(r)!.Id)));

        public bool HasHappenedTimes(uint times, string story, params string[] roles) =>
            Historic.HasHappenedTimes(new Snapshot(story, roles.Select(r => Roles.Get(r)!.Id)), times);

        public bool IsKnown(string function, params string[] individuals)
        {
            if (individuals.Length > 2)
                throw new ArgumentException("A function cannot have more than 2 individuals.");

            var sentence = individuals.Length == 0
                ? Sentence.Build(function)
                : individuals.Length == 1
                    ? Sentence.Build(function, individuals[0])
                    : Sentence.Build(function, individuals[0], individuals[1]);

            return World.Knowledge.Exists(sentence);
        }
    }

    public class PredefinedPreconditions : Predefined
    {
        public PredefinedPreconditions(World world, Roles roles, Historic historic)
            : base(world, roles, historic)
        {
        }

        public bool MainIs(string name) =>
            RoleIs(Descriptor.MainRole, name);

        public bool RoleIs(string role, string name)
        {
            var roleAgent = Agent(role);

            return roleAgent.Name == name;
        }

        public bool PositionIs(string role, string position)
        {
            var roleAgent = Agent(role);

            return roleAgent.Position.Machine.CurrentState == position;
        }

        public bool IsExit(string exitRole)
        {
            var place = MainPlace;

            var exit = Mapped(exitRole);

            return place.Exits.Has(exit);
        }

        public bool EverythingInMainPlace()
        {
            var place = MainPlace;
            var items = place.Items.AllAccessible(World.Items);

            return Roles.Matchers
                .All(matcher => matcherIsInPlace(matcher, place.Agents, items));
        }

        public bool EveryoneConscious() =>
            Roles.Matchers
                .Where(matcher => matcher is IWorldAgent)
                .Cast<IWorldAgent>()
                .All(agent => agent.Status.Machine.CurrentState == Status.Conscious);

        public bool EveryoneStanding() =>
            Roles.Matchers
                .Where(matcher => matcher is IWorldAgent)
                .Cast<IWorldAgent>()
                .All(agent => agent.Position.Machine.CurrentState == Position.Standing);

        public bool RoleInPlace(string role, string placeRole)
        {
            var place = Mapped(placeRole);

            return matcherIsInPlace(
                Roles.Get(role)!,
                place.Agents,
                place.Items.AllAccessible(World.Items));
        }

        public bool RoleOwns(string role, string itemRole)
        {
            var owner = Agent(role);
            if (owner is not ICarrier)
                return false;

            var carrier = owner.Cast<ICarrier>();
            var item = Item(itemRole);

            return carrier.Carrier.GetCarrieds(World.Items).Back is not null
                && carrier.Carrier.GetCarrieds(World.Items).Back!.Inventory.Has(item);
        }

        public bool MainPlaceIsEnlighted()
        {
            var place = MainPlace;

            return place.IsEnlighted(World.Agents, World.Items, World.Time.IsLight);
        }

        public bool IsHourGreaterThan(int hour)
        {
            return World.Time.Hour > hour;
        }

        private bool matcherIsInPlace(IIdentifiable matcher, Mapping.Agents agents, IEnumerable<IWorldItem> items)
        {
            if (matcher is IWorldAgent)
            {
                if (agents.Has((IWorldAgent)matcher))
                    return true;
            }
            else if (matcher is IWorldItem)
            {
                if (items.Contains((IWorldItem)matcher))
                    return true;
            }

            return false;
        }
    }

    public class PredefinedPostconditions : Predefined
    {
        public PredefinedPostconditions(World world, Roles roles, Historic historic)
            : base(world, roles, historic)
        {
        }
    }
}
