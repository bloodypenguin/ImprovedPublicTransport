using System;
using System.Reflection;

namespace ImprovedPublicTransport.Redirection.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class IgnoreConditionAttribute : Attribute
    {
        public abstract bool IsIgnored(MethodInfo methodInfo);
    }
}