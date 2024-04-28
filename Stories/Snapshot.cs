using Saver;

namespace Stories
{
    public class Snapshot : ISavable, ICloneable
    {
        private List<string> participants;

        public string Id { get; private set; }

        public Snapshot(string id, IEnumerable<string> participants)
        {
            if(string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(nameof(id));

            if(participants.Any(participant => string.IsNullOrWhiteSpace(participant)))
                throw new ArgumentException(nameof(participants));

            Id = id;
            this.participants = participants.ToList();
        }

        public Snapshot(string id, params string[] participants)
            : this(id, (IEnumerable<string>)participants) {}

        public bool HasParticipant(string participant) =>
            participants.Contains(participant);

        public override string ToString() =>
            $"{Id} [{string.Join(",", participants)}]";

        public override int GetHashCode() =>
            ToString().GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            var other = obj as Snapshot;
            if (other == null) return false;

            if(Id != other.Id) return false;

            if(participants.Count != other.participants.Count)
                return false;

            for (int i = 0; i < participants.Count; i++)
            {
                if (participants[i] != other.participants[i]) 
                    return false;
            }
            
            return true;
        }

        public bool GloballyEquals(Snapshot snapshot) =>
            Id == snapshot.Id;

        public Save ToSave() =>
            new Save(GetType().Name)
                .With(nameof(Id), Id)
                .WithArray(nameof(participants), participants.ToArray());

        public void Load(Save save)
        {
            Id = save.GetString(nameof(Id));
            participants = new List<string>(save.GetStringArray(nameof(participants)));
        }

        public object Clone() => new Snapshot(Id, participants.ToList());
    }
}
