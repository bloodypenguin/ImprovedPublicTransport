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
using ImprovedPublicTransport2.Redirection;
using ImprovedPublicTransport2.Redirection.Attributes;
using UnityEngine;

namespace ImprovedPublicTransport2.Detour
{
    [TargetType(typeof(TransportLine))]
    public class TransportLineMod
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
                        TransportLineMod._lineData[index].TargetVehicleCount = TransportLineMod.CountLineActiveVehicles(index);
                    }
                    else
                        TransportLineMod._lineData[index].TargetVehicleCount =
                            ImprovedPublicTransportMod.Settings.DefaultVehicleCount;
                    TransportLineMod._lineData[index].BudgetControl = ImprovedPublicTransportMod.Settings.BudgetControl;
                    TransportLineMod._lineData[index].Depot = TransportLineMod.GetClosestDepot((ushort) index,
                        instance1.m_nodes.m_buffer[(int) instance2.m_lines.m_buffer[index].GetStop(0)].m_position);
                    TransportLineMod._lineData[index].Unbunching = ImprovedPublicTransportMod.Settings.Unbunching;
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
                        (float) ImprovedPublicTransportMod.Settings.SpawnTimeInterval);
                    if ((double) num > 0.0)
                        data[(int) lineID].NextSpawnTime = SimHelper.instance.SimulationTime + num;
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
                        ? ImprovedPublicTransportMod.Settings.Unbunching
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
                            TransportLineMod.GetNextSpawnTime(lineID) - SimHelper.instance.SimulationTime, 0.0f)),
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
        public bool CanLeaveStop(ref TransportLine thisLine, ushort nextStop, int waitTime)
        {
            if ((int)nextStop == 0)
                return true;
            ushort prevSegment = TransportLine.GetPrevSegment(nextStop);
            if ((int)prevSegment == 0 || ((int)thisLine.m_averageInterval - (int)Singleton<NetManager>.instance.m_segments.m_buffer[(int)prevSegment].m_trafficLightState0 + 2) / 4 <= 0)
                return true;
            //begin mod(*): compare with interval aggression setup instead of default 16 secs
            var targetWaitTime = Mathf.Min(ImprovedPublicTransportMod.Settings.IntervalAggressionFactor * 4 + 12, byte.MaxValue);
            return waitTime >= targetWaitTime; //4 * 16 = 64s is max waiting time in vanilla, 12s is min waiting time
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
            if (thisLine.Complete)
            {
                //begin mod(-): looks like this was moved to the bottom of the method
                //end mod
                bool flag1 = !Singleton<SimulationManager>.instance.m_isNightTime
                    ? (thisLine.m_flags & TransportLine.Flags.DisabledDay) == TransportLine.Flags.None
                    : (thisLine.m_flags & TransportLine.Flags.DisabledNight) == TransportLine.Flags.None;
                uint range = 0;
                float num5 = 0.0f;
                int num6 = 0;
                bool flag2 = false;
                if ((int)thisLine.m_stops != 0)
                {
                    NetManager instance = Singleton<NetManager>.instance;
                    ushort stops = thisLine.m_stops;
                    ushort num3 = stops;
                    int num4 = 0;
                    while ((int)num3 != 0)
                    {
                        ushort num7 = 0;
                        if (flag1)
                            instance.m_nodes.m_buffer[(int)num3].m_flags &=
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
                            instance.m_nodes.m_buffer[(int)num3].m_flags |= NetNode.Flags.Disabled;
                        for (int index = 0; index < 8; ++index)
                        {
                            ushort segment = instance.m_nodes.m_buffer[(int)num3].GetSegment(index);
                            if ((int)segment != 0 && (int)instance.m_segments.m_buffer[(int)segment].m_startNode ==
                                (int)num3)
                            {
                                num6 +=
                                    Mathf.Max((int)instance.m_segments.m_buffer[(int)segment].m_trafficLightState0,
                                        (int)instance.m_segments.m_buffer[(int)segment].m_trafficLightState1);
                                num5 += instance.m_segments.m_buffer[(int)segment].m_averageLength;
                                num7 = instance.m_segments.m_buffer[(int)segment].m_endNode;
                                if ((instance.m_segments.m_buffer[(int)segment].m_flags &
                                     NetSegment.Flags.PathLength) == NetSegment.Flags.None)
                                {
                                    flag2 = true;
                                    break;
                                }
                                break;
                            }
                        }
                        ++range;
                        num3 = num7;
                        if ((int)num3 != (int)stops)
                        {
                            if (++num4 >= 32768)
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
                    thisLine.m_totalLength = num5;
                if ((int)range != 0)
                    thisLine.m_averageInterval = (byte)Mathf.Min((float)byte.MaxValue,
                        (float)(((long)num6 + (long)(range >> 1)) / (long)range));
                //begin mod(+): something weird happens :)
                bool flag = TransportLineMod.SetLineStatus(lineID, flag1);
                int lineVehicleCount = CountLineActiveVehicles(lineID);
                int targetVehicleCount = 0;
                if (TransportLineMod._lineData[(int) lineID].BudgetControl)
                {
                    targetVehicleCount = !flag1
                        ? 0
                        : (!flag ? thisLine.CalculateTargetVehicleCount() : lineVehicleCount);
                    TransportLineMod._lineData[(int) lineID].TargetVehicleCount = targetVehicleCount;
                }
                else if (flag1)
                    targetVehicleCount = TransportLineMod._lineData[(int) lineID].TargetVehicleCount;
                if (lineVehicleCount < targetVehicleCount)
                {
                    if ((double) SimHelper.instance.SimulationTime >=
                        (double) TransportLineMod._lineData[(int) lineID].NextSpawnTime)
                    {
                        ushort randomStop = thisLine.GetStop(Singleton<SimulationManager>.instance.m_randomizer
                            .Int32((uint)thisLine.CountStops(lineID)));
                        if (info.m_vehicleReason != TransferManager.TransferReason.None && (int) randomStop != 0)
                        {
                            TransferManager.TransferOffer offer = new TransferManager.TransferOffer();
                            offer.Priority = targetVehicleCount - lineVehicleCount + 1;
                            offer.TransportLine = lineID;
                            offer.Position = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) randomStop].m_position;
                            offer.Amount = 1;
                            offer.Active = false;
                            ushort depot = TransportLineMod._lineData[(int) lineID].Depot;
                            if (TransportLineMod.ValidateDepot(lineID, ref depot, ref info))
                            {
                                BuildingManager instance2 = Singleton<BuildingManager>.instance;
                                if (TransportLineMod.CanAddVehicle(depot, ref instance2.m_buildings.m_buffer[(int) depot], info))
                                {
                                    string prefabName;
                                    if (TransportLineMod.EnqueuedVehiclesCount(lineID) > 0)
                                    {
                                        prefabName = TransportLineMod.Dequeue(lineID);
                                    }
                                    else
                                    {
                                        int num3 = targetVehicleCount - lineVehicleCount;
                                        for (int index2 = 0; index2 < num3; ++index2)
                                            TransportLineMod.EnqueueVehicle(lineID,
                                                TransportLineMod.GetRandomPrefab(lineID), false);
                                        prefabName = TransportLineMod.Dequeue(lineID);
                                    }
                                    if (prefabName == "")
                                    {
                                        instance2.m_buildings.m_buffer[(int) depot]
                                            .Info.m_buildingAI
                                            .StartTransfer(depot, ref instance2.m_buildings.m_buffer[(int) depot],
                                                info.m_vehicleReason, offer);
                                    }
                                    else
                                    {
                                        DepotAIMod.StartTransfer(depot,
                                            ref instance2.m_buildings.m_buffer[(int) depot], info.m_vehicleReason,
                                            offer,
                                            prefabName);
                                    }
                                    TransportLineMod._lineData[(int) lineID].NextSpawnTime =
                                        SimHelper.instance.SimulationTime +
                                        (float) ImprovedPublicTransportMod.Settings.SpawnTimeInterval;
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
                }
                else if (lineVehicleCount > targetVehicleCount)
                {
                    TransportLineMod.RemoveActiveVehicle(lineID, false);
                }
                //end mod

                if ((Singleton<SimulationManager>.instance.m_currentFrameIndex & 4095U) < 3840U)
                    return;
                thisLine.m_passengers.Update();
                Singleton<TransportManager>.instance.m_passengers[(int)info.m_transportType].Add(ref thisLine.m_passengers);
                thisLine.m_passengers.Reset();


                //begin mod(+): update statistics + fetch maintenance costs
                ushort stops1 = thisLine.m_stops;
                ushort stop1 = stops1;
                do
                {
                    NetManagerMod.m_cachedNodeData[(int) stop1].StartNewWeek();
                    stop1 = TransportLine.GetNextStop(stop1);
                } while ((int) stops1 != (int) stop1 && (int) stop1 != 0);

                VehicleManager instance3 = Singleton<VehicleManager>.instance;
                var itemClass = info.m_class;
                PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(itemClass.m_service, itemClass.m_subService, itemClass.m_level);
                int amount = 0;

                //this part is very similar to beginning of vanilla method where active vehicles are counted
                if ((int)thisLine.m_vehicles != 0)
                {
                    VehicleManager instance = Singleton<VehicleManager>.instance;
                    ushort index = thisLine.m_vehicles;
                    int num4 = 0;
                    while ((int)index != 0)
                    {
                        ushort nextLineVehicle = instance.m_vehicles.m_buffer[(int)index].m_nextLineVehicle;

                        if ((instance.m_vehicles.m_buffer[(int) index].m_flags & Vehicle.Flags.GoingBack) ==
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
                                Vehicle vehicle = instance3.m_vehicles.m_buffer[(int)index];
                                PrefabData prefabData = Array.Find<PrefabData>(prefabs,
                                    (Predicate<PrefabData>)(item => item.PrefabDataIndex == vehicle.Info.m_prefabDataIndex));
                                if (prefabData != null)
                                {
                                    amount += prefabData.MaintenanceCost;
                                    VehicleManagerMod.m_cachedVehicleData[(int)index].StartNewWeek(prefabData.MaintenanceCost);
                                }
                        }

                        index = nextLineVehicle;
                        if (++num4 > 16384)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core,
                                "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
                //end of similar part
                //end mod

                //TODO(earalov): in vanilla, it looks like maintenance cost is fetched on each SimulationStep (without skips). We probably should fetch like that too
                //begin mod(+): this piece was moved from another place, earlier
                if (amount != 0)
                    Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.Maintenance, amount,
                        itemClass);
                //end mod
            }
        }

        public static void SetLineDefaults(ushort lineID)
        {
            TransportLineMod._lineData[(int) lineID] = new LineData();
            TransportLineMod._lineData[(int) lineID].TargetVehicleCount =
                ImprovedPublicTransportMod.Settings.DefaultVehicleCount;
            TransportLineMod._lineData[(int) lineID].BudgetControl = ImprovedPublicTransportMod.Settings.BudgetControl;
            TransportLineMod._lineData[(int) lineID].Unbunching = ImprovedPublicTransportMod.Settings.Unbunching;
        }

        private static bool SetLineStatus(ushort lineID, bool isLineEnabled)
        {
            bool flag = false;
            ushort stops = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].m_stops;
            if ((int) stops != 0)
            {
                NetManager instance = Singleton<NetManager>.instance;
                NetNode[] buffer = instance.m_nodes.m_buffer;
                ushort num1 = stops;
                int num2 = 0;
                while ((int) num1 != 0)
                {
                    ushort num3 = 0;
                    buffer[(int) num1].m_flags = !isLineEnabled
                        ? buffer[(int) num1].m_flags | NetNode.Flags.Disabled
                        : buffer[(int) num1].m_flags & (NetNode.Flags.OneWayOutTrafficLights |
                                                        NetNode.Flags.UndergroundTransition | NetNode.Flags.Created |
                                                        NetNode.Flags.Deleted | NetNode.Flags.Original |
                                                        NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend |
                                                        NetNode.Flags.Junction | NetNode.Flags.Moveable |
                                                        NetNode.Flags.Untouchable | NetNode.Flags.Outside |
                                                        NetNode.Flags.Temporary | NetNode.Flags.Double |
                                                        NetNode.Flags.Fixed | NetNode.Flags.OnGround |
                                                        NetNode.Flags.Ambiguous | NetNode.Flags.Water |
                                                        NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection |
                                                        NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayIn |
                                                        NetNode.Flags.Heating | NetNode.Flags.Electricity |
                                                        NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle |
                                                        NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward |
                                                        NetNode.Flags.CustomTrafficLights);
                    for (int index = 0; index < 8; ++index)
                    {
                        ushort segment = buffer[(int) num1].GetSegment(index);
                        if ((int) segment != 0 && (int) instance.m_segments.m_buffer[(int) segment].m_startNode ==
                            (int) num1)
                        {
                            num3 = instance.m_segments.m_buffer[(int) segment].m_endNode;
                            if ((instance.m_segments.m_buffer[(int) segment].m_flags & NetSegment.Flags.PathLength) ==
                                NetSegment.Flags.None)
                            {
                                flag = true;
                                break;
                            }
                            break;
                        }
                    }
                    num1 = num3;
                    if ((int) num1 != (int) stops)
                    {
                        if (++num2 >= 32768)
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
            return flag;
        }

        public static float GetLength(ushort lineID)
        {
            var length = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].m_totalLength;
            if (Math.Abs(length) < 0.01f)
            {
                Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].UpdateMeshData(lineID);
                length = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].m_totalLength;
            }
            return length;
        }


        public static bool ValidateDepot(ushort lineID, ref ushort depotID, ref TransportInfo transportInfo)
        {
            if (depotID != 0 &&
                BuildingWatcher.IsValidDepot(ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[depotID], ref transportInfo,
                    out _, out _, out _))
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
                if (transportInfo.m_vehicleType == buildingAi.m_transportInfo.m_vehicleType) //TODO(earalov): allow to serve as depot for secondary vehicle type
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

        public static void RemoveActiveVehicle(ushort lineID, bool descreaseTargetVehicleCount)
        {
            int num2 = CountLineActiveVehicles(lineID);
            ushort activeVehicle = GetActiveVehicle(ref Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineID],
                Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)num2));
            if ((int) activeVehicle != 0)
            {
                TransportLineMod.RemoveVehicle(lineID, activeVehicle, descreaseTargetVehicleCount);
            }
        }

        public static void RemoveVehicle(ushort lineID, ushort vehicleID, bool descreaseTargetVehicleCount)
        {
            if (descreaseTargetVehicleCount)
                TransportLineMod.DecreaseTargetVehicleCount(lineID);
            VehicleManager instance = Singleton<VehicleManager>.instance;
            instance.m_vehicles.m_buffer[(int) vehicleID]
                .Info.m_vehicleAI.SetTransportLine(vehicleID, ref instance.m_vehicles.m_buffer[(int) vehicleID],
                    (ushort) 0);
        }

        public static int CountLineActiveVehicles(ushort lineID)
        {
            
            int activeVehicles = 0;

            TransportLine thisLine = TransportManager.instance.m_lines.m_buffer[lineID];

            //this part is directly taken from beginning of vanilla SimulationStep method
            if (thisLine.Complete)
            {
                int num1 = 0;
                int num2 = 0;
                if ((int) thisLine.m_vehicles != 0)
                {
                    VehicleManager instance = Singleton<VehicleManager>.instance;
                    ushort num3 = thisLine.m_vehicles;
                    int num4 = 0;
                    while ((int) num3 != 0)
                    {
                        ushort nextLineVehicle = instance.m_vehicles.m_buffer[(int) num3].m_nextLineVehicle;
                        ++num1;
                        if ((instance.m_vehicles.m_buffer[(int) num3].m_flags & Vehicle.Flags.GoingBack) ==
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
                            ++num2;
                        num3 = nextLineVehicle;
                        if (++num4 > 16384)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core,
                                "Invalid list detected!\n" + System.Environment.StackTrace);
                            break;
                        }
                    }
                }
                //end of vanilla part
                activeVehicles = num2;
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
            ItemClass itemClass = Singleton<TransportManager>.instance.m_lines
                .m_buffer[(int) lineID]
                .Info.m_class;
            ushort[] depots = BuildingWatcher.instance.GetDepots(itemClass.m_service, itemClass.m_subService, itemClass.m_level);
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