using Items;
using Mapping;
using Outputer;
using Saver;
using Worlding;

namespace Stories.Playground
{
    internal class WorldMapped : Mapped, IWorldMapped, IArticled
    {
        public WorldMapped(string id, Articler articler) 
            : base(id)
        {
            Articler = articler;
        }

        public Articler Articler { get; private set; }

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
