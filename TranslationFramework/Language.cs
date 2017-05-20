using System.Collections.Generic;
using System.Xml.Serialization;

namespace ImprovedPublicTransport2.TranslationFramework
{
    [XmlRoot(ElementName = "Language", Namespace = "", IsNullable = false)]
    public class Language
    {
        [XmlAttribute("UniqueName")]
        public string _uniqueName = "";

        [XmlAttribute("ReadableName")]
        public string _readableName = "";

        [XmlArray("Translations", IsNullable = false)]
        [XmlArrayItem("Translation", IsNullable = false)]
        public LanguageConversion[] _conversionGroups
        {
            get
            {
                LanguageConversion[] returnArray = new LanguageConversion[_conversionDictionary.Count];

                int index = 0;
                foreach (KeyValuePair<string, string> conversion in _conversionDictionary)
                {
                    returnArray[index] = new LanguageConversion() { _key = conversion.Key, _value = conversion.Value };
                    ++index;
                }

                return returnArray;
            }

            set
            {
                foreach (LanguageConversion conversion in value)
                {
                    _conversionDictionary[conversion._key] = conversion._value;
                }
            }
        }

        [XmlIgnore]
        public Dictionary<string, string> _conversionDictionary = new Dictionary<string, string>();
    }

    public class LanguageConversion
    {
        [XmlAttribute("ID")]
        public string _key = "";

        [XmlAttribute("String")]
        public string _value = "";
    }
}