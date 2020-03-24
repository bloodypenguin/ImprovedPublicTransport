using ImprovedPublicTransport2.Detour;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class UnloadPassengersPatch
    {
        private static ushort _transferSize1 = 0;
        
        public static bool UnloadPassengersPre(ref Vehicle data)
        {
            _transferSize1 = data.m_transferSize;
            return true;
        }
        
        public static void UnloadPassengersPost(ushort vehicleID, ref Vehicle data, ushort currentStop)
        {
            var num1 = (ushort)(_transferSize1 - (uint)data.m_transferSize);
            VehicleManagerMod.m_cachedVehicleData[vehicleID].LastStopGonePassengers = num1;
            VehicleManagerMod.m_cachedVehicleData[vehicleID].CurrentStop = currentStop;
            NetManagerMod.m_cachedNodeData[currentStop].PassengersOut += num1;
        }
    }
}