// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.TransportLineMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using ColossalFramework;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.Detour
{
    [TargetType(typeof(TransportLine))]
    public struct TransportLineDetour
    {
        
        [RedirectMethod]
        public static bool CanLeaveStop(ref TransportLine thisLine, ushort nextStop, int waitTime)
        {
            if ((int)nextStop == 0)
                return true;
            ushort prevSegment = TransportLine.GetPrevSegment(nextStop);
            if ((int)prevSegment == 0 || ((int)thisLine.m_averageInterval - (int)Singleton<NetManager>.instance.m_segments.m_buffer[(int)prevSegment].m_trafficLightState0 + 2) / 4 <= 0)
                return true;
            //begin mod(*): compare with interval aggression setup instead of default 64
            var targetWaitTime = Mathf.Min(OptionsWrapper<Settings>.Options.IntervalAggressionFactor + 12, byte.MaxValue);
            return waitTime >= targetWaitTime; //4 * 16 = 64 is max waiting time in vanilla, 12 is min waiting time
            //end mod
        }

        [RedirectMethod]
        public static void SimulationStep(ref TransportLine thisLine, ushort lineID)
        {
            //begin mod(+): change for initialization
            if (!CachedTransportLineData._init)
                return;
            //end mod

            TransportInfo info = thisLine.Info;
            NetManager instance1 = Singleton<NetManager>.instance;
            SimulationManager instance2 = Singleton<SimulationManager>.instance;
            Notification.Problem problems1 = Notification.Problem.None;
            Notification.Problem problem1 = Notification.Problem.None;
            Notification.Problem problem2 = Notification.Problem.None;
            Notification.Problem problems2 = Notification.Problem.TooLong | Notification.Problem.MajorProblem;
            if ((int) thisLine.m_stops != 0)
            {
                problem1 = instance1.m_nodes.m_buffer[(int) thisLine.m_stops].m_problems;
                problems1 = problem1;
            }
            bool flag1 = (instance2.m_currentFrameIndex & 4095U) >= 3840U;
            float num1 = 0.0f;
            bool flag2 = false;
            if (thisLine.Complete)
            {
                //begin mod(*): moved this section to a separate method
                int num3 = CountLineActiveVehicles(lineID, out int num2);
                //end mod
                bool flag3 = !instance2.m_isNightTime
                    ? (thisLine.m_flags & TransportLine.Flags.DisabledDay) == TransportLine.Flags.None
                    : (thisLine.m_flags & TransportLine.Flags.DisabledNight) == TransportLine.Flags.None;
                uint range = 0;
                int num6 = 0;
                int num7 = 0;
                int num8 = 0;
                if ((int) thisLine.m_stops != 0)
                {
                    CitizenManager instance3 = Singleton<CitizenManager>.instance;
                    ushort stops = thisLine.m_stops;
                    ushort num4 = stops;
                    int num5 = 0;
                    while ((int) num4 != 0)
                    {
                        ushort num9 = 0;
                        if (flag3)
                            instance1.m_nodes.m_buffer[(int) num4].m_flags &=
                                NetNode.Flags.OneWayOutTrafficLights | NetNode.Flags.UndergroundTransition |
                                NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original |
                                NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction |
                                NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside |
                                NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed |
                                NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water |
                                NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection |
                                NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayIn | NetNode.Flags.Heating |
                                NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle |
                                NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward |
                                NetNode.Flags.CustomTrafficLights;
                        else
                            instance1.m_nodes.m_buffer[(int) num4].m_flags |= NetNode.Flags.Disabled;
                        problem2 |= (instance1.m_nodes.m_buffer[(int) num4].m_problems ^ problem1) & problems2;
                        for (int index = 0; index < 8; ++index)
                        {
                            ushort segment = instance1.m_nodes.m_buffer[(int) num4].GetSegment(index);
                            if ((int) segment != 0 && (int) instance1.m_segments.m_buffer[(int) segment].m_startNode ==
                                (int) num4)
                            {
                                num6 +=
                                    Mathf.Max((int) instance1.m_segments.m_buffer[(int) segment].m_trafficLightState0,
                                        (int) instance1.m_segments.m_buffer[(int) segment].m_trafficLightState1);
                                num1 += instance1.m_segments.m_buffer[(int) segment].m_averageLength;
                                num7 += (int) instance1.m_segments.m_buffer[(int) segment].m_trafficBuffer;
                                num9 = instance1.m_segments.m_buffer[(int) segment].m_endNode;
                                if ((instance1.m_segments.m_buffer[(int) segment].m_flags &
                                     NetSegment.Flags.PathLength) == NetSegment.Flags.None)
                                {
                                    flag2 = true;
                                    break;
                                }
                                break;
                            }
                        }
                        ushort num10 = instance1.m_nodes.m_buffer[(int) num4].m_targetCitizens;
                        int num11 = 0;
                        while ((int) num10 != 0)
                        {
                            ++num8;
                            num10 = instance3.m_instances.m_buffer[(int) num10].m_nextTargetInstance;
                            if (++num11 >= 32768)
                            {
                                CODebugBase<LogChannel>.Error(LogChannel.Core,
                                    "Invalid list detected!\n" + System.Environment.StackTrace);
                                break;
                            }
                        }
                        ++range;
                        num4 = num9;
                        if ((int) num4 != (int) stops)
                        {
                            if (++num5 >= 32768)
                            {
                                CODebugBase<LogChannel>.Error(LogChannel.Core,
                                    "Invalid list detected!\n" + System.Environment.StackTrace);
                                break;
                            }
                        }
                        else
                            break;
                    }
                }
                if (!flag2)
                    thisLine.m_totalLength = num1;
                if ((int) range != 0)
                    thisLine.m_averageInterval = (byte) Mathf.Min((float) byte.MaxValue,
                        (float) (((long) num6 + (long) (range >> 1)) / (long) range));
                //begin mod(-): let's count maintenance once a week
                //end mod
                TransferManager.TransferReason vehicleReason = info.m_vehicleReason;
                if (vehicleReason != TransferManager.TransferReason.None)
                {
                    //begin mod: calculate target vehicle count or read saved value
                    int num4 = 0;
                    if (CachedTransportLineData._lineData[(int)lineID].BudgetControl || info.m_class.m_service == ItemClass.Service.Disaster)
                    {
                        num4 = !flag3 ? 0 : (!flag2 ? thisLine.CalculateTargetVehicleCount() : num3);
                        CachedTransportLineData._lineData[(int)lineID].TargetVehicleCount = num4;
                    }
                    else if (flag3)
                        num4 = CachedTransportLineData._lineData[(int)lineID].TargetVehicleCount;
                    //end mod
                    if ((int) range != 0 && num2 < num4)
                    {
                        ushort stop = thisLine.GetStop(instance2.m_randomizer.Int32(range));
                        if ((int) stop != 0)
                        //begin mod(+): save offer as variable and directly invoke spawn if it's not evac line
                        {
                            TransferManager.TransferOffer offer =
                                new TransferManager.TransferOffer
                                {
                                    Priority = num4 - num2 + 1,
                                    TransportLine = lineID,
                                    Position = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) stop]
                                        .m_position,
                                    Amount = 1,
                                    Active = false
                                };
                            if (info.m_class.m_service == ItemClass.Service.Disaster)
                            {
                                Singleton<TransferManager>.instance.AddIncomingOffer(vehicleReason, offer);
                            }
                            else
                            {
                                HandleVehicleSpawn(lineID, info, offer);
                            }
                        }
                        //end mod
                    }
                    else if (num3 > num4)
                    {
                        //begin mod(*): encapsulate into method
                        TransportLineDetour.RemoveActiveVehicle(lineID, false, num3);
                        //end mod
                    }
                }
                TransferManager.TransferReason material = info.m_citizenReason;
                switch (material)
                {
                    case TransferManager.TransferReason.None:
                        goto label_99;
                    case TransferManager.TransferReason.Entertainment:
                        switch (instance2.m_randomizer.Int32(4U))
                        {
                            case 0:
                                material = TransferManager.TransferReason.Entertainment;
                                break;
                            case 1:
                                material = TransferManager.TransferReason.EntertainmentB;
                                break;
                            case 2:
                                material = TransferManager.TransferReason.EntertainmentC;
                                break;
                            case 3:
                                material = TransferManager.TransferReason.EntertainmentD;
                                break;
                        }
                        //begin mod: to shut up compiler
                        break;
                    //end mod
                    case TransferManager.TransferReason.TouristA:
                        switch (instance2.m_randomizer.Int32(20U))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                material = TransferManager.TransferReason.TouristA;
                                break;
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                                material = TransferManager.TransferReason.TouristB;
                                break;
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                                material = TransferManager.TransferReason.TouristC;
                                break;
                            case 12:
                            case 13:
                            case 14:
                            case 15:
                                material = TransferManager.TransferReason.TouristD;
                                break;
                            case 16:
                                material = TransferManager.TransferReason.Entertainment;
                                break;
                            case 17:
                                material = TransferManager.TransferReason.EntertainmentB;
                                break;
                            case 18:
                                material = TransferManager.TransferReason.EntertainmentC;
                                break;
                            case 19:
                                material = TransferManager.TransferReason.EntertainmentD;
                                break;
                        }
                        //begin mod: to shut up compiler
                        break;
                    //end mod
                }
                if (vehicleReason == TransferManager.TransferReason.None)
                {
                    if ((double) thisLine.m_totalLength <= 1920.0)
                    {
                        if (!flag2)
                            problems1 = Notification.RemoveProblems(problems1, Notification.Problem.TooLong);
                    }
                    else if ((double) thisLine.m_totalLength <= 3840.0)
                    {
                        if (!flag2)
                            problems1 = Notification.RemoveProblems(problems1, Notification.Problem.TooLong);
                        num7 = (num7 * 17 + 10) / 20;
                    }
                    else if ((double) thisLine.m_totalLength <= 5760.0)
                    {
                        if (!flag2)
                            problems1 =
                                Notification.AddProblems(
                                    Notification.RemoveProblems(problems1, Notification.Problem.TooLong),
                                    Notification.Problem.TooLong);
                        num7 = (num7 * 14 + 10) / 20;
                    }
                    else
                    {
                        if (!flag2)
                            problems1 = Notification.AddProblems(problems1,
                                Notification.Problem.TooLong | Notification.Problem.MajorProblem);
                        num7 = (num7 * 8 + 10) / 20;
                    }
                    if (flag1)
                        Singleton<StatisticsManager>.instance.Acquire<StatisticInt32>(StatisticType.WalkingTourLength)
                            .Add(Mathf.RoundToInt(thisLine.m_totalLength));
                }
                else
                    num7 = Mathf.Max(0,
                        num7 + 50 - (int) thisLine.m_ticketPrice * 50 / Mathf.Max(1, info.m_ticketPrice));
                int num12;
                if (flag3)
                {
                    if (flag2)
                    {
                        num12 = num8;
                    }
                    else
                    {
                        int num4 = Mathf.Max(1, info.m_citizenPullRequirement);
                        num12 = (num7 + num4 - 1) / num4;
                    }
                }
                else
                    num12 = 0;
                if ((int) range != 0 && num8 < num12)
                {
                    ushort stop = thisLine.GetStop(instance2.m_randomizer.Int32(range));
                    if ((int) stop != 0)
                        Singleton<TransferManager>.instance.AddOutgoingOffer(material,
                            new TransferManager.TransferOffer()
                            {
                                Priority = Mathf.Max(1, (num12 - num8) * 8 / num12),
                                TransportLine = lineID,
                                Position = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) stop].m_position,
                                Amount = 1,
                                Active = false
                            });
                }
            }
            else
            {
                if ((int) thisLine.m_stops != 0)
                {
                    ushort stops = thisLine.m_stops;
                    ushort num2 = stops;
                    int num3 = 0;
                    while ((int) num2 != 0)
                    {
                        ushort num4 = 0;
                        problem2 |= (instance1.m_nodes.m_buffer[(int) num2].m_problems ^ problem1) & problems2;
                        for (int index = 0; index < 8; ++index)
                        {
                            ushort segment = instance1.m_nodes.m_buffer[(int) num2].GetSegment(index);
                            if ((int) segment != 0 && (int) instance1.m_segments.m_buffer[(int) segment].m_startNode ==
                                (int) num2)
                            {
                                num1 += instance1.m_segments.m_buffer[(int) segment].m_averageLength;
                                num4 = instance1.m_segments.m_buffer[(int) segment].m_endNode;
                                if ((instance1.m_segments.m_buffer[(int) segment].m_flags &
                                     NetSegment.Flags.PathLength) == NetSegment.Flags.None)
                                {
                                    flag2 = true;
                                    break;
                                }
                                break;
                            }
                        }
                        num2 = num4;
                        if ((int) num2 != (int) stops)
                        {
                            if (++num3 >= 32768)
                            {
                                CODebugBase<LogChannel>.Error(LogChannel.Core,
                                    "Invalid list detected!\n" + System.Environment.StackTrace);
                                break;
                            }
                        }
                        else
                            break;
                    }
                }
                if (!flag2)
                {
                    thisLine.m_totalLength = num1;
                    if (info.m_citizenReason != TransferManager.TransferReason.None &&
                        info.m_vehicleReason == TransferManager.TransferReason.None)
                        problems1 = (double) thisLine.m_totalLength > 1920.0
                            ? ((double) thisLine.m_totalLength > 3840.0
                                ? ((double) thisLine.m_totalLength > 5760.0
                                    ? Notification.AddProblems(problems1,
                                        Notification.Problem.TooLong | Notification.Problem.MajorProblem)
                                    : Notification.AddProblems(
                                        Notification.RemoveProblems(problems1, Notification.Problem.TooLong),
                                        Notification.Problem.TooLong))
                                : Notification.RemoveProblems(problems1, Notification.Problem.TooLong))
                            : Notification.RemoveProblems(problems1, Notification.Problem.TooLong);
                }
            }
            label_99:
            if ((int) thisLine.m_stops != 0 && (problem2 | problems1 ^ problem1) != Notification.Problem.None)
            {
                ushort stops = thisLine.m_stops;
                ushort num2 = stops;
                int num3 = 0;
                while ((int) num2 != 0)
                {
                    Notification.Problem problems = instance1.m_nodes.m_buffer[(int) num2].m_problems;
                    Notification.Problem problem3 = Notification.RemoveProblems(problems, problems2);
                    if ((problems1 & problems2 &
                         ~(Notification.Problem.MajorProblem | Notification.Problem.FatalProblem)) !=
                        Notification.Problem.None)
                        problem3 = Notification.AddProblems(problem3, problems1 & problems2);
                    if (problems != problem3)
                    {
                        instance1.m_nodes.m_buffer[(int) num2].m_problems = problem3;
                        instance1.UpdateNodeNotifications(num2, problems, problem3);
                    }
                    num2 = TransportLine.GetNextStop(num2);
                    if ((int) num2 != (int) stops)
                    {
                        if (++num3 >= 32768)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core,
                                "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                    else
                        break;
                }
            }
            if (!flag1)
                return;
            thisLine.m_passengers.Update();
            Singleton<TransportManager>.instance.m_passengers[(int) info.m_transportType]
                .Add(ref thisLine.m_passengers);
            thisLine.m_passengers.Reset();


            //begin mod(+): update statistics + fetch maintenance costs
            if (thisLine.Complete)
            {
                ushort stops1 = thisLine.m_stops;
                ushort stop1 = stops1;
                do
                {
                    CachedNodeData.m_cachedNodeData[(int) stop1].StartNewWeek();
                    stop1 = TransportLine.GetNextStop(stop1);
                } while ((int) stops1 != (int) stop1 && (int) stop1 != 0);

                var itemClass = info.m_class;
                PrefabData[] prefabs =
                    VehiclePrefabs.instance.GetPrefabs(itemClass.m_service, itemClass.m_subService, itemClass.m_level);
                int amount = 0;
                CountLineActiveVehicles(lineID, out int _, (num3) =>
                {
                    Vehicle vehicle = VehicleManager.instance.m_vehicles.m_buffer[num3];
                    PrefabData prefabData = Array.Find(prefabs,
                        item => item.PrefabDataIndex == vehicle.Info.m_prefabDataIndex);
                    if (prefabData != null)
                    {
                        amount += prefabData.MaintenanceCost;
                        CachedVehicleData.m_cachedVehicleData[num3].StartNewWeek(prefabData.MaintenanceCost);
                    }
                });
                if (amount != 0)
                    Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.Maintenance, amount,
                        info.m_class);
                //end mod
            }
        }

        private static void HandleVehicleSpawn(ushort lineID, TransportInfo info, TransferManager.TransferOffer offer)
        {
            var itemClass = info.m_class;
            if ((double) SimHelper.SimulationTime >=
                (double) CachedTransportLineData._lineData[(int) lineID].NextSpawnTime ||
                itemClass.m_service == ItemClass.Service.Disaster)
            {
                ushort depot = CachedTransportLineData._lineData[(int) lineID].Depot;
                if (DepotUtil.ValidateDepot(lineID, ref depot, info))
                {
                    BuildingManager instance2 = Singleton<BuildingManager>.instance;
                    if (TransportLineDetour.CanAddVehicle(depot,
                        ref instance2.m_buildings.m_buffer[(int) depot], info))
                    {
                        var buildingAi = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)depot].Info?.m_buildingAI;
                        var depotAi = buildingAi as DepotAI;
                        if (depotAi == null)
                        {
                            throw new Exception("Non-depot building was selected as depot! Actual AI type: " + buildingAi?.GetType()); 
                        }
                        instance2.m_buildings.m_buffer[(int) depot]
                                .Info.m_buildingAI
                                .StartTransfer(depot,
                                    ref instance2.m_buildings.m_buffer[(int) depot],
                                    info.m_vehicleReason, offer);
                        CachedTransportLineData._lineData[(int) lineID].NextSpawnTime =
                            SimHelper.SimulationTime +
                            (float) OptionsWrapper<Settings>.Options.SpawnTimeInterval;
                    }
                    else
                    {
                        CachedTransportLineData.ClearEnqueuedVehicles(lineID);
                    }
                }
                else
                {
                    CachedTransportLineData.ClearEnqueuedVehicles(lineID);
                }
            }
        }

        public static bool CanAddVehicle(ushort depotID, ref Building depot, TransportInfo transportInfo)
        {
            if (depot.Info == null)
            {
                return false;
            }
            if (depot.Info.m_buildingAI is DepotAI)
            {
                DepotAI buildingAi = depot.Info.m_buildingAI as DepotAI;
                if (transportInfo.m_vehicleType == buildingAi.m_transportInfo?.m_vehicleType ||
                    transportInfo.m_vehicleType == buildingAi.m_secondaryTransportInfo?.m_vehicleType)
                {
                    int num = (PlayerBuildingAI.GetProductionRate(100,
                                   Singleton<EconomyManager>.instance.GetBudget(buildingAi.m_info.m_class)) * 
                               buildingAi.m_maxVehicleCount + 99) / 100;
                    return buildingAi.GetVehicleCount(depotID, ref depot) < num;
                }
            }
            if (depot.Info.m_buildingAI is ShelterAI)
            {
                ShelterAI buildingAi = depot.Info.m_buildingAI as ShelterAI;
                int num = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(buildingAi.m_info.m_class)) * buildingAi.m_evacuationBusCount + 99) / 100;
                int count = 0;
                int cargo = 0;
                int capacity = 0;
                int outside = 0;
                CommonBuildingAIReverseDetour.CalculateOwnVehicles(buildingAi, depotID, ref depot, buildingAi.m_transportInfo.m_vehicleReason, ref count, ref cargo, ref capacity, ref outside);
                return count < num;
            }
            return false;

        }

        //based off code in the SimulationStep
        public static void RemoveActiveVehicle(ushort lineID, bool descreaseTargetVehicleCount, int activeVehiclesCount)
        {
            ushort activeVehicle = GetActiveVehicle(ref Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineID], 
                Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)activeVehiclesCount));
            if ((int)activeVehicle != 0)
            {
                TransportLineDetour.RemoveVehicle(lineID, activeVehicle, descreaseTargetVehicleCount);
            }
        }

        //based off code in the SimulationStep
        public static void RemoveVehicle(ushort lineID, ushort vehicleID, bool descreaseTargetVehicleCount)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            if ((instance.m_vehicles.m_buffer[(int)vehicleID].m_flags & Vehicle.Flags.GoingBack) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive)) {
                if (descreaseTargetVehicleCount)
                {
                    CachedTransportLineData.DecreaseTargetVehicleCount(lineID);
                }
                instance.m_vehicles.m_buffer[(int)vehicleID].Info.m_vehicleAI.SetTransportLine(vehicleID, ref instance.m_vehicles.m_buffer[(int)vehicleID], (ushort)0);
            }
        }

        //based off code in the SimulationStep
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
                if ((int)thisLine.m_vehicles != 0)
                {
                    VehicleManager instance3 = Singleton<VehicleManager>.instance;
                    ushort num4 = thisLine.m_vehicles;
                    int num5 = 0;
                    while ((int)num4 != 0)
                    {
                        ushort nextLineVehicle = instance3.m_vehicles.m_buffer[(int)num4].m_nextLineVehicle;
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
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
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
        
        [RedirectReverse]
        private static ushort GetActiveVehicle(ref TransportLine thisLine, int index)
        {
            UnityEngine.Debug.Log("GetActiveVehicle");
            return 0;
        }
    }
}