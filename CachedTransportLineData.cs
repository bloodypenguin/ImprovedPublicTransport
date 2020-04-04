using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using Utils = ImprovedPublicTransport2.Util.Utils;

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
                        _lineData[index].TargetVehicleCount = TransportLineUtil.CountLineActiveVehicles(index, out int _);
                    }
                    else
                        _lineData[index].TargetVehicleCount =
                            OptionsWrapper<Settings>.Options.DefaultVehicleCount;
                    _lineData[index].BudgetControl = OptionsWrapper<Settings>.Options.BudgetControl;
                    _lineData[index].Depot = DepotUtil.GetClosestDepot((ushort) index,
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
                        : DepotUtil.GetClosestDepot(lineID,
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
                        BitConverter.GetBytes(GetTargetVehicleCount(lineID)), data);
                    SerializableDataExtension.AddToData(
                        BitConverter.GetBytes(Mathf.Max(
                            GetNextSpawnTime(lineID) - SimHelper.SimulationTime, 0.0f)),
                        data);
                    SerializableDataExtension.AddToData(
                        BitConverter.GetBytes(GetBudgetControlState(lineID)), data);
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(GetDepot(lineID)), data);
                    int num = 0;
                    HashSet<string> prefabs = GetPrefabs(lineID);
                    if (prefabs != null)
                        num = prefabs.Count;
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(num), data);
                    if (num > 0)
                    {
                        foreach (string s in prefabs)
                            SerializableDataExtension.WriteString(s, data);
                    }
                    string[] enqueuedVehicles = GetEnqueuedVehicles(lineID);
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(enqueuedVehicles.Length), data);
                    if (enqueuedVehicles.Length != 0)
                    {
                        foreach (string s in enqueuedVehicles)
                            SerializableDataExtension.WriteString(s, data);
                    }
                    SerializableDataExtension.WriteBool(GetUnbunchingState(lineID), data);
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
        
                public static int GetTargetVehicleCount(ushort lineID)
        {
            return CachedTransportLineData._lineData[(int) lineID].TargetVehicleCount;
        }
                
        public static void SetLineDefaults(ushort lineID)
        {
            CachedTransportLineData._lineData[(int) lineID] = new LineData();
            CachedTransportLineData._lineData[(int) lineID].TargetVehicleCount =
                OptionsWrapper<Settings>.Options.DefaultVehicleCount;
            CachedTransportLineData._lineData[(int) lineID].BudgetControl = OptionsWrapper<Settings>.Options.BudgetControl;
            CachedTransportLineData._lineData[(int) lineID].Unbunching = OptionsWrapper<Settings>.Options.Unbunching;
        }

        public static void SetTargetVehicleCount(ushort lineID, int count)
        {
            CachedTransportLineData._lineData[(int) lineID].TargetVehicleCount = count;
        }

        public static void IncreaseTargetVehicleCount(ushort lineID)
        {
            ++CachedTransportLineData._lineData[(int) lineID].TargetVehicleCount;
        }

        public static void DecreaseTargetVehicleCount(ushort lineID)
        {
            if (CachedTransportLineData._lineData[(int) lineID].TargetVehicleCount == 0)
                return;
            --CachedTransportLineData._lineData[(int) lineID].TargetVehicleCount;
        }

        public static float GetNextSpawnTime(ushort lineID)
        {
            return CachedTransportLineData._lineData[(int) lineID].NextSpawnTime;
        }

        public static void SetNextSpawnTime(ushort lineID, float time)
        {
            CachedTransportLineData._lineData[(int) lineID].NextSpawnTime = time;
        }

        public static bool GetBudgetControlState(ushort lineID)
        {
            return CachedTransportLineData._lineData[(int) lineID].BudgetControl;
        }

        public static void SetBudgetControlState(ushort lineID, bool state)
        {
            CachedTransportLineData._lineData[(int) lineID].BudgetControl = state;
        }

        public static bool GetUnbunchingState(ushort lineID)
        {
            return CachedTransportLineData._lineData[(int) lineID].Unbunching;
        }

        public static void SetUnbunchingState(ushort lineID, bool state)
        {
            CachedTransportLineData._lineData[(int) lineID].Unbunching = state;
        }

        public static ushort GetDepot(ushort lineID)
        {
            return CachedTransportLineData._lineData[(int) lineID].Depot;
        }

        public static void SetDepot(ushort lineID, ushort depotID)
        {
            CachedTransportLineData._lineData[(int) lineID].Depot = depotID;
        }

        public static HashSet<string> GetPrefabs(ushort lineID)
        {
            return CachedTransportLineData._lineData[(int) lineID].Prefabs;
        }

        public static void SetPrefabs(ushort lineID, HashSet<string> prefabs)
        {
            CachedTransportLineData._lineData[(int) lineID].Prefabs = prefabs;
        }

        public static string GetRandomPrefab(ushort lineID)
        {
            if (CachedTransportLineData._lineData[(int) lineID].Prefabs != null)
            {
                string[] array = CachedTransportLineData._lineData[(int) lineID].Prefabs.ToArray<string>();
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
            if (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles == null)
                CachedTransportLineData._lineData[(int) lineID].QueuedVehicles = new Queue<string>();
            if (inscreaseVehicleCount)
                IncreaseTargetVehicleCount(lineID);
            lock (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles)
                CachedTransportLineData._lineData[(int) lineID].QueuedVehicles.Enqueue(prefabName);
        }

        public static string Dequeue(ushort lineID)
        {
            lock (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles)
                return CachedTransportLineData._lineData[(int) lineID].QueuedVehicles.Dequeue();
        }

        public static void DequeueVehicle(ushort lineID)
        {
            if (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles == null)
                return;
            DecreaseTargetVehicleCount(lineID);
            Dequeue(lineID);
        }

        public static void DequeueVehicles(ushort lineID, int[] indexes, bool descreaseVehicleCount = true)
        {
            lock (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles)
            {
                List<string> stringList = new List<string>(
                    (IEnumerable<string>) CachedTransportLineData._lineData[(int) lineID].QueuedVehicles);
                for (int index = indexes.Length - 1; index >= 0; --index)
                {
                    stringList.RemoveAt(indexes[index]);
                    if (descreaseVehicleCount)
                        DecreaseTargetVehicleCount(lineID);
                }
                CachedTransportLineData._lineData[(int) lineID].QueuedVehicles =
                    new Queue<string>((IEnumerable<string>) stringList);
            }
        }

        public static string[] GetEnqueuedVehicles(ushort lineID)
        {
            if (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles == null)
                return new string[0];
            lock (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles)
                return CachedTransportLineData._lineData[(int) lineID].QueuedVehicles.ToArray();
        }

        public static int EnqueuedVehiclesCount(ushort lineID)
        {
            if (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles == null)
                return 0;
            return CachedTransportLineData._lineData[(int) lineID].QueuedVehicles.Count;
        }

        public static void ClearEnqueuedVehicles(ushort lineID)
        {
            if (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles == null ||
                CachedTransportLineData._lineData[(int) lineID].QueuedVehicles.Count <= 0)
                return;
            lock (CachedTransportLineData._lineData[(int) lineID].QueuedVehicles)
                CachedTransportLineData._lineData[(int) lineID].QueuedVehicles.Clear();
        }

    }
}