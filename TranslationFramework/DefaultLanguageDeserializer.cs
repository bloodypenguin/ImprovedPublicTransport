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
        public ILanguage DeserialiseLanguage(string languageFile)
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(DefaultLanguage));
            StreamReader reader = new StreamReader(languageFile);
            try
            {
                return (DefaultLanguage) xmlSerialiser.Deserialize(reader);
            }
            finally
            {
                reader.Close();
            }
        }
    }
}