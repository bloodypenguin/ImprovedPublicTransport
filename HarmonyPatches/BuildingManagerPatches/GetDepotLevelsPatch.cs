using ColossalFramework;
using ImprovedPublicTransport2.HarmonyPatches.DepotAIPatches;
using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.BuildingManagerPatches
{
    public static class GetDepotLevelsPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(BuildingManager), nameof(BuildingManager.GetDepotLevels)),
                new PatchUtil.MethodDefinition(typeof(GetDepotLevelsPatch), nameof(Prefix)),
                null
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(BuildingManager), nameof(BuildingManager.GetDepotLevels))
            );
        }

        private static bool Prefix(ref TransportLine.DepotLevels __result, ushort lineID)
        {
            var info1 = Singleton<TransportManager>.instance.m_lines.m_buffer[lineID].Info;
            if (info1 == null || info1.GetSubService() != ItemClass.SubService.PublicTransportBus ||
                info1.GetClassLevel() != ItemClass.Level.Level1)
            {
                return true;
            }
            __result = new TransportLine.DepotLevels()
            {
                m_level1 = true,
                m_level2 = true
            };
            return false;

        }
    }
}