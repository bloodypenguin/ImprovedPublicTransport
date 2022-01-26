using System;
using ColossalFramework;

namespace ImprovedPublicTransport2.Util
{
    internal static class VehicleUtil
    {
        public static ushort GetTotalPassengerCount(ushort vehicleID, int maxVehicleCount)
        {
            var instance = VehicleManager.instance;
            var data = instance.m_vehicles.m_buffer[vehicleID];
            var trailingVehicle = data.m_trailingVehicle;
            var passengerCount = data.m_transferSize;
            var num = 0;
            while (trailingVehicle != 0)
            {
                var trailingData = instance.m_vehicles.m_buffer[trailingVehicle];
                passengerCount += trailingData.m_transferSize;
                trailingVehicle = trailingData.m_trailingVehicle;
                if (++num <= maxVehicleCount)
                {
                    continue;
                }

                CODebugBase<LogChannel>.Error(LogChannel.Core,
                    "Invalid list detected!\n" + Environment.StackTrace);
                break;
            }

            return passengerCount;
        }

        //TODO: customize per line
        public static int GetTicketPrice(ushort vehicleID)
        {
            var instance = VehicleManager.instance;
            var data = instance.m_vehicles.m_buffer[vehicleID];
            return data.Info.m_vehicleAI.GetTicketPrice(vehicleID, ref data);
        }

        public static bool AllowAllVehicleLevelsOnLine(ItemClass.SubService subService)
        {
            return (subService & (ItemClass.SubService.PublicTransportBus | ItemClass.SubService.PublicTransportTrain)) != ItemClass.SubService.None;
        }
    }
}