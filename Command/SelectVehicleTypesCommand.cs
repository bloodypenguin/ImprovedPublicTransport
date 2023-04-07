using System.Collections.Generic;
using ColossalFramework;
using ImprovedPublicTransport2.Query;
using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.Command
{
    public static class SelectVehicleTypesCommand
    {
        public static void Execute(HashSet<string> selectedItems)
        {
            var lineId = WorldInfoCurrentLineIDQuery.Query(out _);
            if (lineId == 0)
            {
                return;
            }

            CachedTransportLineData.SetPrefabs(lineId, selectedItems);
            Singleton<SimulationManager>.instance.AddAction(() => ReplaceVehicles(lineId));
        }
        
        //roughly based on TransportLine.ReplaceVehicles()
        private static void ReplaceVehicles(ushort lineID)
        {
            var instance = Singleton<VehicleManager>.instance;
            for (var i = 0; i < instance.m_vehicles.m_buffer.Length; ++i)
            {
                if (instance.m_vehicles.m_buffer[i].m_flags == 0 || instance.m_vehicles.m_buffer[i].Info == null ||
                    instance.m_vehicles.m_buffer[i].m_transportLine != lineID)
                {
                    continue;
                }

                if (CachedTransportLineData.GetPrefabs(lineID).Contains(instance.m_vehicles.m_buffer[i].Info.name))
                {
                    continue;
                }
                TransportLineUtil.RemoveVehicle(lineID, (ushort)i, false);
            }
        }
    }
}