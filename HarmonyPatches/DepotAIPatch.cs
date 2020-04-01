using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class DepotAIPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(DepotAI), nameof(DepotAI.StartTransfer)),
                new PatchUtil.MethodDefinition(typeof(DepotAIPatch), nameof(StartTransferPre)),
                null
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(DepotAI), nameof(DepotAI.StartTransfer))
            );
        }

        private static bool StartTransferPre(
            DepotAI __instance,
            ref ushort buildingID, ref Building data,
            TransferManager.TransferReason reason,
            TransferManager.TransferOffer offer)
        {
            var lineID = offer.TransportLine;
            //TODO: fish boats?
            //TODO: also check reason? - see DepotAI
            var info = TransportManager.instance.m_lines.m_buffer[lineID].Info;
            if (lineID <= 0 || info?.m_class == null || info.m_class.m_service == ItemClass.Service.Disaster)
            {
                return true; //if it's not a proper transport line, let's not modify the behavior
            }

            var depot = CachedTransportLineData._lineData[lineID].Depot;
            if (!DepotUtil.ValidateDepotAndFindNewIfNeeded(lineID, ref depot, info))
            {
                if (depot == 0)
                {
                    Debug.LogWarning($"IPT2: No proper depot was found for line {lineID}!");
                    CachedTransportLineData.ClearEnqueuedVehicles(lineID);
                    return false;
                }

                Debug.LogWarning($"IPT2: Invalid or no depot was selected for line {lineID}, resetting to : {depot}!");
                CachedTransportLineData.ClearEnqueuedVehicles(lineID);
                return false;
            }


            if (depot == buildingID)
            {
                if (!(SimHelper.SimulationTime >= CachedTransportLineData._lineData[lineID].NextSpawnTime))
                {
                    return false; //if we need to wait before spawn, let's wait
                }

                if (!DepotUtil.CanAddVehicle(depot, ref BuildingManager.instance.m_buildings.m_buffer[depot], info))
                {
                    CachedTransportLineData.ClearEnqueuedVehicles(lineID);
                    return false;
                }

                CachedTransportLineData._lineData[lineID].NextSpawnTime =
                    SimHelper.SimulationTime + OptionsWrapper<Settings>.Options.SpawnTimeInterval;
            }
            else
            {
                Debug.Log("IPT2: Redirecting from " + buildingID + " to " + depot);
                __instance.StartTransfer(depot, ref BuildingManager.instance.m_buildings.m_buffer[depot], reason,
                    offer);
                return false;
            }


            string prefabName;
            if (CachedTransportLineData.EnqueuedVehiclesCount(lineID) > 0)
            {
                prefabName = CachedTransportLineData.Dequeue(lineID);
            }
            else
            {
                CachedTransportLineData.EnqueueVehicle(lineID,
                    CachedTransportLineData.GetRandomPrefab(lineID), false);
                prefabName = CachedTransportLineData.Dequeue(lineID);
            }

            //this is the most important line, when the AI calls TransportLine.GetLineVehicle(), then this field will be used
            TransportManager.instance.m_lines.m_buffer[lineID].m_vehicleInfoUniqueString = prefabName;
            return true;
        }
    }
}