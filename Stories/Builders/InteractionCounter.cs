namespace Stories.Builders
{
    public class InteractionCounter
    {
        private readonly string storyletId;
        private List<uint> counters;

        private int position;

        public InteractionCounter(string storyletId) 
        {
            this.storyletId = storyletId;

            position = 0;
            counters = new List<uint>
            {
                0
            };
        }

        public void Increase() => counters[position]++;

        public void AddLevel()
        {
            counters.Add(0);
            position++;
        }

        public void RemoveLevel()
        {
            if(position > 0)
            {
                counters.RemoveAt(position);
                position--;
            }
        }

        public string IdSeed => $"{storyletId}_{Ids}";

        private string Ids => string.Join("", counters);
    }
}
