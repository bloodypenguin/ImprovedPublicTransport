// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleManagerMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.Math;
using ImprovedPublicTransport.Detour;
using System;

namespace ImprovedPublicTransport
{
  public class VehicleManagerMod
  {
    private static readonly string _dataID = "IPT_VehicleData";
    private static readonly string _dataVersion = "v003";
    private static bool _isDeployed = false;
    private static VehicleManagerMod.ReleaseVehicleImplementationCallback ReleaseVehicleImplementation;
    private static Redirection<VehicleManager, VehicleManagerMod> _redirection;
    public static VehicleData[] m_cachedVehicleData;

    public static void Init()
    {
      if (VehicleManagerMod._isDeployed)
        return;
      if (!VehicleManagerMod.TryLoadData(out VehicleManagerMod.m_cachedVehicleData))
        Utils.Log((object) "Loading default vehicle data.");
      VehicleManagerMod.ReleaseVehicleImplementation = (VehicleManagerMod.ReleaseVehicleImplementationCallback) Utils.CreateDelegate<VehicleManager, VehicleManagerMod.ReleaseVehicleImplementationCallback>("ReleaseVehicleImplementation", (object) Singleton<VehicleManager>.instance);
      VehicleManagerMod._redirection = new Redirection<VehicleManager, VehicleManagerMod>("ReleaseVehicle");
      SerializableDataExtension.instance.EventSaveData += new SerializableDataExtension.SaveDataEventHandler(VehicleManagerMod.OnSaveData);
      VehicleManagerMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!VehicleManagerMod._isDeployed)
        return;
      VehicleManagerMod.m_cachedVehicleData = (VehicleData[]) null;
      VehicleManagerMod._redirection.Revert();
      VehicleManagerMod._redirection = (Redirection<VehicleManager, VehicleManagerMod>) null;
      VehicleManagerMod.ReleaseVehicleImplementation = (VehicleManagerMod.ReleaseVehicleImplementationCallback) null;
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
      PrefabData prefabData = Array.Find<PrefabData>(VehiclePrefabs.instance.GetPrefabs(m_class.m_subService), (Predicate<PrefabData>) (item => item.ObjectName == prefabName));
      if (prefabData != null)
        return PrefabCollection<VehicleInfo>.GetPrefab((uint) prefabData.PrefabDataIndex);
      Utils.LogWarning((object) ("Unknown prefab: " + prefabName));
      VehicleManager instance = Singleton<VehicleManager>.instance;
      instance.RefreshTransferVehicles();
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      Randomizer& r = @randomizer;
      int service = (int) m_class.m_service;
      int subService = (int) m_class.m_subService;
      int level = (int) m_class.m_level;
      return instance.GetRandomVehicleInfo(r, (ItemClass.Service) service, (ItemClass.SubService) subService, (ItemClass.Level) level);
    }

    public void ReleaseVehicle(ushort vehicleID)
    {
      if (!VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].IsEmpty)
        VehicleManagerMod.m_cachedVehicleData[(int) vehicleID] = new VehicleData();
      VehicleManagerMod.ReleaseVehicleImplementation(vehicleID, ref Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) vehicleID]);
    }

    private delegate void ReleaseVehicleImplementationCallback(ushort vehicle, ref Vehicle data);
  }
}
