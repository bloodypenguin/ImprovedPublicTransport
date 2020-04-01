using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class UnloadPassengersPatch
    {
        public static bool UnloadPassengersPre(ushort vehicleID, out ushort __state)
        {
            if (VehicleManager.instance.m_vehicles.m_buffer[vehicleID].m_leadingVehicle != 0)
            {
                __state = 0;
                return true;
            }

            __state = VehicleUtil.GetTotalPassengerCount(vehicleID, CachedVehicleData.MaxVehicleCount);
            return true;
        }

        public static void UnloadPassengersPost(ushort vehicleID, ushort currentStop, ushort __state)
        {
            if (VehicleManager.instance.m_vehicles.m_buffer[vehicleID].m_leadingVehicle != 0)
            {
                return;
            }

            var currentPassengers = VehicleUtil.GetTotalPassengerCount(vehicleID, CachedVehicleData.MaxVehicleCount);
            var passengersOut = Mathf.Max(0, __state - currentPassengers);
            CachedVehicleData.m_cachedVehicleData[vehicleID].DisembarkPassengers(passengersOut);
            CachedNodeData.m_cachedNodeData[currentStop].PassengersOut += passengersOut;
        }
    }
}