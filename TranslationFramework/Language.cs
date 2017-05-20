using System.Collections.Generic;

namespace ImprovedPublicTransport2.TranslationFramework
{
    public interface ILanguage
    {
        bool HasTranslation(string id);

        string GetTranslation(string id);

        string LocaleName();
    }
}