using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class LoadPassengersPatch
    {

        public static bool LoadPassengersPre(ushort vehicleID, ushort currentStop, out State __state)
        {
            var data = VehicleManager.instance.m_vehicles.m_buffer[vehicleID];
            if (data.m_leadingVehicle != 0)
            {
                __state = new State();
                return true;
            }

            __state = new State
            {
                vehicleID = vehicleID,
                currentStop = currentStop,
                currentPassengers = VehicleUtil.GetTotalPassengerCount(vehicleID, CachedVehicleData.MaxVehicleCount)
            };
            return true;
        }

        public static void LoadPassengersPost(State __state)
        {
            var data = VehicleManager.instance.m_vehicles.m_buffer[__state.vehicleID];
            if (data.m_leadingVehicle != 0)
            {
                return;
            }

            var currentPassengers = VehicleUtil.GetTotalPassengerCount(__state.vehicleID, CachedVehicleData.MaxVehicleCount);
            var passengersIn = Mathf.Max(0, currentPassengers - __state.currentPassengers);
            CachedVehicleData.m_cachedVehicleData[__state.vehicleID]
                .BoardPassengers(passengersIn, VehicleUtil.GetTicketPrice(__state.vehicleID), __state.currentStop);
            CachedNodeData.m_cachedNodeData[__state.currentStop].PassengersIn += passengersIn;
        }
        
        public struct State
        {
            public ushort vehicleID;
            public ushort currentPassengers;
            public ushort currentStop;
        }
    }
}