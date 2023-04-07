using ColossalFramework;

namespace ImprovedPublicTransport2.Query
{
    public static class WorldInfoLineIDForCurrentVehicleQuery
    {
        public static ushort Query(out ushort firstVehicle)
        {
            firstVehicle = 0;
            var currentInstanceId = WorldInfoPanel.GetCurrentInstanceID();
            if (currentInstanceId.Type != InstanceType.Vehicle || currentInstanceId.Vehicle == 0)
            {
                return 0;
            }
            firstVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[currentInstanceId.Vehicle].GetFirstVehicle(currentInstanceId.Vehicle);
            return firstVehicle != 0 ? Singleton<VehicleManager>.instance.m_vehicles.m_buffer[firstVehicle].m_transportLine : (ushort)0;
        }
    }
}