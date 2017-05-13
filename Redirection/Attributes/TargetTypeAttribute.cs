using System;

namespace ImprovedPublicTransport.Redirection.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TargetTypeAttribute : Attribute
    {
        public TargetTypeAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}