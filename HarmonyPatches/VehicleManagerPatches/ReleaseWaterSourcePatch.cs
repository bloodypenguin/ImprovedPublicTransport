using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.VehicleManagerPatches
{
    public class ReleaseWaterSourcePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(VehicleManager), "ReleaseWaterSource"),
                null,
                new PatchUtil.MethodDefinition(typeof(ReleaseWaterSourcePatch), nameof(ReleaseWaterSourcePost))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(VehicleManager), "ReleaseWaterSource")
            );
        }


        //the method is called from within ReleaseVehicle method. Patching it leads to the least chance of conflict
        public static void ReleaseWaterSourcePost(ushort vehicle, ref Vehicle data)
        {
            if (!CachedVehicleData.m_cachedVehicleData[vehicle].IsEmpty)
            {
                CachedVehicleData.m_cachedVehicleData[vehicle] = new VehicleData();
            }
        }
    }
}