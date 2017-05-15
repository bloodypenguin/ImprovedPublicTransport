// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehiclePrefabs
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System.Collections.Generic;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public class VehiclePrefabs //TODO(earalov): add new vehicle types (monorail, blimp, cablecar, ferry, evacuation buses)
    {
    public static VehiclePrefabs instance;
    private PrefabData[] _busPrefabData;
    private PrefabData[] _metroPrefabData;
    private PrefabData[] _trainPrefabData;
    private PrefabData[] _shipPrefabData;
    private PrefabData[] _planePrefabData;
    private PrefabData[] _taxiPrefabData;
    private PrefabData[] _tramPrefabData;

    public static void Init()
    {
      VehiclePrefabs.instance = new VehiclePrefabs();
      VehiclePrefabs.instance.FindAllPrefabs();
    }

    public static void Deinit()
    {
      VehiclePrefabs.instance = (VehiclePrefabs) null;
    }

    public PrefabData[] GetPrefabs(ItemClass.SubService subService) //TODO(earalov): add new vehicle types (monorail, blimp, cablecar, ferry, evacuation buses)
        {
      switch (subService)
      {
        case ItemClass.SubService.PublicTransportBus:
          return this._busPrefabData;
        case ItemClass.SubService.PublicTransportMetro:
          return this._metroPrefabData;
        case ItemClass.SubService.PublicTransportTrain:
          return this._trainPrefabData;
        case ItemClass.SubService.PublicTransportShip:
          return this._shipPrefabData;
        case ItemClass.SubService.PublicTransportPlane:
          return this._planePrefabData;
        case ItemClass.SubService.PublicTransportTaxi:
          return this._taxiPrefabData;
        case ItemClass.SubService.PublicTransportTram:
          return this._tramPrefabData;
        default:
          {
              UnityEngine.Debug.LogWarning("IPT: Vehicles of sub service " + subService + " were requested. They are currently not supported.");
              return new PrefabData[] { };
          }
      }
    }

    private void FindAllPrefabs()  //TODO(earalov): add new vehicle types (monorail, blimp, cablecar, ferry, evacuation buses)
        {
      List<PrefabData> prefabDataList1 = new List<PrefabData>();
      List<PrefabData> prefabDataList2 = new List<PrefabData>();
      List<PrefabData> prefabDataList3 = new List<PrefabData>();
      List<PrefabData> prefabDataList4 = new List<PrefabData>();
      List<PrefabData> prefabDataList5 = new List<PrefabData>();
      List<PrefabData> prefabDataList6 = new List<PrefabData>();
      List<PrefabData> prefabDataList7 = new List<PrefabData>();
      for (int index = 0; index < PrefabCollection<VehicleInfo>.PrefabCount(); ++index)
      {
        VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab((uint) index);
        if ((Object) prefab != (Object) null && !VehiclePrefabs.IsTrailer(prefab) && (prefab.m_class.m_service == ItemClass.Service.PublicTransport && prefab.m_class.m_level == ItemClass.Level.Level1))
        {
          switch (prefab.m_class.m_subService)
          {
            case ItemClass.SubService.PublicTransportBus:
              prefabDataList1.Add(new PrefabData(prefab));
              continue;
            case ItemClass.SubService.PublicTransportMetro:
              prefabDataList2.Add(new PrefabData(prefab));
              continue;
            case ItemClass.SubService.PublicTransportTrain:
              prefabDataList3.Add(new PrefabData(prefab));
              continue;
            case ItemClass.SubService.PublicTransportShip:
              prefabDataList4.Add(new PrefabData(prefab));
              continue;
            case ItemClass.SubService.PublicTransportPlane:
              prefabDataList5.Add(new PrefabData(prefab));
              continue;
            case ItemClass.SubService.PublicTransportTaxi:
              prefabDataList6.Add(new PrefabData(prefab));
              continue;
            case ItemClass.SubService.PublicTransportTram:
              prefabDataList7.Add(new PrefabData(prefab));
              continue;
            default:
              continue;
          }
        }
      }
      this._busPrefabData = prefabDataList1.ToArray();
      this._metroPrefabData = prefabDataList2.ToArray();
      this._trainPrefabData = prefabDataList3.ToArray();
      this._shipPrefabData = prefabDataList4.ToArray();
      this._planePrefabData = prefabDataList5.ToArray();
      this._taxiPrefabData = prefabDataList6.ToArray();
      this._tramPrefabData = prefabDataList7.ToArray();
    }

    private static bool IsTrailer(VehicleInfo info)
    {
      string str = ColossalFramework.Globalization.Locale.GetUnchecked("VEHICLE_TITLE", info.name);
      if (!str.StartsWith("VEHICLE_TITLE"))
        return str.StartsWith("Trailer");
      return true;
    }
  }
}
