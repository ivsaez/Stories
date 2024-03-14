using Agents;
using Outputer;
using Saver;
using Worlding;

namespace Stories.Playground
{
    public class WorldAgent : Agent, IWorldAgent
    {
        public WorldAgent(string id, string name, string surname, Importance importance)
            : base(id, name, surname, importance)
        {
        }

        public object Clone() => null;

        public void Load(Save save)
        {
        }

        public Output OnTurnPassed(IWorld world, int turns) =>
            new Output();

        public Save ToSave() => 
            new Save(GetType().Name);
    }
}
