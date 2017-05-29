namespace ImprovedPublicTransport2
{
    public static class DepotUtil
    {
        public static ItemClassTriplet GetStats(ref Building building,
            out TransportInfo primatyInfo, out TransportInfo secondaryInfo)
        {
            var depotAi = building.Info.m_buildingAI as DepotAI;
            if (depotAi == null || (depotAi.m_transportInfo==null && depotAi.m_secondaryTransportInfo == null))
            {
                primatyInfo = null;
                secondaryInfo = null;
                var shelterAi = building.Info.m_buildingAI as ShelterAI;
                return shelterAi == null ? new ItemClassTriplet(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None) :
                    new ItemClassTriplet(building.Info.GetService(), building.Info.GetSubService(), building.Info.GetClassLevel());
            }
            primatyInfo = depotAi.m_transportInfo;
            secondaryInfo = depotAi.m_secondaryTransportInfo;
            return new ItemClassTriplet(building.Info.GetService(), building.Info.GetSubService(), building.Info.GetClassLevel());
        }


        public static bool IsValidDepot(ref Building building, TransportInfo transportInfo)
        {
            var itemClass = GetStats(ref building, out _, out _);
            if (!itemClass.IsValid())
            {
                return false;
            }
            var service = itemClass.Service;
            var subService = itemClass.SubService;
            var level = itemClass.Level;
            if (building.Info?.m_class == null || (building.m_flags & Building.Flags.Created) == Building.Flags.None)
                return false;
            var depotAi = building.Info.m_buildingAI as DepotAI;
            if (depotAi != null)
            {
                var buildingAi = depotAi;
                if (transportInfo != null &&
                    (buildingAi.m_transportInfo?.m_vehicleType != transportInfo.m_vehicleType &&
                     buildingAi.m_secondaryTransportInfo?.m_vehicleType != transportInfo.m_vehicleType))
                {
                    return false;
                }
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
                            case ItemClass.SubService.PublicTransportShip:
                            case ItemClass.SubService.PublicTransportPlane:
                                return true;
                        }
                    }
                }

            }
            else if (building.Info.m_buildingAI is ShelterAI)
            {
                if (transportInfo != null && (transportInfo.m_vehicleType != ((ShelterAI)building.Info.m_buildingAI).m_transportInfo?.m_vehicleType))
                {
                    return false;
                }
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