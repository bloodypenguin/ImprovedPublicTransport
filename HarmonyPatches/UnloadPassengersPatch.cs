using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class UnloadPassengersPatch
    {
        private static ushort _transferSize1 = 0;
        private static ushort _vehicleID = 0;

        public static bool UnloadPassengersPre(ushort vehicleID)
        {
            _transferSize1 = VehicleUtil.AccumulatePassangers(vehicleID);
            _vehicleID = vehicleID;
            return true;
        }

        public static void UnloadPassengersPost(ushort currentStop)
        {
            var curPassangers = VehicleUtil.AccumulatePassangers(_vehicleID);
            var num1 = _transferSize1 - curPassangers;
            CachedVehicleData.m_cachedVehicleData[_vehicleID].LastStopGonePassengers = num1;
            CachedVehicleData.m_cachedVehicleData[_vehicleID].CurrentStop = currentStop;
            CachedNodeData.m_cachedNodeData[currentStop].PassengersOut += num1;
        }
    }
}