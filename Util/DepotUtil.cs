using System;
using ColossalFramework;
using ImprovedPublicTransport2.Detour;
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


        public static bool IsValidDepot(ushort depotID, TransportInfo transportInfo)
        {
            if (transportInfo == null || depotID == 0)
            {
                return false;
            }

            var building = BuildingManager.instance.m_buildings.m_buffer[depotID];
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
                            case ItemClass.SubService.PublicTransportTrolleybus:    
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
                            case ItemClass.SubService.PublicTransportPlane:
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

        public static bool ValidateDepotAndFindNewIfNeeded(ushort lineID, ref ushort depotID, TransportInfo transportInfo)
        {
            if (transportInfo == null)
            {
                return false;
            }
            if (depotID != 0 &&
                DepotUtil.IsValidDepot(depotID, transportInfo))
            {
                return true;
            }

            depotID = AutoAssignLineDepot(lineID, out _);
            return depotID != 0;
        }
        
        public static ushort AutoAssignLineDepot(ushort lineID, out Vector3 stopPosition)
        {
            stopPosition = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) TransportManager.instance.m_lines.m_buffer[(int) lineID].GetStop(0)]
                .m_position;
            ushort closestDepot = DepotUtil.GetClosestDepot(lineID, stopPosition);
            if ((int) closestDepot != 0)
            {
                CachedTransportLineData.SetDepot(lineID, closestDepot);
                UnityEngine.Debug.LogWarning($"IPT2: auto assigned depot {closestDepot} to line {lineID}");
            }

            return closestDepot;
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

        public static bool CanAddVehicle(ushort depotID, ref Building depot, TransportInfo transportInfo)
        {
            if (depotID == 0 || depot.Info == null)
            {
                return false;
            }
            if (depot.Info.m_buildingAI is DepotAI)
            {
                DepotAI buildingAi = depot.Info.m_buildingAI as DepotAI;
                if (transportInfo.m_vehicleType == buildingAi.m_transportInfo?.m_vehicleType ||
                    transportInfo.m_vehicleType == buildingAi.m_secondaryTransportInfo?.m_vehicleType)
                {
                    int num = (PlayerBuildingAI.GetProductionRate(100,
                            Singleton<EconomyManager>.instance.GetBudget(buildingAi.m_info.m_class)) * 
                        buildingAi.m_maxVehicleCount + 99) / 100;
                    return buildingAi.GetVehicleCount(depotID, ref depot) < num;
                }
            }
            if (depot.Info.m_buildingAI is ShelterAI)
            {
                ShelterAI buildingAi = depot.Info.m_buildingAI as ShelterAI;
                int num = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(buildingAi.m_info.m_class)) * buildingAi.m_evacuationBusCount + 99) / 100;
                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int outside = 0;
                CommonBuildingAIReverseDetour.CalculateOwnVehicles(buildingAi, depotID, ref depot, buildingAi.m_transportInfo.m_vehicleReason, ref count, ref cargo, ref capacity, ref outside);
                return count < num;
            }
            return false;

        }
    }
}