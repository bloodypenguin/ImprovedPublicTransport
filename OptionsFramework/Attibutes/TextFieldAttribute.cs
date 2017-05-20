using System;

namespace ImprovedPublicTransport2.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TextfieldAttribute : AbstractOptionsAttribute
    {
        public TextfieldAttribute(string description, string group = null, string actionClass = null,
            string actionMethod = null) : base(description, group, actionClass, actionMethod)
        {
        }
    }
}