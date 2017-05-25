using System;

namespace ImprovedPublicTransport2.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LabelAttribute : AbstractOptionsAttribute
    {
        public LabelAttribute(string description, string group) :
            base(description, group, null, null)
        {
        }
    }
}