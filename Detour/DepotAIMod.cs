using System;
using ColossalFramework;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ImprovedPublicTransport2.Detour
{
    [TargetType(typeof(DepotAI))]
    public class DepotAIMod : DepotAI
    {

        //TODO(earalov): do we need to redirect this?
        public static void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer, string prefabName)
        {
            //begin mod(+): some initializing
            var buildingAi = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)buildingID].Info?.m_buildingAI;
            var depotAi = buildingAi as DepotAI;
            if (depotAi == null)
            {
              throw new Exception("Non-depot building was selected as depot! Actual AI type: " + buildingAi?.GetType()); 
            }
            //end mod

            if (reason == depotAi.m_transportInfo.m_vehicleReason)
            {
                //begin mod(*): use our custom vehicle manager
                VehicleInfo randomVehicleInfo = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, depotAi.m_transportInfo.m_class, offer.TransportLine, prefabName);
                //end mod
                if (randomVehicleInfo == null)
                    return;
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                Vector3 position;
                Vector3 target;
                depotAi.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out position, out target);
                ushort vehicle;
                if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, reason, false, true))
                    return;
                randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int)vehicle], buildingID);
                randomVehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], reason, offer);
            }
            else if (depotAi.m_secondaryTransportInfo != null && reason == depotAi.m_secondaryTransportInfo.m_vehicleReason)
            {
                //begin mod(*): use our custom vehicle manager
                VehicleInfo randomVehicleInfo = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, depotAi.m_secondaryTransportInfo.m_class, offer.TransportLine, prefabName);
                //end mod
                if (randomVehicleInfo == null)
                    return;
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                Vector3 position;
                Vector3 target;
                depotAi.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out position, out target);
                ushort vehicle;
                if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, reason, false, true))
                    return;
                randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int)vehicle], buildingID);
                randomVehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], reason, offer);
            }
            //begin mod(-): no need fo this else case
            //end mid
        }
    }
}
