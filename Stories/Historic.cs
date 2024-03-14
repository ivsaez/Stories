using Saver;

namespace Stories
{
    public abstract class AbstractHistoric
    {
        protected List<Snapshot> snapshots;
        protected HashSet<int> happeneds;
        protected Dictionary<string, uint> globals;

        public int Count => snapshots.Count;

        public Snapshot? Last =>
            snapshots.Any()
                ? snapshots.Last()
                : null;

        public AbstractHistoric()
        {
            snapshots = new List<Snapshot>();
            globals = new Dictionary<string, uint>();
            happeneds = new HashSet<int>();
        }

        protected void add(Snapshot snapshot)
        {
            snapshots.Add(snapshot);
            happeneds.Add(snapshot.GetHashCode());

            if (!globals.ContainsKey(snapshot.Id))
                globals.Add(snapshot.Id, 0);

            globals[snapshot.Id]++;
        }

        public bool HasHappened(Snapshot snapshot) => happeneds.Contains(snapshot.GetHashCode());

        public bool HasHappenedTimes(Snapshot snapshot, uint times) =>
            snapshots.Count(s => s.Equals(snapshot)) >= times;

        public bool HasGloballyHappened(string id) =>
            globals.ContainsKey(id);

        public bool HasGloballyHappenedTimes(string id, uint times) =>
            HasGloballyHappened(id)
            && globals[id] >= times;

        public bool HasParticipant(string participant) =>
            snapshots.Any(s => s.HasParticipant(participant));
    }

    public class Historic: AbstractHistoric, ISavable, ICloneable
    {
        public Historic()
            : base()
        {
        }

        public void Add(Snapshot snapshot) =>
            add(snapshot);

        public SubHistoric Take(uint count)
        {
            if(count > snapshots.Count)
                return new SubHistoric(snapshots.ToList());

            return new SubHistoric(snapshots.TakeLast((int)count));
        }

        public object Clone()
        {
            var clone = new Historic();
            foreach (var snapshot in snapshots)
            {
                clone.snapshots.Add((Snapshot)snapshot.Clone());
            }

            foreach (var (id, count) in globals)
            {
                clone.globals.Add(id, count);
            }

            foreach (var snapshotHash in happeneds)
            {
                clone.happeneds.Add(snapshotHash);
            }

            return clone;
        }

        public Save ToSave() =>
            new Save(GetType().Name)
                .WithSavablesArray(nameof(snapshots), snapshots.ToArray())
                .WithDictionary(nameof(globals), globals)
                .WithArray(nameof(happeneds), happeneds.ToArray());

        public void Load(Save save)
        {
            snapshots = save.GetSavablesArray<Snapshot>(nameof(snapshots)).ToList();
            happeneds = save.GetIntArray(nameof(happeneds)).ToHashSet();
            globals = save.GetUIntDictionary(nameof(globals));
        }
    }

    public class SubHistoric : AbstractHistoric
    {
        public SubHistoric(IEnumerable<Snapshot> snapshots)
            : base()
        {
            foreach (var snapshot in snapshots)
                add(snapshot);
        }
    }
}
