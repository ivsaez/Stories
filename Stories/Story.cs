using Agents;
using Outputer.Choicing;
using Rolling;
using Worlding;

namespace Stories
{
    public class Story : IStory
    {
        private readonly Interaction initial;
        private readonly World world;
        private readonly Roles roles;
        private readonly Historic historic;

        private Interaction? actualExecution;

        public Story(Interaction initial, World world, Roles roles, Historic historic)
        {
            this.initial = initial;
            this.world = world;
            this.roles = roles;
            this.historic = historic;
            actualExecution = null;
        }

        public IAgent Driver
        {
            get 
            {
                if (actualExecution is null)
                    return roles.Get<IAgent>(initial.Driver);

                return roles.Get<IAgent>(actualExecution.Driver);
            }
        } 

        public IAgent Answerer
        {
            get
            {
                if (actualExecution is null)
                    return roles.Get<IAgent>(initial.Driver);

                return roles.Get<IAgent>(actualExecution.Answerer);
            }
        }

        public Step Interact(Input input)
        {
            if (actualExecution is null && input.IsVoid)
            {
                actualExecution = initial;
                var output = actualExecution.Execute(new PredefinedPostconditions(world, roles, historic));

                return new Step(output, actualExecution.Choices(roles, world.Existents));
            }

            var choices = actualExecution!.Choices(roles, world.Existents);
            var option = choices.Select(input);

            var nextExecution = option.Function() as Interaction;

            actualExecution = nextExecution;
            var anotherOutput = actualExecution!.Execute(new PredefinedPostconditions(world, roles, historic));
            return new Step(anotherOutput, actualExecution.Choices(roles, world.Existents));
        }
    }
}
