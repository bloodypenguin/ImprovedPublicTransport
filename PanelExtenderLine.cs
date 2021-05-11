// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.PanelExtenderLine
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using UIUtils = ImprovedPublicTransport2.Util.UIUtils;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2
{
  public class PanelExtenderLine : MonoBehaviour
  {
    
    private const float PARENT_HEIGHT = 285f;
    
    private bool _initialized;
    private int _cachedLineID;
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
    private VehicleListBox _prefabListBox;
    private VehicleListBox _lineVehicleListBox;
    private VehicleListBox _vehiclesInQueueListBox;

    private UIButton _selectTypes;
    private UIButton _addVehicle;
    private UIPanel _prefabPanel;
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
        this._updateDepots = dictionary;
    }

    private void Update()
    {
      if (!this._initialized)
      {
        this.Init();
      }
      else
      {
        if (!this._initialized || !this._publicTransportWorldInfoPanel.component.isVisible)
          return;
        this.UpdateBindings();
        var lineModelSelector = this._publicTransportWorldInfoPanel.Find<UIPanel>("LineModelSelectorContainer");
        if (lineModelSelector != null)
        {
          lineModelSelector.isVisible = false;
        }
      }
    }

    private void Init()
    {
      this._publicTransportWorldInfoPanel = GameObject.Find("(Library) PublicTransportWorldInfoPanel").GetComponent<PublicTransportWorldInfoPanel>();
      if (!((UnityEngine.Object) this._publicTransportWorldInfoPanel != (UnityEngine.Object) null))
        return;
      UIComponent passengers = this._publicTransportWorldInfoPanel.Find("Passengers");
    passengers.parent.relativePosition = new Vector3(passengers.parent.relativePosition.x,
        76.0f, passengers.parent.relativePosition.z);
        UIComponent agePanel = this._publicTransportWorldInfoPanel.Find("AgePanel");
        agePanel.relativePosition = new Vector3(0.0f,
            84.0f, agePanel.relativePosition.z);
        UIComponent tripSaved = this._publicTransportWorldInfoPanel.Find("TripSaved");
        tripSaved.parent.relativePosition = new Vector3(tripSaved.parent.relativePosition.x,
            200.0f, tripSaved.parent.relativePosition.z);

      UIComponent uiComponent1 = this._publicTransportWorldInfoPanel.Find("DeleteLine");
      if ((UnityEngine.Object) uiComponent1 == (UnityEngine.Object) null)
      {
        Utils.LogError((object) "Could not found Delete button!");
      }
      else
      {
        uiComponent1.isVisible = false;
          UIComponent vehicleAmount = this._publicTransportWorldInfoPanel.Find("VehicleAmount");
          vehicleAmount.AlignTo(uiComponent1.parent, UIAlignAnchor.TopLeft);
          vehicleAmount.relativePosition = uiComponent1.relativePosition;


                UIComponent uiComponent2 = this._publicTransportWorldInfoPanel.Find("LinesOverview");
        if ((UnityEngine.Object) uiComponent2 != (UnityEngine.Object) null)
          uiComponent2.enabled = false;
          this._mainSubPanel = agePanel.parent;
        if ((UnityEngine.Object) this._mainSubPanel == (UnityEngine.Object) null)
        {
          Utils.LogError((object) "Could not found Panel!");
        }
        else
        {
          UIPanel uiPanel = this._mainSubPanel.AddUIComponent<UIPanel>();
          uiPanel.name = "IptContainer";
          uiPanel.width = 301f;
          uiPanel.height = 166f;
          uiPanel.autoLayoutDirection = LayoutDirection.Vertical;
          uiPanel.autoLayoutStart = LayoutStart.TopLeft;
          uiPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 5);
          uiPanel.autoLayout = true;
          uiPanel.relativePosition = new Vector3(10f, 224.0f);
          this._iptContainer = uiPanel;
          this._vehicleAmount = Utils.GetPrivate<UILabel>((object) this._publicTransportWorldInfoPanel, "m_VehicleAmount");
          if ((UnityEngine.Object) this._vehicleAmount == (UnityEngine.Object) null)
          {
            Utils.LogError((object) "Could not found m_VehicleAmount!");
          }
          else
          {
            (this._vehicleAmount.parent as UIPanel).autoLayoutPadding = new RectOffset(0, 10, 0, 0);
            UILabel uiLabel = this._vehicleAmount.parent.AddUIComponent<UILabel>();
            uiLabel.name = "StopCount";
            uiLabel.font = this._vehicleAmount.font;
            uiLabel.textColor = this._vehicleAmount.textColor;
            uiLabel.textScale = this._vehicleAmount.textScale;
            uiLabel.eventMouseEnter += new MouseEventHandler(this.OnMouseEnter);
            this._stopCountLabel = uiLabel;
            this._colorField = Utils.GetPrivate<UIColorField>((object) this._publicTransportWorldInfoPanel, "m_ColorField");
            if ((UnityEngine.Object) this._colorField == (UnityEngine.Object) null)
            {
              Utils.LogError((object) "Could not found m_ColorField!");
            }
            else
            {
              UITextField uiTextField = this._colorField.parent.AddUIComponent<UITextField>();
              uiTextField.name = "ColorTextField";
              uiTextField.text = "000000";
              uiTextField.textColor = (Color32) Color.black;
              uiTextField.textScale = 0.7f;
              uiTextField.selectionSprite = "EmptySprite";
              uiTextField.normalBgSprite = "TextFieldPanelHovered";
              uiTextField.focusedBgSprite = "TextFieldPanel";
              uiTextField.builtinKeyNavigation = true;
              uiTextField.submitOnFocusLost = true;
              uiTextField.eventTextSubmitted += new PropertyChangedEventHandler<string>(this.OnColorTextSubmitted);
              uiTextField.width = 50f;
              uiTextField.height = 23f;
              uiTextField.maxLength = 6;
              uiTextField.verticalAlignment = UIVerticalAlignment.Middle;
              uiTextField.padding = new RectOffset(0, 0, 8, 0);
              uiTextField.relativePosition = new Vector3(135f, 0.0f);
              this._colorTextField = uiTextField;
              this._colorField.eventSelectedColorReleased += new PropertyChangedEventHandler<Color>(this.OnColorChanged);
              this.CreateSpawnTimerPanel();
              this.CreateBudgetControlPanel();
              this.CreateUnbunchingPanel();
              this.CreateDropDownPanel();
              this.CreateButtonPanel1();
              this.CreateButtonPanel2();
              this._publicTransportWorldInfoPanel.component.height = 493f;
              this.CreatePrefabPanel();

              this.CreateVehiclesOnLinePanel();
              this.CreateVehiclesInQueuePanel();
              BuildingExtension.OnDepotAdded += this.OnDepotChanged;
              BuildingExtension.OnDepotRemoved += this.OnDepotChanged;
              this._initialized = true;
            }
          }
        }
      }
    }

    private void UpdateBindings()
    {
      ushort lineId = this.GetLineID();
      if ((int) lineId != 0)
      {
        int lineVehicleCount = TransportLineUtil.CountLineActiveVehicles(lineId, out int _);
        int targetVehicleCount = CachedTransportLineData.GetTargetVehicleCount(lineId);
        int num1 = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineId].CountStops(lineId);
        this._vehicleAmount.text = LocaleFormatter.FormatGeneric("TRANSPORT_LINE_VEHICLECOUNT", (object) (lineVehicleCount.ToString() + " / " + (object) targetVehicleCount));
        this._stopCountLabel.text = string.Format(Localization.Get("LINE_PANEL_STOPS"), (object) num1);
        this._budgetControl.isChecked = CachedTransportLineData.GetBudgetControlState(lineId);

        if ((int) OptionsWrapper<Settings>.Options.IntervalAggressionFactor == 0)
        {
          this._unbunching.Disable();
          this._unbunching.isChecked = false;
          this._unbunching.label.text = Localization.Get("UNBUNCHING_DISABLED");
        }
        else
        {
          this._unbunching.Enable();
          this._unbunching.isChecked = CachedTransportLineData.GetUnbunchingState(lineId);

          if (targetVehicleCount > 1)
          {
            this._unbunching.label.text = string.Format(Localization.Get("UNBUNCHING_ENABLED") + " - " + Localization.Get("UNBUNCHING_TARGET_GAP"), Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineId].m_averageInterval) ;
          }
          else
            this._unbunching.label.text = Localization.Get("UNBUNCHING_ENABLED");
        }
        bool flag1 = false;
        ushort depotID = CachedTransportLineData.GetDepot(lineId);
          TransportInfo info = TransportManager.instance.m_lines.m_buffer[lineId].Info;
          if (!DepotUtil.IsValidDepot(depotID, info))
          {
              flag1 = true;
          }
          bool flag2 = true;
        if ((int) depotID != 0)
          flag2 = DepotUtil.CanAddVehicle(depotID, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) depotID], info);
        if (flag2)
        {
          var currentlyDisabled = SimulationManager.instance.m_isNightTime
            ? (Singleton<TransportManager>.instance.m_lines.m_buffer[lineId].m_flags & TransportLine.Flags.DisabledNight) != TransportLine.Flags.None 
            : (Singleton<TransportManager>.instance.m_lines.m_buffer[lineId].m_flags & TransportLine.Flags.DisabledDay) != TransportLine.Flags.None;

          if (currentlyDisabled || lineVehicleCount >= targetVehicleCount)
          {
            this._spawnTimer.text = string.Format(Localization.Get("LINE_PANEL_SPAWNTIMER"), "∞");
          }
          else
          {
            var timeToNext = Mathf.Max(0, Mathf.CeilToInt(CachedTransportLineData.GetNextSpawnTime(lineId) - SimHelper.SimulationTime)); 
            this._spawnTimer.text = string.Format(Localization.Get("LINE_PANEL_SPAWNTIMER"), "≥" + timeToNext);
          } 
        }
        else
          this._spawnTimer.text = Localization.Get("LINE_PANEL_DEPOT_WARNING");
        this._selectTypes.isEnabled = !flag1;
        this._addVehicle.isEnabled = !flag1 & flag2;

        ItemClass.SubService subService = info.GetSubService();
        ItemClass.Service service = info.GetService();
        ItemClass.Level level = info.GetClassLevel();
                ItemClassTriplet triplet = new ItemClassTriplet(service, subService,  level);

        if (subService != this._cachedSubService | flag1 || this._updateDepots[triplet])
        {
          this.PopulateDepotDropDown(info);
          this.PopulatePrefabListBox(service, subService, level);
          this._updateDepots[triplet] = false;
        }
        if (this._depotDropDown.Items.Length == 0)
          this._depotDropDown.Text = Localization.Get("LINE_PANEL_NO_DEPOT_FOUND");
        else
          this._depotDropDown.SelectedItem = depotID;
        if ((int) lineId != this._cachedLineID)
        {
          this._colorTextField.text = ColorUtility.ToHtmlStringRGB(this._colorField.selectedColor);
          this._prefabListBox.SetSelectionStateToAll(false, false);
          this._prefabListBox.SelectedItems = CachedTransportLineData.GetPrefabs(lineId);
          this._prefabPanel.Hide();
          this.UpdatePanelPositionAndSize();
        }
        if (lineVehicleCount != 0)
        {
          if ((int) lineId != this._cachedLineID || lineVehicleCount != this._cachedSimCount)
          this.PopulateLineVehicleListBox(lineId, service, subService, level);
          this._lineVehiclePanel.Show();
          this.UpdatePanelPositionAndSize();
        }
        else
        {
          this._lineVehiclePanel.Hide();
          this._lineVehicleListBox.ClearItems();
        }
        int num3 = CachedTransportLineData.EnqueuedVehiclesCount(lineId);
        if ((uint) num3 > 0U & flag2)
        {
          if ((int) lineId != this._cachedLineID || num3 != this._cachedQueuedCount)
            this.PopulateQueuedVehicleListBox(lineId, service, subService, level);
          if (this._vehiclesInQueueListBox.Items.Count == 0)
            this._vehiclesInQueuePanel.Hide();
          else
            this._vehiclesInQueuePanel.Show();
          this.UpdatePanelPositionAndSize();
        }
        else
        {
          this._vehiclesInQueuePanel.Hide();
          this._vehiclesInQueueListBox.ClearItems();
        }
        this._cachedLineID = (int) lineId;
        this._cachedSubService = subService;
        this._cachedSimCount = lineVehicleCount;
        this._cachedQueuedCount = num3;
        this._cachedStopCount = num1;
      }
      else
        this._publicTransportWorldInfoPanel.Hide();
    }

    private void UpdatePanelPositionAndSize()
    {
      float num = 0.0f;
      if (this._prefabPanel.isVisible)
        num = this._prefabPanel.width + 1f;
      this._lineVehiclePanel.relativePosition = new Vector3((float) ((double) num + (double) this._lineVehiclePanel.parent.width + 1.0), 0.0f);
      if (this._lineVehiclePanel.isVisible)
      {
        this._vehiclesInQueuePanel.relativePosition = new Vector3((float) ((double) num + (double) this._vehiclesInQueuePanel.parent.width + 1.0), this._lineVehiclePanel.height + 1f);
        this._vehiclesInQueuePanel.height = (float) (((double) PARENT_HEIGHT - 16.0) * 0.5);
        this._vehiclesInQueueListBox.height = 162f;
      }
      else
      {
        this._vehiclesInQueuePanel.relativePosition = new Vector3((float) ((double) num + (double) this._vehiclesInQueuePanel.parent.width + 1.0), 0.0f);
        this._vehiclesInQueuePanel.height = PARENT_HEIGHT - 16f;
        this._vehiclesInQueueListBox.height = PARENT_HEIGHT - 61f;
      }
      if (this._vehiclesInQueuePanel.isVisible)
      {
        this._lineVehiclePanel.height = (float) (((double) PARENT_HEIGHT - 16.0) * 0.5);
        this._lineVehicleListBox.height = 162f;
      }
      else
      {
        this._lineVehiclePanel.height = PARENT_HEIGHT - 16f;
        this._lineVehicleListBox.height = PARENT_HEIGHT - 61f;
      }
    }

    private void OnDestroy()
    {
      this._initialized = false;
      BuildingExtension.OnDepotAdded -= this.OnDepotChanged;
      BuildingExtension.OnDepotRemoved -= this.OnDepotChanged;
      if (this._updateDepots != null)
        this._updateDepots.Clear();
      if ((UnityEngine.Object) this._colorTextField != (UnityEngine.Object) null)
      {
        this._colorField.eventSelectedColorReleased -= new PropertyChangedEventHandler<Color>(this.OnColorChanged);
        UnityEngine.Object.Destroy((UnityEngine.Object) this._colorTextField.gameObject);
      }
      if ((UnityEngine.Object) this._stopCountLabel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._stopCountLabel.gameObject);
      if ((UnityEngine.Object) this._iptContainer != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._iptContainer.gameObject);
      if ((UnityEngine.Object) this._prefabPanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._prefabPanel.gameObject);
      if ((UnityEngine.Object) this._lineVehiclePanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._lineVehiclePanel.gameObject);
      if ((UnityEngine.Object) this._vehiclesInQueuePanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._vehiclesInQueuePanel.gameObject);
    }

    private void CreateSpawnTimerPanel()
    {
      UIPanel uiPanel = this._iptContainer.AddUIComponent<UIPanel>();
      double width = (double) uiPanel.parent.width;
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
      uiLabel.font = this._vehicleAmount.font;
      uiLabel.textColor = this._vehicleAmount.textColor;
      uiLabel.textScale = this._vehicleAmount.textScale;
      uiLabel.processMarkup = true;
      this._spawnTimer = uiLabel;
    }

    private void CreateBudgetControlPanel()
    {
      UIPanel uiPanel = this._iptContainer.AddUIComponent<UIPanel>();
      uiPanel.width = uiPanel.parent.width;
      uiPanel.height = 16f;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayout = true;
      UICheckBox uiCheckBox = uiPanel.AddUIComponent<UICheckBox>();
      uiCheckBox.size = uiPanel.size;
      uiCheckBox.clipChildren = true;
      uiCheckBox.tooltip = Localization.Get("LINE_PANEL_BUDGET_CONTROL_TOOLTIP") + System.Environment.NewLine + Localization.Get("EXPLANATION_BUDGET_CONTROL");
      uiCheckBox.eventClicked += new MouseEventHandler(this.OnBudgetControlClick);
      UISprite uiSprite = uiCheckBox.AddUIComponent<UISprite>();
      uiSprite.spriteName = "check-unchecked";
      uiSprite.size = new Vector2(16f, 16f);
      uiSprite.relativePosition = Vector3.zero;
      uiCheckBox.checkedBoxObject = (UIComponent) uiSprite.AddUIComponent<UISprite>();
      ((UISprite) uiCheckBox.checkedBoxObject).spriteName = "check-checked";
      uiCheckBox.checkedBoxObject.size = new Vector2(16f, 16f);
      uiCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
      uiCheckBox.label = uiCheckBox.AddUIComponent<UILabel>();
      uiCheckBox.label.text = Localization.Get("LINE_PANEL_BUDGET_CONTROL");
      uiCheckBox.label.font = this._vehicleAmount.font;
      uiCheckBox.label.textColor = this._vehicleAmount.textColor;
      uiCheckBox.label.textScale = this._vehicleAmount.textScale;
      uiCheckBox.label.relativePosition = new Vector3(22f, 2f);
      this._budgetControl = uiCheckBox;
    }

    private void CreateUnbunchingPanel()
    {
      UIPanel uiPanel = this._iptContainer.AddUIComponent<UIPanel>();
      uiPanel.width = uiPanel.parent.width;
      uiPanel.height = 16f;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayout = true;
      UICheckBox uiCheckBox = uiPanel.AddUIComponent<UICheckBox>();
      uiCheckBox.size = uiPanel.size;
      uiCheckBox.clipChildren = true;
      uiCheckBox.tooltip = Localization.Get("LINE_PANEL_UNBUNCHING_TOOLTIP") + System.Environment.NewLine + Localization.Get("EXPLANATION_UNBUNCHING");
      uiCheckBox.eventClicked += new MouseEventHandler(this.OnUnbunchingClick);
      UISprite uiSprite = uiCheckBox.AddUIComponent<UISprite>();
      uiSprite.spriteName = "check-unchecked";
      uiSprite.size = new Vector2(16f, 16f);
      uiSprite.relativePosition = Vector3.zero;
      uiCheckBox.checkedBoxObject = (UIComponent) uiSprite.AddUIComponent<UISprite>();
      ((UISprite) uiCheckBox.checkedBoxObject).spriteName = "check-checked";
      uiCheckBox.checkedBoxObject.size = new Vector2(16f, 16f);
      uiCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
      uiCheckBox.label = uiCheckBox.AddUIComponent<UILabel>();
      uiCheckBox.label.font = this._vehicleAmount.font;
      uiCheckBox.label.textColor = this._vehicleAmount.textColor;
      uiCheckBox.label.disabledTextColor = (Color32) Color.black;
      uiCheckBox.label.textScale = this._vehicleAmount.textScale;
      uiCheckBox.label.relativePosition = new Vector3(22f, 2f);
      this._unbunching = uiCheckBox;
    }

    private void CreateDropDownPanel()
    {
      UIPanel uiPanel = this._iptContainer.AddUIComponent<UIPanel>();
      uiPanel.width = uiPanel.parent.width;
      uiPanel.height = 27f;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 6, 0, 0);
      uiPanel.autoLayout = true;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      string str1 = Localization.Get("LINE_PANEL_DEPOT");
      uiLabel.text = str1;
      UIFont font = this._vehicleAmount.font;
      uiLabel.font = font;
      Color32 textColor = this._vehicleAmount.textColor;
      uiLabel.textColor = textColor;
      double textScale = (double) this._vehicleAmount.textScale;
      uiLabel.textScale = (float) textScale;
      int num1 = 0;
      uiLabel.autoSize = num1 != 0;
      double num2 = 27.0;
      uiLabel.height = (float) num2;
      double num3 = 97.0;
      uiLabel.width = (float) num3;
      int num4 = 1;
      uiLabel.verticalAlignment = (UIVerticalAlignment) num4;
      this._depotDropDown = DropDown.Create((UIComponent) uiPanel);
      this._depotDropDown.name = "DepotDropDown";
      this._depotDropDown.Font = this._vehicleAmount.font;
      this._depotDropDown.height = 27f;
      this._depotDropDown.width = 167f;
      this._depotDropDown.DropDownPanelAlignParent = this._publicTransportWorldInfoPanel.component;
      this._depotDropDown.eventSelectedItemChanged += new PropertyChangedEventHandler<ushort>(this.OnSelectedDepotChanged);
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
      MouseEventHandler mouseEventHandler = new MouseEventHandler(this.OnDepotMarkerClicked);
      uiButton.eventClick += mouseEventHandler;
    }

    private void CreateButtonPanel1()
    {
      UIPanel uiPanel = this._iptContainer.AddUIComponent<UIPanel>();
      double width = (double) uiPanel.parent.width;
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
      UIButton button1 = UIUtils.CreateButton((UIComponent) uiPanel);
      button1.name = "SelectTypes";
      button1.textPadding = new RectOffset(10, 10, 4, 0);
      button1.text = Localization.Get("LINE_PANEL_SELECT_TYPES");
      button1.tooltip = Localization.Get("LINE_PANEL_SELECT_TYPES_TOOLTIP");
      button1.textScale = 0.8f;
      button1.width = 97f;
      button1.height = 32f;
      button1.wordWrap = true;
      button1.eventClick += (MouseEventHandler) ((c, p) =>
      {
        if ((int) this._depotDropDown.SelectedItem <= 0)
          return;
        this._prefabPanel.isVisible = !this._prefabPanel.isVisible;
      });
      this._selectTypes = button1;
      UIButton button2 = UIUtils.CreateButton((UIComponent) uiPanel);
      button2.name = "AddVehicle";
      button2.textPadding = new RectOffset(10, 10, 4, 0);
      button2.text = Localization.Get("LINE_PANEL_ADD_VEHICLE");
      button2.textScale = 0.8f;
      button2.tooltip = Localization.Get("LINE_PANEL_ADD_VEHICLE_TOOLTIP");
      button2.width = 97f;
      button2.height = 32f;
      button2.wordWrap = true;
      button2.eventClick += new MouseEventHandler(this.OnAddVehicleClick);
      this._addVehicle = button2;
      UIButton button3 = UIUtils.CreateButton((UIComponent) uiPanel);
      button3.name = "RemoveVehicle";
      button3.textPadding = new RectOffset(10, 10, 4, 0);
      button3.text = Localization.Get("LINE_PANEL_REMOVE_VEHICLE");
      button3.textScale = 0.8f;
      button3.width = 97f;
      button3.height = 32f;
      button3.wordWrap = true;
      button3.eventClick += new MouseEventHandler(this.OnRemoveVehicleClick);
    }

    private void CreateButtonPanel2()
    {
      UIPanel uiPanel = this._iptContainer.AddUIComponent<UIPanel>();
      double width = (double) uiPanel.parent.width;
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
      float num5 = (float) (((double) uiPanel.parent.width - 4.0) / 2.0);
      UIButton button1 = UIUtils.CreateButton((UIComponent) uiPanel);
      string str1 = "VEHICLE_LINESOVERVIEW";
      button1.localeID = str1;
      double num6 = 0.800000011920929;
      button1.textScale = (float) num6;
      double num7 = (double) num5;
      button1.width = (float) num7;
      double num8 = 22.0;
      button1.height = (float) num8;
      MouseEventHandler mouseEventHandler1 = (MouseEventHandler) ((c, p) => this._publicTransportWorldInfoPanel.OnLinesOverviewClicked());
      button1.eventClick += mouseEventHandler1;
      UIButton button2 = UIUtils.CreateButton((UIComponent) uiPanel);
      string str2 = "LINE_DELETE";
      button2.localeID = str2;
      double num9 = 0.800000011920929;
      button2.textScale = (float) num9;
      double num10 = (double) num5;
      button2.width = (float) num10;
      double num11 = 22.0;
      button2.height = (float) num11;
      MouseEventHandler mouseEventHandler2 = new MouseEventHandler(this.OnDeleteLineClick);
      button2.eventClick += mouseEventHandler2;
    }

    private void CreatePrefabPanel()
    {
      UIPanel uiPanel = this._publicTransportWorldInfoPanel.component.AddUIComponent<UIPanel>();
      uiPanel.name = "PrefabPanel";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3(uiPanel.parent.width + 1f, 0.0f);
      uiPanel.width = 180f;
      uiPanel.height = PARENT_HEIGHT - 16f;
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      this._prefabPanel = uiPanel;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.text = Localization.Get("LINE_PANEL_SELECT_TYPES");
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = this._vehicleAmount.font;
      uiLabel.position = new Vector3((float) ((double) uiPanel.width / 2.0 - (double) uiLabel.width / 2.0), (float) ((double) uiLabel.height / 2.0 - 20.0));
      UIButton uiButton = uiPanel.AddUIComponent<UIButton>();
      uiButton.name = "CloseButton";
      uiButton.width = 32f;
      uiButton.height = 32f;
      uiButton.normalBgSprite = "buttonclose";
      uiButton.hoveredBgSprite = "buttonclosehover";
      uiButton.pressedBgSprite = "buttonclosepressed";
      uiButton.relativePosition = new Vector3((float) ((double) uiPanel.width - (double) uiButton.width - 2.0), 2f);
      uiButton.eventClick += (MouseEventHandler) ((c, p) => this._prefabPanel.isVisible = !this._prefabPanel.isVisible);
      this._prefabListBox = VehicleListBox.Create((UIComponent) uiPanel);
      this._prefabListBox.name = "VehicleListBox";
      this._prefabListBox.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopLeft);
      this._prefabListBox.relativePosition = new Vector3(3f, 40f);
      this._prefabListBox.width = uiPanel.width - 6f;
      this._prefabListBox.height = PARENT_HEIGHT - 61f;
      this._prefabListBox.Font = this._vehicleAmount.font;
      this._prefabListBox.eventSelectedItemsChanged += OnSelectedPrefabsChanged;
      this._prefabListBox.eventRowShiftClick += new MouseEventHandler(this.OnAddVehicleClick);
    }

    private void CreateVehiclesOnLinePanel()
    {
      UIPanel uiPanel = this._publicTransportWorldInfoPanel.component.AddUIComponent<UIPanel>();
      uiPanel.name = "VehiclesOnLine";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3((float) ((double) uiPanel.parent.width + (double) this._prefabPanel.width + (double)180f + 3.0), 0.0f);
      uiPanel.width = 180f;
      uiPanel.height = (float) (((double) PARENT_HEIGHT - 16.0) / 2.0);
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      this._lineVehiclePanel = uiPanel;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.text = Localization.Get("LINE_PANEL_LINE_VEHICLES");
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = this._vehicleAmount.font;
      uiLabel.position = new Vector3((float) ((double) uiPanel.width / 2.0 - (double) uiLabel.width / 2.0), (float) ((double) uiLabel.height / 2.0 - 20.0));
      VehicleListBox vehicleListBox = VehicleListBox.Create((UIComponent) uiPanel);
      vehicleListBox.name = "VehicleListBox";
      vehicleListBox.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopLeft);
      vehicleListBox.relativePosition = new Vector3(3f, 40f);
      vehicleListBox.width = uiPanel.width - 6f;
      vehicleListBox.height = 162f;
      vehicleListBox.Font = this._vehicleAmount.font;
      this._lineVehicleListBox = vehicleListBox;
    }

    private void CreateVehiclesInQueuePanel()
    {
      UIPanel uiPanel = this._publicTransportWorldInfoPanel.component.AddUIComponent<UIPanel>();
      uiPanel.name = "VehiclesInQueue";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3(uiPanel.parent.width + 1f, this._lineVehiclePanel.height + 1f);
      uiPanel.width = 180f;
      uiPanel.height = (float) (((double) PARENT_HEIGHT - 16.0) / 2.0);
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      this._vehiclesInQueuePanel = uiPanel;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.text = Localization.Get("LINE_PANEL_ENQUEUED");
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = this._vehicleAmount.font;
      uiLabel.position = new Vector3((float) ((double) uiPanel.width / 2.0 - (double) uiLabel.width / 2.0), (float) ((double) uiLabel.height / 2.0 - 20.0));
      VehicleListBox vehicleListBox = VehicleListBox.Create((UIComponent) uiPanel);
      vehicleListBox.name = "VehicleListBox";
      vehicleListBox.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopLeft);
      vehicleListBox.relativePosition = new Vector3(3f, 40f);
      vehicleListBox.width = uiPanel.width - 6f;
      vehicleListBox.height = 162f;
      vehicleListBox.Font = this._vehicleAmount.font;
      this._vehiclesInQueueListBox = vehicleListBox;
    }

    private void OnMouseEnter(UIComponent component, UIMouseEventParameter p)
    {
      ushort lineId = this.GetLineID();
      if ((int) lineId == 0)
        return;
      TransportLine transportLine = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineId];
      ushort num1 = transportLine.m_stops;
      int num2 = 0;
      for (int index = 0; index < transportLine.CountStops(lineId); ++index)
      {
        ushort nextStop = TransportLine.GetNextStop(num1);
        byte max;
        num2 += PanelExtenderLine.CountWaitingPassengers(num1, nextStop, out max);
        num1 = nextStop;
      }
      component.tooltip = string.Format(Localization.Get("LINE_PANEL_TOTAL_WAITING_PEOPLE_TOOLTIP"), (object) num2);
    }

    private void OnBudgetControlClick(UIComponent component, UIMouseEventParameter p)
    {
        SimulationManager.instance.AddAction(() =>
        {
            ushort lineId = this.GetLineID();
            if ((int) lineId == 0)
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
      ushort lineId = this.GetLineID();
      if ((int) lineId == 0)
        return;
      bool unbunchingState = CachedTransportLineData.GetUnbunchingState(lineId);
      CachedTransportLineData.SetUnbunchingState(lineId, !unbunchingState);
    }

    private void OnAddVehicleClick(UIComponent component, UIMouseEventParameter eventParam)
    {
        SimulationManager.instance.AddAction(() =>
        {
            ushort lineId = this.GetLineID();
            if ((int) lineId == 0)
                return;
            ushort depot = CachedTransportLineData.GetDepot(lineId);
            TransportInfo info = TransportManager.instance.m_lines.m_buffer[lineId].Info;
            if (!DepotUtil.CanAddVehicle(depot,
                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) depot], info))
                return;
            CachedTransportLineData.SetBudgetControlState(lineId, false);
            if ((int) depot == 0)
            {
                CachedTransportLineData.IncreaseTargetVehicleCount(lineId);
            }
            else
            {
                string prefabName =
                    !((UnityEngine.Object) (component as VehicleListBoxRow) != (UnityEngine.Object) null)
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
            ushort lineId = this.GetLineID();
            if ((int) lineId == 0)
                return;
            CachedTransportLineData.SetBudgetControlState(lineId, false);
            int[] selectedIndexes = this._vehiclesInQueueListBox.SelectedIndexes;
            HashSet<ushort> selectedVehicles = this._lineVehicleListBox.SelectedVehicles;
            if (selectedIndexes.Length != 0)
                CachedTransportLineData.DequeueVehicles(lineId, selectedIndexes, true);
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
      ushort lineID = this.GetLineID();
      if ((int) lineID == 0)
        return;
      ConfirmPanel.ShowModal("CONFIRM_LINEDELETE", (UIView.ModalPoppedReturnCallback) ((comp, ret) =>
      {
        if (ret != 1)
          return;
        Singleton<SimulationManager>.instance.AddAction((System.Action) (() => Singleton<TransportManager>.instance.ReleaseLine(lineID)));
        CachedTransportLineData.SetLineDefaults(lineID);
        this._publicTransportWorldInfoPanel.OnCloseButton();
      }));
    }

    private void OnSelectedDepotChanged(UIComponent component, ushort selectedItem)
    {
      ushort lineId = this.GetLineID();
      if ((int) lineId == 0)
        return;
      CachedTransportLineData.SetDepot(lineId, selectedItem);
    }

    private void OnDepotMarkerClicked(UIComponent component, UIMouseEventParameter eventParam)
    {
      component.Unfocus();
      if ((int) this._depotDropDown.SelectedItem == 0)
        return;
      InstanceID id = new InstanceID();
      id.Building = this._depotDropDown.SelectedItem;
      ToolsModifierControl.cameraController.SetTarget(id, ToolsModifierControl.cameraController.transform.position, Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift));
      DefaultTool.OpenWorldInfoPanel(id, ToolsModifierControl.cameraController.transform.position);
    }

    private void OnSelectedPrefabsChanged(UIComponent component, HashSet<string> selectedItems)
    {
      ushort lineId = this.GetLineID();
      if ((int) lineId == 0)
        return;
      CachedTransportLineData.SetPrefabs(lineId, selectedItems);
      Singleton<SimulationManager>.instance.AddAction(() => TransportLineUtil.ReplaceVehicles(lineId));
    }

    private void OnColorChanged(UIComponent component, Color color)
    {
      this._colorTextField.text = ColorUtility.ToHtmlStringRGB(color);
    }

    private void OnColorTextSubmitted(UIComponent component, string text)
    {
      Color color;
      if (!ColorUtility.TryParseHtmlString("#" + text, out color))
        return;
      this._colorField.selectedColor = color;
      this._publicTransportWorldInfoPanel.GetType().GetMethod("OnColorChanged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) this._publicTransportWorldInfoPanel, new object[2]
      {
        (object) component,
        (object) color
      });
    }

    private void OnDepotChanged(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
    {
        this._updateDepots[new ItemClassTriplet(service, subService, level)] = true;
    }

    private void PopulateDepotDropDown(TransportInfo info)
    {
      this._depotDropDown.ClearItems();
        if (info == null)
        {
            return;
        }
      this._depotDropDown.AddItems(BuildingExtension.GetDepots(info), this.IDToName);
    }

    private string IDToName(ushort buildingID)
    {
      BuildingManager instance = Singleton<BuildingManager>.instance;
      if ((instance.m_buildings.m_buffer[(int) buildingID].m_flags & Building.Flags.Untouchable) != Building.Flags.None)
        buildingID = instance.FindBuilding(instance.m_buildings.m_buffer[(int) buildingID].m_position, 100f, ItemClass.Service.None, ItemClass.SubService.None, Building.Flags.Active, Building.Flags.Untouchable);
      return instance.GetBuildingName(buildingID, InstanceID.Empty) ?? "";
    }

    private void PopulatePrefabListBox(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
    {
      this._prefabListBox.ClearItems();
      PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(service, subService, level);
      int length = prefabs.Length;
      for (int index = 0; index < length; ++index)
        this._prefabListBox.AddItem(prefabs[index], (ushort) 0);
    }

    private void PopulateLineVehicleListBox(ushort lineID, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
    {
      this._lineVehicleListBox.ClearItems();
      TransportLine transportLine = Singleton<TransportManager>.instance.m_lines.m_buffer[(int) lineID];
      int num = TransportLineUtil.CountLineActiveVehicles(lineID, out int _);
      PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(service, subService, level);
            int length = prefabs.Length;
      for (int index1 = 0; index1 < num; ++index1)
      {
        ushort vehicle = transportLine.GetVehicle(index1);
        if ((int) vehicle != 0)
        {
            if ((VehicleManager.instance.m_vehicles.m_buffer[vehicle].m_flags & Vehicle.Flags.GoingBack) ==
                ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                  Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource |
                  Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath |
                  Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                  Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                  Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                  Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                  Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                  Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                  Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                  Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive)) //based on beginning of TransportLine.SimulationStep
            {
                VehicleInfo info = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int)vehicle].Info;
                for (int index2 = 0; index2 < length; ++index2)
                {
                    PrefabData data = prefabs[index2];
                    if (info.name == data.ObjectName)
                    {
                        this._lineVehicleListBox.AddItem(data, vehicle);
                        break;
                    }
                }
            }
        }
      }
    }

    private void PopulateQueuedVehicleListBox(ushort lineID, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
    {
      this._vehiclesInQueueListBox.ClearItems();
      string[] enqueuedVehicles = CachedTransportLineData.GetEnqueuedVehicles(lineID);
      int length1 = enqueuedVehicles.Length;
      PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(service, subService, level);
      int length2 = prefabs.Length;
      for (int index1 = 0; index1 < length1; ++index1)
      {
        string str = enqueuedVehicles[index1];
        for (int index2 = 0; index2 < length2; ++index2)
        {
          PrefabData data = prefabs[index2];
          if (data.ObjectName == str)
          {
            this._vehiclesInQueueListBox.AddItem(data, (ushort) 0);
            break;
          }
        }
      }
    }

    public ushort GetLineID()
    {
      InstanceID currentInstanceId = WorldInfoPanel.GetCurrentInstanceID();
      if (currentInstanceId.Type == InstanceType.TransportLine)
        return currentInstanceId.TransportLine;
      if (currentInstanceId.Type == InstanceType.Vehicle)
      {
        ushort firstVehicle = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) currentInstanceId.Vehicle].GetFirstVehicle(currentInstanceId.Vehicle);
        if ((int) firstVehicle != 0)
          return Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) firstVehicle].m_transportLine;
      }
      return 0;
    }

    public static int CountWaitingPassengers(ushort currentStop, ushort nextStop, out byte max)
    {
      max = (byte) 0;
      if ((int) currentStop == 0 || (int) nextStop == 0)
        return 0;
      CitizenManager instance1 = Singleton<CitizenManager>.instance;
      NetManager instance2 = Singleton<NetManager>.instance;
      Vector3 position1 = instance2.m_nodes.m_buffer[(int) currentStop].m_position;
      Vector3 position2 = instance2.m_nodes.m_buffer[(int) nextStop].m_position;
      int num1 = Mathf.Max((int) (((double) position1.x - 64.0) / 8.0 + 1080.0), 0);
      int num2 = Mathf.Max((int) (((double) position1.z - 64.0) / 8.0 + 1080.0), 0);
      int num3 = Mathf.Min((int) (((double) position1.x + 64.0) / 8.0 + 1080.0), 2159);
      int num4 = Mathf.Min((int) (((double) position1.z + 64.0) / 8.0 + 1080.0), 2159);
      int num5 = 0;
      int num6 = 0;
      for (int index1 = num2; index1 <= num4; ++index1)
      {
        for (int index2 = num1; index2 <= num3; ++index2)
        {
          ushort instanceID = instance1.m_citizenGrid[index1 * 2160 + index2];
          int num7 = 0;
          while ((int) instanceID != 0)
          {
            int nextGridInstance = (int) instance1.m_instances.m_buffer[(int) instanceID].m_nextGridInstance;
            if ((instance1.m_instances.m_buffer[(int) instanceID].m_flags & CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None && (double) Vector3.SqrMagnitude((Vector3) instance1.m_instances.m_buffer[(int) instanceID].m_targetPos - position1) < 4096.0 && instance1.m_instances.m_buffer[(int) instanceID].Info.m_citizenAI.TransportArriveAtSource(instanceID, ref instance1.m_instances.m_buffer[(int) instanceID], position1, position2))
            {
              byte waitCounter = instance1.m_instances.m_buffer[(int) instanceID].m_waitCounter;
              max = Math.Max(waitCounter, max);
              num5 += (int) waitCounter;
              ++num6;
            }
            instanceID = (ushort) nextGridInstance;
            if (++num7 > 65536)
            {
              CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
              break;
            }
          }
        }
      }
      return num6;
    }
  }
}
