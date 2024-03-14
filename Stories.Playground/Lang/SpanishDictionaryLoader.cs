using Languager;

namespace Stories.Playground.Lang
{
    public class SpanishDictionaryLoader : IDictionaryLoader
    {
        public IEnumerable<Word> Words => new List<Word>
        {
            new Word("test_description", "Esto es un test de storylet donde actua {main}"),
            new Word("movement_description", "Moverse hacia otra habitación"),

            new Word("test_1_interaction_description", "Esta es la option 1 para {main}"),
            new Word("test_11_interaction_description", "Esta es la option 11 para {main}"),
            new Word("test_12_interaction_description", "Esta es la option 12 para {main}"),
            new Word("test_111_interaction_description", "Esta es la option 111 para {main}"),
            new Word("test_112_interaction_description", "Esta es la option 112 para {main}"),
            new Word("test_121_interaction_description", "Esta es la option 121 para {main}"),
            new Word("test_122_interaction_description", "Esta es la option 122 para {main}"),
            
            new Word("movement_1_interaction_description", "{main} va a {place}"),

            new Word("ejecucion_1", "Enunciado 1 se ha ejecutado"),
            new Word("ejecucion_11", "Enunciado 11 se ha ejecutado"),
            new Word("ejecucion_12", "Enunciado 12 se ha ejecutado"),
            new Word("ejecucion_111", "Enunciado 111 se ha ejecutado"),
            new Word("ejecucion_112", "Enunciado 112 se ha ejecutado"),
            new Word("ejecucion_121", "Enunciado 121 se ha ejecutado"),
            new Word("ejecucion_122", "Enunciado 122 se ha ejecutado"),

            new Word("movement_execution", "{0} camina pesadamente hacia {1}"),

            new Word("room_name", "habitación"),
        };
    }
}
