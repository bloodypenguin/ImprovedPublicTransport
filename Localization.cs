using ImprovedPublicTransport2.LanguageFormat;
using ImprovedPublicTransport2.TranslationFramework;

namespace ImprovedPublicTransport2
{
    public static class Localization
    {
        private static readonly LocalizationManager LocalizationManager = 
            new LocalizationManager(new PlainTextLanguageDeserializer());

        public static string Get(string translationId)
        {
            if (translationId.StartsWith("INFO_PUBLICTRANSPORT"))
            {
                return ColossalFramework.Globalization.Locale.Get(translationId);
            }
            return LocalizationManager.GetTranslation(translationId);
        }
    }
}