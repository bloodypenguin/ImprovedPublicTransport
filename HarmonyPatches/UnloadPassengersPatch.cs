using Harmony;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class UnloadPassengersPatch
    {
        public static bool UnloadPassengersPre(ushort vehicleID, ushort currentStop, out State __state)
        {
            if (VehicleManager.instance.m_vehicles.m_buffer[vehicleID].m_leadingVehicle != 0)
            {
                __state = new State();
                return true;
            }

            __state = new State()
            {
                vehicleID = vehicleID,
                currentStop = currentStop,
                currentPassengers = VehicleUtil.GetTotalPassengerCount(vehicleID, CachedVehicleData.MaxVehicleCount)
            };
            return true;
        }

        public static void UnloadPassengersPost(State  __state)
        {
            if (VehicleManager.instance.m_vehicles.m_buffer[__state.vehicleID].m_leadingVehicle != 0)
            {
                return;
            }
            
            var currentPassengers = VehicleUtil.GetTotalPassengerCount(__state.vehicleID, CachedVehicleData.MaxVehicleCount);
            var passengersOut = Mathf.Max(0, __state.currentPassengers - currentPassengers);
            CachedVehicleData.m_cachedVehicleData[__state.vehicleID].DisembarkPassengers(passengersOut, __state.currentStop);
            CachedNodeData.m_cachedNodeData[__state.currentStop].PassengersOut += passengersOut;
        }
        
        public struct State
        {
            public ushort vehicleID;
            public ushort currentPassengers;
            public ushort currentStop;
        }
    }
}