using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.PublicTransportLineVehicleModelSelectorPatch
{
    public class PopulateModelTemplatePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleModelSelector),
                    "PopulateModelTemplate"),
                new PatchUtil.MethodDefinition(typeof(UpdateLineModelButtonPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportLineVehicleModelSelector),
                    "PopulateModelTemplate")
            );
        }

        public static bool Prefix(PublicTransportLineVehicleModelSelector __instance)
        {
            __instance.component.Hide();
            return false;
        }
    }
}