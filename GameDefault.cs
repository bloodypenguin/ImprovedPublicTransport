namespace ImprovedPublicTransport2
{
    public static class GameDefault
    {
        public static ushort GetCapacity(ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level, VehicleInfo.VehicleType vehicleType)
        {
            if (service == ItemClass.Service.PublicTransport)
            {
                if (level == ItemClass.Level.Level1)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportTram:
                        case ItemClass.SubService.PublicTransportCableCar:
                        case ItemClass.SubService.PublicTransportMonorail:
                        case ItemClass.SubService.PublicTransportBus:
                        case ItemClass.SubService.PublicTransportMetro:
                        case ItemClass.SubService.PublicTransportTrain:
                        case ItemClass.SubService.PublicTransportTrolleybus:
                            return 30;
                        case ItemClass.SubService.PublicTransportShip:
                            return 100;
                        case ItemClass.SubService.PublicTransportPlane:
                            return 200;
                    }
                }
                else if (level == ItemClass.Level.Level2)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportPlane:
                            return (ushort)(vehicleType == VehicleInfo.VehicleType.Blimp ? 35 : 350);
                        case ItemClass.SubService.PublicTransportShip:
                            return 50;
                        case ItemClass.SubService.PublicTransportBus:
                            return 30;
                        case ItemClass.SubService.PublicTransportTrain:
                            return 30;
                    }
                }
                else if (level == ItemClass.Level.Level3)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportTours:
                            return 30;
                        case ItemClass.SubService.PublicTransportPlane:
                            return (ushort)(vehicleType == VehicleInfo.VehicleType.Helicopter ? 20 : 50);
                    }
                }
            }
            UnityEngine.Debug.LogWarning("IPT: Default capacity of item class [serrvice: " + service + ", sub service: " +
                                         subService +
                                         ", level: " + level +
                                         "] were requested. Defaults aren't supported.");
            return 0;
        }
    }
}