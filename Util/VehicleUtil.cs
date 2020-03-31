using ColossalFramework;


namespace ImprovedPublicTransport2.Util
{
    class VehicleUtil
    {
        public static ushort AccumulatePassangers(ushort vehicleID)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort passangers = 0;

            while (vehicleID != 0)
            {
                var data = instance.m_vehicles.m_buffer[vehicleID];
                passangers += data.m_transferSize;
                vehicleID = data.m_trailingVehicle;
            }
            return passangers;
        }
    }
}
