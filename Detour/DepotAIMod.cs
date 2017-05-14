using ColossalFramework;
using ImprovedPublicTransport.Redirection.Attributes;
using UnityEngine;

namespace ImprovedPublicTransport.Detour
{
    [TargetType(typeof(DepotAI))]
    public class DepotAIMod : DepotAI
    {

        //TODO(earalov): add support for two transport line types
        //TODO(earalov): do we actually need to redirect this?
        public static ushort StartTransfer(DepotAI buildingAi, ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer, string prefabName)
        {
            SimulationManager instance1 = Singleton<SimulationManager>.instance;
            VehicleManager instance2 = Singleton<VehicleManager>.instance;
            ushort vehicle = 0;
            if (reason == buildingAi.m_transportInfo.m_vehicleReason)
            {
                VehicleInfo vehicleInfo = VehicleManagerMod.GetVehicleInfo(ref instance1.m_randomizer, buildingAi.m_info.m_class, offer.TransportLine, prefabName);
                if ((Object)vehicleInfo != (Object)null)
                {
                    Vector3 position;
                    Vector3 target;
                    buildingAi.CalculateSpawnPosition(buildingID, ref data, ref instance1.m_randomizer, vehicleInfo, out position, out target);
                    if (instance2.CreateVehicle(out vehicle, ref instance1.m_randomizer, vehicleInfo, position, reason, false, true))
                    {
                        vehicleInfo.m_vehicleAI.SetSource(vehicle, ref instance2.m_vehicles.m_buffer[(int)vehicle], buildingID);
                        vehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref instance2.m_vehicles.m_buffer[(int)vehicle], reason, offer);
                    }
                }
            }
            return vehicle;
        }
    }
}
