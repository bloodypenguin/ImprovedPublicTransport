using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class LoadPassengersPatch
    {
        private static ushort _transferSize1 = 0;
        private static ushort _vehicleID = 0;

        public static bool LoadPassengersPre(ushort vehicleID, ref Vehicle data, ushort currentStop, ushort nextStop)
        {
            _transferSize1 = VehicleUtil.AccumulatePassangers(vehicleID);
            _vehicleID = vehicleID;
            return true;
        }
        
        public static void LoadPassengersPost(ushort doNotUse, ref Vehicle data, ushort currentStop, ushort nextStop)
        {
            var curPassangers = VehicleUtil.AccumulatePassangers(_vehicleID);
            var num2 = curPassangers - _transferSize1;
            var ticketPrice = data.Info.m_vehicleAI.GetTicketPrice(_vehicleID, ref data);
            CachedVehicleData.m_cachedVehicleData[_vehicleID].Add(num2, ticketPrice);
            CachedNodeData.m_cachedNodeData[currentStop].PassengersIn += num2;
        }
    }
}