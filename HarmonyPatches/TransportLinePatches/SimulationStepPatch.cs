using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using ColossalFramework;
using HarmonyLib;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches.TransportLinePatches
{
    public class SimulationStepPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(TransportLine), nameof(TransportLine.SimulationStep)),
                new PatchUtil.MethodDefinition(typeof(SimulationStepPatch), nameof(Prefix)),
                new PatchUtil.MethodDefinition(typeof(SimulationStepPatch), nameof(Postfix)),
                new PatchUtil.MethodDefinition(typeof(SimulationStepPatch), nameof(Transpile))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(TransportLine), nameof(TransportLine.SimulationStep))
            );
        }

        public static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("IPT 2: Transpiling method: " + original.DeclaringType + "." + original);

            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            foreach (var codeInstruction in codes)
            {
                if (SkipInstruction(codeInstruction))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }

                if (codeInstruction.operand.ToString().Contains(nameof(EconomyManager.FetchResource)))
                {
                    Debug.Log("IPT 2: Replacing call to FetchResourceStub()");
                    newCodes.Add(new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(SimulationStepPatch), nameof(FetchResourceStub))));
                    continue;
                }

                Debug.Log("IPT 2: Replacing call to CalculateTargetVehicleCount()");
                var thisInstruction = newCodes[newCodes.Count() - 1];
                newCodes.RemoveAt(newCodes.Count() - 1);

                newCodes.Add(new CodeInstruction(OpCodes.Ldarg_1)
                {
                    labels = thisInstruction.labels //need to preserve the label
                });
                newCodes.Add(new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(SimulationStepPatch), nameof(CalculateTargetVehicleCount))));
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.operand == null ||
                   !(codeInstruction.opcode == OpCodes.Callvirt && codeInstruction.operand.ToString()
                       .Contains(nameof(EconomyManager.FetchResource))) &&
                   !(codeInstruction.opcode == OpCodes.Call && codeInstruction.operand.ToString()
                       .Contains(nameof(TransportLine.CalculateTargetVehicleCount)));
        }

        public static bool Prefix(ushort lineID, out ushort __state)
        {
            __state = lineID;
            return true;
        }

        public static void Postfix(ushort __state)
        {
            var lineID = __state;
            if (!CachedTransportLineData._init || !((SimulationManager.instance.m_currentFrameIndex & 4095U) >= 3840U) ||
                !TransportManager.instance.m_lines.m_buffer[lineID].Complete)
            {
                return;
            }

            var stops1 = TransportManager.instance.m_lines.m_buffer[lineID].m_stops;
            var stop1 = stops1;
            do
            {
                CachedNodeData.m_cachedNodeData[stop1].StartNewWeek();
                stop1 = TransportLine.GetNextStop(stop1);
            } while (stops1 != stop1 && stop1 != 0);

            var itemClass = TransportManager.instance.m_lines.m_buffer[lineID].Info.m_class;
            var prefabs =
                VehiclePrefabs.instance.GetPrefabs(itemClass.m_service, itemClass.m_subService, itemClass.m_level);
            var amount = 0;
            TransportLineUtil.CountLineActiveVehicles(lineID, out _, (num3) =>
            {
                var prefabData = Array.Find(prefabs,
                    item => item.PrefabDataIndex ==
                            VehicleManager.instance.m_vehicles.m_buffer[num3].Info.m_prefabDataIndex);
                if (prefabData == null) return;
                amount += prefabData.MaintenanceCost;
                CachedVehicleData.m_cachedVehicleData[num3].StartNewWeek(prefabData.MaintenanceCost);
            });
            if (amount != 0)
                Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.Maintenance, amount,
                    TransportManager.instance.m_lines.m_buffer[lineID].Info.m_class);
        }

        public static int CalculateTargetVehicleCount(ushort lineID)
        {
            var instance = TransportManager.instance;
            int targetVehicleCount;
            if (CachedTransportLineData._lineData[lineID].BudgetControl ||
                instance.m_lines.m_buffer[lineID].Info.m_class.m_service == ItemClass.Service.Disaster)
            {
                targetVehicleCount = instance.m_lines.m_buffer[lineID].CalculateTargetVehicleCount();
                CachedTransportLineData.SetTargetVehicleCount(lineID, targetVehicleCount);
            }
            else
            {
                targetVehicleCount = CachedTransportLineData.GetTargetVehicleCount(lineID);
            }

            var activeVehicles = TransportLineUtil.CountLineActiveVehicles(lineID, out _);
            for (var i = activeVehicles; i < targetVehicleCount - CachedTransportLineData.EnqueuedVehiclesCount(lineID); i++)
            {
                CachedTransportLineData.EnqueueVehicle(lineID, CachedTransportLineData.GetRandomPrefab(lineID));
            }
            return targetVehicleCount;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int FetchResourceStub(EconomyManager economyManager, EconomyManager.Resource resource,
            int amount,
            ItemClass itemClass)
        {
            return 0;
        }
    }
}