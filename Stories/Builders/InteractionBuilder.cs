using Rolling;

namespace Stories.Builders
{
    public class InteractionBuilder
    {
        private StoryletBuilder storyletBuilder;
        private readonly InteractionCounter counter;
        private InteractionBuilder? parent;
        private string driver;
        private ExecutionFunction? function;
        private List<Interaction> subInteractions;

        private string interactionId;

        private InteractionBuilder(StoryletBuilder storyletBuilder, InteractionCounter counter, InteractionBuilder? parent = null)
        {
            this.storyletBuilder = storyletBuilder;
            this.counter = counter;
            this.parent = parent;
            driver = Descriptor.MainRole;
            subInteractions = new List<Interaction>();

            interactionId = $"{counter.IdSeed}_interaction";
            counter.AddLevel();
        }

        public static InteractionBuilder Create(StoryletBuilder storyletBuilder, InteractionCounter counter, InteractionBuilder? parent = null)
        {
            return new InteractionBuilder(storyletBuilder, counter, parent);
        }

        public InteractionBuilder WithDriver(string driver)
        {
            this.driver = driver;
            return this;
        }

        public InteractionBuilder WithFunction(ExecutionFunction function)
        {
            this.function = function;
            return this;
        }

        public InteractionBuilder WithSubinteraction(ExecutionFunction function)
        {
            counter.Increase();
            return Create(storyletBuilder, counter, this)
                .WithFunction(function);
        }

        public StoryletBuilder SetAsRoot()
        {
            if(parent is not null)
                throw new InvalidOperationException("Set is only posible in the root interaction.");

            storyletBuilder.SetInteraction(BuiltInteraction);

            return storyletBuilder;
        }

        public InteractionBuilder Build() 
        {
            if(parent is null)
                throw new InvalidOperationException("Root interaction cannot be a subinteraction.");

            parent.subInteractions.Add(BuiltInteraction);
            counter.RemoveLevel();

            parent.validateSubinteractionDrivers();

            return parent;
        }

        private Interaction BuiltInteraction
        {
            get
            {
                if (function is null)
                    throw new InvalidOperationException("Cannot build an interaction without the execution function.");

                if (!storyletBuilder.RoleInDescriptor(driver))
                    throw new InvalidOperationException("Driver must be a valid role in the storylet descriptor");

                return new Interaction(interactionId, driver, function, subInteractions.ToArray());
            }
        }

        private void validateSubinteractionDrivers()
        {
            if (!subInteractions.Any())
                return;

            var expectedDriver = subInteractions.First().Driver;
            if (!subInteractions.All(interaction => interaction.Driver == expectedDriver))
                throw new InvalidOperationException("All interactions at the same level must share the same driver");
        }
    }
}
