// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.TransportLineMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ColossalFramework;
using ColossalFramework.Math;
using ImprovedPublicTransport.Redirection;
using ImprovedPublicTransport.Redirection.Attributes;
using UnityEngine;

namespace ImprovedPublicTransport.Detour
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
        for (int index = 0; index < length; ++index)
        {
          if (instance2.m_lines.m_buffer[index].Complete)
          {
            int num = instance2.m_lines.m_buffer[index].CountVehicles((ushort) index);
            TransportLineMod._lineData[index].TargetVehicleCount = num;
          }
          else
            TransportLineMod._lineData[index].TargetVehicleCount = ImprovedPublicTransportMod.Settings.DefaultVehicleCount;
          TransportLineMod._lineData[index].BudgetControl = ImprovedPublicTransportMod.Settings.BudgetControl;
          TransportLineMod._lineData[index].Depot = TransportLineMod.GetClosestDepot((ushort) index, instance1.m_nodes.m_buffer[(int) instance2.m_lines.m_buffer[index].GetStop(0)].m_position);
          TransportLineMod._lineData[index].Unbunching = ImprovedPublicTransportMod.Settings.Unbunching;
        }
      }
      SerializableDataExtension.instance.EventSaveData += new SerializableDataExtension.SaveDataEventHandler(TransportLineMod.OnSaveData);
      Redirector<TransportLineMod>.Deploy();
      TransportLineMod._init = true;
    }

    public static void Deinit()
    {
      Redirector<TransportLineMod>.Revert();
      TransportLineMod._lineData = (LineData[]) null;
      SerializableDataExtension.instance.EventSaveData -= new SerializableDataExtension.SaveDataEventHandler(TransportLineMod.OnSaveData);
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
          float num = Mathf.Min(BitConverter.ToSingle(data1, index1), (float) ImprovedPublicTransportMod.Settings.SpawnTimeInterval);
          if ((double) num > 0.0)
            data[(int) lineID].NextSpawnTime = SimHelper.instance.SimulationTime + num;
          index1 += 4;
          bool boolean = BitConverter.ToBoolean(data1, index1);
          data[(int) lineID].BudgetControl = boolean;
          ++index1;
          ushort uint16 = BitConverter.ToUInt16(data1, index1);
          data[(int) lineID].Depot = (int) uint16 != 0 ? uint16 : TransportLineMod.GetClosestDepot(lineID, instance1.m_nodes.m_buffer[(int) instance2.m_lines.m_buffer[(int) lineID].GetStop(0)].m_position);
          index1 += 2;
          if (str == "v001")
          {
            string name = SerializableDataExtension.ReadString(data1, ref index1);
            if (name != "Random")
            {
              if (data[(int) lineID].Prefabs == null)
                data[(int) lineID].Prefabs = new HashSet<string>();
              if ((UnityEngine.Object) PrefabCollection<VehicleInfo>.FindLoaded(name) != (UnityEngine.Object) null)
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
              if ((UnityEngine.Object) PrefabCollection<VehicleInfo>.FindLoaded(name) != (UnityEngine.Object) null)
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
                if ((UnityEngine.Object) PrefabCollection<VehicleInfo>.FindLoaded(name) != (UnityEngine.Object) null)
                {
                  lock (data[(int) lineID].QueuedVehicles)
                    data[(int) lineID].QueuedVehicles.Enqueue(name);
                }
              }
            }
          }
          if (str == "v003")
            ++index1;
          data[(int) lineID].Unbunching = !(str == "v004") ? ImprovedPublicTransportMod.Settings.Unbunching : SerializableDataExtension.ReadBool(data1, ref index1);
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
          SerializableDataExtension.AddToData(BitConverter.GetBytes(TransportLineMod.GetTargetVehicleCount(lineID)), data);
          SerializableDataExtension.AddToData(BitConverter.GetBytes(Mathf.Max(TransportLineMod.GetNextSpawnTime(lineID) - SimHelper.instance.SimulationTime, 0.0f)), data);
          SerializableDataExtension.AddToData(BitConverter.GetBytes(TransportLineMod.GetBudgetControlState(lineID)), data);
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

        //TODO(earalov): restore
        //    [RedirectMethod]  
        //    public void SimulationStep(ushort lineID)
        //    {
        //      if (!TransportLineMod._init)
        //        return;
        //      TransportLineMod.SimulationStepImpl(ref Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID], lineID);
        //    }
        //
        //    public static void SimulationStepImpl(ref TransportLine line, ushort lineID)
        //    {
        //      if (!line.Complete)
        //        return;
        //      TransportInfo info = line.Info;
        //      SimulationManager instance1 = Singleton<SimulationManager>.instance;
        //      bool isLineEnabled = !instance1.m_isNightTime ? (line.m_flags & TransportLine.Flags.DisabledDay) == TransportLine.Flags.None : (line.m_flags & TransportLine.Flags.DisabledNight) == TransportLine.Flags.None;
        //      bool flag = TransportLineMod.SetLineStatus(lineID, isLineEnabled);
        //      int num1 = line.CountVehicles(lineID);
        //      int num2 = 0;
        //      if (TransportLineMod._lineData[(int) lineID].BudgetControl)
        //      {
        //        num2 = !isLineEnabled ? 0 : (!flag ? Mathf.CeilToInt((float) ((double) TransportLineMod.GetBudget(lineID, instance1.m_isNightTime, info.m_class) * (double) TransportLineMod.GetLength(lineID) / ((double) info.m_defaultVehicleDistance * 100.0))) : num1);
        //        TransportLineMod._lineData[(int) lineID].TargetVehicleCount = num2;
        //      }
        //      else if (isLineEnabled)
        //        num2 = TransportLineMod._lineData[(int) lineID].TargetVehicleCount;
        //      if (num1 < num2)
        //      {
        //        if ((double) SimHelper.instance.SimulationTime >= (double) TransportLineMod._lineData[(int) lineID].NextSpawnTime)
        //        {
        //          int index1 = instance1.m_randomizer.Int32((uint) line.CountStops(lineID));
        //          ushort stop = line.GetStop(index1);
        //          if (info.m_vehicleReason != TransferManager.TransferReason.None && (int) stop != 0)
        //          {
        //            TransferManager.TransferOffer offer = new TransferManager.TransferOffer();
        //            offer.Priority = num2 - num1 + 1;
        //            offer.TransportLine = lineID;
        //            offer.Position = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) stop].m_position;
        //            offer.Amount = 1;
        //            offer.Active = false;
        //            ushort depot = TransportLineMod._lineData[(int) lineID].Depot;
        //            if (TransportLineMod.IsLineDepotStillValid(lineID, ref depot))
        //            {
        //              BuildingManager instance2 = Singleton<BuildingManager>.instance;
        //              if (TransportLineMod.CanAddVehicle(depot, ref instance2.m_buildings.m_buffer[(int) depot]))
        //              {
        //                string prefabName;
        //                if (TransportLineMod.EnqueuedVehiclesCount(lineID) > 0)
        //                {
        //                  prefabName = TransportLineMod.Dequeue(lineID);
        //                }
        //                else
        //                {
        //                  int num3 = num2 - num1;
        //                  for (int index2 = 0; index2 < num3; ++index2)
        //                    TransportLineMod.EnqueueVehicle(lineID, TransportLineMod.GetRandomPrefab(lineID), false);
        //                  prefabName = TransportLineMod.Dequeue(lineID);
        //                }
        //                if (prefabName != "")
        //                {
        //                  int num4 = (int) DepotAIMod.StartTransfer(depot, ref instance2.m_buildings.m_buffer[(int) depot], info.m_vehicleReason, offer, prefabName);
        //                }
        //                else
        //                  instance2.m_buildings.m_buffer[(int) depot].Info.m_buildingAI.StartTransfer(depot, ref instance2.m_buildings.m_buffer[(int) depot], info.m_vehicleReason, offer);
        //                TransportLineMod._lineData[(int) lineID].NextSpawnTime = SimHelper.instance.SimulationTime + (float) ImprovedPublicTransportMod.Settings.SpawnTimeInterval;
        //              }
        //              else
        //                TransportLineMod.ClearEnqueuedVehicles(lineID);
        //            }
        //          }
        //        }
        //      }
        //      else if (num1 > num2)
        //        TransportLineMod.RemoveRandomVehicle(lineID, false);
        //      if ((instance1.m_currentFrameIndex & 4095U) < 3840U)
        //        return;
        //      line.m_passengers.Update();
        //      Singleton<TransportManager>.instance.m_passengers[(int) info.m_transportType].Add(ref line.m_passengers);
        //      line.m_passengers.Reset();
        //      ushort stops = line.m_stops;
        //      ushort stop1 = stops;
        //      do
        //      {
        //        NetManagerMod.m_cachedNodeData[(int) stop1].StartNewWeek();
        //        stop1 = TransportLine.GetNextStop(stop1);
        //      }
        //      while ((int) stops != (int) stop1 && (int) stop1 != 0);
        //      VehicleManager instance3 = Singleton<VehicleManager>.instance;
        //      PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(info.m_class.m_subService);
        //      int amount = 0;
        //      for (ushort index = line.m_vehicles; (int) index != 0; index = instance3.m_vehicles.m_buffer[(int) index].m_nextLineVehicle)
      //      {
      //        Vehicle vehicle = instance3.m_vehicles.m_buffer[(int) index];
      //        PrefabData prefabData = Array.Find<PrefabData>(prefabs, (Predicate<PrefabData>) (item => item.PrefabDataIndex == vehicle.Info.m_prefabDataIndex));
      //        if (prefabData != null)
      //        {
      //          amount += prefabData.MaintenanceCost;
      //          VehicleManagerMod.m_cachedVehicleData[(int) index].StartNewWeek(prefabData.MaintenanceCost);
      //        }
      //      }
      //      if (amount == 0)
      //        return;
      //      Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.Maintenance, amount, info.m_class);
      //    }

        public static void SetLineDefaults(ushort lineID)
    {
      TransportLineMod._lineData[(int) lineID] = new LineData();
      TransportLineMod._lineData[(int) lineID].TargetVehicleCount = ImprovedPublicTransportMod.Settings.DefaultVehicleCount;
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
          buffer[(int) num1].m_flags = !isLineEnabled ? buffer[(int) num1].m_flags | NetNode.Flags.Disabled : buffer[(int) num1].m_flags & (NetNode.Flags.OneWayOutTrafficLights | NetNode.Flags.UndergroundTransition | NetNode.Flags.Created | NetNode.Flags.Deleted | NetNode.Flags.Original | NetNode.Flags.End | NetNode.Flags.Middle | NetNode.Flags.Bend | NetNode.Flags.Junction | NetNode.Flags.Moveable | NetNode.Flags.Untouchable | NetNode.Flags.Outside | NetNode.Flags.Temporary | NetNode.Flags.Double | NetNode.Flags.Fixed | NetNode.Flags.OnGround | NetNode.Flags.Ambiguous | NetNode.Flags.Water | NetNode.Flags.Sewage | NetNode.Flags.ForbidLaneConnection | NetNode.Flags.LevelCrossing | NetNode.Flags.OneWayIn | NetNode.Flags.Heating | NetNode.Flags.Electricity | NetNode.Flags.Collapsed | NetNode.Flags.DisableOnlyMiddle | NetNode.Flags.AsymForward | NetNode.Flags.AsymBackward | NetNode.Flags.CustomTrafficLights);
          for (int index = 0; index < 8; ++index)
          {
            ushort segment = buffer[(int) num1].GetSegment(index);
            if ((int) segment != 0 && (int) instance.m_segments.m_buffer[(int) segment].m_startNode == (int) num1)
            {
              num3 = instance.m_segments.m_buffer[(int) segment].m_endNode;
              if ((instance.m_segments.m_buffer[(int) segment].m_flags & NetSegment.Flags.PathLength) == NetSegment.Flags.None)
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
              CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
              break;
            }
          }
          else
            break;
        }
      }
      return flag;
    }

    public static int GetBudget(ushort lineID, bool isNightTime, ItemClass itemClass)
    {
      if (!isNightTime && TransportLineMod._lineData[(int) lineID].DayBudget > 0)
        return TransportLineMod._lineData[(int) lineID].DayBudget;
      if (isNightTime && TransportLineMod._lineData[(int) lineID].NightBudget > 0)
        return TransportLineMod._lineData[(int) lineID].NightBudget;
      return Singleton<EconomyManager>.instance.GetBudget(itemClass);
    }

    public static void SetBudget(ushort lineID, bool isNight, int value)
    {
      if (!isNight)
        TransportLineMod._lineData[(int) lineID].DayBudget = value;
      else
        TransportLineMod._lineData[(int) lineID].NightBudget = value;
    }

    public static float GetLength(ushort lineID)
    {
      var length = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].m_totalLength;
      if (Math.Abs(length) < 0.01f)
        Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].UpdateMeshData(lineID);
      return length;
    }


    public static bool IsLineDepotStillValid(ushort lineID, ref ushort depotID)
    {
      ItemClass.SubService subService;
      if ((int) depotID == 0 || !BuildingWatcher.IsValidDepot(ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) depotID], out subService))
      {
        depotID = TransportLineMod.GetClosestDepot(lineID, Singleton<NetManager>.instance.m_nodes.m_buffer[(int) Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].GetStop(0)].m_position);
        TransportLineMod._lineData[(int) lineID].Depot = depotID;
        if ((int) depotID == 0)
        {
          TransportLineMod.ClearEnqueuedVehicles(lineID);
          return false;
        }
      }
      return true;
    }

    public static bool CanAddVehicle(ushort depotID, ref Building depot)
    {
      DepotAI buildingAi = depot.Info.m_buildingAI as DepotAI;
      int num = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(buildingAi.m_info.m_class)) * buildingAi.m_maxVehicleCount + 99) / 100;
      return buildingAi.GetVehicleCount(depotID, ref depot) < num;
    }

    public static void RemoveRandomVehicle(ushort lineID, bool descreaseVehicleCount = true)
    {
      TransportLine transportLine = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID];
      int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) transportLine.CountVehicles(lineID));
      TransportLineMod.RemoveVehicle(lineID, transportLine.GetVehicle(index), descreaseVehicleCount);
    }

    public static void RemoveVehicle(ushort lineID, ushort vehicleID, bool descreaseVehicleCount = true)
    {
      if (descreaseVehicleCount)
        TransportLineMod.DecreaseTargetVehicleCount(lineID);
      VehicleManager instance = Singleton<VehicleManager>.instance;
      instance.m_vehicles.m_buffer[(int) vehicleID].Info.m_vehicleAI.SetTransportLine(vehicleID, ref instance.m_vehicles.m_buffer[(int) vehicleID], (ushort) 0);
    }

    public static int GetLineVehicleCount(ushort lineID)
    {
      return Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].CountVehicles(lineID);
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
      ItemClass.SubService subService = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].Info.m_class.m_subService;
      PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(subService);
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
        List<string> stringList = new List<string>((IEnumerable<string>) TransportLineMod._lineData[(int) lineID].QueuedVehicles);
        for (int index = indexes.Length - 1; index >= 0; --index)
        {
          stringList.RemoveAt(indexes[index]);
          if (descreaseVehicleCount)
            TransportLineMod.DecreaseTargetVehicleCount(lineID);
        }
        TransportLineMod._lineData[(int) lineID].QueuedVehicles = new Queue<string>((IEnumerable<string>) stringList);
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
      if (TransportLineMod._lineData[(int) lineID].QueuedVehicles == null || TransportLineMod._lineData[(int) lineID].QueuedVehicles.Count <= 0)
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
          CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
          break;
        }
      }
      return 0;
    }

    public static ushort GetNextVehicle(ushort lineID, ushort vehicleID)
    {
      ushort vehicles = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].m_vehicles;
      ushort nextLineVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) vehicleID].m_nextLineVehicle;
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

    public static ushort GetClosestDepot(ushort lineID, Vector3 stopPosition)
    {
      ushort num1 = 0;
      float num2 = float.MaxValue;
      BuildingManager instance = Singleton<BuildingManager>.instance;
      ushort[] depots = BuildingWatcher.instance.GetDepots(Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID].Info.m_class.m_subService);
      for (int index = 0; index < depots.Length; ++index)
      {
        float num3 = Vector3.Distance(stopPosition, instance.m_buildings.m_buffer[(int) depots[index]].m_position);
        if ((double) num3 < (double) num2)
        {
          num1 = depots[index];
          num2 = num3;
        }
      }
      return num1;
    }
  }
}
