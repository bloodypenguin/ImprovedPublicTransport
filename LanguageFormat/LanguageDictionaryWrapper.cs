using System.Collections.Generic;
using ImprovedPublicTransport2.TranslationFramework;

namespace ImprovedPublicTransport2.LanguageFormat
{
    public class LanguageDictionaryWrapper : ILanguage
    {
        private string localeName;
        private Dictionary<string, string> dictionary;

        public LanguageDictionaryWrapper(string localeName, Dictionary<string, string> dictionary)
        {
            this.localeName = localeName;
            this.dictionary = dictionary;
        }

        public bool HasTranslation(string id)
        {
            return dictionary.ContainsKey(id);
        }

        public string GetTranslation(string id)
        {
            return dictionary[id];
        }

        public string LocaleName()
        {
            return localeName;
        }
    }
}