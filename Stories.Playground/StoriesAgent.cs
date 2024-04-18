using Agents;
using Saver;
using Worlding;

namespace Stories.Playground
{
    public class StoriesAgent : WorldAgent
    {
        public StoriesAgent(string id, string name, string surname, Importance importance)
            : base(id, name, surname, importance)
        {
        }

        protected override object clone() =>
            null;

        protected override void load(Save save)
        {
        }

        protected override Save save() => 
            new Save(GetType().Name);
    }
}
