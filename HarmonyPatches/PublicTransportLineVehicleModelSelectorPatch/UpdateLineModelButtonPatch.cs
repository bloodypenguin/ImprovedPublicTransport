using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.PublicTransportLineVehicleModelSelectorPatch
{
    public class UpdateLineModelButtonPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleModelSelector),
                    "UpdateLineModelButton"),
                new PatchUtil.MethodDefinition(typeof(UpdateLineModelButtonPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleModelSelector),
                    "UpdateLineModelButton")
            );
        }

        public static bool Prefix(PublicTransportLineVehicleModelSelector __instance)
        {
            __instance.component.Hide();
            return false;
        }
    }
}