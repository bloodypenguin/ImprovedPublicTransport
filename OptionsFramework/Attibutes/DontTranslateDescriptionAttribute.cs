using System;
using System.ComponentModel;

namespace ImprovedPublicTransport2.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.All)]
    public class DontTranslateDescriptionAttribute : DescriptionAttribute
    {
        public DontTranslateDescriptionAttribute(string description) :
            base(description)
        {
            
        }
    }
}