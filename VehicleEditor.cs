// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleEditor
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Reflection;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public class VehicleEditor : UIPanel
  {
    private bool _firstShow = true;
    private int _selectedIndex = -1;
    public static VehicleEditor Instance;
    private bool _initialized;
    private ItemClass.SubService _selectedSubService;
    private ItemClass.Service _selectedService;
    private ItemClass.Level _selectedLevel;

    private int _position;
    private bool _hide;
    private PublicTransportInfoViewPanel _publicTransportInfoViewPanel;
    private SettingsPanel _settingsPanel;
    private UIPanel _containerPanel;
    private UIPanel _rightSidePanel;

    public override void Update()
    {
      base.Update();
      if (ImprovedPublicTransportMod.Settings.HideVehicleEditor != this._hide)
      {
        this._hide = ImprovedPublicTransportMod.Settings.HideVehicleEditor;
        this.isVisible = !this._hide;
      }
      if (ImprovedPublicTransportMod.Settings.VehicleEditorPosition != this._position)
      {
        this._position = ImprovedPublicTransportMod.Settings.VehicleEditorPosition;
        if (this.isVisible)
          this.UpdatePosition();
      }
      if (!this._initialized)
      {
        this._publicTransportInfoViewPanel = GameObject.Find("(Library) PublicTransportInfoViewPanel").GetComponent<PublicTransportInfoViewPanel>();
        if (!((UnityEngine.Object) this._publicTransportInfoViewPanel != (UnityEngine.Object) null))
          return;
        this._publicTransportInfoViewPanel.component.eventVisibilityChanged += new PropertyChangedEventHandler<bool>(this.ParentVisibilityChanged);
        this._publicTransportInfoViewPanel.component.eventPositionChanged += new PropertyChangedEventHandler<Vector2>(this.ParentPositionChanged);
        this.CreatePanel();
        this._settingsPanel = this.AddUIComponent<SettingsPanel>();
        VehicleEditor.Instance = this;
        this._initialized = true;
      }
      else if (this._initialized && this.isVisible && this._firstShow)
      {
        this.FirstShowInit(TransportInfo.TransportType.Bus, (VehicleInfo) null);
      }
      else
      {
        if (!this._initialized || !this.isVisible)
          return;
        this._rightSidePanel.Find<UILabel>("MaintenanceCostLabel").text = (Utils.ToSingle(this._rightSidePanel.Find<UITextField>("MaintenanceCost").text) * 0.01f).ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
        this._rightSidePanel.Find<UILabel>("TicketPriceLabel").text = (Utils.ToSingle(this._rightSidePanel.Find<UITextField>("TicketPrice").text) * 0.01f).ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
        this._rightSidePanel.Find<UILabel>("MaxSpeedLabel").text = (Utils.ToInt32(this._rightSidePanel.Find<UITextField>("MaxSpeed").text) * 5).ToString() + " " + ImprovedPublicTransportMod.Settings.SpeedString;
      }
    }

    public override void OnDestroy()
    {
      this._initialized = false;
      VehicleEditor.Instance = (VehicleEditor) null;
      if ((UnityEngine.Object) this._publicTransportInfoViewPanel != (UnityEngine.Object) null)
      {
        this._publicTransportInfoViewPanel.component.eventVisibilityChanged -= new PropertyChangedEventHandler<bool>(this.ParentVisibilityChanged);
        this._publicTransportInfoViewPanel.component.eventPositionChanged -= new PropertyChangedEventHandler<Vector2>(this.ParentPositionChanged);
      }
      if ((UnityEngine.Object) this._containerPanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._containerPanel.gameObject);
      if ((UnityEngine.Object) this._rightSidePanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._rightSidePanel.gameObject);
      if ((UnityEngine.Object) this._settingsPanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._settingsPanel.gameObject);
      base.OnDestroy();
    }

    private void FirstShowInit(TransportInfo.TransportType type, VehicleInfo info = null)
    {
      this._firstShow = false;
      TransportManager instance = Singleton<TransportManager>.instance;
      int num1 = 5;
      bool flag1 = instance.TransportTypeLoaded((TransportInfo.TransportType) num1);
      int num2 = 6;
      bool flag2 = instance.TransportTypeLoaded((TransportInfo.TransportType) num2);
      if (ImprovedPublicTransportMod.Settings.VehicleEditorPosition == 0)
      {
        float x = this._publicTransportInfoViewPanel.component.absolutePosition.x;
        if (flag1 & flag2)
          this.relativePosition = new Vector3(x, 486f);
        else if (flag1 | flag2)
          this.relativePosition = new Vector3(x, 445f);
        else
          this.relativePosition = new Vector3(x, 404f);
      }
      else
        this.relativePosition = this._publicTransportInfoViewPanel.component.absolutePosition + new Vector3(this._publicTransportInfoViewPanel.component.size.x + 1f, 0.0f);
      this.SetTransportType(type, info);
    }

    private void CreatePanel()
    {
      this.name = "VehicleEditor";
      this.width = 314f;
      this.height = 303f;
      this.backgroundSprite = "MenuPanel2";
      this.canFocus = true;
      this.isInteractive = true;
      this.isVisible = false;
      UILabel uiLabel = this.AddUIComponent<UILabel>();
      uiLabel.name = "Title";
      uiLabel.text = Localization.Get("VEHICLE_EDITOR_TITLE");
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = UIHelper.Font;
      uiLabel.position = new Vector3((float) ((double) this.width / 2.0 - (double) uiLabel.width / 2.0), (float) ((double) uiLabel.height / 2.0 - 20.0));
      UIButton uiButton = this.AddUIComponent<UIButton>();
      string str1 = "SettingsButton";
      uiButton.name = str1;
      double num1 = 32.0;
      uiButton.width = (float) num1;
      double num2 = 32.0;
      uiButton.height = (float) num2;
      string str2 = "Options";
      uiButton.normalBgSprite = str2;
      string str3 = "OptionsHovered";
      uiButton.hoveredBgSprite = str3;
      string str4 = "OptionsPressed";
      uiButton.pressedBgSprite = str4;
      Vector3 vector3 = new Vector3(278f, 4f);
      uiButton.relativePosition = vector3;
      MouseEventHandler mouseEventHandler = new MouseEventHandler(this.OnSettingsButtonClick);
      uiButton.eventClick += mouseEventHandler;
      this._containerPanel = this.AddUIComponent<UIPanel>();
      this._containerPanel.name = "ContainerPanel";
      this._containerPanel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
      this._containerPanel.transform.localPosition = Vector3.zero;
      this._containerPanel.width = 300f;
      this._containerPanel.height = this.height - 60f;
      this._containerPanel.autoLayout = true;
      this._containerPanel.autoLayoutDirection = LayoutDirection.Vertical;
      this._containerPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 1);
      this._containerPanel.autoLayoutStart = LayoutStart.TopLeft;
      this._containerPanel.relativePosition = new Vector3(6f, 50f);
      UIPanel uiPanel = this._containerPanel.AddUIComponent<UIPanel>();
      double num3 = 70.0;
      uiPanel.width = (float) num3;
      double num4 = 13.0;
      uiPanel.height = (float) num4;
      TransportManager instance = Singleton<TransportManager>.instance;
      this.CreateTabButton(TransportInfo.TransportType.Bus);
      this.CreateTabButton(TransportInfo.TransportType.Metro);
      this.CreateTabButton(TransportInfo.TransportType.Train);
      this.CreateTabButton(TransportInfo.TransportType.Ship);
      this.CreateTabButton(TransportInfo.TransportType.Airplane);
      if (instance.TransportTypeLoaded(TransportInfo.TransportType.Taxi))
        this.CreateTabButton(TransportInfo.TransportType.Taxi);
      if (instance.TransportTypeLoaded(TransportInfo.TransportType.Tram))
        this.CreateTabButton(TransportInfo.TransportType.Tram);
      if (instance.TransportTypeLoaded(TransportInfo.TransportType.Monorail))
        this.CreateTabButton(TransportInfo.TransportType.Monorail);
      if (instance.TransportTypeLoaded(TransportInfo.TransportType.CableCar))
        this.CreateTabButton(TransportInfo.TransportType.CableCar);
      this.CreateVehicleOptionsPanel();
    }

    private void CreateTabButton(TransportInfo.TransportType transportType)
    {
      UIPanel uiPanel = this._containerPanel.AddUIComponent<UIPanel>();
      uiPanel.transform.localPosition = Vector3.zero;
      double num1 = 70.0;
      uiPanel.width = (float) num1;
      double num2 = 30.0;
      uiPanel.height = (float) num2;
      int num3 = 0;
      uiPanel.autoLayoutDirection = (LayoutDirection) num3;
      int num4 = 0;
      uiPanel.autoLayoutStart = (LayoutStart) num4;
      RectOffset rectOffset = new RectOffset(8, 0, 4, 4);
      uiPanel.autoLayoutPadding = rectOffset;
      int num5 = 1;
      uiPanel.autoLayout = num5 != 0;
      Color32 transportColor = (Color32) Singleton<TransportManager>.instance.m_properties.m_transportColors[(int) transportType];
      uiPanel.color = transportColor;
      string str1 = "InfoviewPanel";
      uiPanel.backgroundSprite = str1;
      string vehicleTypeIcon = PublicTransportWorldInfoPanel.GetVehicleTypeIcon(transportType > TransportInfo.TransportType.EvacuationBus ? transportType - 1 : transportType);
      UIButton uiButton = uiPanel.AddUIComponent<UIButton>();
      double num6 = 32.0;
      uiButton.width = (float) num6;
      double num7 = 22.0;
      uiButton.height = (float) num7;
      string str2 = vehicleTypeIcon;
      uiButton.normalBgSprite = str2;
      string str3 = vehicleTypeIcon + "Focused";
      uiButton.hoveredBgSprite = str3;
      Vector3 vector3 = new Vector3(278f, 4f);
      uiButton.relativePosition = vector3;
      MouseEventHandler mouseEventHandler = (MouseEventHandler) ((c, p) => this.SetTransportType(transportType, (VehicleInfo) null));
      uiButton.eventClick += mouseEventHandler;
    }

    private void CreateVehicleOptionsPanel()
    {
      UIPanel uiPanel1 = this.AddUIComponent<UIPanel>();
      uiPanel1.name = "RightSidePanel";
      uiPanel1.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
      uiPanel1.transform.localPosition = Vector3.zero;
      uiPanel1.width = 246f;
      uiPanel1.height = 244f;
      uiPanel1.autoLayout = true;
      uiPanel1.autoLayoutDirection = LayoutDirection.Vertical;
      uiPanel1.autoLayoutPadding = new RectOffset(3, 3, 0, 0);
      uiPanel1.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel1.backgroundSprite = "InfoviewPanel";
      uiPanel1.relativePosition = new Vector3(60f, 50f);
      this._rightSidePanel = uiPanel1;
      UIPanel uiPanel2 = uiPanel1.AddUIComponent<UIPanel>();
      uiPanel2.name = "CaptionPanel";
      uiPanel2.width = uiPanel2.parent.width - 6f;
      uiPanel2.height = 30f;
      uiPanel2.backgroundSprite = "InfoviewPanel";
      UILabel uiLabel1 = uiPanel2.AddUIComponent<UILabel>();
      uiLabel1.name = "CaptionLabel";
      uiLabel1.font = UIHelper.Font;
      uiLabel1.textColor = (Color32) Color.white;
      uiLabel1.textScale = 0.95f;
      uiLabel1.textAlignment = UIHorizontalAlignment.Center;
      uiLabel1.useOutline = true;
      uiLabel1.autoSize = false;
      uiLabel1.height = 18f;
      uiLabel1.width = uiPanel2.width;
      uiLabel1.position = new Vector3((float) ((double) uiPanel2.width / 2.0 - (double) uiPanel2.width / 2.0), (float) ((double) uiPanel2.height / 2.0 - 20.0));
      UIPanel uiPanel3 = uiPanel1.AddUIComponent<UIPanel>();
      string str1 = "RowContainer";
      uiPanel3.name = str1;
      double num1 = (double) uiPanel3.parent.width - 6.0;
      uiPanel3.width = (float) num1;
      double num2 = 211.0;
      uiPanel3.height = (float) num2;
      int num3 = 1;
      uiPanel3.autoLayoutDirection = (LayoutDirection) num3;
      int num4 = 0;
      uiPanel3.autoLayoutStart = (LayoutStart) num4;
      RectOffset rectOffset1 = new RectOffset(8, 0, 0, 0);
      uiPanel3.autoLayoutPadding = rectOffset1;
      int num5 = 1;
      uiPanel3.autoLayout = num5 != 0;
      string str2 = "GenericPanelWhite";
      uiPanel3.backgroundSprite = str2;
      Color32 color32 = new Color32((byte) 91, (byte) 97, (byte) 106, byte.MaxValue);
      uiPanel3.color = color32;
      UIPanel uiPanel4 = uiPanel3.AddUIComponent<UIPanel>();
      double num6 = (double) uiPanel4.parent.width - 8.0;
      uiPanel4.width = (float) num6;
      double num7 = 4.0;
      uiPanel4.height = (float) num7;
      int num8 = 0;
      uiPanel4.autoLayoutDirection = (LayoutDirection) num8;
      int num9 = 0;
      uiPanel4.autoLayoutStart = (LayoutStart) num9;
      RectOffset rectOffset2 = new RectOffset(0, 6, 0, 0);
      uiPanel4.autoLayoutPadding = rectOffset2;
      int num10 = 1;
      uiPanel4.autoLayout = num10 != 0;
      UIPanel uiPanel5 = uiPanel3.AddUIComponent<UIPanel>();
      double num11 = (double) uiPanel5.parent.width - 8.0;
      uiPanel5.width = (float) num11;
      double num12 = 30.0;
      uiPanel5.height = (float) num12;
      int num13 = 0;
      uiPanel5.autoLayoutDirection = (LayoutDirection) num13;
      int num14 = 0;
      uiPanel5.autoLayoutStart = (LayoutStart) num14;
      RectOffset rectOffset3 = new RectOffset(0, 6, 0, 0);
      uiPanel5.autoLayoutPadding = rectOffset3;
      int num15 = 1;
      uiPanel5.autoLayout = num15 != 0;
      DropDown dropDown = DropDown.Create((UIComponent) uiPanel5);
      string str3 = "AssetDropDown";
      dropDown.name = str3;
      double num16 = 27.0;
      dropDown.height = (float) num16;
      double num17 = (double) dropDown.parent.width - 6.0;
      dropDown.width = (float) num17;
      dropDown.DropDownPanelAlignParent = (UIComponent) this;
      UIFont font = UIHelper.Font;
      dropDown.Font = font;
      PropertyChangedEventHandler<ushort> changedEventHandler1 = new PropertyChangedEventHandler<ushort>(this.OnSelectedItemChanged);
      dropDown.eventSelectedItemChanged += changedEventHandler1;
      UIPanel uiPanel6 = uiPanel3.AddUIComponent<UIPanel>();
      double num18 = (double) uiPanel6.parent.width - 8.0;
      uiPanel6.width = (float) num18;
      double num19 = 30.0;
      uiPanel6.height = (float) num19;
      int num20 = 0;
      uiPanel6.autoLayoutDirection = (LayoutDirection) num20;
      int num21 = 0;
      uiPanel6.autoLayoutStart = (LayoutStart) num21;
      RectOffset rectOffset4 = new RectOffset(0, 3, 0, 0);
      uiPanel6.autoLayoutPadding = rectOffset4;
      int num22 = 1;
      uiPanel6.autoLayout = num22 != 0;
      UILabel uiLabel2 = uiPanel6.AddUIComponent<UILabel>();
      uiLabel2.name = "CapacityLabel";
      uiLabel2.text = Localization.Get("VEHICLE_EDITOR_CAPACITY");
      uiLabel2.font = UIHelper.Font;
      uiLabel2.textColor = (Color32) Color.white;
      uiLabel2.textScale = 0.8f;
      uiLabel2.autoSize = false;
      uiLabel2.height = 30f;
      uiLabel2.width = 115f;
      uiLabel2.wordWrap = true;
      uiLabel2.verticalAlignment = UIVerticalAlignment.Middle;
      UITextField uiTextField1 = uiPanel6.AddUIComponent<UITextField>();
      string str4 = "Capacity";
      uiTextField1.name = str4;
      string str5 = "0";
      uiTextField1.text = str5;
      uiLabel2.tooltip = "";
      Color32 black1 = (Color32) Color.black;
      uiTextField1.textColor = black1;
      string str6 = "EmptySprite";
      uiTextField1.selectionSprite = str6;
      string str7 = "TextFieldPanelHovered";
      uiTextField1.normalBgSprite = str7;
      string str8 = "TextFieldPanel";
      uiTextField1.focusedBgSprite = str8;
      int num23 = 1;
      uiTextField1.builtinKeyNavigation = num23 != 0;
      int num24 = 1;
      uiTextField1.submitOnFocusLost = num24 != 0;
      PropertyChangedEventHandler<string> changedEventHandler2 = new PropertyChangedEventHandler<string>(this.OnCapacitySubmitted);
      uiTextField1.eventTextSubmitted += changedEventHandler2;
      double num25 = 45.0;
      uiTextField1.width = (float) num25;
      double num26 = 22.0;
      uiTextField1.height = (float) num26;
      int num27 = 4;
      uiTextField1.maxLength = num27;
      int num28 = 1;
      uiTextField1.numericalOnly = num28 != 0;
      int num29 = 1;
      uiTextField1.verticalAlignment = (UIVerticalAlignment) num29;
      RectOffset rectOffset5 = new RectOffset(0, 0, 4, 0);
      uiTextField1.padding = rectOffset5;
      UIPanel uiPanel7 = uiPanel3.AddUIComponent<UIPanel>();
      string str9 = "MaintenanceRow";
      uiPanel7.name = str9;
      double num30 = (double) uiPanel7.parent.width - 8.0;
      uiPanel7.width = (float) num30;
      double num31 = 30.0;
      uiPanel7.height = (float) num31;
      int num32 = 0;
      uiPanel7.autoLayoutDirection = (LayoutDirection) num32;
      int num33 = 0;
      uiPanel7.autoLayoutStart = (LayoutStart) num33;
      RectOffset rectOffset6 = new RectOffset(0, 3, 0, 0);
      uiPanel7.autoLayoutPadding = rectOffset6;
      int num34 = 1;
      uiPanel7.autoLayout = num34 != 0;
      UILabel uiLabel3 = uiPanel7.AddUIComponent<UILabel>();
      uiLabel3.text = Localization.Get("VEHICLE_EDITOR_MAINTENANCE");
      uiLabel3.font = UIHelper.Font;
      uiLabel3.textColor = (Color32) Color.white;
      uiLabel3.textScale = 0.8f;
      uiLabel3.autoSize = false;
      uiLabel3.height = 30f;
      uiLabel3.width = 115f;
      uiLabel3.wordWrap = true;
      uiLabel3.verticalAlignment = UIVerticalAlignment.Middle;
      UITextField uiTextField2 = uiPanel7.AddUIComponent<UITextField>();
      string str10 = "MaintenanceCost";
      uiTextField2.name = str10;
      string str11 = "0";
      uiTextField2.text = str11;
      Color32 black2 = (Color32) Color.black;
      uiTextField2.textColor = black2;
      string str12 = "EmptySprite";
      uiTextField2.selectionSprite = str12;
      string str13 = "TextFieldPanelHovered";
      uiTextField2.normalBgSprite = str13;
      string str14 = "TextFieldPanel";
      uiTextField2.focusedBgSprite = str14;
      int num35 = 1;
      uiTextField2.builtinKeyNavigation = num35 != 0;
      int num36 = 1;
      uiTextField2.submitOnFocusLost = num36 != 0;
      double num37 = 45.0;
      uiTextField2.width = (float) num37;
      double num38 = 22.0;
      uiTextField2.height = (float) num38;
      int num39 = 6;
      uiTextField2.maxLength = num39;
      int num40 = 1;
      uiTextField2.numericalOnly = num40 != 0;
      int num41 = 1;
      uiTextField2.verticalAlignment = (UIVerticalAlignment) num41;
      RectOffset rectOffset7 = new RectOffset(0, 0, 4, 0);
      uiTextField2.padding = rectOffset7;
      UILabel uiLabel4 = uiPanel7.AddUIComponent<UILabel>();
      uiLabel4.name = "MaintenanceCostLabel";
      uiLabel4.text = "0";
      uiLabel4.font = UIHelper.Font;
      uiLabel4.textColor = (Color32) Color.white;
      uiLabel4.textScale = 0.8f;
      uiLabel4.textAlignment = UIHorizontalAlignment.Right;
      uiLabel4.autoSize = false;
      uiLabel4.height = 30f;
      uiLabel4.width = 60f;
      uiLabel4.verticalAlignment = UIVerticalAlignment.Middle;
      UIPanel uiPanel8 = uiPanel3.AddUIComponent<UIPanel>();
      double num42 = (double) uiPanel8.parent.width - 8.0;
      uiPanel8.width = (float) num42;
      double num43 = 30.0;
      uiPanel8.height = (float) num43;
      int num44 = 0;
      uiPanel8.autoLayoutDirection = (LayoutDirection) num44;
      int num45 = 0;
      uiPanel8.autoLayoutStart = (LayoutStart) num45;
      RectOffset rectOffset8 = new RectOffset(0, 3, 0, 0);
      uiPanel8.autoLayoutPadding = rectOffset8;
      int num46 = 1;
      uiPanel8.autoLayout = num46 != 0;
      UILabel uiLabel5 = uiPanel8.AddUIComponent<UILabel>();
      uiLabel5.name = "TicketPriceLabel2";
      uiLabel5.text = Localization.Get("VEHICLE_EDITOR_TICKET_PRICE");
      uiLabel5.font = UIHelper.Font;
      uiLabel5.textColor = (Color32) Color.white;
      uiLabel5.textScale = 0.8f;
      uiLabel5.autoSize = false;
      uiLabel5.height = 30f;
      uiLabel5.width = 115f;
      uiLabel5.wordWrap = true;
      uiLabel5.verticalAlignment = UIVerticalAlignment.Middle;
      UITextField uiTextField3 = uiPanel8.AddUIComponent<UITextField>();
      string str15 = "TicketPrice";
      uiTextField3.name = str15;
      string str16 = "0";
      uiTextField3.text = str16;
      Color32 black3 = (Color32) Color.black;
      uiTextField3.textColor = black3;
      string str17 = "EmptySprite";
      uiTextField3.selectionSprite = str17;
      string str18 = "TextFieldPanelHovered";
      uiTextField3.normalBgSprite = str18;
      string str19 = "TextFieldPanel";
      uiTextField3.focusedBgSprite = str19;
      int num47 = 1;
      uiTextField3.builtinKeyNavigation = num47 != 0;
      int num48 = 1;
      uiTextField3.submitOnFocusLost = num48 != 0;
      double num49 = 45.0;
      uiTextField3.width = (float) num49;
      double num50 = 22.0;
      uiTextField3.height = (float) num50;
      int num51 = 4;
      uiTextField3.maxLength = num51;
      int num52 = 1;
      uiTextField3.numericalOnly = num52 != 0;
      int num53 = 1;
      uiTextField3.verticalAlignment = (UIVerticalAlignment) num53;
      RectOffset rectOffset9 = new RectOffset(0, 0, 4, 0);
      uiTextField3.padding = rectOffset9;
      UILabel uiLabel6 = uiPanel8.AddUIComponent<UILabel>();
      uiLabel6.name = "TicketPriceLabel";
      uiLabel6.text = "0";
      uiLabel6.font = UIHelper.Font;
      uiLabel6.textColor = (Color32) Color.white;
      uiLabel6.textScale = 0.8f;
      uiLabel6.textAlignment = UIHorizontalAlignment.Right;
      uiLabel6.autoSize = false;
      uiLabel6.height = 30f;
      uiLabel6.width = 60f;
      uiLabel6.verticalAlignment = UIVerticalAlignment.Middle;
      UIPanel uiPanel9 = uiPanel3.AddUIComponent<UIPanel>();
      double num54 = (double) uiPanel9.parent.width - 8.0;
      uiPanel9.width = (float) num54;
      double num55 = 30.0;
      uiPanel9.height = (float) num55;
      int num56 = 0;
      uiPanel9.autoLayoutDirection = (LayoutDirection) num56;
      int num57 = 0;
      uiPanel9.autoLayoutStart = (LayoutStart) num57;
      RectOffset rectOffset10 = new RectOffset(0, 3, 0, 0);
      uiPanel9.autoLayoutPadding = rectOffset10;
      int num58 = 1;
      uiPanel9.autoLayout = num58 != 0;
      UILabel uiLabel7 = uiPanel9.AddUIComponent<UILabel>();
      uiLabel7.text = Localization.Get("VEHICLE_EDITOR_MAX_SPEED");
      uiLabel7.font = UIHelper.Font;
      uiLabel7.textColor = (Color32) Color.white;
      uiLabel7.textScale = 0.8f;
      uiLabel7.autoSize = false;
      uiLabel7.height = 30f;
      uiLabel7.width = 115f;
      uiLabel7.wordWrap = true;
      uiLabel7.verticalAlignment = UIVerticalAlignment.Middle;
      UITextField uiTextField4 = uiPanel9.AddUIComponent<UITextField>();
      string str20 = "MaxSpeed";
      uiTextField4.name = str20;
      string str21 = "0";
      uiTextField4.text = str21;
      Color32 black4 = (Color32) Color.black;
      uiTextField4.textColor = black4;
      string str22 = "EmptySprite";
      uiTextField4.selectionSprite = str22;
      string str23 = "TextFieldPanelHovered";
      uiTextField4.normalBgSprite = str23;
      string str24 = "TextFieldPanel";
      uiTextField4.focusedBgSprite = str24;
      int num59 = 1;
      uiTextField4.builtinKeyNavigation = num59 != 0;
      int num60 = 1;
      uiTextField4.submitOnFocusLost = num60 != 0;
      double num61 = 45.0;
      uiTextField4.width = (float) num61;
      double num62 = 22.0;
      uiTextField4.height = (float) num62;
      int num63 = 3;
      uiTextField4.maxLength = num63;
      int num64 = 1;
      uiTextField4.numericalOnly = num64 != 0;
      int num65 = 1;
      uiTextField4.verticalAlignment = (UIVerticalAlignment) num65;
      RectOffset rectOffset11 = new RectOffset(0, 0, 4, 0);
      uiTextField4.padding = rectOffset11;
      UILabel uiLabel8 = uiPanel9.AddUIComponent<UILabel>();
      uiLabel8.name = "MaxSpeedLabel";
      uiLabel8.text = "0";
      uiLabel8.font = UIHelper.Font;
      uiLabel8.textColor = (Color32) Color.white;
      uiLabel8.textScale = 0.8f;
      uiLabel8.textAlignment = UIHorizontalAlignment.Right;
      uiLabel8.autoSize = false;
      uiLabel8.height = 30f;
      uiLabel8.width = 60f;
      uiLabel8.verticalAlignment = UIVerticalAlignment.Middle;
      UIPanel uiPanel10 = uiPanel3.AddUIComponent<UIPanel>();
      string str25 = "EngineRow";
      uiPanel10.name = str25;
      double num66 = (double) uiPanel10.parent.width - 8.0;
      uiPanel10.width = (float) num66;
      double num67 = 30.0;
      uiPanel10.height = (float) num67;
      int num68 = 0;
      uiPanel10.autoLayoutDirection = (LayoutDirection) num68;
      int num69 = 0;
      uiPanel10.autoLayoutStart = (LayoutStart) num69;
      RectOffset rectOffset12 = new RectOffset(0, 3, 0, 0);
      uiPanel10.autoLayoutPadding = rectOffset12;
      int num70 = 1;
      uiPanel10.autoLayout = num70 != 0;
      UICheckBox checkBox = UIHelper.CreateCheckBox((UIComponent) uiPanel10);
      string str26 = "EngineOnBothEnds";
      checkBox.name = str26;
      string str27 = Localization.Get("VEHICLE_EDITOR_ENGINE_ON_BOTH_ENDS_TOOLTIP");
      checkBox.tooltip = str27;
      checkBox.label.text = Localization.Get("VEHICLE_EDITOR_ENGINE_ON_BOTH_ENDS");
      UIPanel uiPanel11 = uiPanel3.AddUIComponent<UIPanel>();
      string str28 = "ButtonRow";
      uiPanel11.name = str28;
      double num71 = (double) uiPanel11.parent.width - 8.0;
      uiPanel11.width = (float) num71;
      double num72 = 30.0;
      uiPanel11.height = (float) num72;
      int num73 = 0;
      uiPanel11.autoLayoutDirection = (LayoutDirection) num73;
      int num74 = 0;
      uiPanel11.autoLayoutStart = (LayoutStart) num74;
      RectOffset rectOffset13 = new RectOffset(0, 6, 4, 0);
      uiPanel11.autoLayoutPadding = rectOffset13;
      int num75 = 1;
      uiPanel11.autoLayout = num75 != 0;
      UIButton button1 = UIHelper.CreateButton((UIComponent) uiPanel11);
      string str29 = "Apply";
      button1.name = str29;
      string str30 = Localization.Get("VEHICLE_EDITOR_APPLY");
      button1.text = str30;
      double num76 = 0.800000011920929;
      button1.textScale = (float) num76;
      double num77 = 110.0;
      button1.width = (float) num77;
      double num78 = 22.0;
      button1.height = (float) num78;
      MouseEventHandler mouseEventHandler1 = new MouseEventHandler(this.OnApplyButtonClick);
      button1.eventClick += mouseEventHandler1;
      UIButton button2 = UIHelper.CreateButton((UIComponent) uiPanel11);
      string str31 = "Default";
      button2.name = str31;
      string str32 = Localization.Get("VEHICLE_EDITOR_DEFAULT");
      button2.text = str32;
      double num79 = 0.800000011920929;
      button2.textScale = (float) num79;
      double num80 = 110.0;
      button2.width = (float) num80;
      double num81 = 22.0;
      button2.height = (float) num81;
      MouseEventHandler mouseEventHandler2 = new MouseEventHandler(this.OnDefaultButtonClick);
      button2.eventClick += mouseEventHandler2;
    }

    private void ParentVisibilityChanged(UIComponent component, bool value)
    {
      if (this._hide)
        return;
      this.isVisible = value;
      if (!this.isVisible)
        return;
      this.UpdatePosition();
    }

    private void ParentPositionChanged(UIComponent component, Vector2 value)
    {
      this.UpdatePosition();
    }

    private void OnSettingsButtonClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      this._settingsPanel.isVisible = !this._settingsPanel.isVisible;
    }

    private void OnSelectedItemChanged(UIComponent component, ushort item)
    {
      this._selectedIndex = (component as DropDown).SelectedIndex;
      this.UpdateBindings();
    }

    private void OnCapacitySubmitted(UIComponent component, string text)
    {
      if (this._selectedIndex <= -1 || this._selectedSubService == ItemClass.SubService.PublicTransportTaxi)
        return;
      PrefabData prefab = VehiclePrefabs.instance.GetPrefabs(this._selectedService, this._selectedSubService, this._selectedLevel)[this._selectedIndex];
      int carCount = prefab.CarCount;
      int int32 = Utils.ToInt32(text);
      int num1 = 0;
      if (int32 > 0)
      {
        num1 = Utils.RoundToNearest((float) (int32 / carCount), 5) * carCount;
        (component as UITextField).text = num1.ToString();
      }
      UITextField uiTextField = this._rightSidePanel.Find<UITextField>("MaintenanceCost");
      if (!uiTextField.parent.enabled)
        return;
      float num2 = (float) num1 / (float) carCount / (float) GameDefault.GetCapacity(this._selectedSubService);
      uiTextField.text = Mathf.RoundToInt((float) (PrefabData.GetMaintenanceCost(this._selectedSubService, prefab.Info.m_vehicleAI) * 16) * num2).ToString();
    }

    private void OnApplyButtonClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      if (this._selectedIndex <= -1)
        return;
      UITextField uiTextField1 = this._rightSidePanel.Find<UITextField>("Capacity");
      UITextField uiTextField2 = this._rightSidePanel.Find<UITextField>("MaintenanceCost");
      UITextField uiTextField3 = this._rightSidePanel.Find<UITextField>("TicketPrice");
      UITextField uiTextField4 = this._rightSidePanel.Find<UITextField>("MaxSpeed");
      UICheckBox uiCheckBox = this._rightSidePanel.Find<UICheckBox>("EngineOnBothEnds");
      PrefabData prefab = VehiclePrefabs.instance.GetPrefabs(this._selectedService, this._selectedSubService, this._selectedLevel)[this._selectedIndex];
      int capacity = Utils.ToInt32(uiTextField1.text) / prefab.CarCount;
      int int32_1 = Utils.ToInt32(uiTextField2.text);
      int int32_2 = Utils.ToInt32(uiTextField3.text);
      int int32_3 = Utils.ToInt32(uiTextField4.text);
      bool isChecked = uiCheckBox.isChecked;
      prefab.SetValues(capacity, int32_1, int32_2, int32_3, isChecked);
      this.UpdateBindings();
    }

    private void OnDefaultButtonClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      if (this._selectedIndex > -1)
        VehiclePrefabs.instance.GetPrefabs(this._selectedService, this._selectedSubService, this._selectedLevel)[this._selectedIndex].SetDefaults();
      this.UpdateBindings();
    }

    private void SetTransportType(TransportInfo.TransportType transportType, VehicleInfo selectedPrefab = null)
    {
      Color transportColor = Singleton<TransportManager>.instance.m_properties.m_transportColors[(int) transportType];
      ItemClassTriplet classTriplet = VehicleEditor.GetSubService(transportType);
      this._selectedService = classTriplet.Service;
      this._selectedSubService = classTriplet.SubService;
      this._selectedLevel = classTriplet.Level;
      this._rightSidePanel.color = (Color32) transportColor;
      UIComponent uiComponent = this._rightSidePanel.Find("CaptionPanel");
      UIPanel uiPanel1 = this._rightSidePanel.Find<UIPanel>("MaintenanceRow");
      UILabel uiLabel1 = this._rightSidePanel.Find<UILabel>("CapacityLabel");
      UITextField uiTextField = this._rightSidePanel.Find<UITextField>("Capacity");
      UILabel uiLabel2 = this._rightSidePanel.Find<UILabel>("TicketPriceLabel2");
      UIPanel uiPanel2 = this._rightSidePanel.Find<UIPanel>("EngineRow");
      this._rightSidePanel.Find<UIPanel>("ButtonRow");
      Color32 color32 = (Color32) transportColor;
      uiComponent.color = color32;
      //TODO(earalov): disable some elements for new transport types
      if (this._selectedSubService == ItemClass.SubService.PublicTransportTaxi)
      {
        uiPanel1.enabled = false;
        uiPanel2.enabled = false;
        uiLabel1.text = Localization.Get("VEHICLE_EDITOR_CAPACITY_TAXI");
        uiTextField.tooltip = Localization.Get("VEHICLE_EDITOR_CAPACITY_TAXI_TOOLTIP");
        uiLabel2.text = Localization.Get("VEHICLE_EDITOR_PRICE_PER_KILOMETER");
      }
      else
      {
        uiPanel1.enabled = true;
        if (this._selectedSubService == ItemClass.SubService.PublicTransportTrain)
          uiPanel2.enabled = true;
        else
          uiPanel2.enabled = false;
        uiLabel1.text = Localization.Get("VEHICLE_EDITOR_CAPACITY");
        uiTextField.tooltip = "";
        uiLabel2.text = Localization.Get("VEHICLE_EDITOR_TICKET_PRICE");
      }
      (this._rightSidePanel.Find("CaptionLabel") as UILabel).text = string.Format(Localization.Get("VEHICLE_EDITOR_SUB_TITLE"), (object) ColossalFramework.Globalization.Locale.Get(VehicleEditor.GetLocaleID(transportType)));
      this.PopulateAssetDropDown(selectedPrefab);
    }

    public void SetPrefab(VehicleInfo prefab)
    {
      if ((UnityEngine.Object) prefab == (UnityEngine.Object) null)
        return;
      PrefabAI ai = prefab.GetAI();
      if ((UnityEngine.Object) ai == (UnityEngine.Object) null)
        return;
      FieldInfo field = ai.GetType().GetField("m_transportInfo");
      if (field == null)
        return;
      TransportInfo.TransportType transportType = (field.GetValue((object) ai) as TransportInfo).m_transportType;
      if (this._firstShow)
        this.FirstShowInit(transportType, prefab);
      else
        this.SetTransportType(transportType, prefab);
    }

    public struct ItemClassTriplet
    {

        public ItemClassTriplet(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            this.Service = service;
            this.SubService = subService;
            this.Level = level;
        }


        public ItemClass.Service Service { get; }
        public ItemClass.SubService SubService { get; }
        public ItemClass.Level Level { get; }
     }


    private static ItemClassTriplet GetSubService(TransportInfo.TransportType transportType)
        {
      switch (transportType)
      {
        case TransportInfo.TransportType.Bus:
          return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportBus, ItemClass.Level.Level1);
        case TransportInfo.TransportType.EvacuationBus:
              return new ItemClassTriplet(ItemClass.Service.Disaster, ItemClass.SubService.None, ItemClass.Level.Level4);
        case TransportInfo.TransportType.Metro:
          return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMetro, ItemClass.Level.Level1);
        case TransportInfo.TransportType.Train:
            return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, ItemClass.Level.Level1);
        case TransportInfo.TransportType.Ship: //TODO(earalov): handle regional ships
            return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, ItemClass.Level.Level2);
        case TransportInfo.TransportType.Airplane: //TODO(earalov): handle regional planes
            return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, ItemClass.Level.Level2);
        case TransportInfo.TransportType.Taxi:
            return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTaxi, ItemClass.Level.Level1);
        case TransportInfo.TransportType.Tram:
            return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTram, ItemClass.Level.Level1);
        case TransportInfo.TransportType.Monorail:
            return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMonorail, ItemClass.Level.Level1);
        case TransportInfo.TransportType.CableCar:
           return new ItemClassTriplet(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportCableCar, ItemClass.Level.Level1);
        default:
          return new ItemClassTriplet(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);
      }
    }

    private static string GetLocaleID(TransportInfo.TransportType transportType)
    {
      switch (transportType)
      {
        case TransportInfo.TransportType.Bus:
          return "INFO_PUBLICTRANSPORT_BUS";
        case TransportInfo.TransportType.Metro:
          return "INFO_PUBLICTRANSPORT_METRO";
        case TransportInfo.TransportType.Train:
          return "INFO_PUBLICTRANSPORT_TRAIN";
        case TransportInfo.TransportType.Ship:
          return "INFO_PUBLICTRANSPORT_SHIP";
        case TransportInfo.TransportType.Airplane:
          return "INFO_PUBLICTRANSPORT_PLANE";
        case TransportInfo.TransportType.Taxi:
          return "INFO_PUBLICTRANSPORT_TAXI";
        case TransportInfo.TransportType.Tram:
          return "INFO_PUBLICTRANSPORT_TRAM";
        case TransportInfo.TransportType.CableCar:
          return "INFO_PUBLICTRANSPORT_CABLECAR";
        case TransportInfo.TransportType.Monorail:
          return "INFO_PUBLICTRANSPORT_MONORAIL";
        default:
          return string.Empty;
      }
    }

    private void UpdateBindings()
    {
      UITextField uiTextField1 = this._rightSidePanel.Find<UITextField>("Capacity");
      UITextField uiTextField2 = this._rightSidePanel.Find<UITextField>("MaintenanceCost");
      UITextField uiTextField3 = this._rightSidePanel.Find<UITextField>("TicketPrice");
      UITextField uiTextField4 = this._rightSidePanel.Find<UITextField>("MaxSpeed");
      UICheckBox uiCheckBox = this._rightSidePanel.Find<UICheckBox>("EngineOnBothEnds");
      if (this._selectedIndex > -1)
      {
        PrefabData prefab = VehiclePrefabs.instance.GetPrefabs(this._selectedService, this._selectedSubService, this._selectedLevel)[this._selectedIndex];
        uiTextField1.text = prefab.TotalCapacity.ToString();
        UITextField uiTextField5 = uiTextField2;
        int num = prefab.MaintenanceCost;
        string str1 = num.ToString();
        uiTextField5.text = str1;
        UITextField uiTextField6 = uiTextField3;
        num = prefab.TicketPrice;
        string str2 = num.ToString();
        uiTextField6.text = str2;
        UITextField uiTextField7 = uiTextField4;
        num = prefab.MaxSpeed;
        string str3 = num.ToString();
        uiTextField7.text = str3;
        uiCheckBox.isChecked = prefab.EngineOnBothEnds;
      }
      else
      {
        uiTextField1.text = "0";
        uiTextField2.text = "0";
        uiTextField3.text = "0";
        uiTextField4.text = "0";
        uiCheckBox.isChecked = false;
      }
    }

    private void UpdatePosition()
    {
      try
      {
        if (this._position == 0)
          this.relativePosition = this._publicTransportInfoViewPanel.component.absolutePosition + new Vector3(0.0f, this._publicTransportInfoViewPanel.component.size.y + 1f);
        else
          this.relativePosition = this._publicTransportInfoViewPanel.component.absolutePosition + new Vector3(this._publicTransportInfoViewPanel.component.size.x + 1f, 0.0f);
      }
      catch
      {
      }
    }

    private void PopulateAssetDropDown(VehicleInfo selectedPrefab = null)
    {
      DropDown dropDown = this._rightSidePanel.Find<DropDown>("AssetDropDown");
      if ((UnityEngine.Object) dropDown == (UnityEngine.Object) null)
        return;
      dropDown.ClearItems();
      PrefabData[] prefabs = VehiclePrefabs.instance.GetPrefabs(this._selectedService, this._selectedSubService, this._selectedLevel);
      int length = prefabs.Length;
      int num = 0;
      for (int index = 0; index < length; ++index)
      {
        dropDown.AddItem((ushort) prefabs[index].PrefabDataIndex, new Func<ushort, string>(this.IDToName));
        if ((UnityEngine.Object) selectedPrefab != (UnityEngine.Object) null && prefabs[index].PrefabDataIndex == selectedPrefab.m_prefabDataIndex)
          num = index;
      }
      dropDown.SelectedIndex = num;
      this._selectedIndex = num;
      this.UpdateBindings();
    }

    private string IDToName(ushort ID)
    {
      return ColossalFramework.Globalization.Locale.Get("VEHICLE_TITLE", PrefabCollection<VehicleInfo>.PrefabName((uint) ID)) ?? "";
    }
  }
}
