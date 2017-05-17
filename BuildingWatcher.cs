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
    private Dictionary<ItemClassTriplet, HashSet<ushort>> _depotMap;

    public event BuildingWatcher.DepotAdded OnDepotAdded;

    public event BuildingWatcher.DepotRemoved OnDepotRemoved;

    public override void OnCreated(IThreading threading)
    {
      BuildingWatcher.instance = this;
      this._initialized = false;
      this._delta = 0.0f;
      this._depotMap = new Dictionary<ItemClassTriplet, HashSet<ushort>>();
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
                TransportInfo info = null;
              if (BuildingWatcher.IsValidDepot(ref _buildingManager.m_buildings.m_buffer[(int) num1], ref info,
                  out ItemClass.Service service, out ItemClass.SubService subService, out ItemClass.Level level)) 
              {
                HashSet<ushort> ushortSet;
                var itemClassTriplet = new ItemClassTriplet(service, subService, level);
                if (this._depotMap.TryGetValue(itemClassTriplet, out ushortSet))
                {
                  ushortSet.Add(num1);
                }
                else
                {
                  ushortSet = new HashSet<ushort>();
                  ushortSet.Add(num1);
                  this._depotMap.Add(itemClassTriplet, ushortSet);
                }
                this.OnDepotAdded?.Invoke(service, subService, level);
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
        foreach (KeyValuePair<ItemClassTriplet, HashSet<ushort>> depot in this._depotMap)
        {
          foreach (ushort num in depot.Value)
          {
              TransportInfo info = null;
            if (!BuildingWatcher.IsValidDepot(ref this._buildingManager.m_buildings.m_buffer[(int) num],ref info,
                out ItemClass.Service service, out ItemClass.SubService subService, out ItemClass.Level level))
                            ushortSet1.Add(num);
          }
          if (ushortSet1.Count != 0)
          {
            HashSet<ushort> ushortSet2;
            if (this._depotMap.TryGetValue(depot.Key, out ushortSet2))
            {  
              ushortSet2.ExceptWith((IEnumerable<ushort>) ushortSet1);
              this.OnDepotRemoved?.Invoke(depot.Key.Service, depot.Key.SubService, depot.Key.Level);
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
        TransportInfo info = null;
        if (BuildingWatcher.IsValidDepot(ref this._buildingManager.m_buildings.m_buffer[index], ref info,
            out ItemClass.Service service, out ItemClass.SubService subService, out ItemClass.Level level))  
                {
          HashSet<ushort> ushortSet;
          var itemClassTriplet = new ItemClassTriplet(service, subService, level);
          if (this._depotMap.TryGetValue(itemClassTriplet, out ushortSet))
                    {
            ushortSet.Add((ushort) index);
          }
          else
          {
            ushortSet = new HashSet<ushort>();
            ushortSet.Add((ushort) index);
            this._depotMap.Add(itemClassTriplet, ushortSet);
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

    public static bool IsValidDepot(ref Building building,
        ref TransportInfo transportInfo, 
        out ItemClass.Service service,
        out ItemClass.SubService subService,
        out ItemClass.Level level)
    {
      service = ItemClass.Service.None;
      subService = ItemClass.SubService.None;
      level = ItemClass.Level.None;
      if ((Object) building.Info == (Object) null || (building.m_flags & Building.Flags.Created) == Building.Flags.None)
        return false;
      if (building.Info.m_buildingAI is DepotAI)
        {
            DepotAI buildingAi = building.Info.m_buildingAI as DepotAI;
            if (transportInfo!= null && buildingAi.m_transportInfo.m_vehicleType != transportInfo.m_vehicleType)  //TODO(earalov): allow to serve as depot for secondary vehicle type
            {
                return false;
            }
            if (transportInfo == null)
            {
                transportInfo = buildingAi.m_transportInfo;
            }
            if (buildingAi.m_maxVehicleCount == 0)
            {
                return false;
            }
            service = building.Info.m_class.m_service;
            subService = building.Info.m_class.m_subService;
            level = building.Info.m_class.m_level;
            if(service == ItemClass.Service.PublicTransport) { 
                if (level == ItemClass.Level.Level1)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportBus:
                        case ItemClass.SubService.PublicTransportMetro:
                        case ItemClass.SubService.PublicTransportTrain:
                        case ItemClass.SubService.PublicTransportShip:
                        case ItemClass.SubService.PublicTransportPlane:
                        case ItemClass.SubService.PublicTransportTram:
                        case ItemClass.SubService.PublicTransportMonorail:
                        case ItemClass.SubService.PublicTransportTaxi:
                        case ItemClass.SubService.PublicTransportCableCar:
                                return true;
                    }
                } else if (level == ItemClass.Level.Level2)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportShip:
                        case ItemClass.SubService.PublicTransportPlane:
                            return true;
                    }
                }
            }

      }
      else if (building.Info.m_buildingAI is ShelterAI)
      {
          service = building.Info.m_class.m_service;
          subService = building.Info.m_class.m_subService;
          level = building.Info.m_class.m_level;
          if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None &&
              level == ItemClass.Level.Level4)
          {
              return true;
          }
      }
      return false;

    }

    public ushort[] GetDepots(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)  
        {
      HashSet<ushort> source;
      if (this._depotMap.TryGetValue(new ItemClassTriplet(service, subService, level), out source))
                return source.ToArray<ushort>();
      return new ushort[0];
    }

    public delegate void DepotAdded(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level);

    public delegate void DepotRemoved(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level);
  }
}
