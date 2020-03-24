using ColossalFramework;

namespace ImprovedPublicTransport2.Util
{
    public static class TransportLineUtil
    {
        public static ushort GetNextVehicle(ushort lineID, ushort vehicleID)
        {
            ushort vehicles = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].m_vehicles;
            ushort nextLineVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) vehicleID]
                .m_nextLineVehicle;
            if ((int) nextLineVehicle == 0)
                return vehicles;
            return nextLineVehicle;
        }

        public static ushort GetPreviousVehicle(ushort lineID, ushort vehicleID)
        {
            TransportLine transportLine = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID];
            int num1 = transportLine.CountVehicles(lineID);
            ushort num2 = 0;
            for (int index = 0; index < num1; ++index)
            {
                ushort vehicle = transportLine.GetVehicle(index);
                if ((int) vehicle == (int) vehicleID)
                {
                    if ((int) num2 == 0)
                        return transportLine.GetVehicle(num1 - 1);
                    return num2;
                }
                num2 = vehicle;
            }
            return transportLine.m_vehicles;
        }

        public static int GetStopIndex(ushort lineID, ushort stopID)
        {
            ushort stop = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].m_stops;
            int num1 = 0;
            int num2 = 0;
            while ((int) stop != 0)
            {
                if ((int) stopID == (int) stop)
                    return num1;
                ++num1;
                stop = TransportLine.GetNextStop(stop);
                if (++num2 >= 32768)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core,
                        "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
            return 0;
        }
    }
}