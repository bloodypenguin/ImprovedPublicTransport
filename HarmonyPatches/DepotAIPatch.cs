using System.Reflection;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    
    //DepotAI.StartTransfer() is directly invoked by TransportLineDetour, in contrast with original TransportLine that
    //makes use of TransferManager
    public class DepotAIPatch
    {
        public static MethodInfo GetPrefix()
        {
            return typeof(DepotAIPatch).GetMethod(nameof(StartTransferPre),
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static bool StartTransferPre(ushort buildingID, ref Building data,
            TransferManager.TransferReason reason,
            TransferManager.TransferOffer offer)
        {
            var lineID = offer.TransportLine;
            if (lineID <= 0)
            {
                return true;
            }
            string prefabName;
            if (CachedTransportLineData.EnqueuedVehiclesCount(lineID) > 0)
            {
                prefabName = CachedTransportLineData.Dequeue(lineID);
            }
            else
            {
                //Priority: targetVehicleCount - activeVehicleCount + 1
                for (var index2 = 0; index2 < offer.Priority - 1; ++index2)
                {
                    CachedTransportLineData.EnqueueVehicle(lineID,
                        CachedTransportLineData.GetRandomPrefab(lineID), false);
                }
                //we enqueue the number of vehicles we need and dequeue one immediately
                prefabName = CachedTransportLineData.Dequeue(lineID);
            }

            //this is the most important line, when the AI calls TransportLine.GetLineVehicle(), then this field will be used
            TransportManager.instance.m_lines.m_buffer[lineID].m_vehicleInfoUniqueString = prefabName;
            return true;
        }
    }
}