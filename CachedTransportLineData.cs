using System;
using System.Collections.Generic;
using ColossalFramework;
using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.OptionsFramework;
using UnityEngine;

namespace ImprovedPublicTransport2
{
    public static class CachedTransportLineData
    {
        private static readonly string _dataID = "ImprovedPublicTransport";
        private static readonly string _dataVersion = "v004";

        public static bool _init = false;
        public static LineData[] _lineData;
        
        public static void Init()
        {
            if (!TryLoadData(out _lineData))
            {
                Utils.Log((object) "Loading default transport line data.");
                NetManager instance1 = Singleton<NetManager>.instance;
                TransportManager instance2 = Singleton<TransportManager>.instance;
                int length = instance2.m_lines.m_buffer.Length;
                for (ushort index = 0; index < length; ++index)
                {
                    if (instance2.m_lines.m_buffer[index].Complete)
                    {
                        _lineData[index].TargetVehicleCount = TransportLineMod.CountLineActiveVehicles(index, out int _);
                    }
                    else
                        _lineData[index].TargetVehicleCount =
                            OptionsWrapper<Settings>.Options.DefaultVehicleCount;
                    _lineData[index].BudgetControl = OptionsWrapper<Settings>.Options.BudgetControl;
                    _lineData[index].Depot = TransportLineMod.GetClosestDepot((ushort) index,
                        instance1.m_nodes.m_buffer[(int) instance2.m_lines.m_buffer[index].GetStop(0)].m_position);
                    _lineData[index].Unbunching = OptionsWrapper<Settings>.Options.Unbunching;
                }
            }
            SerializableDataExtension.instance.EventSaveData +=
                new SerializableDataExtension.SaveDataEventHandler(OnSaveData);

            _init = true;
        }

        public static void Deinit()
        {
            _lineData = (LineData[]) null;
            SerializableDataExtension.instance.EventSaveData -=
                new SerializableDataExtension.SaveDataEventHandler(OnSaveData);
            _init = false;
        }

        public static bool TryLoadData(out LineData[] data)
        {
            data = new LineData[256];
            byte[] data1 = SerializableDataExtension.instance.SerializableData.LoadData(_dataID);
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
                SerializableDataExtension.WriteString(_dataVersion, data);
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
                SerializableDataExtension.instance.SerializableData.SaveData(_dataID, data.ToArray());
            }
            catch (Exception ex)
            {
                string msg = "Error while saving transport line data! " + ex.Message + " " + (object) ex.InnerException;
                Utils.LogError((object) msg);
                CODebugBase<LogChannel>.Log(LogChannel.Modding, msg, ErrorLevel.Error);
            }
        }
    }
}