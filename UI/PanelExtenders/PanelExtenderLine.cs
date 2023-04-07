// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.PanelExtenderLine
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Query;
using ImprovedPublicTransport2.UI.DontCryJustDieCommons;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using UIUtils = ImprovedPublicTransport2.Util.UIUtils;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2.UI.PanelExtenders
{
  public class PanelExtenderLine : MonoBehaviour
  {
    
    private const float PARENT_HEIGHT = 285f;
    
    private bool _initialized;
    private ushort _cachedLineID;
    private ItemClass.SubService _cachedSubService;
    private Dictionary<ItemClassTriplet, bool> _updateDepots;
    private int _cachedSimCount;
    private int _cachedQueuedCount;
    private int _cachedStopCount;
    private PublicTransportWorldInfoPanel _publicTransportWorldInfoPanel;
    private UIComponent _mainSubPanel;
    private UIPanel _iptContainer;
    private UIColorField _colorField;
    private UILabel _vehicleAmount;
    private UILabel _stopCountLabel;
    private UILabel _spawnTimer;
    private UICheckBox _budgetControl;
    private UICheckBox _unbunching;
    private DropDown _depotDropDown;

    private VehicleListBox _lineVehicleListBox;
    private VehicleListBox _vehiclesInQueueListBox;
    private PrefabPanel _prefabPanel;

    private UIButton _selectTypes;
    private UIButton _addVehicle;
    
    private UIPanel _lineVehiclePanel;
    private UIPanel _vehiclesInQueuePanel;

    private UITextField _colorTextField;

    public PanelExtenderLine()
    {
      Dictionary<ItemClassTriplet, bool> dictionary = new Dictionary<ItemClassTriplet, bool>();
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportBus,
            ItemClass.Level.Level1), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain,
            ItemClass.Level.Level1), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain,
          ItemClass.Level.Level2), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMetro,
            ItemClass.Level.Level1), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane,
            ItemClass.Level.Level1), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane,
            ItemClass.Level.Level2), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane,
          ItemClass.Level.Level3), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip,
            ItemClass.Level.Level1), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip,
            ItemClass.Level.Level2), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTram,
            ItemClass.Level.Level1), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMonorail,
            ItemClass.Level.Level1), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTours,
            ItemClass.Level.Level3), true);
        dictionary.Add(new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrolleybus,
          ItemClass.Level.Level1), true);
        _updateDepots = dictionary;
    }

    private void Update()
    {
      if (!_initialized)
      {
        Init();
      }
      else
      {
        if (!_initialized || !_publicTransportWorldInfoPanel.component.isVisible)
          return;
        UpdateBindings();
        var lineModelSelector = _publicTransportWorldInfoPanel.Find<UIPanel>("LineModelSelectorContainer");
        if (lineModelSelector != null)
        {
          lineModelSelector.isVisible = false;
        }
      }
    }

    private void Init()
    {
      _publicTransportWorldInfoPanel = GameObject.Find("(Library) PublicTransportWorldInfoPanel").GetComponent<PublicTransportWorldInfoPanel>();
      if (!(_publicTransportWorldInfoPanel != null))
        return;
      UIComponent passengers = _publicTransportWorldInfoPanel.Find("Passengers");
    passengers.parent.relativePosition = new Vector3(passengers.parent.relativePosition.x,
        76.0f, passengers.parent.relativePosition.z);
        UIComponent agePanel = _publicTransportWorldInfoPanel.Find("AgePanel");
        agePanel.relativePosition = new Vector3(0.0f,
            84.0f, agePanel.relativePosition.z);
        UIComponent tripSaved = _publicTransportWorldInfoPanel.Find("TripSaved");
        tripSaved.parent.relativePosition = new Vector3(tripSaved.parent.relativePosition.x,
            200.0f, tripSaved.parent.relativePosition.z);


      UIComponent uiComponent1 = _publicTransportWorldInfoPanel.Find("DeleteLine");
      if (uiComponent1 == null)
      {
        Utils.LogError("Could not found Delete button!");
      }
      else
      {
        uiComponent1.isVisible = false;
          UIComponent vehicleAmount = _publicTransportWorldInfoPanel.Find("VehicleAmount");
          vehicleAmount.AlignTo(uiComponent1.parent, UIAlignAnchor.TopLeft);
          vehicleAmount.relativePosition = uiComponent1.relativePosition;


                UIComponent uiComponent2 = _publicTransportWorldInfoPanel.Find("LinesOverview");
        if (uiComponent2 != null)
          uiComponent2.enabled = false;
          _mainSubPanel = agePanel.parent;
        if (_mainSubPanel == null)
        {
          Utils.LogError("Could not found Panel!");
        }
        else
        {
          UIPanel uiPanel = _mainSubPanel.AddUIComponent<UIPanel>();
          uiPanel.name = "IptContainer";
          uiPanel.width = 301f;
          uiPanel.height = 166f;
          uiPanel.autoLayoutDirection = LayoutDirection.Vertical;
          uiPanel.autoLayoutStart = LayoutStart.TopLeft;
          uiPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 5);
          uiPanel.autoLayout = true;
          uiPanel.relativePosition = new Vector3(10f, 224.0f);
          _iptContainer = uiPanel;
          _vehicleAmount = Utils.GetPrivate<UILabel>(_publicTransportWorldInfoPanel, "m_VehicleAmount");
          if (_vehicleAmount == null)
          {
            Utils.LogError("Could not found m_VehicleAmount!");
          }
          else
          {
            (_vehicleAmount.parent as UIPanel).autoLayoutPadding = new RectOffset(0, 10, 0, 0);
            UILabel uiLabel = _vehicleAmount.parent.AddUIComponent<UILabel>();
            uiLabel.name = "StopCount";
            uiLabel.font = _vehicleAmount.font;
            uiLabel.textColor = _vehicleAmount.textColor;
            uiLabel.textScale = _vehicleAmount.textScale;
            uiLabel.eventMouseEnter += OnMouseEnter;
            _stopCountLabel = uiLabel;
            _colorField = Utils.GetPrivate<UIColorField>(_publicTransportWorldInfoPanel, "m_ColorField");
            if (_colorField == null)
            {
              Utils.LogError("Could not found m_ColorField!");
            }
            else
            {
              UITextField uiTextField = _colorField.parent.AddUIComponent<UITextField>();
              uiTextField.name = "ColorTextField";
              uiTextField.text = "000000";
              uiTextField.textColor = Color.black;
              uiTextField.textScale = 0.7f;
              uiTextField.selectionSprite = "EmptySprite";
              uiTextField.normalBgSprite = "TextFieldPanelHovered";
              uiTextField.focusedBgSprite = "TextFieldPanel";
              uiTextField.builtinKeyNavigation = true;
              uiTextField.submitOnFocusLost = true;
              uiTextField.eventTextSubmitted += OnColorTextSubmitted;
              uiTextField.width = 50f;
              uiTextField.height = 23f;
              uiTextField.maxLength = 6;
              uiTextField.verticalAlignment = UIVerticalAlignment.Middle;
              uiTextField.padding = new RectOffset(0, 0, 8, 0);
              uiTextField.relativePosition = new Vector3(135f, 0.0f);
              _colorTextField = uiTextField;
              _colorField.eventSelectedColorReleased += OnColorChanged;
              CreateSpawnTimerPanel();
              CreateBudgetControlPanel();
              CreateUnbunchingPanel();
              CreateDropDownPanel();
              CreateButtonPanel1();
              CreateButtonPanel2();
              _publicTransportWorldInfoPanel.component.height = 493f;
              CreatePrefabPanel(uiPanel);
              CreateVehiclesOnLinePanel();
              CreateVehiclesInQueuePanel();
              BuildingExtension.OnDepotAdded += OnDepotChanged;
              BuildingExtension.OnDepotRemoved += OnDepotChanged;
              _initialized = true;
            }
          }
        }
      }
    }

    private void UpdateBindings()
    {
      ushort lineId = GetLineID();
      if (lineId != 0)
      {
        int lineVehicleCount = TransportLineUtil.CountLineActiveVehicles(lineId, out int _);
        int targetVehicleCount = CachedTransportLineData.GetTargetVehicleCount(lineId);
        int num1 = Singleton<TransportManager>.instance.m_lines.m_buffer[lineId].CountStops(lineId);
        _vehicleAmount.text = LocaleFormatter.FormatGeneric("TRANSPORT_LINE_VEHICLECOUNT", lineVehicleCount + " / " + targetVehicleCount);
        _stopCountLabel.text = string.Format(Localization.Get("LINE_PANEL_STOPS"), num1);
        _budgetControl.isChecked = CachedTransportLineData.GetBudgetControlState(lineId);

        if (OptionsWrapper<Settings.Settings>.Options.IntervalAggressionFactor == 0)
        {
          _unbunching.Disable();
          _unbunching.isChecked = false;
          _unbunching.label.text = Localization.Get("UNBUNCHING_DISABLED");
        }
        else
        {
          _unbunching.Enable();
          _unbunching.isChecked = CachedTransportLineData.GetUnbunchingState(lineId);

          if (targetVehicleCount > 1)
          {
            _unbunching.label.text = string.Format(Localization.Get("UNBUNCHING_ENABLED") + " - " + Localization.Get("UNBUNCHING_TARGET_GAP"), Singleton<TransportManager>.instance.m_lines.m_buffer[lineId].m_averageInterval) ;
          }
          else
            _unbunching.label.text = Localization.Get("UNBUNCHING_ENABLED");
        }
        bool depotNotValid = false;
        ushort depotID = CachedTransportLineData.GetDepot(lineId);
          TransportInfo info = TransportManager.instance.m_lines.m_buffer[lineId].Info;
          if (!DepotUtil.IsValidDepot(depotID, info))
          {
              depotNotValid = true;
          }
          bool depotCanAddVehicle = true;
        if (depotID != 0)
          depotCanAddVehicle = DepotUtil.CanAddVehicle(depotID, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[depotID], info);
        if (depotCanAddVehicle)
        {
          var currentlyDisabled = SimulationManager.instance.m_isNightTime
            ? (Singleton<TransportManager>.instance.m_lines.m_buffer[lineId].m_flags & TransportLine.Flags.DisabledNight) != TransportLine.Flags.None 
            : (Singleton<TransportManager>.instance.m_lines.m_buffer[lineId].m_flags & TransportLine.Flags.DisabledDay) != TransportLine.Flags.None;

          if (currentlyDisabled || lineVehicleCount >= targetVehicleCount)
          {
            _spawnTimer.text = string.Format(Localization.Get("LINE_PANEL_SPAWNTIMER"), "∞");
          }
          else
          {
            var timeToNext = Mathf.Max(0, Mathf.CeilToInt(CachedTransportLineData.GetNextSpawnTime(lineId) - SimHelper.SimulationTime)); 
            _spawnTimer.text = string.Format(Localization.Get("LINE_PANEL_SPAWNTIMER"), "≥" + timeToNext);
          } 
        }
        else
          _spawnTimer.text = Localization.Get("LINE_PANEL_DEPOT_WARNING");
        _selectTypes.isEnabled = !depotNotValid;
        _addVehicle.isEnabled = !depotNotValid & depotCanAddVehicle;

        ItemClass.SubService subService = info.GetSubService();
        ItemClass.Service service = info.GetService();
        ItemClass.Level level = info.GetClassLevel();
                ItemClassTriplet triplet = new ItemClassTriplet(service, subService,  level);

        if (subService != _cachedSubService || depotNotValid || _updateDepots[triplet])
        {
          PopulateDepotDropDown(info);
          // PopulatePrefabListBox(service, subService, level);
          _updateDepots[triplet] = false;
        }
        if (_depotDropDown.Items.Length == 0)
          _depotDropDown.Text = Localization.Get("LINE_PANEL_NO_DEPOT_FOUND");
        else
          _depotDropDown.SelectedItem = depotID;
        if (lineId != _cachedLineID)
        {
          _colorTextField.text = ColorUtility.ToHtmlStringRGB(_colorField.selectedColor);
          _prefabPanel.Hide();
          UpdatePanelPositionAndSize();
        }
        if (lineVehicleCount != 0)
        {
          // if (lineId != _cachedLineID || lineVehicleCount != _cachedSimCount)
          // PopulateLineVehicleListBox(lineId, service, subService, level);
          _lineVehiclePanel.Show();
          UpdatePanelPositionAndSize();
        }
        else
        {
          _lineVehiclePanel.Hide();
          _lineVehicleListBox.ClearItems();
        }
        int num3 = CachedTransportLineData.EnqueuedVehiclesCount(lineId);
        if ((uint) num3 > 0U & depotCanAddVehicle)
        {
          // if (lineId != _cachedLineID || num3 != _cachedQueuedCount)
          //   PopulateQueuedVehicleListBox(lineId, service, subService, level);
          if (_vehiclesInQueueListBox.Items.Count == 0)
            _vehiclesInQueuePanel.Hide();
          else
            _vehiclesInQueuePanel.Show();
          UpdatePanelPositionAndSize();
        }
        else
        {
          _vehiclesInQueuePanel.Hide();
          _vehiclesInQueueListBox.ClearItems();
        }
        _cachedLineID = lineId;
        _cachedSubService = subService;
        _cachedSimCount = lineVehicleCount;
        _cachedQueuedCount = num3;
        _cachedStopCount = num1;
      }
      else
        _publicTransportWorldInfoPanel.Hide();
    }

    private void UpdatePanelPositionAndSize()
    {
      float num = 0.0f;
      // if (_prefabPanel.isVisible)
      //   num = _prefabPanel.width + 1f;
      _lineVehiclePanel.relativePosition = new Vector3((float) (num + (double) _lineVehiclePanel.parent.width + 1.0), 0.0f);
      if (_lineVehiclePanel.isVisible)
      {
        _vehiclesInQueuePanel.relativePosition = new Vector3((float) (num + (double) _vehiclesInQueuePanel.parent.width + 1.0), _lineVehiclePanel.height + 1f);
        _vehiclesInQueuePanel.height = (float) ((PARENT_HEIGHT - 16.0) * 0.5);
        _vehiclesInQueueListBox.height = 162f;
      }
      else
      {
        _vehiclesInQueuePanel.relativePosition = new Vector3((float) (num + (double) _vehiclesInQueuePanel.parent.width + 1.0), 0.0f);
        _vehiclesInQueuePanel.height = PARENT_HEIGHT - 16f;
        _vehiclesInQueueListBox.height = PARENT_HEIGHT - 61f;
      }
      if (_vehiclesInQueuePanel.isVisible)
      {
        _lineVehiclePanel.height = (float) ((PARENT_HEIGHT - 16.0) * 0.5);
        _lineVehicleListBox.height = 162f;
      }
      else
      {
        _lineVehiclePanel.height = PARENT_HEIGHT - 16f;
        _lineVehicleListBox.height = PARENT_HEIGHT - 61f;
      }
    }

    private void OnDestroy()
    {
      _initialized = false;
      BuildingExtension.OnDepotAdded -= OnDepotChanged;
      BuildingExtension.OnDepotRemoved -= OnDepotChanged;
      if (_updateDepots != null)
        _updateDepots.Clear();
      if (_colorTextField != null)
      {
        _colorField.eventSelectedColorReleased -= OnColorChanged;
        Destroy(_colorTextField.gameObject);
      }
      if (_stopCountLabel != null)
        Destroy(_stopCountLabel.gameObject);
      if (_iptContainer != null)
        Destroy(_iptContainer.gameObject);
      if (_prefabPanel != null)
        Destroy(_prefabPanel.gameObject);
      if (_lineVehiclePanel != null)
        Destroy(_lineVehiclePanel.gameObject);
      if (_vehiclesInQueuePanel != null)
        Destroy(_vehiclesInQueuePanel.gameObject);
    }

    private void CreateSpawnTimerPanel()
    {
      UIPanel uiPanel = _iptContainer.AddUIComponent<UIPanel>();
      double width = uiPanel.parent.width;
      uiPanel.width = (float) width;
      double num1 = 14.0;
      uiPanel.height = (float) num1;
      int num2 = 0;
      uiPanel.autoLayoutDirection = (LayoutDirection) num2;
      int num3 = 0;
      uiPanel.autoLayoutStart = (LayoutStart) num3;
      RectOffset rectOffset = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayoutPadding = rectOffset;
      int num4 = 1;
      uiPanel.autoLayout = num4 != 0;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.font = _vehicleAmount.font;
      uiLabel.textColor = _vehicleAmount.textColor;
      uiLabel.textScale = _vehicleAmount.textScale;
      uiLabel.processMarkup = true;
      _spawnTimer = uiLabel;
    }

    private void CreateBudgetControlPanel()
    {
      UIPanel uiPanel = _iptContainer.AddUIComponent<UIPanel>();
      uiPanel.width = uiPanel.parent.width;
      uiPanel.height = 16f;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayout = true;
      UICheckBox uiCheckBox = uiPanel.AddUIComponent<UICheckBox>();
      uiCheckBox.size = uiPanel.size;
      uiCheckBox.clipChildren = true;
      uiCheckBox.tooltip = Localization.Get("LINE_PANEL_BUDGET_CONTROL_TOOLTIP") + Environment.NewLine + Localization.Get("EXPLANATION_BUDGET_CONTROL");
      uiCheckBox.eventClicked += OnBudgetControlClick;
      UISprite uiSprite = uiCheckBox.AddUIComponent<UISprite>();
      uiSprite.spriteName = "check-unchecked";
      uiSprite.size = new Vector2(16f, 16f);
      uiSprite.relativePosition = Vector3.zero;
      uiCheckBox.checkedBoxObject = uiSprite.AddUIComponent<UISprite>();
      ((UISprite) uiCheckBox.checkedBoxObject).spriteName = "check-checked";
      uiCheckBox.checkedBoxObject.size = new Vector2(16f, 16f);
      uiCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
      uiCheckBox.label = uiCheckBox.AddUIComponent<UILabel>();
      uiCheckBox.label.text = Localization.Get("LINE_PANEL_BUDGET_CONTROL");
      uiCheckBox.label.font = _vehicleAmount.font;
      uiCheckBox.label.textColor = _vehicleAmount.textColor;
      uiCheckBox.label.textScale = _vehicleAmount.textScale;
      uiCheckBox.label.relativePosition = new Vector3(22f, 2f);
      _budgetControl = uiCheckBox;
    }

    private void CreateUnbunchingPanel()
    {
      UIPanel uiPanel = _iptContainer.AddUIComponent<UIPanel>();
      uiPanel.width = uiPanel.parent.width;
      uiPanel.height = 16f;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayout = true;
      UICheckBox uiCheckBox = uiPanel.AddUIComponent<UICheckBox>();
      uiCheckBox.size = uiPanel.size;
      uiCheckBox.clipChildren = true;
      uiCheckBox.tooltip = Localization.Get("LINE_PANEL_UNBUNCHING_TOOLTIP") + Environment.NewLine + Localization.Get("EXPLANATION_UNBUNCHING");
      uiCheckBox.eventClicked += OnUnbunchingClick;
      UISprite uiSprite = uiCheckBox.AddUIComponent<UISprite>();
      uiSprite.spriteName = "check-unchecked";
      uiSprite.size = new Vector2(16f, 16f);
      uiSprite.relativePosition = Vector3.zero;
      uiCheckBox.checkedBoxObject = uiSprite.AddUIComponent<UISprite>();
      ((UISprite) uiCheckBox.checkedBoxObject).spriteName = "check-checked";
      uiCheckBox.checkedBoxObject.size = new Vector2(16f, 16f);
      uiCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
      uiCheckBox.label = uiCheckBox.AddUIComponent<UILabel>();
      uiCheckBox.label.font = _vehicleAmount.font;
      uiCheckBox.label.textColor = _vehicleAmount.textColor;
      uiCheckBox.label.disabledTextColor = Color.black;
      uiCheckBox.label.textScale = _vehicleAmount.textScale;
      uiCheckBox.label.relativePosition = new Vector3(22f, 2f);
      _unbunching = uiCheckBox;
    }

    private void CreateDropDownPanel()
    {
      UIPanel uiPanel = _iptContainer.AddUIComponent<UIPanel>();
      uiPanel.width = uiPanel.parent.width;
      uiPanel.height = 27f;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayout = true;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      string str1 = Localization.Get("LINE_PANEL_DEPOT");
      uiLabel.text = str1;
      UIFont font = _vehicleAmount.font;
      uiLabel.font = font;
      Color32 textColor = _vehicleAmount.textColor;
      uiLabel.textColor = textColor;
      double textScale = _vehicleAmount.textScale;
      uiLabel.textScale = (float) textScale;
      int num1 = 0;
      uiLabel.autoSize = num1 != 0;
      double num2 = 27.0;
      uiLabel.height = (float) num2;
      double num3 = 97.0;
      uiLabel.width = (float) num3;
      int num4 = 1;
      uiLabel.verticalAlignment = (UIVerticalAlignment) num4;
      _depotDropDown = DropDown.Create(uiPanel);
      _depotDropDown.name = "DepotDropDown";
      _depotDropDown.Font = _vehicleAmount.font;
      _depotDropDown.height = 27f;
      _depotDropDown.width = 167f;
      _depotDropDown.DropDownPanelAlignParent = _publicTransportWorldInfoPanel.component;
      _depotDropDown.eventSelectedItemChanged += OnSelectedDepotChanged;
      UIButton uiButton = uiPanel.AddUIComponent<UIButton>();
      string str2 = "DepotMarker";
      uiButton.name = str2;
      string str3 = "LocationMarkerNormal";
      uiButton.normalBgSprite = str3;
      string str4 = "LocationMarkerDisabled";
      uiButton.disabledBgSprite = str4;
      string str5 = "LocationMarkerHovered";
      uiButton.hoveredBgSprite = str5;
      string str6 = "LocationMarkerFocused";
      uiButton.focusedBgSprite = str6;
      string str7 = "LocationMarkerPressed";
      uiButton.pressedBgSprite = str7;
      Vector2 vector2 = new Vector2(27f, 27f);
      uiButton.size = vector2;
      string str8 = Localization.Get("LINE_PANEL_DEPOT_MARKER_TOOLTIP");
      uiButton.tooltip = str8;
      MouseEventHandler mouseEventHandler = OnDepotMarkerClicked;
      uiButton.eventClick += mouseEventHandler;
    }

    private void CreateButtonPanel1()
    {
      UIPanel uiPanel = _iptContainer.AddUIComponent<UIPanel>();
      double width = uiPanel.parent.width;
      uiPanel.width = (float) width;
      double num1 = 32.0;
      uiPanel.height = (float) num1;
      int num2 = 0;
      uiPanel.autoLayoutDirection = (LayoutDirection) num2;
      int num3 = 0;
      uiPanel.autoLayoutStart = (LayoutStart) num3;
      RectOffset rectOffset = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayoutPadding = rectOffset;
      int num4 = 1;
      uiPanel.autoLayout = num4 != 0;
      UIButton button1 = UIUtils.CreateButton(uiPanel);
      button1.name = "SelectTypes";
      button1.textPadding = new RectOffset(10, 10, 4, 0);
      button1.text = Localization.Get("LINE_PANEL_SELECT_TYPES");
      button1.tooltip = Localization.Get("LINE_PANEL_SELECT_TYPES_TOOLTIP");
      button1.textScale = 0.8f;
      button1.width = 97f;
      button1.height = 32f;
      button1.wordWrap = true;
      button1.eventClick += (c, p) =>
      {
        if (_depotDropDown.SelectedItem <= 0)
          return;
        _prefabPanel.isVisible = !_prefabPanel.isVisible;
        if (_prefabPanel.isVisible)
        {
          _prefabPanel.SetTarget(_cachedLineID);
        }
      };
      _selectTypes = button1;
      UIButton button2 = UIUtils.CreateButton(uiPanel);
      button2.name = "AddVehicle";
      button2.textPadding = new RectOffset(10, 10, 4, 0);
      button2.text = Localization.Get("LINE_PANEL_ADD_VEHICLE");
      button2.textScale = 0.8f;
      button2.tooltip = Localization.Get("LINE_PANEL_ADD_VEHICLE_TOOLTIP");
      button2.width = 97f;
      button2.height = 32f;
      button2.wordWrap = true;
      button2.eventClick += OnAddVehicleClick;
      _addVehicle = button2;
      UIButton button3 = UIUtils.CreateButton(uiPanel);
      button3.name = "RemoveVehicle";
      button3.textPadding = new RectOffset(10, 10, 4, 0);
      button3.text = Localization.Get("LINE_PANEL_REMOVE_VEHICLE");
      button3.textScale = 0.8f;
      button3.width = 97f;
      button3.height = 32f;
      button3.wordWrap = true;
      button3.eventClick += OnRemoveVehicleClick;
    }

    private void CreateButtonPanel2()
    {
      UIPanel uiPanel = _iptContainer.AddUIComponent<UIPanel>();
      double width = uiPanel.parent.width;
      uiPanel.width = (float) width;
      double num1 = 22.0;
      uiPanel.height = (float) num1;
      int num2 = 0;
      uiPanel.autoLayoutDirection = (LayoutDirection) num2;
      int num3 = 0;
      uiPanel.autoLayoutStart = (LayoutStart) num3;
      RectOffset rectOffset = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayoutPadding = rectOffset;
      int num4 = 1;
      uiPanel.autoLayout = num4 != 0;
      float num5 = (float) ((uiPanel.parent.width - 4.0) / 2.0);
      UIButton button1 = UIUtils.CreateButton(uiPanel);
      string str1 = "VEHICLE_LINESOVERVIEW";
      button1.localeID = str1;
      double num6 = 0.800000011920929;
      button1.textScale = (float) num6;
      double num7 = num5;
      button1.width = (float) num7;
      double num8 = 22.0;
      button1.height = (float) num8;
      MouseEventHandler mouseEventHandler1 = (c, p) => _publicTransportWorldInfoPanel.OnLinesOverviewClicked();
      button1.eventClick += mouseEventHandler1;
      UIButton button2 = UIUtils.CreateButton(uiPanel);
      string str2 = "LINE_DELETE";
      button2.localeID = str2;
      double num9 = 0.800000011920929;
      button2.textScale = (float) num9;
      double num10 = num5;
      button2.width = (float) num10;
      double num11 = 22.0;
      button2.height = (float) num11;
      MouseEventHandler mouseEventHandler2 = OnDeleteLineClick;
      button2.eventClick += mouseEventHandler2;
    }

    private void CreatePrefabPanel(UIPanel uiPanel)
    {
      _prefabPanel = _publicTransportWorldInfoPanel.component.AddUIComponent<PrefabPanel>();
      _prefabPanel.name = "PrefabPanel";
      _prefabPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
    }

    private void CreateVehiclesOnLinePanel()
    {
      UIPanel uiPanel = _publicTransportWorldInfoPanel.component.AddUIComponent<UIPanel>();
      uiPanel.name = "VehiclesOnLine";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3((float) (uiPanel.parent.width + /*(double) _prefabPanel.width */+ 180f + 3.0), 0.0f);
      uiPanel.width = 180f;
      uiPanel.height = (float) ((PARENT_HEIGHT - 16.0) / 2.0);
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      _lineVehiclePanel = uiPanel;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.text = Localization.Get("LINE_PANEL_LINE_VEHICLES");
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = _vehicleAmount.font;
      uiLabel.position = new Vector3((float) (uiPanel.width / 2.0 - uiLabel.width / 2.0), (float) (uiLabel.height / 2.0 - 20.0));
      VehicleListBox vehicleListBox = VehicleListBox.Create(uiPanel);
      vehicleListBox.name = "VehicleListBox";
      vehicleListBox.AlignTo(uiPanel, UIAlignAnchor.TopLeft);
      vehicleListBox.relativePosition = new Vector3(3f, 40f);
      vehicleListBox.width = uiPanel.width - 6f;
      vehicleListBox.height = 162f;
      vehicleListBox.Font = _vehicleAmount.font;
      _lineVehicleListBox = vehicleListBox;
    }

    private void CreateVehiclesInQueuePanel()
    {
      UIPanel uiPanel = _publicTransportWorldInfoPanel.component.AddUIComponent<UIPanel>();
      uiPanel.name = "VehiclesInQueue";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3(uiPanel.parent.width + 1f, _lineVehiclePanel.height + 1f);
      uiPanel.width = 180f;
      uiPanel.height = (float) ((PARENT_HEIGHT - 16.0) / 2.0);
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      _vehiclesInQueuePanel = uiPanel;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.text = Localization.Get("LINE_PANEL_ENQUEUED");
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = _vehicleAmount.font;
      uiLabel.position = new Vector3((float) (uiPanel.width / 2.0 - uiLabel.width / 2.0), (float) (uiLabel.height / 2.0 - 20.0));
      VehicleListBox vehicleListBox = VehicleListBox.Create(uiPanel);
      vehicleListBox.name = "VehicleListBox";
      vehicleListBox.AlignTo(uiPanel, UIAlignAnchor.TopLeft);
      vehicleListBox.relativePosition = new Vector3(3f, 40f);
      vehicleListBox.width = uiPanel.width - 6f;
      vehicleListBox.height = 162f;
      vehicleListBox.Font = _vehicleAmount.font;
      _vehiclesInQueueListBox = vehicleListBox;
    }

    private void OnMouseEnter(UIComponent component, UIMouseEventParameter p)
    {
      ushort lineId = GetLineID();
      if (lineId == 0)
        return;
      TransportLine transportLine = Singleton<TransportManager>.instance.m_lines.m_buffer[lineId];
      ushort num1 = transportLine.m_stops;
      int num2 = 0;
      for (int index = 0; index < transportLine.CountStops(lineId); ++index)
      {
        num2 += WaitingPassengerCountQuery.Query(num1, out var nextStop, out _);
        num1 = nextStop;
      }
      component.tooltip = string.Format(Localization.Get("LINE_PANEL_TOTAL_WAITING_PEOPLE_TOOLTIP"), num2);
    }

    private void OnBudgetControlClick(UIComponent component, UIMouseEventParameter p)
    {
        SimulationManager.instance.AddAction(() =>
        {
            ushort lineId = GetLineID();
            if (lineId == 0)
                return;
            bool budgetControlState = CachedTransportLineData.GetBudgetControlState(lineId);
            CachedTransportLineData.SetBudgetControlState(lineId, !budgetControlState);
            if (budgetControlState)
                return;
            CachedTransportLineData.ClearEnqueuedVehicles(lineId);
        });
    }

    private void OnUnbunchingClick(UIComponent component, UIMouseEventParameter p)
    {
      ushort lineId = GetLineID();
      if (lineId == 0)
        return;
      bool unbunchingState = CachedTransportLineData.GetUnbunchingState(lineId);
      CachedTransportLineData.SetUnbunchingState(lineId, !unbunchingState);
    }

    private void OnAddVehicleClick(UIComponent component, UIMouseEventParameter eventParam)
    {
        SimulationManager.instance.AddAction(() =>
        {
            ushort lineId = GetLineID();
            if (lineId == 0)
                return;
            ushort depot = CachedTransportLineData.GetDepot(lineId);
            TransportInfo info = TransportManager.instance.m_lines.m_buffer[lineId].Info;
            if (!DepotUtil.CanAddVehicle(depot,
                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[depot], info))
                return;
            CachedTransportLineData.SetBudgetControlState(lineId, false);
            if (depot == 0)
            {
                CachedTransportLineData.IncreaseTargetVehicleCount(lineId);
            }
            else
            {
                string prefabName =
                    !(component as VehicleListBoxRow != null)
                        ? CachedTransportLineData.GetRandomPrefab(lineId)
                        : (component as VehicleListBoxRow).Prefab.ObjectName;
                CachedTransportLineData.EnqueueVehicle(lineId, prefabName); //we need to enqueue vehicle first
                CachedTransportLineData.IncreaseTargetVehicleCount(lineId);
            }
        });
    }

    //TODO(earalov): consider corner cases
    private void OnRemoveVehicleClick(UIComponent component, UIMouseEventParameter eventParam)
    {
        SimulationManager.instance.AddAction(() =>
        {
            ushort lineId = GetLineID();
            if (lineId == 0)
                return;
            CachedTransportLineData.SetBudgetControlState(lineId, false);
            int[] selectedIndexes = _vehiclesInQueueListBox.SelectedIndexes;
            HashSet<ushort> selectedVehicles = _lineVehicleListBox.SelectedVehicles;
            if (selectedIndexes.Length != 0)
                CachedTransportLineData.DequeueVehicles(lineId, selectedIndexes);
            else if (selectedVehicles.Count > 0)
            {
                foreach (ushort vehicleID in selectedVehicles)
                    TransportLineUtil.RemoveVehicle(lineId, vehicleID, true);
            }
            else if (CachedTransportLineData.EnqueuedVehiclesCount(lineId) > 0)
            {
                CachedTransportLineData.DequeueVehicle(lineId);
            }
            else
            {
                var activeVehicles = TransportLineUtil.CountLineActiveVehicles(lineId, out int _);
                if (activeVehicles > 0)
                {
                  TransportLineUtil.RemoveActiveVehicle(lineId, true, activeVehicles);
                }
                else
                {
                    if (CachedTransportLineData.GetTargetVehicleCount(lineId) <= 0)
                        return;
                    CachedTransportLineData.DecreaseTargetVehicleCount(lineId);
                }
            }
        });
    }

      private void OnDeleteLineClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      ushort lineID = GetLineID();
      if (lineID == 0)
        return;
      ConfirmPanel.ShowModal("CONFIRM_LINEDELETE", (comp, ret) =>
      {
        if (ret != 1)
          return;
        Singleton<SimulationManager>.instance.AddAction(() => Singleton<TransportManager>.instance.ReleaseLine(lineID));
        CachedTransportLineData.SetLineDefaults(lineID);
        _publicTransportWorldInfoPanel.OnCloseButton();
      });
    }

    private void OnSelectedDepotChanged(UIComponent component, ushort selectedItem)
    {
      ushort lineId = GetLineID();
      if (lineId == 0)
        return;
      CachedTransportLineData.SetDepot(lineId, selectedItem);
    }

    private void OnDepotMarkerClicked(UIComponent component, UIMouseEventParameter eventParam)
    {
      component.Unfocus();
      if (_depotDropDown.SelectedItem == 0)
        return;
      InstanceID id = new InstanceID();
      id.Building = _depotDropDown.SelectedItem;
      ToolsModifierControl.cameraController.SetTarget(id, ToolsModifierControl.cameraController.transform.position, Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift));
      DefaultTool.OpenWorldInfoPanel(id, ToolsModifierControl.cameraController.transform.position);
    }



    private void OnColorChanged(UIComponent component, Color color)
    {
      _colorTextField.text = ColorUtility.ToHtmlStringRGB(color);
    }

    private void OnColorTextSubmitted(UIComponent component, string text)
    {
      Color color;
      if (!ColorUtility.TryParseHtmlString("#" + text, out color))
        return;
      _colorField.selectedColor = color;
      _publicTransportWorldInfoPanel.GetType().GetMethod("OnColorChanged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_publicTransportWorldInfoPanel, new object[2]
      {
        component,
        color
      });
    }

    private void OnDepotChanged(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
    {
        _updateDepots[new ItemClassTriplet(service, subService, level)] = true;
    }

    private void PopulateDepotDropDown(TransportInfo info)
    {
      _depotDropDown.ClearItems();
        if (info == null)
        {
            return;
        }
      _depotDropDown.AddItems(BuildingExtension.GetDepots(info), IDToName);
    }

    private string IDToName(ushort buildingID)
    {
      BuildingManager instance = Singleton<BuildingManager>.instance;
      if ((instance.m_buildings.m_buffer[buildingID].m_flags & Building.Flags.Untouchable) != Building.Flags.None)
        buildingID = instance.FindBuilding(instance.m_buildings.m_buffer[buildingID].m_position, 100f, ItemClass.Service.None, ItemClass.SubService.None, Building.Flags.Active, Building.Flags.Untouchable);
      return instance.GetBuildingName(buildingID, InstanceID.Empty) ?? "";
    }
    
    public ushort GetLineID()
    {
      InstanceID currentInstanceId = WorldInfoPanel.GetCurrentInstanceID();
      if (currentInstanceId.Type == InstanceType.TransportLine)
        return currentInstanceId.TransportLine;
      if (currentInstanceId.Type == InstanceType.Vehicle)
      {
        ushort firstVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[currentInstanceId.Vehicle].GetFirstVehicle(currentInstanceId.Vehicle);
        if (firstVehicle != 0)
          return Singleton<VehicleManager>.instance.m_vehicles.m_buffer[firstVehicle].m_transportLine;
      }
      return 0;
    }
  }
}
