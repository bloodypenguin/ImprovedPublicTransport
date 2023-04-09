using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2.Data
{
    public static class CachedTransportLineData
    {
        private static readonly string _dataID = "ImprovedPublicTransport";
        private static readonly string _dataVersion = "v004";

        public static bool _init;
        public static LineData[] _lineData;
        
        public static void Init()
        {
            if (!TryLoadData(out _lineData))
            {
                Utils.Log("Loading default transport line data.");
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
                            OptionsWrapper<Settings.Settings>.Options.DefaultVehicleCount;
                    _lineData[index].BudgetControl = OptionsWrapper<Settings.Settings>.Options.BudgetControl;
                    _lineData[index].Depot = DepotUtil.GetClosestDepot(index,
                        instance1.m_nodes.m_buffer[instance2.m_lines.m_buffer[index].GetStop(0)].m_position);
                    _lineData[index].Unbunching = OptionsWrapper<Settings.Settings>.Options.Unbunching;
                }
            }
            SerializableDataExtension.instance.EventSaveData += OnSaveData;

            _init = true;
        }

        public static void Deinit()
        {
            _lineData = null;
            SerializableDataExtension.instance.EventSaveData -= OnSaveData;
            _init = false;
        }

        public static bool TryLoadData(out LineData[] data)
        {
            data = new LineData[256];
            var data1 = SerializableDataExtension.instance.SerializableData.LoadData(_dataID);
            if (data1 == null)
                return false;
            var index1 = 0;
            ushort lineID = 0;
            try
            {
                Utils.Log("Try to load transport line data.");
                var str = SerializableDataExtension.ReadString(data1, ref index1);
                if (string.IsNullOrEmpty(str) || str.Length != 4)
                {
                    Utils.LogWarning("Unknown data found.");
                    return false;
                }
                Utils.Log("Found transport line data version: " + str);
                var instance1 = Singleton<NetManager>.instance;
                var instance2 = Singleton<TransportManager>.instance;
                while (index1 < data1.Length)
                {
                    if (instance2.m_lines.m_buffer[lineID].Complete)
                    {
                        var int32 = BitConverter.ToInt32(data1, index1);
                        data[lineID].TargetVehicleCount = int32;
                    }
                    index1 += 4;
                    var num = Mathf.Min(BitConverter.ToSingle(data1, index1),
                        OptionsWrapper<Settings.Settings>.Options.SpawnTimeInterval);
                    if (num > 0.0)
                        data[lineID].NextSpawnTime = SimHelper.SimulationTime + num;
                    index1 += 4;
                    var boolean = BitConverter.ToBoolean(data1, index1);
                    data[lineID].BudgetControl = boolean;
                    ++index1;
                    var uint16 = BitConverter.ToUInt16(data1, index1);
                    data[lineID].Depot = uint16 != 0
                        ? uint16
                        : DepotUtil.GetClosestDepot(lineID,
                            instance1.m_nodes.m_buffer[instance2.m_lines.m_buffer[lineID].GetStop(0)]
                                .m_position);
                    index1 += 2;
                    if (str == "v001")
                    {
                        var name = SerializableDataExtension.ReadString(data1, ref index1);
                        if (name != "Random")
                        {
                            data[lineID].Prefabs ??= new HashSet<string>();
                            if (PrefabCollection<VehicleInfo>.FindLoaded(name) !=
                                null)
                                data[lineID].Prefabs.Add(name);
                        }
                    }
                    else
                    {
                        var int32 = BitConverter.ToInt32(data1, index1);
                        index1 += 4;
                        for (var index2 = 0; index2 < int32; ++index2)
                        {
                            var name = SerializableDataExtension.ReadString(data1, ref index1);
                            data[lineID].Prefabs ??= new HashSet<string>();
                            if (PrefabCollection<VehicleInfo>.FindLoaded(name) !=
                                null)
                                data[lineID].Prefabs.Add(name);
                        }
                    }
                    if (str != "v001")
                    {
                        var int32 = BitConverter.ToInt32(data1, index1);
                        index1 += 4;
                        for (var index2 = 0; index2 < int32; ++index2)
                        {
                            var name = SerializableDataExtension.ReadString(data1, ref index1);
                            if (boolean)
                            {
                                continue;
                            }
                            data[lineID].QueuedVehicles ??= new Queue<string>();
                            if (PrefabCollection<VehicleInfo>.FindLoaded(name) == null)
                            {
                                continue;
                            }
                            lock (data[lineID].QueuedVehicles) 
                                data[lineID].QueuedVehicles.Enqueue(name);
                        }
                    }
                    if (str == "v003")
                        ++index1;
                    data[lineID].Unbunching = str != "v004"
                        ? OptionsWrapper<Settings.Settings>.Options.Unbunching
                        : SerializableDataExtension.ReadBool(data1, ref index1);
                    ++lineID;
                }
                return true;
            }
            catch (Exception ex)
            {
                Utils.LogWarning("Could not load transport line data. " + ex.Message);
                data = new LineData[256];
                return false;
            }
        }

        private static void OnSaveData()
        {
            var data = new FastList<byte>();
            try
            {
                SerializableDataExtension.WriteString(_dataVersion, data);
                for (ushort lineID = 0; lineID < 256; ++lineID)
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
                    var num = 0;
                    var prefabs = GetPrefabs(lineID);
                    if (prefabs != null)
                        num = prefabs.Count;
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(num), data);
                    if (num > 0)
                    {
                        foreach (var s in prefabs)
                            SerializableDataExtension.WriteString(s, data);
                    }
                    var enqueuedVehicles = GetEnqueuedVehicles(lineID);
                    SerializableDataExtension.AddToData(BitConverter.GetBytes(enqueuedVehicles.Length), data);
                    if (enqueuedVehicles.Length != 0)
                    {
                        foreach (var s in enqueuedVehicles)
                            SerializableDataExtension.WriteString(s, data);
                    }
                    SerializableDataExtension.WriteBool(GetUnbunchingState(lineID), data);
                }
                SerializableDataExtension.instance.SerializableData.SaveData(_dataID, data.ToArray());
            }
            catch (Exception ex)
            {
                var msg = "Error while saving transport line data! " + ex.Message + " " + ex.InnerException;
                Utils.LogError(msg);
                CODebugBase<LogChannel>.Log(LogChannel.Modding, msg, ErrorLevel.Error);
            }
        }
        
        public static int GetTargetVehicleCount(ushort lineID)
        {
            return _lineData[lineID].TargetVehicleCount;
        }
                
        public static void SetLineDefaults(ushort lineID)
        {
            _lineData[lineID] = new LineData
            {
                TargetVehicleCount = OptionsWrapper<Settings.Settings>.Options.DefaultVehicleCount,
                BudgetControl = OptionsWrapper<Settings.Settings>.Options.BudgetControl,
                Unbunching = OptionsWrapper<Settings.Settings>.Options.Unbunching
            };
        }

        public static void SetTargetVehicleCount(ushort lineID, int count)
        {
            _lineData[lineID].TargetVehicleCount = count;
        }

        public static void IncreaseTargetVehicleCount(ushort lineID)
        {
            ++_lineData[lineID].TargetVehicleCount;
        }

        public static void DecreaseTargetVehicleCount(ushort lineID)
        {
            if (_lineData[lineID].TargetVehicleCount == 0)
                return;
            --_lineData[lineID].TargetVehicleCount;
        }

        public static float GetNextSpawnTime(ushort lineID)
        {
            return _lineData[lineID].NextSpawnTime;
        }

        public static void SetNextSpawnTime(ushort lineID, float time)
        {
            _lineData[lineID].NextSpawnTime = time;
        }

        public static bool GetBudgetControlState(ushort lineID)
        {
            return _lineData[lineID].BudgetControl;
        }

        public static void SetBudgetControlState(ushort lineID, bool state)
        {
            _lineData[lineID].BudgetControl = state;
        }

        public static bool GetUnbunchingState(ushort lineID)
        {
            return _lineData[lineID].Unbunching;
        }

        public static void SetUnbunchingState(ushort lineID, bool state)
        {
            _lineData[lineID].Unbunching = state;
        }

        public static ushort GetDepot(ushort lineID)
        {
            return _lineData[lineID].Depot;
        }

        public static void SetDepot(ushort lineID, ushort depotID)
        {
            _lineData[lineID].Depot = depotID;
        }

        public static HashSet<string> GetPrefabs(ushort lineID)
        {
            return _lineData[lineID].Prefabs;
        }

        public static void SetPrefabs(ushort lineID, HashSet<string> prefabs)
        {
            _lineData[lineID].Prefabs = prefabs;
        }

        public static string GetRandomPrefab(ushort lineID)
        {
            if (_lineData[lineID].Prefabs != null)
            {
                var array = _lineData[lineID].Prefabs.ToArray();
                if (array.Length != 0)
                {
                    var index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
                    return array[index];
                }
            }
            var itemClass = Singleton<TransportManager>.instance.m_lines.m_buffer[lineID]
                .Info.m_class;
            var prefabs = VehiclePrefabs.instance.GetPrefabs(itemClass.m_service, itemClass.m_subService, itemClass.m_level);
            var index1 = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) prefabs.Length);
            return prefabs[index1].Name;
        }

        public static void EnqueueVehicle(ushort lineID, string prefabName)
        {
            _lineData[lineID].QueuedVehicles ??= new Queue<string>();
            lock (_lineData[lineID].QueuedVehicles)
                _lineData[lineID].QueuedVehicles.Enqueue(prefabName);
        }

        public static string Dequeue(ushort lineID)
        {
            if (_lineData[lineID].QueuedVehicles is not { Count: not 0 })
            {
                return null;
            }
            lock (_lineData[lineID].QueuedVehicles)
                return _lineData[lineID].QueuedVehicles.Dequeue();
        }

        public static void DequeueVehicle(ushort lineID)
        {
            if (_lineData[lineID].QueuedVehicles is not { Count: not 0 })
            {
                return;
            }

            DecreaseTargetVehicleCount(lineID);
            Dequeue(lineID);
        }

        public static void DequeueVehicles(ushort lineID, int[] indexes, bool decreaseVehicleCount = true)
        {
            if (_lineData[lineID].QueuedVehicles is not { Count: not 0 })
            {
                return;
            }
            lock (_lineData[lineID].QueuedVehicles)
            {
                var stringList = new List<string>(
                    _lineData[lineID].QueuedVehicles);
                for (var index = indexes.Length - 1; index >= 0; --index)
                {
                    stringList.RemoveAt(indexes[index]);
                    if (decreaseVehicleCount)
                        DecreaseTargetVehicleCount(lineID);
                }
                _lineData[lineID].QueuedVehicles =
                    new Queue<string>(stringList);
            }
        }

        public static string[] GetEnqueuedVehicles(ushort lineID)
        {
            if (_lineData[lineID].QueuedVehicles is not { Count: not 0 })
                return new string[0];
            lock (_lineData[lineID].QueuedVehicles)
                return _lineData[lineID].QueuedVehicles.ToArray();
        }

        public static int EnqueuedVehiclesCount(ushort lineID)
        {
            return _lineData[lineID].QueuedVehicles == null ? 0 : _lineData[lineID].QueuedVehicles.Count;
        }

        public static void ClearEnqueuedVehicles(ushort lineID)
        {
            if (_lineData[lineID].QueuedVehicles is not { Count: > 0 })
                return;
            lock (_lineData[lineID].QueuedVehicles)
                _lineData[lineID].QueuedVehicles.Clear();
        }

    }
}