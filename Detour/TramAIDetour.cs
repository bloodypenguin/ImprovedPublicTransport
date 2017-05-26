// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.TramAIMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ImprovedPublicTransport2.Detour
{
    [TargetType(typeof(TramAI))] //detoured methods are same as in PassengerTrainAI
    public class TramAIDetour : TramBaseAI
    {
        [RedirectMethod]
        public override bool CanLeave(ushort vehicleID, ref Vehicle vehicleData)
        {
            if ((int) vehicleData.m_leadingVehicle == 0 && (int) vehicleData.m_waitCounter < 12 ||
                !base.CanLeave(vehicleID, ref vehicleData))
            {
                //begin mod(+): track if unbunching happens
                VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].IsUnbunchingInProgress = false;
                //end mod
                return false;
            }
            //begin mod(+): no unbunching for only 1 vehicle on line!
            if ((vehicleData.m_nextLineVehicle == 0 && Singleton<TransportManager>.instance.m_lines
                     .m_buffer[(int)vehicleData.m_transportLine].m_vehicles == vehicleID))
            {
                VehicleManagerMod.m_cachedVehicleData[vehicleID].IsUnbunchingInProgress = false;
                return true;
            }
            //end mod
            if ((int) vehicleData.m_leadingVehicle == 0 && (int) vehicleData.m_transportLine != 0)
            {
                //begin mod(+): Check if unbunching enabled for this line & stop. track if unbunching happens. Don't divide m_waitCounter by 2^4
                ushort currentStop = VehicleManagerMod.m_cachedVehicleData[vehicleID].CurrentStop;
                if (currentStop != 0 && NetManagerMod.m_cachedNodeData[currentStop].Unbunching &&
                    TransportLineMod.GetUnbunchingState(vehicleData.m_transportLine))
                {
                    var canLeaveStop = Singleton<TransportManager>.instance.m_lines
                        .m_buffer[(int) vehicleData.m_transportLine]
                        .CanLeaveStop(vehicleData.m_targetBuilding, (int) vehicleData.m_waitCounter);
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


        [RedirectMethod]
        private void LoadPassengers(ushort vehicleID, ref Vehicle data, ushort currentStop, ushort nextStop)
        {
            if ((int) currentStop == 0 || (int) nextStop == 0)
                return;
            CitizenManager instance1 = Singleton<CitizenManager>.instance;
            VehicleManager instance2 = Singleton<VehicleManager>.instance;
            NetManager instance3 = Singleton<NetManager>.instance;
            Vector3 position1 = instance3.m_nodes.m_buffer[(int) currentStop].m_position;
            Vector3 position2 = instance3.m_nodes.m_buffer[(int) nextStop].m_position;
            instance3.m_nodes.m_buffer[(int) currentStop].m_maxWaitTime = (byte) 0;
            int tempCounter = (int) instance3.m_nodes.m_buffer[(int) currentStop].m_tempCounter;
            bool flag = false;
            int num1 = Mathf.Max((int) (((double) position1.x - 64.0) / 8.0 + 1080.0), 0);
            int num2 = Mathf.Max((int) (((double) position1.z - 64.0) / 8.0 + 1080.0), 0);
            int num3 = Mathf.Min((int) (((double) position1.x + 64.0) / 8.0 + 1080.0), 2159);
            int num4 = Mathf.Min((int) (((double) position1.z + 64.0) / 8.0 + 1080.0), 2159);
            //begin mod(+): track passengers in
            int passengersIn = 0;
            //end mod
            for (int index1 = num2; index1 <= num4 && !flag; ++index1)
            {
                for (int index2 = num1; index2 <= num3 && !flag; ++index2)
                {
                    ushort instanceID = instance1.m_citizenGrid[index1 * 2160 + index2];
                    int num5 = 0;
                    while ((int) instanceID != 0 && !flag)
                    {
                        ushort nextGridInstance = instance1.m_instances.m_buffer[(int) instanceID].m_nextGridInstance;
                        if ((instance1.m_instances.m_buffer[(int) instanceID].m_flags &
                             CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None)
                        {
                            Vector3 targetPos = (Vector3) instance1.m_instances.m_buffer[(int) instanceID].m_targetPos;
                            if ((double) Vector3.SqrMagnitude(targetPos - position1) < 4096.0)
                            {
                                CitizenInfo info = instance1.m_instances.m_buffer[(int) instanceID].Info;
                                if (info.m_citizenAI.TransportArriveAtSource(instanceID,
                                    ref instance1.m_instances.m_buffer[(int) instanceID], position1, position2))
                                {
                                    ushort trailerID;
                                    uint unitID;
                                    if (Vehicle.GetClosestFreeTrailer(vehicleID, targetPos, out trailerID, out unitID))
                                    {
                                        if (info.m_citizenAI.SetCurrentVehicle(instanceID,
                                            ref instance1.m_instances.m_buffer[(int) instanceID], trailerID, unitID,
                                            position1))
                                        {
                                            ++tempCounter;
                                            ++instance2.m_vehicles.m_buffer[(int) trailerID].m_transferSize;
                                            //begin mod(+): increment passengers in
                                            ++passengersIn;
                                            //end mod
                                        }
                                        else
                                            flag = true;
                                    }
                                    else
                                        flag = true;
                                }
                            }
                        }
                        instanceID = nextGridInstance;
                        if (++num5 > 65536)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core,
                                "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
            }
            instance3.m_nodes.m_buffer[(int) currentStop].m_tempCounter =
                (ushort) Mathf.Min(tempCounter, (int) ushort.MaxValue);

            //begin mod (+): calculate statistics after loading passengers
            int ticketPrice = data.Info.m_vehicleAI.GetTicketPrice(vehicleID, ref data);
            VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].Add(passengersIn, ticketPrice);
            NetManagerMod.m_cachedNodeData[(int) currentStop].PassengersIn += passengersIn;
            //end mod
        }

        [RedirectMethod]
        private void UnloadPassengers(ushort vehicleID, ref Vehicle data, ushort currentStop, ushort nextStop)
        {
            if ((int) currentStop == 0)
                return;
            VehicleManager instance1 = Singleton<VehicleManager>.instance;
            NetManager instance2 = Singleton<NetManager>.instance;
            TransportManager instance3 = Singleton<TransportManager>.instance;
            Vector3 position = instance2.m_nodes.m_buffer[(int) currentStop].m_position;
            Vector3 targetPos = Vector3.zero;
            if ((int) nextStop != 0)
                targetPos = instance2.m_nodes.m_buffer[(int) nextStop].m_position;
            int serviceCounter = 0;
            int num = 0;

            //begin mod (+): calculate passenger count before unloading passengers
            ushort vehicleID1 = vehicleID;
            GetBufferStatus(vehicleID1, ref instance1.m_vehicles.m_buffer[(int) vehicleID1], out string localeKey,
                out int bufferStatusBefore, out int max);
            //end mod

            while ((int) vehicleID != 0)
            {
                if ((int) data.m_transportLine != 0)
                    BusAI.TransportArriveAtTarget(vehicleID, ref instance1.m_vehicles.m_buffer[(int) vehicleID],
                        position, targetPos, ref serviceCounter,
                        ref instance3.m_lines.m_buffer[(int) data.m_transportLine].m_passengers, (int) nextStop == 0);
                else
                    BusAI.TransportArriveAtTarget(vehicleID, ref instance1.m_vehicles.m_buffer[(int) vehicleID],
                        position, targetPos, ref serviceCounter,
                        ref instance3.m_passengers[(int) this.m_transportInfo.m_transportType], (int) nextStop == 0);
                vehicleID = instance1.m_vehicles.m_buffer[(int) vehicleID].m_trailingVehicle;
                if (++num > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core,
                        "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
            Singleton<StatisticsManager>.instance.Acquire<StatisticArray>(StatisticType.PassengerCount)
                .Acquire<StatisticInt32>((int) this.m_transportInfo.m_transportType, 10)
                .Add(serviceCounter);
            serviceCounter += (int) instance2.m_nodes.m_buffer[(int) currentStop].m_tempCounter;
            instance2.m_nodes.m_buffer[(int) currentStop].m_tempCounter =
                (ushort) Mathf.Min(serviceCounter, (int) ushort.MaxValue);

            //begin mod (+): calculate passenger count after unloading passengers
            GetBufferStatus(vehicleID1, ref instance1.m_vehicles.m_buffer[(int) vehicleID1], out localeKey,
                out int bufferStatusAfter, out max);
            int diff = bufferStatusBefore - bufferStatusAfter;
            VehicleManagerMod.m_cachedVehicleData[(int) vehicleID1].LastStopGonePassengers = diff;
            VehicleManagerMod.m_cachedVehicleData[(int) vehicleID1].CurrentStop = currentStop;
            NetManagerMod.m_cachedNodeData[(int) currentStop].PassengersOut += diff;
            //end mod
        }

        [RedirectReverse]
        public override void GetBufferStatus(ushort vehicleID, ref Vehicle data, out string localeKey, out int current,
            out int max)
        {
            UnityEngine.Debug.Log("GetBufferStatus");
            localeKey = "";
            current = 0;
            max = 0;
        }
    }
}