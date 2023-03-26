using System;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches.XYZVehicleAIPatches
{
    public class LoadPassengersPatch
    {
        private const string LoadPassengersMethod = "LoadPassengers";

        public static void Apply()
        {
            PatchLoadPassengers(typeof(BusAI));
            PatchLoadPassengers(typeof(TrolleybusAI));
            PatchLoadPassengers(typeof(TramAI));
            PatchLoadPassengers(typeof(PassengerTrainAI));
            PatchLoadPassengers(typeof(PassengerPlaneAI));
            PatchLoadPassengers(typeof(PassengerHelicopterAI));
            PatchLoadPassengers(typeof(PassengerBlimpAI));
            PatchLoadPassengers(typeof(PassengerFerryAI));
            PatchLoadPassengers(typeof(PassengerShipAI));
        }

        public static void Undo()
        {
            UnpatchLoadPassengers(typeof(BusAI));
            UnpatchLoadPassengers(typeof(TrolleybusAI));
            UnpatchLoadPassengers(typeof(TramAI));
            UnpatchLoadPassengers(typeof(PassengerTrainAI));
            UnpatchLoadPassengers(typeof(PassengerPlaneAI));
            UnpatchLoadPassengers(typeof(PassengerHelicopterAI));
            UnpatchLoadPassengers(typeof(PassengerBlimpAI));
            UnpatchLoadPassengers(typeof(PassengerFerryAI));
            UnpatchLoadPassengers(typeof(PassengerShipAI));
        }


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

            var currentPassengers =
                VehicleUtil.GetTotalPassengerCount(__state.vehicleID, CachedVehicleData.MaxVehicleCount);
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

        private static void PatchLoadPassengers(Type type)
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(type, LoadPassengersMethod),
                new PatchUtil.MethodDefinition(typeof(LoadPassengersPatch), nameof(LoadPassengersPre)),
                new PatchUtil.MethodDefinition(typeof(LoadPassengersPatch), nameof(LoadPassengersPost))
            );
        }

        private static void UnpatchLoadPassengers(Type type)
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(type, LoadPassengersMethod));
        }
    }
}