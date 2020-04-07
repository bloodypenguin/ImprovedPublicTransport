using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class PublicTransportLineVehicleModelSelectorRefreshPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleModelSelector),
                    nameof(PublicTransportLineVehicleModelSelector.Refresh)),
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleModelSelectorRefreshPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleModelSelector),
                    nameof(PublicTransportLineVehicleModelSelector.Refresh))
            );
        }

        public static void Prefix(PublicTransportLineVehicleModelSelector __instance)
        {
            __instance.component.Hide();
        }
    }
}