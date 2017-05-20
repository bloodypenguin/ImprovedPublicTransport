using System.Collections.Generic;
using System.Xml.Serialization;

namespace ImprovedPublicTransport2.TranslationFramework
{
    [XmlRoot(ElementName = "Language", Namespace = "", IsNullable = false)]
    public class DefaultLanguage : ILanguage
    {
        [XmlAttribute("UniqueName")]
        public string _uniqueName = "";

        [XmlAttribute("ReadableName")]
        public string _readableName = "";

        [XmlArray("Translations", IsNullable = false)]
        [XmlArrayItem("Translation", IsNullable = false)]
        public Translation[] _translations
        {
            get
            {
                Translation[] returnArray = new Translation[_conversionDictionary.Count];

                int index = 0;
                foreach (KeyValuePair<string, string> conversion in _conversionDictionary)
                {
                    returnArray[index] = new Translation() { _key = conversion.Key, _value = conversion.Value };
                    ++index;
                }

                return returnArray;
            }

            set
            {
                foreach (Translation conversion in value)
                {
                    _conversionDictionary[conversion._key] = conversion._value;
                }
            }
        }

        [XmlIgnore]
        private readonly Dictionary<string, string> _conversionDictionary = new Dictionary<string, string>();

        public bool HasTranslation(string id)
        {
            return _conversionDictionary.ContainsKey(id);
        }

        public string GetTranslation(string id)
        {
            return _conversionDictionary[id];
        }

        public string LocaleName()
        {
            return _uniqueName;
        }
    }

    public class Translation
    {
        [XmlAttribute("ID")]
        public string _key = "";

        [XmlAttribute("String")]
        public string _value = "";
    }
}