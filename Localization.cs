using ImprovedPublicTransport2.LanguageFormat;
using ImprovedPublicTransport2.TranslationFramework;

namespace ImprovedPublicTransport2
{
    public static class Localization
    {
        private static readonly LocalizationManager localizationManager = 
            new LocalizationManager(new PlainTextLanguageDeserializer());

        public static string Get(string translationId)
        {
            return localizationManager.GetTranslation(translationId);
        }
    }
}