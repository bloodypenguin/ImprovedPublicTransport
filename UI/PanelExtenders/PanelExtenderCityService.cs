// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.PanelExtenderCityService
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using UIUtils = ImprovedPublicTransport2.Util.UIUtils;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2.UI.PanelExtenders
{
  public class PanelExtenderCityService : MonoBehaviour
  {
    private  const float VerticalOffset = 40f; //TODO: needed due to the UI issue, revert if CO fixes the panel
    
    private bool _initialized;
    private ushort _cachedBuildingID;
    private int _cachedStopCount;
    private int _cachedVehicleCount;
    private CityServiceWorldInfoPanel _cityServiceWorldInfoPanel;
    private UIPanel _listBoxPanel;
    private UILabel _titleLabel;
    private StopListBox _stopsListBox;
    private VehicleListBox _vehicleListBox;

    private void Update()
    {
      if (!this._initialized)
      {
        this._cityServiceWorldInfoPanel = GameObject.Find("(Library) CityServiceWorldInfoPanel").GetComponent<CityServiceWorldInfoPanel>();
        if (!((UnityEngine.Object) this._cityServiceWorldInfoPanel != (UnityEngine.Object) null))
          return;
        this.CreateStopsPanel();
        this._initialized = true;
      }
      else
      {
        if (!this._initialized || !this._cityServiceWorldInfoPanel.component.isVisible)
          return;
        BuildingManager instance1 = Singleton<BuildingManager>.instance;
        ushort building = Utils.GetPrivate<InstanceID>((object) this._cityServiceWorldInfoPanel, "m_InstanceID").Building;
        ItemClass.SubService subService = instance1.m_buildings.m_buffer[(int) building].Info.GetSubService();
        ItemClass.Service service = instance1.m_buildings.m_buffer[(int) building].Info.GetService();
        ItemClass.Level level = instance1.m_buildings.m_buffer[(int) building].Info.GetClassLevel();

        switch (subService)
        {
          case ItemClass.SubService.PublicTransportBus:
          case ItemClass.SubService.PublicTransportTours:
          case ItemClass.SubService.PublicTransportMetro:
          case ItemClass.SubService.PublicTransportTrain:
          case ItemClass.SubService.PublicTransportShip:
          case ItemClass.SubService.PublicTransportPlane:
          case ItemClass.SubService.PublicTransportMonorail:
          case ItemClass.SubService.PublicTransportTrolleybus:
            this._vehicleListBox.Hide(); //TODO(earalov): display depot's vehicles? Also, maybe it makes sense to display list of lines served by depot?
            this._stopsListBox.Show();
            ushort[] numArray = PanelExtenderCityService.GetStationStops(building);
            BuildingInfo.SubInfo[] subBuildings = instance1.m_buildings.m_buffer[(int) building].Info.m_subBuildings;
            if (subBuildings != null && subBuildings.Length != 0)
            {
              Vector3 position = instance1.m_buildings.m_buffer[(int) building].m_position;
              building = instance1.FindBuilding(position, 100f, ItemClass.Service.PublicTransport, ItemClass.SubService.None, Building.Flags.Untouchable, Building.Flags.None);
              if ((int) building != 0)
              {
                ushort[] stationStops = PanelExtenderCityService.GetStationStops(building);
                if (stationStops.Length != 0)
                  numArray = ((IEnumerable<ushort>) numArray).Concat<ushort>((IEnumerable<ushort>) stationStops).ToArray<ushort>();
              }
            }
            int length = numArray.Length;
            if (length > 0)
            {
              this._titleLabel.text = Localization.Get("CITY_SERVICE_PANEL_TITLE_STATION_STOPS");
              this._listBoxPanel.relativePosition = new Vector3(this._listBoxPanel.parent.width + 1f, VerticalOffset);
              this._listBoxPanel.Show();
              if ((int) this._cachedBuildingID != (int) building || this._cachedStopCount != length)
              {
                this._stopsListBox.ClearItems();
                for (int index = 0; index < length; ++index)
                  this._stopsListBox.AddItem(numArray[index], -1);
              }
            }
            else
              this._listBoxPanel.Hide();
            this._cachedStopCount = length;
            break;
          case ItemClass.SubService.PublicTransportTaxi:
          case ItemClass.SubService.PublicTransportCableCar:
            this._vehicleListBox.Show();
            this._stopsListBox.Hide();
            UIPanel uiPanel = this._cityServiceWorldInfoPanel.Find<UIPanel>("SvsVehicleTypes");
            if ((UnityEngine.Object) uiPanel != (UnityEngine.Object) null)
              this._listBoxPanel.relativePosition = new Vector3((float) ((double) this._listBoxPanel.parent.width + (double) uiPanel.width + 2.0), VerticalOffset);
            List<ushort> depotVehicles = PanelExtenderCityService.GetDepotVehicles(building);
            int count = depotVehicles.Count;
            if (count > 0)
            {
              this._titleLabel.text = Localization.Get("CITY_SERVICE_PANEL_TITLE_DEPOT_VEHICLES");
              this._listBoxPanel.Show();
              if ((int) this._cachedBuildingID != (int) building || this._cachedVehicleCount != count)
              {
                this._vehicleListBox.ClearItems();
                PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(service, subService, level);
                VehicleManager instance2 = Singleton<VehicleManager>.instance;
                foreach (ushort vehicleID in depotVehicles)
                {
                  VehicleInfo info = instance2.m_vehicles.m_buffer[(int) vehicleID].Info;
                  for (int index = 0; index < prefabs.Length; ++index)
                  {
                    PrefabData data = prefabs[index];
                    if (info.name == data.Name)
                    {
                      this._vehicleListBox.AddItem(data, vehicleID);
                      break;
                    }
                  }
                }
              }
            }
            else
              this._listBoxPanel.Hide();
            this._cachedVehicleCount = count;
            break;
          default:
            this._listBoxPanel.Hide();
            break;
        }
        this._cachedBuildingID = building;
      }
    }

    private void OnDestroy()
    {
      if (!((UnityEngine.Object) this._listBoxPanel != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this._listBoxPanel.gameObject);
    }

    private void CreateStopsPanel()
    {
      
      var parentHeight = 285f; //uiPanel.parent.height; broken due to autoformat
      
      UIPanel uiPanel = this._cityServiceWorldInfoPanel.component.AddUIComponent<UIPanel>();
      uiPanel.name = "ListBoxPanel";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3(uiPanel.parent.width + 1f, VerticalOffset);
      uiPanel.width = 180f;
      uiPanel.height = parentHeight - 16f;
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      this._listBoxPanel = uiPanel;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.size = new Vector2(180f, 20f);
      uiLabel.autoSize = false;
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = UIUtils.Font;
      uiLabel.position = new Vector3((float) ((double) uiPanel.width / 2.0 - (double) uiLabel.width / 2.0), (float) ((double) uiLabel.height / 2.0 - 20.0));
      this._titleLabel = uiLabel;
      StopListBox stopListBox = StopListBox.Create((UIComponent) uiPanel);
      stopListBox.name = "StationStopsListBox";
      stopListBox.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopLeft);
      stopListBox.relativePosition = new Vector3(3f, 40f);
      stopListBox.width = uiPanel.width - 6f;
      stopListBox.height = parentHeight - 61f;
      stopListBox.Font = UIUtils.Font;
      this._stopsListBox = stopListBox;
      VehicleListBox vehicleListBox = VehicleListBox.Create((UIComponent) uiPanel);
      vehicleListBox.name = "DepotVehiclesListBox";
      vehicleListBox.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopLeft);
      vehicleListBox.relativePosition = new Vector3(3f, 40f);
      vehicleListBox.width = uiPanel.width - 6f;
      vehicleListBox.height = parentHeight - 61f;
      vehicleListBox.Font = UIUtils.Font;
      this._vehicleListBox = vehicleListBox;
    }

    public static ushort[] GetStationStops(ushort buildingID)
    {
      List<ushort> stationStops = new List<ushort>();
      NetManager instance = Singleton<NetManager>.instance;
      ushort num1 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) buildingID].m_netNode;
      int num2 = 0;
      while ((int) num1 != 0)
      {
        if (instance.m_nodes.m_buffer[(int) num1].Info.m_class.m_layer != ItemClass.Layer.PublicTransport)
        {
          for (int index = 0; index < 8; ++index)
          {
            ushort segment = instance.m_nodes.m_buffer[(int) num1].GetSegment(index);
            if ((int) segment != 0 && (int) instance.m_segments.m_buffer[(int) segment].m_startNode == (int) num1)
              PanelExtenderCityService.CalculateLanes(instance.m_segments.m_buffer[(int) segment].m_lanes, ref stationStops);
          }
        }
        num1 = instance.m_nodes.m_buffer[(int) num1].m_nextBuildingNode;
        if (++num2 > 32768)
        {
          CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
          break;
        }
      }
      return stationStops.ToArray();
    }

    private static void CalculateLanes(uint firstLane, ref List<ushort> stationStops)
    {
      NetManager instance = Singleton<NetManager>.instance;
      uint num1 = firstLane;
      int num2 = 0;
      while ((int) num1 != 0)
      {
        ushort nodes = instance.m_lanes.m_buffer[(int) (uint) (UIntPtr) num1].m_nodes;
        if ((int) nodes != 0)
          PanelExtenderCityService.CalculateLaneNodes(nodes, ref stationStops);
        num1 = instance.m_lanes.m_buffer[(int) (uint) (UIntPtr) num1].m_nextLane;
        if (++num2 > 262144)
        {
          CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
          break;
        }
      }
    }

    private static void CalculateLaneNodes(ushort firstNode, ref List<ushort> stationStops)
    {
      NetManager instance = Singleton<NetManager>.instance;
      ushort num1 = firstNode;
      int num2 = 0;
      while ((int) num1 != 0)
      {
        if ((int) instance.m_nodes.m_buffer[(int) num1].m_transportLine != 0)
          stationStops.Add(num1);
        num1 = instance.m_nodes.m_buffer[(int) num1].m_nextLaneNode;
        if (++num2 > 32768)
        {
          CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
          break;
        }
      }
    }

    private static List<ushort> GetDepotVehicles(ushort buildingID)
    {
      List<ushort> ushortList = new List<ushort>();
      VehicleManager instance = Singleton<VehicleManager>.instance;
      int nextOwnVehicle;
      for (ushort index = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) buildingID].m_ownVehicles; (int) index != 0; index = (ushort) nextOwnVehicle)
      {
        nextOwnVehicle = (int) instance.m_vehicles.m_buffer[(int) index].m_nextOwnVehicle;
        ushortList.Add(index);
      }
      return ushortList;
    }
  }
}
