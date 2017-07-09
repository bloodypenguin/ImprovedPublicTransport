// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleManagerMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using ColossalFramework;
using ColossalFramework.Math;
using ImprovedPublicTransport2.RedirectionFramework;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;

namespace ImprovedPublicTransport2.Detour
{
  [TargetType(typeof(VehicleManager))]
  public class VehicleManagerMod
  {
    private static readonly string _dataID = "IPT_VehicleData";
    private static readonly string _dataVersion = "v003";
    private static bool _isDeployed = false;

    public static VehicleData[] m_cachedVehicleData;

    public static void Init()
    {
      if (VehicleManagerMod._isDeployed)
        return;
      if (!VehicleManagerMod.TryLoadData(out VehicleManagerMod.m_cachedVehicleData))
        Utils.Log((object) "Loading default vehicle data.");
      Redirector<VehicleManagerMod>.Deploy();
      SerializableDataExtension.instance.EventSaveData += new SerializableDataExtension.SaveDataEventHandler(VehicleManagerMod.OnSaveData);
      VehicleManagerMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!VehicleManagerMod._isDeployed)
        return;
      VehicleManagerMod.m_cachedVehicleData = (VehicleData[]) null;
      Redirector<VehicleManagerMod>.Revert();
      SerializableDataExtension.instance.EventSaveData -= new SerializableDataExtension.SaveDataEventHandler(VehicleManagerMod.OnSaveData);
      VehicleManagerMod._isDeployed = false;
    }

    public static bool TryLoadData(out VehicleData[] data)
    {
      data = new VehicleData[16384];
      byte[] data1 = SerializableDataExtension.instance.SerializableData.LoadData(VehicleManagerMod._dataID);
      if (data1 == null)
        return false;
      int index1 = 0;
      string empty = string.Empty;
      try
      {
        Utils.Log((object) "Try to load vehicle data.");
        string str = SerializableDataExtension.ReadString(data1, ref index1);
        if (string.IsNullOrEmpty(str) || str.Length != 4)
        {
          Utils.LogWarning((object) "Unknown data found.");
          return false;
        }
        Utils.Log((object) ("Found vehicle data version: " + str));
        while (index1 < data1.Length)
        {
          int index2 = SerializableDataExtension.ReadInt32(data1, ref index1);
          if (str == "v001")
          {
            int num = (int) SerializableDataExtension.ReadByte(data1, ref index1);
          }
          data[index2].LastStopNewPassengers = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].LastStopGonePassengers = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].PassengersThisWeek = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].PassengersLastWeek = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].IncomeThisWeek = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].IncomeLastWeek = SerializableDataExtension.ReadInt32(data1, ref index1);
          data[index2].PassengerData = SerializableDataExtension.ReadFloatArray(data1, ref index1);
          data[index2].IncomeData = SerializableDataExtension.ReadFloatArray(data1, ref index1);
          if (str != "v001" && str != "v002")
            data[index2].CurrentStop = SerializableDataExtension.ReadUInt16(data1, ref index1);
        }
        return true;
      }
      catch (Exception ex)
      {
        Utils.LogWarning((object) ("Could not load vehicle data. " + ex.Message));
        data = new VehicleData[16384];
        return false;
      }
    }

    private static void OnSaveData()
    {
      FastList<byte> data = new FastList<byte>();
      try
      {
        SerializableDataExtension.WriteString(VehicleManagerMod._dataVersion, data);
        for (int index = 0; index < 16384; ++index)
        {
          if (!VehicleManagerMod.m_cachedVehicleData[index].IsEmpty)
          {
            SerializableDataExtension.WriteInt32(index, data);
            SerializableDataExtension.WriteInt32(VehicleManagerMod.m_cachedVehicleData[index].LastStopNewPassengers, data);
            SerializableDataExtension.WriteInt32(VehicleManagerMod.m_cachedVehicleData[index].LastStopGonePassengers, data);
            SerializableDataExtension.WriteInt32(VehicleManagerMod.m_cachedVehicleData[index].PassengersThisWeek, data);
            SerializableDataExtension.WriteInt32(VehicleManagerMod.m_cachedVehicleData[index].PassengersLastWeek, data);
            SerializableDataExtension.WriteInt32(VehicleManagerMod.m_cachedVehicleData[index].IncomeThisWeek, data);
            SerializableDataExtension.WriteInt32(VehicleManagerMod.m_cachedVehicleData[index].IncomeLastWeek, data);
            SerializableDataExtension.WriteFloatArray(VehicleManagerMod.m_cachedVehicleData[index].PassengerData, data);
            SerializableDataExtension.WriteFloatArray(VehicleManagerMod.m_cachedVehicleData[index].IncomeData, data);
            SerializableDataExtension.WriteUInt16(VehicleManagerMod.m_cachedVehicleData[index].CurrentStop, data);
          }
        }
        SerializableDataExtension.instance.SerializableData.SaveData(VehicleManagerMod._dataID, data.ToArray());
      }
      catch (Exception ex)
      {
        Utils.LogError((object) ("Error while saving vehicle data! " + ex.Message + " " + (object) ex.InnerException));
      }
    }

    public static VehicleInfo GetVehicleInfo(ref Randomizer randomizer, ItemClass m_class, ushort lineID, string prefabName)
    {
      PrefabData prefabData = Array.Find(VehiclePrefabs.instance.GetPrefabs(m_class.m_service, m_class.m_subService, m_class.m_level),
          item => item.ObjectName == prefabName);
      if (prefabData != null)
        return PrefabCollection<VehicleInfo>.GetPrefab((uint) prefabData.PrefabDataIndex);
      Utils.LogWarning((object) ("Unknown prefab: " + prefabName));
      VehicleManager instance = Singleton<VehicleManager>.instance;
      instance.RefreshTransferVehicles();

      int service = (int) m_class.m_service;
      int subService = (int) m_class.m_subService;
      int level = (int) m_class.m_level;
      return instance.GetRandomVehicleInfo(ref randomizer, (ItemClass.Service) service, (ItemClass.SubService) subService, (ItemClass.Level) level);
    }

    [RedirectMethod]
    public static void ReleaseVehicle(VehicleManager instance, ushort vehicleID)
    {
        if (!VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].IsEmpty)
        {
            VehicleManagerMod.m_cachedVehicleData[(int) vehicleID] = new VehicleData();
        }
        ReleaseVehicleImplementation(instance, vehicleID, ref instance.m_vehicles.m_buffer[(int) vehicleID]);
    }

    [RedirectReverse]
    private static void ReleaseVehicleImplementation(VehicleManager instance, ushort vehicle, ref Vehicle data)
    {
        UnityEngine.Debug.Log("ReleaseVehicleImplementation");
    }
  
  }
}
