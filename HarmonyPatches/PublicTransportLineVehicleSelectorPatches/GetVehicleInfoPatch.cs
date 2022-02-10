using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.PublicTransportLineVehicleSelectorPatches
{
    public class GetVehicleInfoPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleSelector),
                    "GetVehicleInfo"),
                new PatchUtil.MethodDefinition(typeof(GetVehicleInfoPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleSelector),
                    "GetVehicleInfo")
            );
        }

        public static bool Prefix(PublicTransportLineVehicleSelector __instance)
        {
            __instance.component.Hide();
            return false;
        }
    }
}