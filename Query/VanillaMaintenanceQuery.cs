namespace ImprovedPublicTransport2.Query
{
    public static class VanillaMaintenanceQuery
    {
        public static int Query(ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level, VehicleAI ai)
        {
            var num = 0;
            switch (service)
            {
                case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level1:
                {
                    num = subService switch
                    {
                        ItemClass.SubService.PublicTransportBus when ai is BusAI busAI => busAI.m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportMetro when ai is MetroTrainAI metroTrainAI => metroTrainAI
                            .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI => trainAI
                            .m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportShip when ai is PassengerShipAI shipAI => shipAI
                            .m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI
                            .m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportTaxi when ai is TaxiAI taxiAI => taxiAI.m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportTram when ai is TramAI tramAI => tramAI.m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportMonorail when ai is PassengerTrainAI trainAI => trainAI
                            .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportCableCar when ai is CableCarAI carAI => carAI
                            .m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportTrolleybus when ai is TrolleybusAI trolleybusAI =>
                            trolleybusAI
                                .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                        _ => 0
                    };

                    break;
                }
                case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level2:
                {
                    num = subService switch
                    {
                        ItemClass.SubService.PublicTransportBus when ai is BusAI busAI => busAI.m_transportInfo
                            ?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportShip when ai is PassengerFerryAI ferryAI => ferryAI
                            .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportPlane when ai is PassengerBlimpAI blimpAI => blimpAI
                            .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI
                            .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                        ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI => trainAI
                            .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                        _ => 0
                    };

                    break;
                }
                case ItemClass.Service.PublicTransport:
                {
                    if (level == ItemClass.Level.Level3)
                    {
                        num = subService switch
                        {
                            ItemClass.SubService.PublicTransportTours when ai is BusAI busAI => busAI.m_transportInfo
                                ?.m_maintenanceCostPerVehicle ?? 0,
                            ItemClass.SubService.PublicTransportPlane when ai is PassengerHelicopterAI helicopterAI =>
                                helicopterAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                            ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI
                                .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                            _ => 0
                        };
                    }

                    break;
                }
                case ItemClass.Service.Disaster when subService == ItemClass.SubService.None &&
                                                     level == ItemClass.Level.Level4 && ai is BusAI busAI:
                    num = busAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    break;
            }

            return num;
        }
    }
}