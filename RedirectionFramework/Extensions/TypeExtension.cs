using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;

namespace ImprovedPublicTransport2.RedirectionFramework.Extensions
{
    public static class TypeExtension
    {
        private static readonly Dictionary<Type, Dictionary<MethodInfo, RedirectCallsState>> Redirects
            = new Dictionary<Type, Dictionary<MethodInfo, RedirectCallsState>>();

        public static void Redirect(this Type type)
        {
            if (IsRedirected(type))
            {
                return;
            }
            var customAttributes = type.GetCustomAttributes(typeof(IgnoreConditionAttribute), false);
            if (customAttributes.Any(a => ((IgnoreConditionAttribute)a).IsIgnored(type)))
            {
                return;
            }
            Redirects[type] = RedirectionUtil.RedirectType(type);
        }

        public static void Revert(this Type type)
        {
            if (!IsRedirected(type))
            {
                return;
            }
            RedirectionUtil.RevertRedirects(Redirects[type]);
            Redirects[type].Clear();
        }

        public static bool IsRedirected(this Type type)
        {
            return Redirects.ContainsKey(type) && Redirects[type].Count != 0;
        }



        public static bool IsAnonymousType(this Type t)
        {
            var name = t.Name;
            if (name.Length < 3)
            {
                return false;
            }
            return name[0] == '<'
                && name[1] == '>'
                && name.IndexOf("AnonymousType", StringComparison.Ordinal) > 0;
        }
    }
}