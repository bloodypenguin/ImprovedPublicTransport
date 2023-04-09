using System;
using System.Collections.Generic;
using ColossalFramework;
using ImprovedPublicTransport2.Data;
using ImprovedPublicTransport2.ReverseDetours;

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

        //based off code in the SimulationStep of TransportLine
        public static int CountLineActiveVehicles(ushort lineID, out int allVehicles, Action<Int32> callback = null)
        {
            TransportLine thisLine = TransportManager.instance.m_lines.m_buffer[lineID];
            int activeVehicles = 0;
            allVehicles = 0;
            //this part is directly taken from beginning of vanilla SimulationStep method (except for marked part)

            if (thisLine.Complete)
            {
                int num2 = 0;
                int num3 = 0;
                if ((int) thisLine.m_vehicles != 0)
                {
                    VehicleManager instance3 = Singleton<VehicleManager>.instance;
                    ushort num4 = thisLine.m_vehicles;
                    int num5 = 0;
                    while ((int) num4 != 0)
                    {
                        ushort nextLineVehicle = instance3.m_vehicles.m_buffer[(int) num4].m_nextLineVehicle;
                        ++num2;
                        if ((instance3.m_vehicles.m_buffer[(int) num4].m_flags & Vehicle.Flags.GoingBack) ==
                            ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                              Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource |
                              Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath |
                              Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                              Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                              Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                              Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                              Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                              Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                              Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                              Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
                        {
                            //begin mod(+): callback
                            callback?.Invoke(num4);
                            //end mod
                            ++num3;
                        }

                        num4 = nextLineVehicle;
                        if (++num5 > CachedVehicleData.MaxVehicleCount)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core,
                                "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }

                //end of vanilla part
                activeVehicles = num3;
                allVehicles = num2;
            }

            return activeVehicles;
        }

        //based off code in TransportLine.SimulationStep
        public static void RemoveActiveVehicle(ushort lineID, bool descreaseTargetVehicleCount, int activeVehiclesCount)
        {
            ushort activeVehicle = TransportLineReverseDetour.GetActiveVehicle(
                ref Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID],
                Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) activeVehiclesCount));
            if ((int) activeVehicle != 0)
            {
                TransportLineUtil.RemoveVehicle(lineID, activeVehicle, descreaseTargetVehicleCount);
            }
        }

        //based off code in TransportLine.SimulationStep
        public static void RemoveVehicle(ushort lineID, ushort vehicleID, bool descreaseTargetVehicleCount)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            if ((instance.m_vehicles.m_buffer[(int) vehicleID].m_flags & Vehicle.Flags.GoingBack) ==
                ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                  Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                  Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                  Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                  Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                  Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                  Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                  Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                  Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                  Vehicle.Flags.LeftHandDrive))
            {
                if (descreaseTargetVehicleCount)
                {
                    CachedTransportLineData.DecreaseTargetVehicleCount(lineID);
                }

                instance.m_vehicles.m_buffer[(int) vehicleID].Info.m_vehicleAI.SetTransportLine(vehicleID,
                    ref instance.m_vehicles.m_buffer[(int) vehicleID], (ushort) 0);
            }
        }
    }
}