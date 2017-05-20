using System.IO;

namespace ImprovedPublicTransport2.TranslationFramework
{
    public interface ILanguageDeserializer
    {
        ILanguage DeserialiseLanguage(string fileName, TextReader reader);
    }
}