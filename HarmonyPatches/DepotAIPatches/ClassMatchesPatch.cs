using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.DepotAIPatches
{
    public static class ClassMatchesPatch
    {
        
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(DepotAI), "ClassMatches"),
                new PatchUtil.MethodDefinition(typeof(ClassMatchesPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(DepotAI), "ClassMatches")
            );
        }
        
        private static bool Prefix(ref bool __result, ItemClass itemClass, ItemClass otherItemClass)
        {
            if (itemClass.m_subService == ItemClass.SubService.PublicTransportBus 
                && (itemClass.m_level == ItemClass.Level.Level1 || itemClass.m_level == ItemClass.Level.Level2))    
            {
                __result = otherItemClass.m_subService == ItemClass.SubService.PublicTransportBus && (otherItemClass.m_level == ItemClass.Level.Level1 || otherItemClass.m_level == ItemClass.Level.Level2);
                return false;
            }

            return true;
        }
    }
}