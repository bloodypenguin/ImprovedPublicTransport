// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleManagerMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using ColossalFramework;
using ColossalFramework.Math;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2
{

  public static class CachedVehicleData
  {
    public static int MaxVehicleCount;
    private static readonly string _dataID = "IPT_VehicleData";
    private static readonly string _dataVersion = "v003";
    private static bool _isDeployed = false;

    public static VehicleData[] m_cachedVehicleData;

    public static void Init()
    {
      if (Utils.IsModActive(1764208250))
      {
        UnityEngine.Debug.LogWarning("More Vehicles is enabled, applying compatibility workaround");
        MaxVehicleCount = ushort.MaxValue + 1;
      }
      else
      {
        UnityEngine.Debug.Log("More Vehicles is not enabled");
        MaxVehicleCount = VehicleManager.MAX_VEHICLE_COUNT;
      }
      
      if (CachedVehicleData._isDeployed)
        return;
      if (!CachedVehicleData.TryLoadData(out CachedVehicleData.m_cachedVehicleData))
        Utils.Log((object) "Loading default vehicle data.");

      SerializableDataExtension.instance.EventSaveData += new SerializableDataExtension.SaveDataEventHandler(CachedVehicleData.OnSaveData);
      CachedVehicleData._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!CachedVehicleData._isDeployed)
        return;
      CachedVehicleData.m_cachedVehicleData = (VehicleData[]) null;

      SerializableDataExtension.instance.EventSaveData -= new SerializableDataExtension.SaveDataEventHandler(CachedVehicleData.OnSaveData);
      CachedVehicleData._isDeployed = false;
    }

    public static bool TryLoadData(out VehicleData[] data)
    {
      data = new VehicleData[CachedVehicleData.MaxVehicleCount];
      byte[] data1 = SerializableDataExtension.instance.SerializableData.LoadData(CachedVehicleData._dataID);
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
        while (index1 < Math.Min(data1.Length, CachedVehicleData.MaxVehicleCount))
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
        data = new VehicleData[CachedVehicleData.MaxVehicleCount];
        return false;
      }
    }

    private static void OnSaveData()
    {
      FastList<byte> data = new FastList<byte>();
      try
      {
        SerializableDataExtension.WriteString(CachedVehicleData._dataVersion, data);
        for (int index = 0; index < CachedVehicleData.MaxVehicleCount; ++index)
        {
          if (!CachedVehicleData.m_cachedVehicleData[index].IsEmpty)
          {
            SerializableDataExtension.WriteInt32(index, data);
            SerializableDataExtension.WriteInt32(CachedVehicleData.m_cachedVehicleData[index].LastStopNewPassengers, data);
            SerializableDataExtension.WriteInt32(CachedVehicleData.m_cachedVehicleData[index].LastStopGonePassengers, data);
            SerializableDataExtension.WriteInt32(CachedVehicleData.m_cachedVehicleData[index].PassengersThisWeek, data);
            SerializableDataExtension.WriteInt32(CachedVehicleData.m_cachedVehicleData[index].PassengersLastWeek, data);
            SerializableDataExtension.WriteInt32(CachedVehicleData.m_cachedVehicleData[index].IncomeThisWeek, data);
            SerializableDataExtension.WriteInt32(CachedVehicleData.m_cachedVehicleData[index].IncomeLastWeek, data);
            SerializableDataExtension.WriteFloatArray(CachedVehicleData.m_cachedVehicleData[index].PassengerData, data);
            SerializableDataExtension.WriteFloatArray(CachedVehicleData.m_cachedVehicleData[index].IncomeData, data);
            SerializableDataExtension.WriteUInt16(CachedVehicleData.m_cachedVehicleData[index].CurrentStop, data);
          }
        }
        SerializableDataExtension.instance.SerializableData.SaveData(CachedVehicleData._dataID, data.ToArray());
      }
      catch (Exception ex)
      {
        Utils.LogError((object) ("Error while saving vehicle data! " + ex.Message + " " + (object) ex.InnerException));
      }
    }
  }
}
