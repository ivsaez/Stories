using Items;
using Mapping;
using Saver;
using Worlding;

namespace Stories.Playground
{
    internal class StoriesMapped : WorldMapped, IArticled
    {
        public StoriesMapped(string id, Externality externality, Articler articler) 
            : base(id, externality)
        {
            Articler = articler;
        }

        public Articler Articler { get; private set; }

        protected override object clone() => null;

        protected override void load(Save save)
        {
        }

        protected override Save save() =>
            new Save(GetType().Name);
    }
}
