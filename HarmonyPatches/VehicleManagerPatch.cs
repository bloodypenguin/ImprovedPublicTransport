using ImprovedPublicTransport2.Detour;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class VehicleManagerPatch
    {
        //the method is called from within ReleaseVehicle method. Patching it leads to the least chance of conflict
        public static void ReleaseWaterSource(ushort vehicle, ref Vehicle data)
        {
            if (!CachedVehicleData.m_cachedVehicleData[vehicle].IsEmpty)
            {
                CachedVehicleData.m_cachedVehicleData[vehicle] = new VehicleData();
            }
        }
    }
}