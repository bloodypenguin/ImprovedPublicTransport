using System;

namespace ImprovedPublicTransport2.TranslationFramework
{
    /// <summary>
    /// Translates the specified field into the value denoted by the identifier
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TranslatableAttribute : Attribute
    {
        /// <summary>
        /// The translation ID
        /// </summary>
        public string identifier = "";

        public TranslatableAttribute()
        {
        }

        /// <summary>
        /// Translates the specified field into the value denoted by the identifier
        /// </summary>
        /// <param name="identifier">The translation ID</param>
        public TranslatableAttribute(string identifier)
        {
            this.identifier = identifier;
        }
    }
}