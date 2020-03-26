using ImprovedPublicTransport2.Detour;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class LoadPassengersPatch
    {
        private static ushort _transferSize1 = 0;
        
        public static bool LoadPassengersPre(ref Vehicle data)
        {
            _transferSize1 = data.m_transferSize;
            return true;
        }
        
        public static void LoadPassengersPost(ushort vehicleID, ref Vehicle data, ushort currentStop)
        {
            var num2 = (ushort)(data.m_transferSize - (uint)_transferSize1);
            var ticketPrice = data.Info.m_vehicleAI.GetTicketPrice(vehicleID, ref data);
            CachedVehicleData.m_cachedVehicleData[vehicleID].Add(num2, ticketPrice);
            CachedNodeData.m_cachedNodeData[currentStop].PassengersIn += num2;
        }
    }
}