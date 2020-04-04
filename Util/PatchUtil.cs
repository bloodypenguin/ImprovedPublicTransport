using System;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace ImprovedPublicTransport2.Util
{
    public static class PatchUtil
    {
        private const string HarmonyId = "github.com/bloodypenguin/ImprovedPublicTransport";
        private static HarmonyInstance _harmonyInstance = null;

        private static HarmonyInstance HarmonyInstance =>
            _harmonyInstance ?? (_harmonyInstance = HarmonyInstance.Create(HarmonyId));

        public static void Patch(
            MethodDefinition original,
            MethodDefinition prefix = null,
            MethodDefinition postfix = null,
            MethodDefinition transpiler = null)
        {
            if (prefix == null && postfix == null && transpiler == null)
            {
                throw new Exception(
                    $"IPT 2: prefix, postfix and transpiler are null for method {original.Type.FullName}.{original.MethodName}");
            }

            try
            {
                Debug.Log($"IPT 2: Patching method {original.Type.FullName}.{original.MethodName}");
                var methodInfo = GetOriginal(original);
                HarmonyInstance.Patch(methodInfo,
                    prefix: prefix == null ? null : new HarmonyMethod(GetPatch(prefix)),
                    postfix: postfix == null ? null : new HarmonyMethod(GetPatch(postfix)),
                    transpiler == null ? null : new HarmonyMethod(GetPatch(transpiler))
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"IPT 2: Failed to patch method {original.Type.FullName}.{original.MethodName}");
                Debug.LogException(e);
            }
        }

        public static void Unpatch(MethodDefinition original)
        {
            Debug.Log($"IPT 2: Unpatching method {original.Type.FullName}.{original.MethodName}");
            HarmonyInstance.Unpatch(GetOriginal(original), HarmonyPatchType.All, HarmonyId);
        }

        private static MethodInfo GetOriginal(MethodDefinition original)
        {
            var methodInfo = original.Type.GetMethod(original.MethodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new Exception(
                    $"IPT2: Failed to find original method {original.Type.FullName}.{original.MethodName}");
            }

            return methodInfo;
        }

        private static MethodInfo GetPatch(MethodDefinition patch)
        {
            var methodInfo = patch.Type.GetMethod(patch.MethodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (methodInfo == null)
            {
                throw new Exception($"IPT2: Failed to find patch method {patch.Type.FullName}.{patch.MethodName}");
            }

            return methodInfo;
        }

        public class MethodDefinition
        {
            public MethodDefinition(Type type, string methodName)
            {
                Type = type;
                MethodName = methodName;
            }

            public Type Type { get; }
            public string MethodName { get; set; }
        }
    }
}