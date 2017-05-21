using System.Collections.Generic;
using System.Linq;

namespace ImprovedPublicTransport2
{
    public static class DepotUtil
    {
        public static bool IsValidDepot(ref Building building,
            ref TransportInfo transportInfo,
            out ItemClass.Service service,
            out ItemClass.SubService subService,
            out ItemClass.Level level)
        {
            service = ItemClass.Service.None;
            subService = ItemClass.SubService.None;
            level = ItemClass.Level.None;
            if (building.Info == null || (building.m_flags & Building.Flags.Created) == Building.Flags.None)
                return false;
            if (building.Info.m_buildingAI is DepotAI)
            {
                DepotAI buildingAi = building.Info.m_buildingAI as DepotAI;
                if (transportInfo != null && buildingAi.m_transportInfo.m_vehicleType != transportInfo.m_vehicleType)  //TODO(earalov): allow to serve as depot for secondary vehicle type
                {
                    return false;
                }
                if (transportInfo == null)
                {
                    transportInfo = buildingAi.m_transportInfo;
                }
                if (buildingAi.m_maxVehicleCount == 0)
                {
                    return false;
                }
                service = building.Info.m_class.m_service;
                subService = building.Info.m_class.m_subService;
                level = building.Info.m_class.m_level;
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
                            case ItemClass.SubService.PublicTransportShip:
                            case ItemClass.SubService.PublicTransportPlane:
                                return true;
                        }
                    }
                }

            }
            else if (building.Info.m_buildingAI is ShelterAI)
            {
                service = building.Info.m_class.m_service;
                subService = building.Info.m_class.m_subService;
                level = building.Info.m_class.m_level;
                if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None &&
                    level == ItemClass.Level.Level4)
                {
                    return true;
                }
            }
            return false;

        }
    }
}