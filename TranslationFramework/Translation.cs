using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework.Globalization;

namespace ImprovedPublicTransport2.TranslationFramework
{
    /// <summary>
    /// Handles localisation for a mod.
    /// </summary>
    public class LocalizationManager
    {
        protected List<ILanguage> _languages = new List<ILanguage>();
        protected ILanguage _currentLanguage = null;
        protected bool _languagesLoaded = false;
        protected bool _loadLanguageAutomatically = true;
        private string fallbackLanguage;
        private ILanguageDeserializer languageDeserializer;

        public LocalizationManager(ILanguageDeserializer languageDeserializer = null, bool loadLanguageAutomatically = true, 
            string fallbackLanguage = "en")
        {
            this.languageDeserializer = languageDeserializer ?? new DefaultLanguageDeserializer();
            this._loadLanguageAutomatically = loadLanguageAutomatically;
            this.fallbackLanguage = fallbackLanguage;
            LocaleManager.eventLocaleChanged += SetCurrentLanguage;
        }

        private void SetCurrentLanguage()
        {
            if (_languages == null || _languages.Count ==0 || !LocaleManager.exists)
            {
                return;
            }
            _currentLanguage = _languages.Find(l => l.LocaleName() == LocaleManager.instance.language) ??
                               _languages.Find(l => l.LocaleName() == fallbackLanguage);
        }


        /// <summary>
        /// Loads all languages up if not already loaded.
        /// </summary>
        public void LoadLanguages()
        {
            if (!_languagesLoaded && _loadLanguageAutomatically)
            {
                _languagesLoaded = true;
                RefreshLanguages();
                SetCurrentLanguage();
            }
        }

        /// <summary>
        /// Forces a reload of the languages, even if they're already loaded
        /// </summary>
        public void RefreshLanguages()
        {
            _languages.Clear();

            string basePath = Util.AssemblyPath;

            if (basePath != "")
            {
                string languagePath = basePath + System.IO.Path.DirectorySeparatorChar + "Locale";

                if (Directory.Exists(languagePath))
                {
                    string[] languageFiles = Directory.GetFiles(languagePath);

                    foreach (string languageFile in languageFiles)
                    {
                        ILanguage loadedLanguage = null;
                        try
                        {
                            loadedLanguage = languageDeserializer.DeserialiseLanguage(languageFile);
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError(
                                "Error happened when deserializing language file " + languageFile);
                            UnityEngine.Debug.LogException(e);
                        }
                        if (loadedLanguage != null)
                        {
                            _languages.Add(loadedLanguage);
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Can't load any localisation files");
                }
            }
        }
        
        /// <summary>
        /// Returns whether you can translate into a specific translation ID
        /// </summary>
        /// <param name="translationId">The ID of the translation to check</param>
        /// <returns>Whether a translation into this ID is possible</returns>
        private bool HasTranslation(string translationId)
        {
            LoadLanguages();

            return _currentLanguage != null && _currentLanguage.HasTranslation(translationId);
        }

        /// <summary>
        /// Gets a translation for a specific translation ID
        /// </summary>
        /// <param name="translationId">The ID to return the translation for</param>
        /// <returns>A translation of the translationId</returns>
        public string GetTranslation(string translationId)
        {
            LoadLanguages();

            string translatedText = translationId;

            if (_currentLanguage != null)
            {
                if (HasTranslation(translationId))
                {
                    translatedText = _currentLanguage.GetTranslation(translationId);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Returned translation for language \"" + _currentLanguage.LocaleName() + "\" doesn't contain a suitable translation for \"" + translationId + "\"");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Can't get a translation for \"" + translationId + "\" as there is not a language defined");
            }

            return translatedText;
        }
    }
}