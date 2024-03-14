namespace Stories
{
    public class StoriesVault
    {
        private HashSet<IStorylet> storylets;

        public StoriesVault(params IStorylet[] storylets)
        {
            this.storylets = new HashSet<IStorylet>(storylets);
        }

        internal ISet<IStorylet> All => storylets;
    }
}
