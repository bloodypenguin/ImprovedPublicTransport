// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.BuildingWatcher
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ICities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public class BuildingWatcher : ThreadingExtensionBase
  {
    public static BuildingWatcher instance;
    private bool _initialized;
    private float _delta;
    private BuildingManager _buildingManager;
    private Dictionary<ItemClass.SubService, HashSet<ushort>> _depotMap;

    public event BuildingWatcher.DepotAdded OnDepotAdded;

    public event BuildingWatcher.DepotRemoved OnDepotRemoved;

    public override void OnCreated(IThreading threading)
    {
      BuildingWatcher.instance = this;
      this._initialized = false;
      this._delta = 0.0f;
      this._depotMap = new Dictionary<ItemClass.SubService, HashSet<ushort>>();
      base.OnCreated(threading);
    }

    public override void OnBeforeSimulationTick()
    {
      if (!this._initialized || !this._buildingManager.m_buildingsUpdated)
        return;
      for (int index1 = 0; index1 < this._buildingManager.m_updatedBuildings.Length; ++index1)
      {
        ulong updatedBuilding = this._buildingManager.m_updatedBuildings[index1];
        if ((long) updatedBuilding != 0L)
        {
          for (int index2 = 0; index2 < 64; ++index2)
          {
            if (((long) updatedBuilding & 1L << index2) != 0L)
            {
              ushort num1 = (ushort) (index1 << 6 | index2);
              ItemClass.SubService subService;
              if (BuildingWatcher.IsValidDepot(ref this._buildingManager.m_buildings.m_buffer[(int) num1], out subService))
              {
                HashSet<ushort> ushortSet;
                if (this._depotMap.TryGetValue(subService, out ushortSet))
                {
                  ushortSet.Add(num1);
                }
                else
                {
                  ushortSet = new HashSet<ushort>();
                  ushortSet.Add(num1);
                  this._depotMap.Add(subService, ushortSet);
                }
                // ISSUE: reference to a compiler-generated field
                BuildingWatcher.DepotAdded onDepotAdded = this.OnDepotAdded;
                if (onDepotAdded != null)
                {
                  int num2 = (int) subService;
                  onDepotAdded((ItemClass.SubService) num2);
                }
              }
            }
          }
        }
      }
      base.OnBeforeSimulationTick();
    }

    public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
    {
      if (!this._initialized)
        return;
      if ((double) this._delta > 1.0)
      {
        this._delta = 0.0f;
        HashSet<ushort> ushortSet1 = new HashSet<ushort>();
        foreach (KeyValuePair<ItemClass.SubService, HashSet<ushort>> depot in this._depotMap)
        {
          foreach (ushort num in depot.Value)
          {
            ItemClass.SubService subService;
            if (!BuildingWatcher.IsValidDepot(ref this._buildingManager.m_buildings.m_buffer[(int) num], out subService))
              ushortSet1.Add(num);
          }
          if (ushortSet1.Count != 0)
          {
            HashSet<ushort> ushortSet2;
            if (this._depotMap.TryGetValue(depot.Key, out ushortSet2))
            {
              ushortSet2.ExceptWith((IEnumerable<ushort>) ushortSet1);
              // ISSUE: reference to a compiler-generated field
              BuildingWatcher.DepotRemoved onDepotRemoved = this.OnDepotRemoved;
              if (onDepotRemoved != null)
              {
                int key = (int) depot.Key;
                onDepotRemoved((ItemClass.SubService) key);
              }
            }
            ushortSet1.Clear();
          }
        }
      }
      this._delta = this._delta + realTimeDelta;
      base.OnUpdate(realTimeDelta, simulationTimeDelta);
    }

    public override void OnReleased()
    {
      BuildingWatcher.instance = (BuildingWatcher) null;
      base.OnReleased();
    }

    public void Init()
    {
      this._buildingManager = Singleton<BuildingManager>.instance;
      for (int index = 0; index < this._buildingManager.m_buildings.m_buffer.Length; ++index)
      {
        ItemClass.SubService subService;
        if (BuildingWatcher.IsValidDepot(ref this._buildingManager.m_buildings.m_buffer[index], out subService))
        {
          HashSet<ushort> ushortSet;
          if (this._depotMap.TryGetValue(subService, out ushortSet))
          {
            ushortSet.Add((ushort) index);
          }
          else
          {
            ushortSet = new HashSet<ushort>();
            ushortSet.Add((ushort) index);
            this._depotMap.Add(subService, ushortSet);
          }
        }
      }
      this._initialized = true;
    }

    public void Deinit()
    {
      this._initialized = false;
      this._buildingManager = (BuildingManager) null;
      this._delta = 0.0f;
      // ISSUE: reference to a compiler-generated field
      this.OnDepotAdded = (BuildingWatcher.DepotAdded) null;
      // ISSUE: reference to a compiler-generated field
      this.OnDepotRemoved = (BuildingWatcher.DepotRemoved) null;
      if (this._depotMap == null)
        return;
      this._depotMap.Clear();
    }

    public static bool IsValidDepot(ref Building building, out ItemClass.SubService subService) //TODO(earalov): add support for Mass Transit vehicle types
    {
      subService = ItemClass.SubService.None;
      if ((Object) building.Info == (Object) null)
        return false;
      DepotAI buildingAi = building.Info.m_buildingAI as DepotAI;
      if ((Object) buildingAi == (Object) null || (building.m_flags & Building.Flags.Created) == Building.Flags.None || buildingAi.m_maxVehicleCount == 0)
        return false;
      subService = building.Info.m_class.m_subService;
      switch (subService)
      {
        case ItemClass.SubService.PublicTransportBus:
        case ItemClass.SubService.PublicTransportMetro:
        case ItemClass.SubService.PublicTransportTrain:
        case ItemClass.SubService.PublicTransportShip:
        case ItemClass.SubService.PublicTransportPlane:
        case ItemClass.SubService.PublicTransportTram:
          return true;
        default:
          return false;
      }
    }

    public ushort[] GetDepots(ItemClass.SubService subService)
    {
      HashSet<ushort> source;
      if (this._depotMap.TryGetValue(subService, out source))
        return source.ToArray<ushort>();
      return new ushort[0];
    }

    public delegate void DepotAdded(ItemClass.SubService subService);

    public delegate void DepotRemoved(ItemClass.SubService subService);
  }
}
