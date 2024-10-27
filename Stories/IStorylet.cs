using Agents;
using Contexting;
using Identification;
using Rolling;
using Worlding;

namespace Stories
{
    public interface IStorylet : IIdentifiable
    {
        EnvironmentPreconditions EnvironmentPreconditions { get; }

        ParticularPreconditions ParticularPreconditions { get; }

        Interaction Interaction { get; }

        ISet<string> InvolvedRoles { get; }

        uint Priority { get; }

        IStory Execute(World world, Roles roles, Historic historic);

        bool MatchesPotentialUser(IIdentifiable identifiable);

        Permutations CalculatePermutations(IIdentifiable mainIdentifiable, Context<IWorldAgent, IWorldItem, IWorldMapped> context);

        bool MetsHistoricGlobalConditions(Historic historic);

        bool MetsHistoricRolledConditions(Historic historic, Roles roles);

        uint Cost { get; }
    }

    public delegate bool EnvironmentPreconditions(EnvPredefinedPreconditions pre);
    public delegate bool ParticularPreconditions(PredefinedPreconditions pre);

    public enum Timing
    {
        Single,
        GlobalSingle,
        Repeteable
    }

    public enum PotencialUser
    {
        HumanOrMachine,
        Human,
        Machine,
        Narrator
    }

    public enum RoleScope
    {
        Any,
        Agents,
        AwakenAgents,
        Items,
        Mappeds
    }
}