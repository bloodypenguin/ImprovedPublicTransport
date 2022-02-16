using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.TransportManagerPatches
{
    public static class CheckTransportLineVehiclesPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(TransportManager),
                    nameof(TransportManager.CheckTransportLineVehicles)),
                new PatchUtil.MethodDefinition(typeof(CheckTransportLineVehiclesPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(TransportManager),
                    nameof(TransportManager.CheckTransportLineVehicles))
            );
        }

        public static bool Prefix()
        {

            return false; //we don't want to despawn buses of the 'wrong type'
        }
    }
}