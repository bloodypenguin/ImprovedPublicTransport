// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.TransportLineMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using ColossalFramework;
using ColossalFramework.Math;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.RedirectionFramework;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;
using ImprovedPublicTransport2.RedirectionFramework.Attributes.IgnoreConditions;
using UnityEngine;

namespace ImprovedPublicTransport2.Detour
{
    [TargetType(typeof(TransportLine))]
    public struct TransportLineMod
    {
        private static readonly string _dataID = "ImprovedPublicTransport";
        private static readonly string _dataVersion = "v004";

        private static bool _init = false;
        private static LineData[] _lineData;

        public static void Init()
        {
            if (!TransportLineMod.TryLoadData(out TransportLineMod._lineData))
            {
                Utils.Log((object) "Loading default transport line data.");
                NetManager instance1 = Singleton<NetManager>.instance;
                TransportManager instance2 = Singleton<TransportManager>.instance;
                int length = instance2.m_lines.m_buffer.Length;
                for (ushort index = 0; index < length; ++index)
                {
                    if (instance2.m_lines.m_buffer[index].Complete)
                    {
                        TransportLineMod._lineData[index].TargetVehicleCount = TransportLineMod.CountLineActiveVehicles(index, out int _);
                    }
                    else
                        TransportLineMod._lineData[index].TargetVehicleCount =
                            OptionsWrapper<Settings>.Options.DefaultVehicleCount;
                    TransportLineMod._lineData[index].BudgetControl = OptionsWrapper<Settings>.Options.BudgetControl;
                    TransportLineMod._lineData[index].Depot = TransportLineMod.GetClosestDepot((ushort) index,
                        instance1.m_nodes.m_buffer[(int) instance2.m_lines.m_buffer[index].GetStop(0)].m_position);
                    TransportLineMod._lineData[index].Unbunching = OptionsWrapper<Settings>.Options.Unbunching;
                }
            }
            SerializableDataExtension.instance.EventSaveData +=
                new SerializableDataExtension.SaveDataEventHandler(TransportLineMod.OnSaveData);
            Redirector<TransportLineMod>.Deploy();
            TransportLineMod._init = true;
        }

        public static void Deinit()
        {
            Redirector<TransportLineMod>.Revert();
            TransportLineMod._lineData = (LineData[]) null;
            SerializableDataExtension.instance.EventSaveData -=
                new SerializableDataExtension.SaveDataEventHandler(TransportLineMod.OnSaveData);
            TransportLineMod._init = false;
        }

        public static bool TryLoadData(out LineData[] data)
        {
            data = new LineData[256];
            byte[] data1 = SerializableDataExtension.instance.SerializableData.LoadData(TransportLineMod._dataID);
            if (data1 == null)
                return false;
            int index1 = 0;
            ushort lineID = 0;
            string empty = string.Empty;
            try
            {
                Utils.Log((object) "Try to load transport line data.");
                string str = SerializableDataExtension.ReadString(data1, ref index1);
                if (string.IsNullOrEmpty(str) || str.Length != 4)
                {
                    Utils.LogWarning((object) "Unknown data found.");
                    return false;
                }
                Utils.Log((object) ("Found transport line data version: " + str));
                NetManager instance1 = Singleton<NetManager>.instance;
                TransportManager instance2 = Singleton<TransportManager>.instance;
                while (index1 < data1.Length)
                {
                    if (instance2.m_lines.m_buffer[(int) lineID].Complete)
                    {
                        int int32 = BitConverter.ToInt32(data1, index1);
                        data[(int) lineID].TargetVehicleCount = int32;
                    }
                    index1 += 4;
                    float num = Mathf.Min(BitConverter.ToSingle(data1, index1),
                        (float) OptionsWrapper<Settings>.Options.SpawnTimeInterval);
                    if ((double) num > 0.0)
                        data[(int) lineID].NextSpawnTime = SimHelper.SimulationTime + num;
                    index1 += 4;
                    bool boolean = BitConverter.ToBoolean(data1, index1);
                    data[(int) lineID].BudgetControl = boolean;
                    ++index1;
                    ushort uint16 = BitConverter.ToUInt16(data1, index1);
                    data[(int) lineID].Depot = (int) uint16 != 0
                        ? uint16
                        : TransportLineMod.GetClosestDepot(lineID,
                            instance1.m_nodes.m_buffer[(int) instance2.m_lines.m_buffer[(int) lineID].GetStop(0)]
                                .m_position);
                    index1 += 2;
                    if (str == "v001")
                    {
                        string name = SerializableDataExtension.ReadString(data1, ref index1);
                        if (name != "Random")
                        {
                            if (data[(int) lineID].Prefabs == null)
                                data[(int) lineID].Prefabs = new HashSet<string>();
                            if ((UnityEngine.Object) PrefabCollection<VehicleInfo>.FindLoaded(name) !=
                                (UnityEngine.Object) null)
                                data[(int) lineID].Prefabs.Add(name);
                        }
                    }
                    else
                    {
                        int int32 = BitConverter.ToInt32(data1, index1);
                        index1 += 4;
                        for (int index2 = 0; index2 < int32; ++index2)
                        {
                            string name = SerializableDataExtension.ReadString(data1, ref index1);
                            if (data[(int) lineID].Prefabs == null)
                                data[(int) lineID].Prefabs = new HashSet<string>();
                            if ((UnityEngine.Object) PrefabCollection<VehicleInfo>.FindLoaded(name) !=
                                (UnityEngine.Object) null)
                                data[(int) lineID].Prefabs.Add(name);
                        }
                    }
                    if (str != "v001")
                    {
                        int int32 = BitConverter.ToInt32(data1, index1);
                        index1 += 4;
                        for (int index2 = 0; index2 < int32; ++index2)
                        {
                            string name = SerializableDataExtension.ReadString(data1, ref index1);
                            if (!boolean)
                            {
                                if (data[(int) lineID].QueuedVehicles == null)
                                    data[(int) lineID].QueuedVehicles = new Queue<string>();
                                if ((UnityEngine.Object) PrefabCollection<VehicleInfo>.FindLoaded(name) !=
                                    (UnityEngine.Object) null)
                                {
                                    lock (data[(int) lineID].QueuedVehicles)
                                        data[(int) lineID].QueuedVehicles.Enqueue(name);
                                }
                            }
                        }
                    }
                    if (str == "v003")
                        ++index1;
                    data[(int) lineID].Unbunching = str != "v004"
                        ? OptionsWrapper<Settings>.Options.Unbunching
                        : SerializableDataExtension.ReadBool(data1, ref index1);
                    ++lineID;
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogWarning((object) ("Could not load transport line data. " + ex.Message));
                data = new LineData[256];
                return false;
            }
        }

        private static void OnSaveData()
        {
            FastList<byte> data = new FastList<byte>();
            try
            {
                SerializableDataExtension.WriteString(TransportLineMod._dataVersion, data);
                for (ushort lineID = 0; (int) lineID < 256; ++lineID)
                {
                    SerializableDataExtension.AddToData(
                        BitConverter.GetBytes(TransportLineMod.GetTargetVehicleCount(lineID)), data);
                    SerializableDataExtension.AddToData(
                        BitConverter.GetBytes(Mathf.Max(
                            TransportLineMod.GetNextSpawnTime(lineID) - SimHelper.SimulationTime, 0.0f)),
                        data);
                    SerializableDataExtension.AddToData(
                        BitConverter.GetBytes(TransportLineMod.GetBudgetControlState(lineID)), data);
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(TransportLineMod.GetDepot(lineID)), data);
                    int num = 0;
                    HashSet<string> prefabs = TransportLineMod.GetPrefabs(lineID);
                    if (prefabs != null)
                        num = prefabs.Count;
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(num), data);
                    if (num > 0)
                    {
                        foreach (string s in prefabs)
                            SerializableDataExtension.WriteString(s, data);
                    }
                    string[] enqueuedVehicles = TransportLineMod.GetEnqueuedVehicles(lineID);
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(enqueuedVehicles.Length), data);
                    if (enqueuedVehicles.Length != 0)
                    {
                        foreach (string s in enqueuedVehicles)
                            SerializableDataExtension.WriteString(s, data);
                    }
                    SerializableDataExtension.WriteBool(TransportLineMod.GetUnbunchingState(lineID), data);
                }
                SerializableDataExtension.instance.SerializableData.SaveData(TransportLineMod._dataID, data.ToArray());
            }
            catch (Exception ex)
            {
                string msg = "Error while saving transport line data! " + ex.Message + " " + (object) ex.InnerException;
                Utils.LogError((object) msg);
                CODebugBase<LogChannel>.Log(LogChannel.Modding, msg, ErrorLevel.Error);
            }
        }

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
            if (!TransportLineMod._init)
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
                    if (TransportLineMod._lineData[(int)lineID].BudgetControl || info.m_class.m_service == ItemClass.Service.Disaster)
                    {
                        num4 = !flag3 ? 0 : (!flag2 ? thisLine.CalculateTargetVehicleCount() : num3);
                        TransportLineMod._lineData[(int)lineID].TargetVehicleCount = num4;
                    }
                    else if (flag3)
                        num4 = TransportLineMod._lineData[(int)lineID].TargetVehicleCount;
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
                                HandleVehicleSpawn(lineID, info, num4, num2, offer);
                            }
                        }
                        //end mod
                    }
                    else if (num3 > num4)
                    {
                        //begin mod(*): encapsulate into method
                        TransportLineMod.RemoveActiveVehicle(lineID, false, num3);
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

        private static void HandleVehicleSpawn(ushort lineID, TransportInfo info, int targetVehicleCount,
            int activeVehicleCount, TransferManager.TransferOffer offer)
        {
            var itemClass = info.m_class;
            if ((double) SimHelper.SimulationTime >=
                (double) TransportLineMod._lineData[(int) lineID].NextSpawnTime ||
                itemClass.m_service == ItemClass.Service.Disaster)
            {
                ushort depot = TransportLineMod._lineData[(int) lineID].Depot;
                if (TransportLineMod.ValidateDepot(lineID, ref depot, info))
                {
                    BuildingManager instance2 = Singleton<BuildingManager>.instance;
                    if (TransportLineMod.CanAddVehicle(depot,
                        ref instance2.m_buildings.m_buffer[(int) depot], info))
                    {
                        string prefabName;
                        if (TransportLineMod.EnqueuedVehiclesCount(lineID) > 0)
                        {
                            prefabName = TransportLineMod.Dequeue(lineID);
                        }
                        else
                        {
                            int diffToTarget = targetVehicleCount - activeVehicleCount;
                            for (int index2 = 0; index2 < diffToTarget; ++index2)
                            {
                                TransportLineMod.EnqueueVehicle(lineID,
                                    TransportLineMod.GetRandomPrefab(lineID), false);
                            }
                            prefabName = TransportLineMod.Dequeue(lineID);
                        }
                        if (prefabName == "")
                        {
                            instance2.m_buildings.m_buffer[(int) depot]
                                .Info.m_buildingAI
                                .StartTransfer(depot,
                                    ref instance2.m_buildings.m_buffer[(int) depot],
                                    info.m_vehicleReason, offer);
                        }
                        else
                        {
                            DepotAIMod.StartTransfer(depot,
                                ref instance2.m_buildings.m_buffer[(int) depot],
                                info.m_vehicleReason,
                                offer,
                                prefabName);
                        }
                        TransportLineMod._lineData[(int) lineID].NextSpawnTime =
                            SimHelper.SimulationTime +
                            (float) OptionsWrapper<Settings>.Options.SpawnTimeInterval;
                    }
                    else
                    {
                        TransportLineMod.ClearEnqueuedVehicles(lineID);
                    }
                }
                else
                {
                    TransportLineMod.ClearEnqueuedVehicles(lineID);
                }
            }
        }

        public static void SetLineDefaults(ushort lineID)
        {
            TransportLineMod._lineData[(int) lineID] = new LineData();
            TransportLineMod._lineData[(int) lineID].TargetVehicleCount =
                OptionsWrapper<Settings>.Options.DefaultVehicleCount;
            TransportLineMod._lineData[(int) lineID].BudgetControl = OptionsWrapper<Settings>.Options.BudgetControl;
            TransportLineMod._lineData[(int) lineID].Unbunching = OptionsWrapper<Settings>.Options.Unbunching;
        }

        public static bool ValidateDepot(ushort lineID, ref ushort depotID, TransportInfo transportInfo)
        {
            if (transportInfo == null)
            {
                return false;
            }
            if (depotID != 0 &&
                DepotUtil.IsValidDepot(ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[depotID], transportInfo))
            {
                return true;
            }
            depotID = TransportLineMod.GetClosestDepot(lineID,
                Singleton<NetManager>.instance.m_nodes
                    .m_buffer[Singleton<TransportManager>.instance.m_lines.m_buffer[lineID].GetStop(0)]
                    .m_position);
            TransportLineMod._lineData[lineID].Depot = depotID;
            return depotID != 0;
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
                TransportLineMod.RemoveVehicle(lineID, activeVehicle, descreaseTargetVehicleCount);
            }
        }

        //based off code in the SimulationStep
        public static void RemoveVehicle(ushort lineID, ushort vehicleID, bool descreaseTargetVehicleCount)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            if ((instance.m_vehicles.m_buffer[(int)vehicleID].m_flags & Vehicle.Flags.GoingBack) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive)) {
                if (descreaseTargetVehicleCount)
                {
                    TransportLineMod.DecreaseTargetVehicleCount(lineID);
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

        public static int GetTargetVehicleCount(ushort lineID)
        {
            return TransportLineMod._lineData[(int) lineID].TargetVehicleCount;
        }

        public static void SetTargetVehicleCount(ushort lineID, int count)
        {
            TransportLineMod._lineData[(int) lineID].TargetVehicleCount = count;
        }

        public static void IncreaseTargetVehicleCount(ushort lineID)
        {
            ++TransportLineMod._lineData[(int) lineID].TargetVehicleCount;
        }

        public static void DecreaseTargetVehicleCount(ushort lineID)
        {
            if (TransportLineMod._lineData[(int) lineID].TargetVehicleCount == 0)
                return;
            --TransportLineMod._lineData[(int) lineID].TargetVehicleCount;
        }

        public static float GetNextSpawnTime(ushort lineID)
        {
            return TransportLineMod._lineData[(int) lineID].NextSpawnTime;
        }

        public static void SetNextSpawnTime(ushort lineID, float time)
        {
            TransportLineMod._lineData[(int) lineID].NextSpawnTime = time;
        }

        public static bool GetBudgetControlState(ushort lineID)
        {
            return TransportLineMod._lineData[(int) lineID].BudgetControl;
        }

        public static void SetBudgetControlState(ushort lineID, bool state)
        {
            TransportLineMod._lineData[(int) lineID].BudgetControl = state;
        }

        public static bool GetUnbunchingState(ushort lineID)
        {
            return TransportLineMod._lineData[(int) lineID].Unbunching;
        }

        public static void SetUnbunchingState(ushort lineID, bool state)
        {
            TransportLineMod._lineData[(int) lineID].Unbunching = state;
        }

        public static ushort GetDepot(ushort lineID)
        {
            return TransportLineMod._lineData[(int) lineID].Depot;
        }

        public static void SetDepot(ushort lineID, ushort depotID)
        {
            TransportLineMod._lineData[(int) lineID].Depot = depotID;
        }

        public static void AddPrefab(ushort lineID, string prefabName)
        {
            if (TransportLineMod._lineData[(int) lineID].Prefabs == null)
                TransportLineMod._lineData[(int) lineID].Prefabs = new HashSet<string>();
            if (TransportLineMod._lineData[(int) lineID].Prefabs.Contains(prefabName))
                return;
            TransportLineMod._lineData[(int) lineID].Prefabs.Add(prefabName);
        }

        public static HashSet<string> GetPrefabs(ushort lineID)
        {
            return TransportLineMod._lineData[(int) lineID].Prefabs;
        }

        public static void SetPrefabs(ushort lineID, HashSet<string> prefabs)
        {
            TransportLineMod._lineData[(int) lineID].Prefabs = prefabs;
        }

        public static void RemovePrefab(ushort lineID, string prefabName)
        {
            if (TransportLineMod._lineData[(int) lineID].Prefabs == null)
                return;
            TransportLineMod._lineData[(int) lineID].Prefabs.Remove(prefabName);
        }

        public static string GetRandomPrefab(ushort lineID)
        {
            if (TransportLineMod._lineData[(int) lineID].Prefabs != null)
            {
                string[] array = TransportLineMod._lineData[(int) lineID].Prefabs.ToArray<string>();
                if (array.Length != 0)
                {
                    int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
                    return array[index];
                }
            }
            var itemClass = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID]
                .Info.m_class;
            PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(itemClass.m_service, itemClass.m_subService, itemClass.m_level);
            int index1 = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) prefabs.Length);
            return prefabs[index1].ObjectName;
        }

        public static void EnqueueVehicle(ushort lineID, string prefabName, bool inscreaseVehicleCount = true)
        {
            if (TransportLineMod._lineData[(int) lineID].QueuedVehicles == null)
                TransportLineMod._lineData[(int) lineID].QueuedVehicles = new Queue<string>();
            if (inscreaseVehicleCount)
                TransportLineMod.IncreaseTargetVehicleCount(lineID);
            lock (TransportLineMod._lineData[(int) lineID].QueuedVehicles)
                TransportLineMod._lineData[(int) lineID].QueuedVehicles.Enqueue(prefabName);
        }

        public static string Dequeue(ushort lineID)
        {
            lock (TransportLineMod._lineData[(int) lineID].QueuedVehicles)
                return TransportLineMod._lineData[(int) lineID].QueuedVehicles.Dequeue();
        }

        public static void DequeueVehicle(ushort lineID)
        {
            if (TransportLineMod._lineData[(int) lineID].QueuedVehicles == null)
                return;
            TransportLineMod.DecreaseTargetVehicleCount(lineID);
            TransportLineMod.Dequeue(lineID);
        }

        public static void DequeueVehicles(ushort lineID, int[] indexes, bool descreaseVehicleCount = true)
        {
            lock (TransportLineMod._lineData[(int) lineID].QueuedVehicles)
            {
                List<string> stringList = new List<string>(
                    (IEnumerable<string>) TransportLineMod._lineData[(int) lineID].QueuedVehicles);
                for (int index = indexes.Length - 1; index >= 0; --index)
                {
                    stringList.RemoveAt(indexes[index]);
                    if (descreaseVehicleCount)
                        TransportLineMod.DecreaseTargetVehicleCount(lineID);
                }
                TransportLineMod._lineData[(int) lineID].QueuedVehicles =
                    new Queue<string>((IEnumerable<string>) stringList);
            }
        }

        public static string[] GetEnqueuedVehicles(ushort lineID)
        {
            if (TransportLineMod._lineData[(int) lineID].QueuedVehicles == null)
                return new string[0];
            lock (TransportLineMod._lineData[(int) lineID].QueuedVehicles)
                return TransportLineMod._lineData[(int) lineID].QueuedVehicles.ToArray();
        }

        public static int EnqueuedVehiclesCount(ushort lineID)
        {
            if (TransportLineMod._lineData[(int) lineID].QueuedVehicles == null)
                return 0;
            return TransportLineMod._lineData[(int) lineID].QueuedVehicles.Count;
        }

        public static void ClearEnqueuedVehicles(ushort lineID)
        {
            if (TransportLineMod._lineData[(int) lineID].QueuedVehicles == null ||
                TransportLineMod._lineData[(int) lineID].QueuedVehicles.Count <= 0)
                return;
            lock (TransportLineMod._lineData[(int) lineID].QueuedVehicles)
                TransportLineMod._lineData[(int) lineID].QueuedVehicles.Clear();
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

        public static ushort GetClosestDepot(ushort lineID, Vector3 stopPosition) //TODO(earalov): What happens if closest depot is not connected/not reachable?
        {
            ushort num1 = 0;
            float num2 = float.MaxValue;
            BuildingManager instance = Singleton<BuildingManager>.instance;
            TransportInfo info = Singleton<TransportManager>.instance.m_lines
                .m_buffer[(int) lineID]
                .Info;
            ushort[] depots = BuildingExtension.GetDepots(info);
            for (int index = 0; index < depots.Length; ++index)
            {
                float num3 = Vector3.Distance(stopPosition,
                    instance.m_buildings.m_buffer[(int) depots[index]].m_position);
                if ((double) num3 < (double) num2)
                {
                    num1 = depots[index];
                    num2 = num3;
                }
            }
            return num1;
        }

        [RedirectReverse]
        private static ushort GetActiveVehicle(ref TransportLine thisLine, int index)
        {
            UnityEngine.Debug.Log("GetActiveVehicle");
            return 0;
        }
    }
}