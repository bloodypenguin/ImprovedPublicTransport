using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ColossalFramework;
using HarmonyLib;
using ImprovedPublicTransport2.Data;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using static ImprovedPublicTransport2.ImprovedPublicTransportMod;

namespace ImprovedPublicTransport2.HarmonyPatches.XYZVehicleAIPatches
{
    public static class CanLeavePatch
    {
        private const string CanLeaveMethod = "CanLeave";
        private static ushort currentVehicleID;
        private static ushort currentStop;
        
        
        public static void Apply()
        {
            PatchCanLeave(typeof(BusAI));
            PatchCanLeave(typeof(TrolleybusAI));
            PatchCanLeave(typeof(TramAI));
            PatchCanLeave(typeof(PassengerTrainAI));
            PatchCanLeave(typeof(PassengerPlaneAI));
            PatchCanLeave(typeof(PassengerHelicopterAI));
            PatchCanLeave(typeof(PassengerBlimpAI));
            PatchCanLeave(typeof(PassengerFerryAI));
            PatchCanLeave(typeof(PassengerShipAI));
        }

        public static void Undo()
        {
            UnpatchCanLeave(typeof(BusAI));
            UnpatchCanLeave(typeof(TrolleybusAI));
            UnpatchCanLeave(typeof(TramAI));
            UnpatchCanLeave(typeof(PassengerTrainAI));
            UnpatchCanLeave(typeof(PassengerPlaneAI));
            UnpatchCanLeave(typeof(PassengerHelicopterAI));
            UnpatchCanLeave(typeof(PassengerBlimpAI));
            UnpatchCanLeave(typeof(PassengerFerryAI));
            UnpatchCanLeave(typeof(PassengerShipAI));
        }
        
        private static void PatchCanLeave(Type type)
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(type, CanLeaveMethod),
                new PatchUtil.MethodDefinition(typeof(CanLeavePatch), nameof(Prefix), priority: Priority.Normal),
                new PatchUtil.MethodDefinition(typeof(CanLeavePatch), nameof(Postfix), priority: Priority.Normal),
                new PatchUtil.MethodDefinition(typeof(CanLeavePatch), nameof(Transpile), priority: Priority.Normal)
            );
        }

        private static void UnpatchCanLeave(Type type)
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(type, CanLeaveMethod));
        }
        
        public static void Prefix(ushort vehicleID)
        {
            currentVehicleID = vehicleID;
            currentStop = CachedVehicleData.m_cachedVehicleData[currentVehicleID].CurrentStop;
        }


        public static void Postfix(ushort vehicleID, ref bool __result)
        {
            if (__result) //we don't know whether it was unbunching or just a traffic light or something else
            {
                CachedVehicleData.m_cachedVehicleData[vehicleID].IsUnbunchingInProgress = false;
            }

            currentVehicleID = 0;
            currentStop = 0;
        }
        
        public static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log($"{ShortModName}: CanLeavePatch - Transpiling method: {original.DeclaringType}.{original}");
            var replaced = false;
            foreach (var codeInstruction in instructions)
            {
                if (replaced || SkipInstruction(codeInstruction))
                {
                    yield return codeInstruction;
                    continue;
                }

                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(CanLeavePatch), nameof(TransportLineCanLeaveStopWrapper)))
                {
                    labels = codeInstruction.labels
                };
                Debug.Log($"{ShortModName}: CanLeavePatch - Replaced a call to CanLeaveStop");
                replaced = true;
            }
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Call || codeInstruction.operand  == null || !codeInstruction.operand.ToString().Contains("CanLeaveStop");
        }

        public static bool TransportLineCanLeaveStopWrapper(ref TransportLine transportLine, ushort nextStop, int waitTime) //the args are for the same signature
        {
            // no unbunching for evac buses or if only 1 bus on line!
            var vehicleData = VehicleManager.instance.m_vehicles.m_buffer[currentVehicleID];
            if (vehicleData.Info?.m_class?.m_service == ItemClass.Service.Disaster ||
                vehicleData.m_nextLineVehicle == 0 && Singleton<TransportManager>.instance.m_lines
                    .m_buffer[vehicleData.m_transportLine].m_vehicles == currentStop)
            {
                return true;
            }
            
            if (currentStop == 0 || !CachedNodeData.m_cachedNodeData[currentStop].Unbunching ||
                !CachedTransportLineData.GetUnbunchingState(vehicleData.m_transportLine))
            {
                return true;
            }
            // ignore the provided waitTime as it's divided.
            // Don't divide m_waitCounter by 2^4! Because the call goes to our CanLeaveStop patch too where it expects a non-divided value..
            var canLeaveStop = Singleton<TransportManager>.instance.m_lines
                .m_buffer[vehicleData.m_transportLine]
                .CanLeaveStop(vehicleData.m_targetBuilding, vehicleData.m_waitCounter); 
            CachedVehicleData.m_cachedVehicleData[currentVehicleID].IsUnbunchingInProgress = !canLeaveStop;
            return canLeaveStop;

        }
    }
}