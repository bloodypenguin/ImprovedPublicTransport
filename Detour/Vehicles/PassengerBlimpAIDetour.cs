﻿using ColossalFramework;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;

namespace ImprovedPublicTransport2.Detour.Vehicles
{
    [TargetType(typeof(PassengerBlimpAI))]
    public class PassengerBlimpAIDetour : BlimpAI
    {
        [RedirectMethod]
        public override bool CanLeave(ushort vehicleID, ref Vehicle vehicleData)
        {
            if ((int)vehicleData.m_leadingVehicle == 0 && (int)vehicleData.m_waitCounter < 12 ||
                !base.CanLeave(vehicleID, ref vehicleData))
            {
                //begin mod(+): track if unbunching happens
                VehicleManagerMod.m_cachedVehicleData[(int)vehicleID].IsUnbunchingInProgress = false;
                //end mod
                return false;
            }
            //begin mod(+): no unbunching for evac buses or if only 1 vehicle on line!
            if (vehicleData.Info?.m_class?.m_service == ItemClass.Service.Disaster || (vehicleData.m_nextLineVehicle == 0 && Singleton<TransportManager>.instance.m_lines
                                                                                           .m_buffer[(int)vehicleData.m_transportLine].m_vehicles == vehicleID))
            {
                VehicleManagerMod.m_cachedVehicleData[vehicleID].IsUnbunchingInProgress = false;
                return true;
            }
            //end mod
            if ((int)vehicleData.m_leadingVehicle == 0 && (int)vehicleData.m_transportLine != 0)
            {
                //begin mod(+): Check if unbunching enabled for this line & stop. track if unbunching happens. Don't divide m_waitCounter by 2^4
                ushort currentStop = VehicleManagerMod.m_cachedVehicleData[vehicleID].CurrentStop;
                if (currentStop != 0 && NetManagerMod.m_cachedNodeData[currentStop].Unbunching &&
                    TransportLineMod.GetUnbunchingState(vehicleData.m_transportLine))
                {
                    var canLeaveStop = Singleton<TransportManager>.instance.m_lines
                        .m_buffer[(int)vehicleData.m_transportLine]
                        .CanLeaveStop(vehicleData.m_targetBuilding, (int)vehicleData.m_waitCounter);
                    VehicleManagerMod.m_cachedVehicleData[vehicleID].IsUnbunchingInProgress = !canLeaveStop;
                    return canLeaveStop;
                }
                //end mod
            }
            //begin mod(+): track if unbunching happens
            VehicleManagerMod.m_cachedVehicleData[vehicleID].IsUnbunchingInProgress = false;
            //end mod
            return true;
        }
    }
}