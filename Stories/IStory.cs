using Agents;
using Outputer.Choicing;

namespace Stories
{
    public interface IStory
    {
        IAgent Driver { get; }

        IAgent Answerer { get; }

        Step Interact(Input input);
    }
}
