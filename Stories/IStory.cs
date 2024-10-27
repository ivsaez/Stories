using Agents;
using Outputer.Choicing;

namespace Stories
{
    public interface IStory
    {
        string Driver { get; }

        string Answerer { get; }

        Step Interact(Input input);
    }
}
