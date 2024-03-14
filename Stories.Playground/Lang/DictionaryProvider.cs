using Languager;

namespace Stories.Playground.Lang
{
    public class PlaygroundDictionaryProvider : DictionaryProvider
    {
        protected override IDictionaryLoader Spanish => new SpanishDictionaryLoader();
    }
}
