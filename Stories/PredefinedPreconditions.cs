using AgentBody;
using Agents;
using Agents.Extensions;
using Identification;
using Rolling;
using Worlding;

namespace Stories
{
    public class PredefinedPreconditions
    {
        public World World { get; }
        public Roles Roles { get; }
        public Historic Historic { get; }

        public PredefinedPreconditions(World world, Roles roles, Historic historic)
        {
            World = world;
            Roles = roles;
            Historic = historic;
        }

        public bool MainIs(string name) =>
            RoleIs(Descriptor.MainRole, name);

        public bool RoleIs(string role, string name)
        {
            var roleAgent = Roles.Get<IWorldAgent>(role);

            return roleAgent.Name == name;
        }

        public bool IsExit(string exitRole)
        {
            var main = Roles.Get<IWorldAgent>(Descriptor.MainRole);
            var place = World.Map.GetUbication(main);

            var exit = Roles.Get<IWorldMapped>(exitRole);

            return place.Exits.Has(exit);
        }

        public bool EverythingInMainPlace()
        {
            var main = Roles.Get<IWorldAgent>(Descriptor.MainRole);
            var place = World.Map.GetUbication(main);
            var items = place.Items.AllAccessible(World.Items);

            return Roles.Matchers
                .All(matcher => matcherIsInPlace(matcher, place.Agents, items));
        }

        public bool EveryoneConscious() =>
            Roles.Matchers
                .Where(matcher => matcher is IWorldAgent)
                .Cast<IWorldAgent>()
                .All(agent => agent.Status.Machine.CurrentState == Status.Conscious);

        public bool RoleInPlace(string role, string placeRole)
        {
            var place = Roles.Get<IWorldMapped>(placeRole);

            return matcherIsInPlace(
                Roles.Get(role)!,
                place.Agents,
                place.Items.AllAccessible(World.Items));
        }

        public bool RoleOwns(string role, string itemRole)
        {
            var owner = Roles.Get<IWorldAgent>(role);
            if (owner is not ICarrier)
                return false;

            var carrier = owner.Cast<ICarrier>();
            var item = Roles.Get<IWorldItem>(itemRole);

            return carrier.Carrier.GetCarrieds(World.Items).Back is not null
                && carrier.Carrier.GetCarrieds(World.Items).Back!.Inventory.Has(item);
        }

        public bool HasHappened(string story, params string[] roles) =>
            Historic.HasHappened(new Snapshot(story, roles.Select(r => this.Roles.Get(r)!.Id)));

        public bool MainPlaceIsEnlighted()
        {
            var main = Roles.Get<IWorldAgent>(Descriptor.MainRole);
            var place = World.Map.GetUbication(main);

            return place.IsEnlighted(World.Agents, World.Items, World.Time.IsLight);
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
}
