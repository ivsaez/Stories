using Outputer;
using Outputer.Choicing;

namespace Stories
{
    public sealed class Step
    {
        public Output Output { get; }

        public Choices Choices { get; }

        public bool IsEnding => Choices.IsEmpty;

        public Step(Output output, Choices choices)
        {
            Output = output;
            Choices = choices;
        }

        public override string ToString() => Output.ToString();
    }
}
