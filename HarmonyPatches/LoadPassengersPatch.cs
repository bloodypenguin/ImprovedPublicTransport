using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class LoadPassengersPatch
    {
        public static bool LoadPassengersPre(ushort vehicleID, out ushort __state)
        {
            var data = VehicleManager.instance.m_vehicles.m_buffer[vehicleID];
            if (data.m_leadingVehicle != 0)
            {
                __state = 0;
                return true;
            }

            __state = VehicleUtil.GetTotalPassengerCount(vehicleID, CachedVehicleData.MaxVehicleCount);
            return true;
        }

        public static void LoadPassengersPost(ushort vehicleID, ushort currentStop, ushort __state)
        {
            var data = VehicleManager.instance.m_vehicles.m_buffer[vehicleID];
            if (data.m_leadingVehicle != 0)
            {
                return;
            }

            var currentPassengers = VehicleUtil.GetTotalPassengerCount(vehicleID, CachedVehicleData.MaxVehicleCount);
            var newPassengers = Mathf.Max(0, currentPassengers - __state);
            CachedVehicleData.m_cachedVehicleData[vehicleID]
                .BoardPassengers(newPassengers, VehicleUtil.GetTicketPrice(vehicleID), currentStop);
            CachedNodeData.m_cachedNodeData[currentStop].PassengersIn += newPassengers;
        }
    }
}