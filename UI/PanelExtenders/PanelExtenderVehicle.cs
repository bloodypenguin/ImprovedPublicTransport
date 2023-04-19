// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.PanelExtenderVehicle
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ImprovedPublicTransport2.HarmonyPatches.TransportLinePatches;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Query;
using ImprovedPublicTransport2.Data;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using UIUtils = ImprovedPublicTransport2.Util.UIUtils;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2.UI.PanelExtenders
{
  public class PanelExtenderVehicle : MonoBehaviour
  {
    private bool _initialized;
    private PublicTransportVehicleWorldInfoPanel _publicTransportVehicleWorldInfoPanel;
    private UIButton _editType;
    private UIPanel _passengerPanel;
    private UILabel _lastStopExchange;
    private UIPanel _statsPanel;
    private UILabel _passengersCurrentWeek;
    private UILabel _passengersLastWeek;
    private UILabel _passengersAverage;
    private UILabel _earningsCurrentWeek;
    private UILabel _earningsLastWeek;
    private UILabel _earningsAverage;
    private UIPanel _buttonPanel;
    private UILabel _status;
    private UIButton _target;
    private UILabel _distance;
    private UIProgressBar _distanceTraveled;
    private UILabel _distanceProgress;
    private FieldInfo _cachedCurrentProgress;
    private FieldInfo _cachedTotalProgress;
    private FieldInfo _cachedProgressVehicle;

    public void LateUpdate()
    {
      if (!this._initialized)
      {
        this.Init();
      }
      else
      {
        if (!this._initialized || !this._publicTransportVehicleWorldInfoPanel.component.isVisible)
          return;
        this.UpdateBindings();
      }
    }

    private void Init()
    {
      this._publicTransportVehicleWorldInfoPanel = GameObject.Find("(Library) PublicTransportVehicleWorldInfoPanel").GetComponent<PublicTransportVehicleWorldInfoPanel>();
      if (!((UnityEngine.Object) this._publicTransportVehicleWorldInfoPanel != (UnityEngine.Object) null))
        return;
      this._status = this._publicTransportVehicleWorldInfoPanel.Find<UILabel>("Status");
      this._target = this._publicTransportVehicleWorldInfoPanel.Find<UIButton>("Target");
      this._target.eventClick += new MouseEventHandler(this.OnTargetClick);
      this._distance = this._publicTransportVehicleWorldInfoPanel.Find<UILabel>("Distance");
      this._distanceTraveled = Utils.GetPrivate<UIProgressBar>((object) this._publicTransportVehicleWorldInfoPanel, "m_DistanceTraveled");
      this._distanceProgress = Utils.GetPrivate<UILabel>((object) this._publicTransportVehicleWorldInfoPanel, "m_DistanceProgress");
      this._cachedCurrentProgress = this._publicTransportVehicleWorldInfoPanel.GetType().GetField("m_cachedCurrentProgress", BindingFlags.Instance | BindingFlags.NonPublic);
      this._cachedTotalProgress = this._publicTransportVehicleWorldInfoPanel.GetType().GetField("m_cachedTotalProgress", BindingFlags.Instance | BindingFlags.NonPublic);
      this._cachedProgressVehicle = this._publicTransportVehicleWorldInfoPanel.GetType().GetField("m_cachedProgressVehicle", BindingFlags.Instance | BindingFlags.NonPublic);
      this.AddPanelControls();
      this._initialized = true;
    }

    private void UpdateBindings()
    {
      var lineId = WorldInfoCurrentLineIDQuery.Query(out var vehicleID);
      if ((int) lineId == 0)
      {
        this._passengerPanel.Hide();
        this._statsPanel.Hide();
        this._buttonPanel.Hide();
        this._publicTransportVehicleWorldInfoPanel.component.height = 229f;
      }
      else
      {
        this._publicTransportVehicleWorldInfoPanel.component.height = 377f;
        this._editType.isVisible = !OptionsWrapper<Settings.Settings>.Options.HideVehicleEditor;
          ItemClass itemClass = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineId].Info.m_class;
          ItemClass.SubService subService = itemClass.m_subService;
          ItemClass.Service service = itemClass.m_service;
          ItemClass.Level level = itemClass.m_level;

        switch (subService)
        {
          case ItemClass.SubService.PublicTransportBus:
          case ItemClass.SubService.PublicTransportTours:
          case ItemClass.SubService.PublicTransportMetro:
          case ItemClass.SubService.PublicTransportTrain:
          case ItemClass.SubService.PublicTransportTram:
          case ItemClass.SubService.PublicTransportShip:
          case ItemClass.SubService.PublicTransportPlane:
          case ItemClass.SubService.PublicTransportMonorail:
          case ItemClass.SubService.PublicTransportCableCar:
          case ItemClass.SubService.PublicTransportTrolleybus:
            this._passengerPanel.Show();
            if ((int) vehicleID != 0)
            {
              this._lastStopExchange.text = string.Format(Localization.Get("VEHICLE_PANEL_LAST_STOP_EXCHANGE"),
                  (object) CachedVehicleData.m_cachedVehicleData[(int) vehicleID].LastStopGonePassengers, 
                  (object) CachedVehicleData.m_cachedVehicleData[(int) vehicleID].LastStopNewPassengers);
            }
            break;
         case ItemClass.SubService.None:
             if (service == ItemClass.Service.Disaster && level == ItemClass.Level.Level4)
             {
                 this._passengerPanel.Show();
                 if ((int) vehicleID != 0)
                 {
                     this._lastStopExchange.text = string.Format(Localization.Get("VEHICLE_PANEL_LAST_STOP_EXCHANGE"),
                         (object) CachedVehicleData.m_cachedVehicleData[(int) vehicleID].LastStopGonePassengers,
                         (object) CachedVehicleData.m_cachedVehicleData[(int) vehicleID].LastStopNewPassengers);
                 }
             }
             else
             {
                 this._passengerPanel.Hide();
             }
             break;
          default:
            this._passengerPanel.Hide();
            break;
        }
        this._distanceTraveled.parent.Show();
        this._distanceProgress.parent.Show();
        VehicleManager vm = Singleton<VehicleManager>.instance;
        if ((vm.m_vehicles.m_buffer[(int) vehicleID].m_flags & Vehicle.Flags.Stopped) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        {
          if (CachedVehicleData.m_cachedVehicleData[(int) vehicleID].IsUnbunchingInProgress)
            this._status.text = Localization.Get("VEHICLE_PANEL_STATUS_UNBUNCHING");
          this._distance.text = this._status.text;
          var boardingTime = vm.m_vehicles.m_buffer[(int)vehicleID].Info?.m_vehicleType == VehicleInfo.VehicleType.Plane
            ? CanLeaveStopPatch.AirplaneBoardingTime
            : CanLeaveStopPatch.BoardingTime;
          
          var timeSinceBoardingFinished = vm.m_vehicles.m_buffer[vehicleID].m_waitCounter - boardingTime;
          float progress;
          if (timeSinceBoardingFinished <= 0)
          {
            progress = (boardingTime + timeSinceBoardingFinished) / (float)boardingTime;
          }
          else
          {
            var maxUnbunchingTime = (float) Mathf.Min(OptionsWrapper<Settings.Settings>.Options.IntervalAggressionFactor, CanLeaveStopPatch.MaxUnbunchingTime);
            progress = timeSinceBoardingFinished / maxUnbunchingTime;
          }
          this._distanceTraveled.progressColor = Color.green;
          this._distanceTraveled.value = progress;
          this._distanceProgress.text = LocaleFormatter.FormatPercentage( Mathf.RoundToInt(progress * 100f));
          
        }
        else
        {
          bool flag = true;
          string text = Localization.Get("VEHICLE_PANEL_STATUS_NEXT_STOP");
          if (subService == ItemClass.SubService.PublicTransportShip)
            this.UpdateProgress();
          else if (subService == ItemClass.SubService.PublicTransportPlane)
          {
            if ((vm.m_vehicles.m_buffer[(int) vehicleID].m_flags & Vehicle.Flags.Landing) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) || (vm.m_vehicles.m_buffer[(int) vehicleID].m_flags & Vehicle.Flags.TakingOff) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive) || (vm.m_vehicles.m_buffer[(int) vehicleID].m_flags & Vehicle.Flags.Flying) == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
            {
              text = this._status.text;
              flag = false;
            }
            this.UpdateProgress();
          }
          this._status.text = text;
          if (flag)
          {
            ushort targetBuilding = vm.m_vehicles.m_buffer[(int) vehicleID].m_targetBuilding;
            InstanceID id = new InstanceID();
            id.NetNode = targetBuilding;
            string name = Singleton<InstanceManager>.instance.GetName(id);
            this._target.objectUserData = (object) id;
            this._target.text = name == null ? string.Format(Localization.Get("STOP_LIST_BOX_ROW_STOP"), (object) (TransportLineUtil.GetStopIndex(lineId, targetBuilding) + 1)) : name;
            this._target.Enable();
            this._target.Show();
          }
          this._distance.text = ColossalFramework.Globalization.Locale.Get(this._distance.localeID);
          this._distanceTraveled.progressColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }
        this._statsPanel.Show();
        this._passengersCurrentWeek.text = CachedVehicleData.m_cachedVehicleData[(int) vehicleID].PassengersThisWeek.ToString();
        this._passengersLastWeek.text = CachedVehicleData.m_cachedVehicleData[(int) vehicleID].PassengersLastWeek.ToString();
        this._passengersAverage.text = CachedVehicleData.m_cachedVehicleData[(int) vehicleID].PassengersAverage.ToString();
        PrefabData prefabData = Array.Find(VehiclePrefabs.instance.GetPrefabs(service, subService, level), item => item.PrefabDataIndex == vm.m_vehicles.m_buffer[(int) vehicleID].Info.m_prefabDataIndex);
        int num1 = CachedVehicleData.m_cachedVehicleData[(int) vehicleID].IncomeThisWeek - prefabData.MaintenanceCost;
        UILabel earningsCurrentWeek = this._earningsCurrentWeek;
        float num2 = (float) num1 * 0.01f;
        string str1 = num2.ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
        earningsCurrentWeek.text = str1;
        this._earningsCurrentWeek.textColor = (Color32) this.GetColor((float) num1);
        int incomeLastWeek = CachedVehicleData.m_cachedVehicleData[(int) vehicleID].IncomeLastWeek;
        UILabel earningsLastWeek = this._earningsLastWeek;
        num2 = (float) incomeLastWeek * 0.01f;
        string str2 = num2.ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
        earningsLastWeek.text = str2;
        this._earningsLastWeek.textColor = (Color32) this.GetColor((float) incomeLastWeek);
        int incomeAverage = CachedVehicleData.m_cachedVehicleData[(int) vehicleID].IncomeAverage;
        UILabel earningsAverage = this._earningsAverage;
        num2 = (float) incomeAverage * 0.01f;
        string str3 = num2.ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
        earningsAverage.text = str3;
        this._earningsAverage.textColor = (Color32) this.GetColor((float) incomeAverage);
        this._buttonPanel.Show();
      }
    }

    private void OnDestroy()
    {
      if ((UnityEngine.Object) this._target != (UnityEngine.Object) null)
        this._target.eventClick -= new MouseEventHandler(this.OnTargetClick);
      if ((UnityEngine.Object) this._editType != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._editType.gameObject);
      if ((UnityEngine.Object) this._passengerPanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._passengerPanel.gameObject);
      if ((UnityEngine.Object) this._statsPanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._statsPanel.gameObject);
      if (!((UnityEngine.Object) this._buttonPanel != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this._buttonPanel.gameObject);
    }

    private void AddPanelControls()
    {
      UILabel uiLabel1 = Utils.GetPrivate<UILabel>((object) this._publicTransportVehicleWorldInfoPanel, "m_Type");
      _publicTransportVehicleWorldInfoPanel.Find<UIButton>("LinesOverview").parent.height = 20f;
      int num1 = 132;
      uiLabel1.anchor = (UIAnchorStyle) num1;
      UIPanel parent = (UIPanel) uiLabel1.parent;
      RectOffset rectOffset = new RectOffset(0, 10, 0, 0);
      parent.autoLayoutPadding = rectOffset;
      double num2 = 25.0;
      parent.height = (float) num2;
      int num3 = 1;
      parent.useCenter = num3 != 0;
      UIButton button1 = UIUtils.CreateButton((UIComponent) parent);
      button1.name = "EditType";
      button1.autoSize = true;
      button1.anchor = UIAnchorStyle.Left | UIAnchorStyle.Right | UIAnchorStyle.CenterVertical;
      button1.textPadding = new RectOffset(10, 10, 4, 2);
      button1.text = Localization.Get("VEHICLE_PANEL_EDIT_TYPE");
      button1.tooltip = string.Format(Localization.Get("VEHICLE_PANEL_EDIT_TYPE_TOOLTIP"));
      button1.textScale = 0.75f;
      button1.eventClick += new MouseEventHandler(this.OnEditTypeClick);
      button1.isVisible = !OptionsWrapper<Settings.Settings>.Options.HideVehicleEditor;
      this._editType = button1;
      UILabel uiLabel2 = Utils.GetPrivate<UILabel>((object) this._publicTransportVehicleWorldInfoPanel, "m_Passengers");
      UIPanel uiPanel1 = this._publicTransportVehicleWorldInfoPanel.component.Find<UIPanel>("Panel");
      UIPanel uiPanel2 = uiPanel1.AddUIComponent<UIPanel>();
      uiPanel2.autoLayout = true;
      uiPanel2.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel2.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      uiPanel2.height = uiLabel2.parent.height;
      uiPanel2.width = uiLabel2.parent.width;
      uiPanel2.zOrder = 4;
      this._passengerPanel = uiPanel2;
      UILabel uiLabel3 = uiPanel2.AddUIComponent<UILabel>();
      uiLabel3.name = "LastStopExchange";
      uiLabel3.font = uiLabel2.font;
      uiLabel3.textColor = uiLabel2.textColor;
      uiLabel3.textScale = uiLabel2.textScale;
      uiLabel3.processMarkup = true;
      this._lastStopExchange = uiLabel3;
      UIPanel uiPanel3 = uiPanel1.AddUIComponent<UIPanel>();
      uiPanel3.name = "PassengerStats";
      uiPanel3.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
      uiPanel3.autoLayout = true;
      uiPanel3.autoLayoutDirection = LayoutDirection.Vertical;
      uiPanel3.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      uiPanel3.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel3.size = new Vector2(349f, 60f);
      uiPanel3.zOrder = 5;
      this._statsPanel = uiPanel3;
      UILabel label1;
      UILabel label2;
      UILabel label3;
      UILabel label4;
      PublicTransportStopWorldInfoPanel.CreateStatisticRow((UIComponent) uiPanel3, out label1, out label2, out label3, out label4, true);
      label2.text = Localization.Get("CURRENT_WEEK");
      label3.text = Localization.Get("LAST_WEEK");
      label4.text = Localization.Get("AVERAGE");
      label4.tooltip = string.Format(Localization.Get("AVERAGE_TOOLTIP"), (object) OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
      PublicTransportStopWorldInfoPanel.CreateStatisticRow((UIComponent) uiPanel3, out label1, out this._passengersCurrentWeek, out this._passengersLastWeek, out this._passengersAverage, false);
      label1.text = Localization.Get("VEHICLE_PANEL_PASSENGERS");
      PublicTransportStopWorldInfoPanel.CreateStatisticRow((UIComponent) uiPanel3, out label1, out this._earningsCurrentWeek, out this._earningsLastWeek, out this._earningsAverage, false);
      label1.text = Localization.Get("VEHICLE_PANEL_EARNINGS");
      label1.tooltip = Localization.Get("VEHICLE_PANEL_EARNINGS_TOOLTIP");
      UIPanel uiPanel4 = uiPanel1.AddUIComponent<UIPanel>();
      uiPanel4.name = "Buttons";
      uiPanel4.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
      uiPanel4.autoLayout = true;
      uiPanel4.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel4.autoLayoutPadding = new RectOffset(0, 5, 0, 0);
      uiPanel4.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel4.size = new Vector2(345f, 32f);
      this._buttonPanel = uiPanel4;
      UIButton button2 = UIUtils.CreateButton((UIComponent) uiPanel4);
      button2.name = "PreviousVehicle";
      button2.textPadding = new RectOffset(10, 10, 4, 0);
      button2.text = Localization.Get("VEHICLE_PANEL_PREVIOUS");
      button2.tooltip = Localization.Get("VEHICLE_PANEL_PREVIOUS_TOOLTIP");
      button2.textScale = 0.75f;
      button2.size = new Vector2(110f, 32f);
      button2.wordWrap = true;
      button2.eventClick += new MouseEventHandler(this.OnChangeVehicleClick);
      UIButton button3 = UIUtils.CreateButton((UIComponent) uiPanel4);
      button3.name = "RemoveVehicle";
      button3.textPadding = new RectOffset(10, 10, 4, 0);
      button3.text = Localization.Get("VEHICLE_PANEL_REMOVE_VEHICLE");
      button3.textScale = 0.75f;
      button3.size = new Vector2(100f, 32f);
      button3.wordWrap = true;
      button3.hoveredTextColor = (Color32) Color.red;
      button3.focusedTextColor = (Color32) Color.red;
      button3.pressedTextColor = (Color32) Color.red;
      button3.eventClick += new MouseEventHandler(this.OnRemoveVehicleClick);
      UIButton button4 = UIUtils.CreateButton((UIComponent) uiPanel4);
      button4.name = "NextVehicle";
      button4.textPadding = new RectOffset(10, 10, 4, 0);
      button4.text = Localization.Get("VEHICLE_PANEL_NEXT");
      button4.tooltip = Localization.Get("VEHICLE_PANEL_NEXT_TOOLTIP");
      button4.textScale = 0.75f;
      button4.size = new Vector2(110f, 32f);
      button4.wordWrap = true;
      button4.eventClick += new MouseEventHandler(this.OnChangeVehicleClick);
    }

    private void OnEditTypeClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      if ((UnityEngine.Object) VehicleEditor.Instance == (UnityEngine.Object) null)
        return;
      InstanceID currentInstanceId = WorldInfoPanel.GetCurrentInstanceID();
      ushort firstVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) currentInstanceId.Vehicle].GetFirstVehicle(currentInstanceId.Vehicle);
      if ((int) firstVehicle == 0)
        return;
      VehicleInfo info = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) firstVehicle].Info;
      Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
      VehicleEditor.Instance.SetPrefab(info);
    }

    private void OnTargetClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      if (!((UnityEngine.Object) eventParam.source != (UnityEngine.Object) null))
        return;
      InstanceID objectUserData = (InstanceID) eventParam.source.objectUserData;
      if (objectUserData.Type != InstanceType.NetNode)
        return;
      PublicTransportStopWorldInfoPanel.instance.Show(Singleton<NetManager>.instance.m_nodes.m_buffer[(int) objectUserData.NetNode].m_position, objectUserData);
    }

    private void OnChangeVehicleClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      var lineId = WorldInfoCurrentLineIDQuery.Query(out var firstVehicle);
      if (lineId == 0)
        return;
      var num = component.name != "PreviousVehicle" ? TransportLineUtil.GetNextVehicle(lineId, firstVehicle) : TransportLineUtil.GetPreviousVehicle(lineId, firstVehicle);
      if (firstVehicle == (int) num)
        return;
      var instanceId = new InstanceID
      {
        Vehicle = num
      };
      WorldInfoPanel.ChangeInstanceID(WorldInfoPanel.GetCurrentInstanceID(), instanceId);
      ToolsModifierControl.cameraController.SetTarget(instanceId, ToolsModifierControl.cameraController.transform.position, Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift));
    }

    private void OnRemoveVehicleClick(UIComponent component, UIMouseEventParameter eventParam)
    {
        SimulationManager.instance.AddAction(() =>
        {
          var lineId = WorldInfoCurrentLineIDQuery.Query(out var firstVehicle);
            if (lineId == 0 || firstVehicle == 0)
                return;
            CachedTransportLineData.SetBudgetControlState(lineId, false);
            TransportLineUtil.RemoveVehicle(lineId, firstVehicle, true);
        });
    }

    private void UpdateProgress()
    {
      VehicleManager instance = Singleton<VehicleManager>.instance;
      ushort vehicle = WorldInfoPanel.GetCurrentInstanceID().Vehicle;
      ushort firstVehicle = instance.m_vehicles.m_buffer[(int) vehicle].GetFirstVehicle(vehicle);
      float current;
      float max;
      if (GetProgressStatus(firstVehicle, ref instance.m_vehicles.m_buffer[(int) firstVehicle], out current, out max) || (int) firstVehicle != (int) (ushort) this._cachedProgressVehicle.GetValue((object) this._publicTransportVehicleWorldInfoPanel))
      {
        this._cachedCurrentProgress.SetValue((object) this._publicTransportVehicleWorldInfoPanel, (object) current);
        this._cachedTotalProgress.SetValue((object) this._publicTransportVehicleWorldInfoPanel, (object) max);
        this._cachedProgressVehicle.SetValue((object) this._publicTransportVehicleWorldInfoPanel, (object) firstVehicle);
      }
      else
      {
        current = (float) this._cachedCurrentProgress.GetValue((object) this._publicTransportVehicleWorldInfoPanel);
        max = (float) this._cachedTotalProgress.GetValue((object) this._publicTransportVehicleWorldInfoPanel);
      }
      if ((double) max == 0.0)
        return;
      this._distanceTraveled.parent.Show();
      this._distanceProgress.parent.Show();
      float num = current / max;
      int p = Mathf.RoundToInt(num * 100f);
      this._distanceTraveled.value = num;
      this._distanceProgress.text = LocaleFormatter.FormatPercentage(p);
    }

    public static bool GetProgressStatus(ushort vehicleID, ref Vehicle data, out float current, out float max)
    {
        ushort transportLine = data.m_transportLine;
        ushort targetBuilding = data.m_targetBuilding;
        if ((int) transportLine != 0 && (int) targetBuilding != 0)
        {
            float min;
            float max1;
            float total;
            Singleton<TransportManager>.instance.m_lines.m_buffer[(int) transportLine]
                .GetStopProgress(targetBuilding, out min, out max1, out total);
            uint path = data.m_path;
            bool valid;
            if ((int) path == 0 || (data.m_flags & Vehicle.Flags.WaitingPath) !=
                ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted |
                    Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                    Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                    Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                    Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                    Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                    Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting |
                    Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel |
                    Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                    Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding |
                    Vehicle.Flags.LeftHandDrive))
            {
                current = min;
                valid = false;
            }
            else
                current = BusAI.GetPathProgress(path, (int) data.m_pathPositionIndex, min, max1, out valid);
            max = total;
            return valid;
        }
        current = 0.0f;
        max = 0.0f;
        return true;
    }

    private Color GetColor(float value)
    {
      if ((double) value >= 0.0)
        return Color.green;
      return Color.red;
    }
  }
}
