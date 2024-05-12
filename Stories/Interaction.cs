using Agents;
using Identification;
using Instanciation;
using Items;
using Mapping;
using Outputer;
using Outputer.Choicing;
using Rolling;
using Saver;
using Stories.Lang;
using Worlding;

namespace Stories
{
    public delegate Output ExecutionFunction(PredefinedPostconditions post);

    public class Interaction : Identifiable, IIdentifiable
    {
        private readonly ExecutionFunction function;
        private readonly Interaction[] nexts;

        internal Interaction(string id, string driver, ExecutionFunction function, params Interaction[] nexts)
            : base(id)
        {
            if(string.IsNullOrWhiteSpace(driver))
                throw new ArgumentNullException(nameof(driver));

            Driver = driver;
            this.function = function;
            this.nexts = nexts;
        }

        public string Driver { get; private set; }

        public string Answerer
        {
            get
            {
                if (nexts is null || !nexts.Any())
                    return Driver;

                return nexts.First().Driver;
            }
        }

        public Choices Choices<A, I, M>(Roles roles,
            Existents<A, I, M> existents)
            where A : IAgent, ISavable, ICloneable
            where I : IItem, ISavable, ICloneable
            where M : IMapped, ISavable, ICloneable =>
            nexts.Aggregate(
                new Choices(), 
                (choices, interaction) => 
                    choices.With(
                        new InteractionDescriptor(interaction).MatchedDescription(roles, existents), 
                        () => interaction,
                        Storylet.LowestPriority));


        public Output Execute(PredefinedPostconditions post) =>
            function(post);
    }
}
