using System;
using System.Reflection;

namespace ImprovedPublicTransport2.OptionsFramework.Attibutes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractOptionsAttribute : Attribute
    {
        protected AbstractOptionsAttribute(string description, string group, string actionClass, string actionMethod)
        {
            Description = description;
            Group = group;
            ActionClass = actionClass;
            ActionMethod = actionMethod;
        }

        public string Description { get; }
        public string Group { get; }

        public Action<T> Action<T>()
        {
            if (ActionClass == null || ActionMethod == null)
            {
                return s => { };
            }
            var method = Util.FindType(ActionClass).GetMethod(ActionMethod, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return s => { };
            }
            return s =>
            {
                method.Invoke(null, new object[] { s });
            };
        }

        public Action Action()
        {
            if (ActionClass == null || ActionMethod == null)
            {
                return () => { };
            }
            var method = Util.FindType(ActionClass).GetMethod(ActionMethod, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return () => { };
            }
            return () =>
            {
                method.Invoke(null, new object[] { });
            };
        }

        private string ActionClass { get; }

        private string ActionMethod { get; }


    }
}