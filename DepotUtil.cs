namespace ImprovedPublicTransport2
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
    }
}