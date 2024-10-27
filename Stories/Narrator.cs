using Identification;

namespace Stories
{
    public class Narrator : IIdentifiable
    {
        public static string NarratorId => "Narrator";

        public string Id => NarratorId;

        public static Narrator Instance => new Narrator();
    }
}
