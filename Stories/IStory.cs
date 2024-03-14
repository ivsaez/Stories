using Agents;
using Outputer.Choicing;

namespace Stories
{
    public interface IStory
    {
        IAgent Driver { get; }

        Step Interact(Input input);
    }
}
