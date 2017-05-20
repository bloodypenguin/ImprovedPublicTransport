using System.IO;
using System.Xml.Serialization;

namespace ImprovedPublicTransport2.TranslationFramework
{
    public class DefaultLanguageDeserializer : ILanguageDeserializer
    {
        /// <summary>
        /// Deserialise a language file using a TextReader
        /// </summary>
        /// <param name="reader">The text to deserialise</param>
        /// <returns>A deserialised language</returns>
        public ILanguage DeserialiseLanguage(string fileName, TextReader reader)
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(DefaultLanguage));

            DefaultLanguage loadedLanguage = (DefaultLanguage)xmlSerialiser.Deserialize(reader);
            return loadedLanguage;
        }
    }
}