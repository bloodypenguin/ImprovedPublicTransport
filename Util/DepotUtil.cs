using System;
using ColossalFramework;
using UnityEngine;

namespace ImprovedPublicTransport2.Util
{
    
    public static class DepotUtil
    {
        public static void GetStats(ref Building building,
            out TransportInfo primatyInfo, out TransportInfo secondaryInfo)
        {
            var depotAi = building.Info?.m_buildingAI as DepotAI;
            if (depotAi == null || (depotAi.m_transportInfo == null && depotAi.m_secondaryTransportInfo == null))
            {
                var shelterAi = building.Info?.m_buildingAI as ShelterAI;
                if (shelterAi == null || shelterAi.m_transportInfo == null)
                {
                    primatyInfo = null;
                    secondaryInfo = null;
                }
                else
                {
                    primatyInfo = shelterAi.m_transportInfo;
                    secondaryInfo = null;
                }
            }
            else
            {
                primatyInfo = depotAi.m_transportInfo;
                secondaryInfo = depotAi.m_secondaryTransportInfo;
            }
        }


        public static bool IsValidDepot(ref Building building, TransportInfo transportInfo)
        {
            if (transportInfo == null)
            {
                return false;
            }
            if (building.Info?.m_class == null || (building.m_flags & Building.Flags.Created) == Building.Flags.None)
                return false;
            GetStats(ref building, out TransportInfo primaryInfo, out TransportInfo secondaryInfo);
            if (primaryInfo == null && secondaryInfo == null)
            {
                return false;
            }
            ItemClass.Service service;
            ItemClass.SubService subService;
            ItemClass.Level level;
            if (transportInfo.m_vehicleType == primaryInfo?.m_vehicleType)
            {
                service = primaryInfo.GetService();
                subService = primaryInfo.GetSubService();
                level = primaryInfo.GetClassLevel();
            }
            else if (transportInfo.m_vehicleType == secondaryInfo?.m_vehicleType && transportInfo.m_vehicleType != VehicleInfo.VehicleType.Car)
            {
                service = secondaryInfo.GetService();
                subService = secondaryInfo.GetSubService();
                level = secondaryInfo.GetClassLevel();
            }
            else
            {
                return false;
            }
            var depotAi = building.Info.m_buildingAI as DepotAI;
            if (depotAi != null)
            {
                var buildingAi = depotAi;
                if (buildingAi.m_maxVehicleCount == 0)
                {
                    return false;
                }
                if (service == ItemClass.Service.PublicTransport)
                {
                    if (level == ItemClass.Level.Level1)
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportBus:
                            case ItemClass.SubService.PublicTransportMetro:
                            case ItemClass.SubService.PublicTransportTrain:
                            case ItemClass.SubService.PublicTransportShip:
                            case ItemClass.SubService.PublicTransportPlane:
                            case ItemClass.SubService.PublicTransportTram:
                            case ItemClass.SubService.PublicTransportMonorail:
                            case ItemClass.SubService.PublicTransportTaxi:
                            case ItemClass.SubService.PublicTransportCableCar:
                                return true;
                        }
                    }
                    else if (level == ItemClass.Level.Level2)
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportBus:
                            case ItemClass.SubService.PublicTransportShip:
                            case ItemClass.SubService.PublicTransportPlane:
                                return true;
                        }
                    }
                    else if (level == ItemClass.Level.Level3)
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportTours:
                                return true;
                        }
                    }
                }
            }
            else if (building.Info.m_buildingAI is ShelterAI)
            {
                if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None &&
                    level == ItemClass.Level.Level4)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ValidateDepot(ushort lineID, ref ushort depotID, TransportInfo transportInfo)
        {
            if (transportInfo == null)
            {
                return false;
            }
            if (depotID != 0 &&
                DepotUtil.IsValidDepot(ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[depotID], transportInfo))
            {
                return true;
            }
            depotID = GetClosestDepot(lineID,
                Singleton<NetManager>.instance.m_nodes
                    .m_buffer[Singleton<TransportManager>.instance.m_lines.m_buffer[lineID].GetStop(0)]
                    .m_position);
            CachedTransportLineData._lineData[lineID].Depot = depotID;
            return depotID != 0;
        }

        public static ushort GetClosestDepot(ushort lineID, Vector3 stopPosition) //TODO(earalov): What happens if closest depot is not connected/not reachable?
        {
            ushort num1 = 0;
            float num2 = Single.MaxValue;
            BuildingManager instance = Singleton<BuildingManager>.instance;
            TransportInfo info = Singleton<TransportManager>.instance.m_lines
                .m_buffer[(int) lineID]
                .Info;
            ushort[] depots = BuildingExtension.GetDepots(info);
            for (int index = 0; index < depots.Length; ++index)
            {
                float num3 = Vector3.Distance(stopPosition,
                    instance.m_buildings.m_buffer[(int) depots[index]].m_position);
                if ((double) num3 < (double) num2)
                {
                    num1 = depots[index];
                    num2 = num3;
                }
            }
            return num1;
        }

        //Based on DepotAI method of the same name
        //TODO replace with a proper detour
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
                VehicleInfo randomVehicleInfo = CachedVehicleData.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, depotAi.m_transportInfo.m_class, offer.TransportLine, prefabName);
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
                VehicleInfo randomVehicleInfo = CachedVehicleData.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, depotAi.m_secondaryTransportInfo.m_class, offer.TransportLine, prefabName);
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