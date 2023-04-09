using System;
using System.Reflection;
using HarmonyLib;
using ImprovedPublicTransport2.HarmonyPatches;
using UnityEngine;
using static ImprovedPublicTransport2.ImprovedPublicTransportMod;

namespace ImprovedPublicTransport2.Util
{
    internal static class PatchUtil
    {

        private static Harmony _harmonyInstance;

        private static Harmony HarmonyInstance =>
            _harmonyInstance ??= new Harmony(HarmonyId.Value);


        public static void Patch(
            MethodDefinition original,
            MethodDefinition prefix = null,
            MethodDefinition postfix = null,
            MethodDefinition transpiler = null)
        {
            if (prefix == null && postfix == null && transpiler == null)
            {
                throw new Exception(
                    $"{ShortModName}: prefix, postfix and transpiler are null for method {original.Type.FullName}.{original.MethodName}");
            }

            try
            {
                Debug.Log($"{ShortModName}: Patching method {original.Type.FullName}.{original.MethodName}");
                var methodInfo = GetOriginal(original);
                HarmonyInstance.Patch(methodInfo,
                    prefix == null ? null : new HarmonyMethod(GetPatch(prefix), before: prefix.Before, after: prefix.After, priority: prefix.Priority),
                    postfix == null ? null : new HarmonyMethod(GetPatch(postfix), before: postfix.Before, after: postfix.After, priority: postfix.Priority),
                    transpiler == null ? null : new HarmonyMethod(GetPatch(transpiler), before: transpiler.Before, after: transpiler.After, priority: transpiler.Priority)
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"{ShortModName}: Failed to patch method {original.Type.FullName}.{original.MethodName}");
                Debug.LogException(e);
            }
        }

        public static void Unpatch(MethodDefinition original)
        {
            Debug.Log($"{ShortModName}: Unpatching method {original.Type.FullName}.{original.MethodName}");
            HarmonyInstance.Unpatch(GetOriginal(original), HarmonyPatchType.All, HarmonyId.Value);
        }

        private static MethodInfo GetOriginal(MethodDefinition original)
        {
            var bindingFlags = original.BindingFlags == BindingFlags.Default
                ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly
                : original.BindingFlags;
            var methodInfo = original.ArgumentTypes == null
                ? original.Type.GetMethod(original.MethodName, bindingFlags)
                : original.Type.GetMethod(original.MethodName, bindingFlags, null, original.ArgumentTypes, null);
            if (methodInfo == null)
            {
                throw new Exception(
                    $"{ShortModName}: Failed to find original method {original.Type.FullName}.{original.MethodName}");
            }

            return methodInfo;
        }

        private static MethodInfo GetPatch(MethodDefinition patch)
        {
            var bindingFlags = patch.BindingFlags == BindingFlags.Default
                ? BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly
                : patch.BindingFlags;
            var methodInfo = patch.ArgumentTypes == null
                ? patch.Type.GetMethod(patch.MethodName, bindingFlags)
                : patch.Type.GetMethod(patch.MethodName, bindingFlags, null, patch.ArgumentTypes, null);
            
            if (methodInfo == null)
            {
                throw new Exception($"{ShortModName}: Failed to find patch method {patch.Type.FullName}.{patch.MethodName}");
            }

            return methodInfo;
        }

        public class MethodDefinition
        {
            public MethodDefinition(Type type, string methodName,
                BindingFlags bindingFlags = BindingFlags.Default,
                Type[] argumentTypes = null,
                string[] before = null,
                string[] after = null,
                int priority = -1)
            {
                Type = type;
                MethodName = methodName;
                BindingFlags = bindingFlags;
                ArgumentTypes = argumentTypes;
                Before = before;
                After = after;
                Priority = priority;
            }

            public Type Type { get; }
            public string MethodName { get; }

            public BindingFlags BindingFlags { get; }
            
            public Type[] ArgumentTypes { get; }

            public string[] Before { get;  }
            
            public string[] After { get;  }
            
            public int Priority { get;  }
        }
    }
}