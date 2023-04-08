// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.PublicTransportStopWorldInfoPanel
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Query;
using ImprovedPublicTransport2.PersistentData;
using ImprovedPublicTransport2.UI.DontCryJustDieCommons;
using ImprovedPublicTransport2.UI.PanelExtenders;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using UIUtils = ImprovedPublicTransport2.Util.UIUtils;

namespace ImprovedPublicTransport2.UI
{
    public class PublicTransportStopWorldInfoPanel : UIPanel
    {
        private Vector3 m_WorldMousePosition = Vector3.zero;
        public static PublicTransportStopWorldInfoPanel instance;
        private UIView m_uiView;
        private InstanceID m_InstanceID;
        private int m_StopIndex;
        private Transform m_CameraTransform;
        private UIComponent m_FullscreenContainer;
        private UISprite m_VehicleType;
        private UITextField m_StopName;
        private DropDown m_SuggestedNames;
        private UILabel m_PassengerCount;
        private UILabel m_BoredCountdown;
        private UILabel m_passengersInCurrent;
        private UILabel m_passengersInLast;
        private UILabel m_passengersInAverage;
        private UILabel m_passengersOutCurrent;
        private UILabel m_passengersOutLast;
        private UILabel m_passengersOutAverage;
        private UILabel m_passengersTotalCurrent;
        private UILabel m_passengersTotalLast;
        private UILabel m_passengersTotalAverage;
        private UICheckBox m_unbunching;
        private UIButton m_closeStopsUnbunching;
        private UILabel m_Line;
        private UIButton m_DeleteStop;

        public override void Start()
        {
            PublicTransportStopWorldInfoPanel.instance = this;
            base.Start();
            this.m_uiView = this.GetUIView();
            if ((UnityEngine.Object) Camera.main != (UnityEngine.Object) null)
                this.m_CameraTransform = Camera.main.transform;
            this.m_FullscreenContainer = UIView.Find("FullScreenContainer");
            this.SetupPanel();
        }

        public override void Update()
        {
            base.Update();
            if (!this.isVisible)
                return;
            this.CheckForClose();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            if (!this.isVisible)
                return;
            if ((UnityEngine.Object) this.m_closeStopsUnbunching == (UnityEngine.Object) null)
                this.CreateButton();
            this.UpdatePosition();
            this.UpdateBindings();
            this.CheckForAltKey();
        }

        public new void Hide()
        {
            UnityEngine.Debug.Log("Hide");
            System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            UnityEngine.Debug.Log(t.ToString());


            this.m_InstanceID = InstanceID.Empty;
            this.isVisible = false;
        }

        private void SetupPanel()
        {
            this.name = "PublicTransportStopWorldInfoPanel";
            this.isVisible = false;
            this.canFocus = true;
            this.isInteractive = true;
            this.anchor = UIAnchorStyle.None;
            this.pivot = UIPivotPoint.BottomLeft;
            this.width = 380f;
            this.height = 280f;
            this.backgroundSprite = "InfoBubbleVehicle";
            UIPanel uiPanel1 = this.AddUIComponent<UIPanel>();
            string str1 = "Caption";
            uiPanel1.name = str1;
            double width = (double) this.width;
            uiPanel1.width = (float) width;
            double num1 = 40.0;
            uiPanel1.height = (float) num1;
            Vector3 vector3_1 = new Vector3(0.0f, 0.0f);
            uiPanel1.relativePosition = vector3_1;
            UISprite uiSprite1 = uiPanel1.AddUIComponent<UISprite>();
            uiSprite1.name = "VehicleType";
            uiSprite1.size = new Vector2(32f, 22f);
            uiSprite1.relativePosition = new Vector3(8f, 9f, 0.0f);
            this.m_VehicleType = uiSprite1;
            UITextField uiTextField = uiPanel1.AddUIComponent<UITextField>();
            uiTextField.name = "StopName";
            uiTextField.font = UIUtils.Font;
            uiTextField.height = 25f;
            uiTextField.width = 200f;
            uiTextField.maxLength = 32;
            uiTextField.builtinKeyNavigation = true;
            uiTextField.submitOnFocusLost = true;
            uiTextField.focusedBgSprite = "TextFieldPanel";
            uiTextField.hoveredBgSprite = "TextFieldPanelHovered";
            uiTextField.padding = new RectOffset(0, 0, 4, 0);
            uiTextField.selectionSprite = "EmptySprite";
            uiTextField.verticalAlignment = UIVerticalAlignment.Middle;
            uiTextField.position = new Vector3((float) ((double) this.width / 2.0 - (double) uiTextField.width / 2.0),
                (float) ((double) uiTextField.height / 2.0 - 20.0));
            uiTextField.eventTextSubmitted += new PropertyChangedEventHandler<string>(this.OnRename);
            this.m_StopName = uiTextField;
            DropDown dropDown = DropDown.Create((UIComponent) uiPanel1);
            dropDown.name = "SuggestedNames";
            dropDown.size = new Vector2(30f, 25f);
            dropDown.ListWidth = 200f;
            dropDown.DropDownPanelAlignParent = (UIComponent) this;
            dropDown.Font = UIUtils.Font;
            dropDown.position = new Vector3((float) ((double) this.width / 2.0 + (double) uiTextField.width / 2.0),
                (float) ((double) dropDown.height / 2.0 - 20.0));
            dropDown.tooltip = Localization.Get("STOP_PANEL_SUGGESTED_NAMES_TOOLTIP");
            dropDown.ShowPanel = false;
            dropDown.eventSelectedItemChanged += new PropertyChangedEventHandler<ushort>(this.OnSelectedItemChanged);
            this.m_SuggestedNames = dropDown;
            UIButton uiButton1 = uiPanel1.AddUIComponent<UIButton>();
            uiButton1.name = "ReuseName";
            uiButton1.tooltip = Localization.Get("STOP_PANEL_REUSE_NAME_TOOLTIP");
            uiButton1.size = new Vector2(30f, 30f);
            uiButton1.normalBgSprite = "IconPolicyRecycling";
            uiButton1.hoveredBgSprite = "IconPolicyRecyclingHovered";
            uiButton1.pressedBgSprite = "IconPolicyRecyclingPressed";
            uiButton1.relativePosition =
                new Vector3((float) ((double) this.width - 32.0 - (double) uiButton1.width - 2.0), 6f);
            uiButton1.eventClick += new MouseEventHandler(this.OnReuseNameButtonClick);
            UIButton uiButton2 = uiPanel1.AddUIComponent<UIButton>();
            uiButton2.name = "Close";
            uiButton2.size = new Vector2(32f, 32f);
            uiButton2.normalBgSprite = "buttonclose";
            uiButton2.hoveredBgSprite = "buttonclosehover";
            uiButton2.pressedBgSprite = "buttonclosepressed";
            uiButton2.relativePosition = new Vector3((float) ((double) this.width - (double) uiButton2.width - 2.0),
                2f);
            uiButton2.eventClick += new MouseEventHandler(this.OnCloseButtonClick);
            UIPanel uiPanel2 = this.AddUIComponent<UIPanel>();
            string str2 = "Container";
            uiPanel2.name = str2;
            double num2 = 365.0;
            uiPanel2.width = (float) num2;
            double num3 = 197.0;
            uiPanel2.height = (float) num3;
            int num4 = 1;
            uiPanel2.autoLayout = num4 != 0;
            int num5 = 1;
            uiPanel2.autoLayoutDirection = (LayoutDirection) num5;
            RectOffset rectOffset1 = new RectOffset(10, 10, 5, 0);
            uiPanel2.autoLayoutPadding = rectOffset1;
            int num6 = 0;
            uiPanel2.autoLayoutStart = (LayoutStart) num6;
            Vector3 vector3_2 = new Vector3(6f, 46f);
            uiPanel2.relativePosition = vector3_2;
            UIPanel uiPanel3 = uiPanel2.AddUIComponent<UIPanel>();
            string str3 = "PassengerCountPanel";
            uiPanel3.name = str3;
            int num7 = 13;
            uiPanel3.anchor = (UIAnchorStyle) num7;
            int num8 = 1;
            uiPanel3.autoLayout = num8 != 0;
            int num9 = 0;
            uiPanel3.autoLayoutDirection = (LayoutDirection) num9;
            RectOffset rectOffset2 = new RectOffset(0, 5, 0, 0);
            uiPanel3.autoLayoutPadding = rectOffset2;
            int num10 = 0;
            uiPanel3.autoLayoutStart = (LayoutStart) num10;
            Vector2 vector2_1 = new Vector2(345f, 14f);
            uiPanel3.size = vector2_1;
            UILabel uiLabel1 = uiPanel3.AddUIComponent<UILabel>();
            uiLabel1.name = "PassengerCount";
            uiLabel1.font = UIUtils.Font;
            uiLabel1.autoSize = true;
            uiLabel1.height = 15f;
            uiLabel1.textScale = 13f / 16f;
            uiLabel1.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            this.m_PassengerCount = uiLabel1;
            UIPanel uiPanel4 = uiPanel2.AddUIComponent<UIPanel>();
            string str4 = "BoredCountdownPanel";
            uiPanel4.name = str4;
            int num11 = 13;
            uiPanel4.anchor = (UIAnchorStyle) num11;
            int num12 = 1;
            uiPanel4.autoLayout = num12 != 0;
            int num13 = 0;
            uiPanel4.autoLayoutDirection = (LayoutDirection) num13;
            RectOffset rectOffset3 = new RectOffset(0, 5, 0, 0);
            uiPanel4.autoLayoutPadding = rectOffset3;
            int num14 = 0;
            uiPanel4.autoLayoutStart = (LayoutStart) num14;
            Vector2 vector2_2 = new Vector2(345f, 14f);
            uiPanel4.size = vector2_2;
            UILabel uiLabel2 = uiPanel4.AddUIComponent<UILabel>();
            uiLabel2.name = "BoredCountdown";
            uiLabel2.tooltip = Localization.Get("STOP_PANEL_BORED_TIMER_TOOLTIP");
            uiLabel2.font = UIUtils.Font;
            uiLabel2.autoSize = true;
            uiLabel2.height = 15f;
            uiLabel2.textScale = 13f / 16f;
            uiLabel2.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            uiLabel2.processMarkup = true;
            this.m_BoredCountdown = uiLabel2;
            UIPanel uiPanel5 = uiPanel2.AddUIComponent<UIPanel>();
            string str5 = "PassengerStats";
            uiPanel5.name = str5;
            int num15 = 13;
            uiPanel5.anchor = (UIAnchorStyle) num15;
            int num16 = 1;
            uiPanel5.autoLayout = num16 != 0;
            int num17 = 1;
            uiPanel5.autoLayoutDirection = (LayoutDirection) num17;
            RectOffset rectOffset4 = new RectOffset(0, 0, 0, 0);
            uiPanel5.autoLayoutPadding = rectOffset4;
            int num18 = 0;
            uiPanel5.autoLayoutStart = (LayoutStart) num18;
            Vector2 vector2_3 = new Vector2(349f, 75f);
            uiPanel5.size = vector2_3;
            UILabel uiLabel3;
            UILabel uiLabel4;
            UILabel uiLabel5;
            UILabel uiLabel6;
            int num19 = 1;
            PublicTransportStopWorldInfoPanel.CreateStatisticRow((UIComponent) uiPanel5, out uiLabel3, out uiLabel4,
                out uiLabel5, out uiLabel6, num19 != 0);
            uiLabel4.text = Localization.Get("CURRENT_WEEK");
            uiLabel5.text = Localization.Get("LAST_WEEK");
            uiLabel6.text = Localization.Get("AVERAGE");
            uiLabel6.tooltip = string.Format(Localization.Get("AVERAGE_TOOLTIP"),
                (object) OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);

            int num20 = 0;
            PublicTransportStopWorldInfoPanel.CreateStatisticRow((UIComponent) uiPanel5, out uiLabel3,
                out this.m_passengersInCurrent, out this.m_passengersInLast, out this.m_passengersInAverage,
                num20 != 0);
            uiLabel3.text = Localization.Get("STOP_PANEL_PASSENGERS_IN");
            uiLabel3.tooltip = Localization.Get("STOP_PANEL_PASSENGERS_IN_TOOLTIP");
            int num21 = 0;
            PublicTransportStopWorldInfoPanel.CreateStatisticRow((UIComponent) uiPanel5, out uiLabel3,
                out this.m_passengersOutCurrent, out this.m_passengersOutLast, out this.m_passengersOutAverage,
                num21 != 0);
            uiLabel3.text = Localization.Get("STOP_PANEL_PASSENGERS_OUT");
            uiLabel3.tooltip = Localization.Get("STOP_PANEL_PASSENGERS_OUT_TOOLTIP");
            int num22 = 0;
            PublicTransportStopWorldInfoPanel.CreateStatisticRow((UIComponent) uiPanel5, out uiLabel3,
                out this.m_passengersTotalCurrent, out this.m_passengersTotalLast, out this.m_passengersTotalAverage,
                num22 != 0);
            uiLabel3.text = Localization.Get("STOP_PANEL_PASSENGERS_TOTAL");
            uiLabel3.tooltip = Localization.Get("STOP_PANEL_PASSENGERS_TOTAL_TOOLTIP");
            UIPanel uiPanel6 = uiPanel2.AddUIComponent<UIPanel>();
            string str6 = "Unbunching";
            uiPanel6.name = str6;
            int num23 = 13;
            uiPanel6.anchor = (UIAnchorStyle) num23;
            int num24 = 1;
            uiPanel6.autoLayout = num24 != 0;
            int num25 = 0;
            uiPanel6.autoLayoutDirection = (LayoutDirection) num25;
            RectOffset rectOffset5 = new RectOffset(0, 5, 0, 0);
            uiPanel6.autoLayoutPadding = rectOffset5;
            int num26 = 0;
            uiPanel6.autoLayoutStart = (LayoutStart) num26;
            Vector2 vector2_4 = new Vector2(345f, 25f);
            uiPanel6.size = vector2_4;
            int num27 = 1;
            uiPanel6.useCenter = num27 != 0;
            UICheckBox uiCheckBox = uiPanel6.AddUIComponent<UICheckBox>();
            uiCheckBox.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;
            uiCheckBox.clipChildren = true;
            uiCheckBox.tooltip = Localization.Get("STOP_PANEL_UNBUNCHING_TOOLTIP") + System.Environment.NewLine +
                                 Localization.Get("EXPLANATION_UNBUNCHING");
            uiCheckBox.eventClicked += new MouseEventHandler(this.OnUnbunchingClick);
            UISprite uiSprite2 = uiCheckBox.AddUIComponent<UISprite>();
            uiSprite2.spriteName = "check-unchecked";
            uiSprite2.size = new Vector2(16f, 16f);
            uiSprite2.relativePosition = Vector3.zero;
            uiCheckBox.checkedBoxObject = (UIComponent) uiSprite2.AddUIComponent<UISprite>();
            ((UISprite) uiCheckBox.checkedBoxObject).spriteName = "check-checked";
            uiCheckBox.checkedBoxObject.size = new Vector2(16f, 16f);
            uiCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            uiCheckBox.label = uiCheckBox.AddUIComponent<UILabel>();
            uiCheckBox.label.font = UIUtils.Font;
            uiCheckBox.label.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            uiCheckBox.label.disabledTextColor = (Color32) Color.black;
            uiCheckBox.label.textScale = 13f / 16f;
            uiCheckBox.label.text = (int) OptionsWrapper<Settings.Settings>.Options.IntervalAggressionFactor == 0
                ? Localization.Get("UNBUNCHING_DISABLED")
                : Localization.Get("UNBUNCHING_ENABLED");
            uiCheckBox.label.relativePosition = new Vector3(22f, 2f);
            uiCheckBox.size = new Vector2(uiCheckBox.label.width + 22f, 16f);
            this.m_unbunching = uiCheckBox;
            UIPanel uiPanel7 = uiPanel2.AddUIComponent<UIPanel>();
            string str7 = "Line";
            uiPanel7.name = str7;
            int num28 = 13;
            uiPanel7.anchor = (UIAnchorStyle) num28;
            Vector2 vector2_5 = new Vector2(345f, 25f);
            uiPanel7.size = vector2_5;
            int num29 = 1;
            uiPanel7.autoLayout = num29 != 0;
            int num30 = 0;
            uiPanel7.autoLayoutDirection = (LayoutDirection) num30;
            RectOffset rectOffset6 = new RectOffset(0, 10, 0, 0);
            uiPanel7.autoLayoutPadding = rectOffset6;
            int num31 = 0;
            uiPanel7.autoLayoutStart = (LayoutStart) num31;
            int num32 = 1;
            uiPanel7.useCenter = num32 != 0;
            UILabel uiLabel7 = uiPanel7.AddUIComponent<UILabel>();
            uiLabel7.name = "Line";
            uiLabel7.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;
            uiLabel7.font = UIUtils.Font;
            uiLabel7.autoSize = true;
            uiLabel7.height = 25f;
            uiLabel7.textScale = 13f / 16f;
            uiLabel7.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            uiLabel7.verticalAlignment = UIVerticalAlignment.Middle;
            uiLabel7.relativePosition = new Vector3(0.0f, 5f);
            this.m_Line = uiLabel7;
            UIButton button1 = UIUtils.CreateButton((UIComponent) uiPanel7);
            button1.name = "ModifyLine";
            button1.autoSize = true;
            button1.textPadding = new RectOffset(10, 10, 4, 2);
            button1.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;
            button1.localeID = "VEHICLE_MODIFYLINE";
            button1.textScale = 0.75f;
            button1.eventClick += new MouseEventHandler(this.OnModifyLineClick);
            UIPanel uiPanel8 = uiPanel2.AddUIComponent<UIPanel>();
            string str8 = "Buttons";
            uiPanel8.name = str8;
            int num33 = 13;
            uiPanel8.anchor = (UIAnchorStyle) num33;
            int num34 = 1;
            uiPanel8.autoLayout = num34 != 0;
            int num35 = 0;
            uiPanel8.autoLayoutDirection = (LayoutDirection) num35;
            RectOffset rectOffset7 = new RectOffset(0, 5, 0, 0);
            uiPanel8.autoLayoutPadding = rectOffset7;
            int num36 = 0;
            uiPanel8.autoLayoutStart = (LayoutStart) num36;
            Vector2 vector2_6 = new Vector2(345f, 32f);
            uiPanel8.size = vector2_6;
            UIButton button2 = UIUtils.CreateButton((UIComponent) uiPanel8);
            button2.name = "PreviousStop";
            button2.textPadding = new RectOffset(10, 10, 4, 0);
            button2.text = Localization.Get("STOP_PANEL_PREVIOUS");
            button2.tooltip = Localization.Get("STOP_PANEL_PREVIOUS_TOOLTIP");
            button2.textScale = 0.75f;
            button2.size = new Vector2(110f, 32f);
            button2.wordWrap = true;
            button2.eventClick += new MouseEventHandler(this.OnPreviousStopClick);
            UIButton button3 = UIUtils.CreateButton((UIComponent) uiPanel8);
            button3.name = "DeleteStop";
            button3.textPadding = new RectOffset(10, 10, 4, 0);
            button3.text = Localization.Get("STOP_PANEL_DELETE_STOP");
            button3.tooltip = Localization.Get("STOP_PANEL_DELETE_STOP_TOOLTIP");
            button3.isEnabled = false;
            button3.textScale = 0.75f;
            button3.size = new Vector2(110f, 32f);
            button3.wordWrap = true;
            button3.hoveredTextColor = (Color32) Color.red;
            button3.focusedTextColor = (Color32) Color.red;
            button3.pressedTextColor = (Color32) Color.red;
            button3.eventClick += new MouseEventHandler(this.OnDeleteStopClick);
            this.m_DeleteStop = button3;
            UIButton button4 = UIUtils.CreateButton((UIComponent) uiPanel8);
            button4.name = "NextStop";
            button4.textPadding = new RectOffset(10, 10, 4, 0);
            button4.text = Localization.Get("STOP_PANEL_NEXT");
            button4.tooltip = Localization.Get("STOP_PANEL_NEXT_TOOLTIP");
            button4.textScale = 0.75f;
            button4.size = new Vector2(110f, 32f);
            button4.wordWrap = true;
            button4.eventClick += new MouseEventHandler(this.OnNextStopClick);
        }

        public static UIPanel CreateStatisticRow(UIComponent parent, out UILabel label1, out UILabel label2,
            out UILabel label3, out UILabel label4, bool isCaptionRow = false)
        {
            float width = parent.width;
            float y = 15f;
            if (isCaptionRow)
                y = 30f;
            UIPanel uiPanel = parent.AddUIComponent<UIPanel>();
            uiPanel.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right;
            uiPanel.autoLayout = true;
            uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            uiPanel.autoLayoutPadding = new RectOffset(0, 1, 0, 0);
            uiPanel.autoLayoutStart = LayoutStart.TopLeft;
            uiPanel.size = new Vector2(width, y);
            label1 = uiPanel.AddUIComponent<UILabel>();
            label1.font = UIUtils.Font;
            label1.autoSize = false;
            label1.size = new Vector2((float) (((double) width - 3.0) * 0.340000003576279), y);
            label1.textScale = 13f / 16f;
            label1.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            label1.wordWrap = true;
            label2 = uiPanel.AddUIComponent<UILabel>();
            label2.font = UIUtils.Font;
            label2.autoSize = false;
            label2.size = new Vector2((float) (((double) width - 3.0) * 0.219999998807907), y);
            label2.textScale = 13f / 16f;
            label2.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            label2.textAlignment = UIHorizontalAlignment.Right;
            label2.wordWrap = true;
            label3 = uiPanel.AddUIComponent<UILabel>();
            label3.font = UIUtils.Font;
            label3.autoSize = false;
            label3.size = new Vector2((float) (((double) width - 3.0) * 0.219999998807907), y);
            label3.textScale = 13f / 16f;
            label3.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            label3.textAlignment = UIHorizontalAlignment.Right;
            label3.wordWrap = true;
            label4 = uiPanel.AddUIComponent<UILabel>();
            label4.font = UIUtils.Font;
            label4.autoSize = false;
            label4.size = new Vector2((float) (((double) width - 3.0) * 0.219999998807907), y);
            label4.textScale = 13f / 16f;
            label4.textColor = new Color32((byte) 185, (byte) 221, (byte) 254, byte.MaxValue);
            label4.textAlignment = UIHorizontalAlignment.Right;
            label4.wordWrap = true;
            return uiPanel;
        }

        private void OnRename(UIComponent component, string newName)
        {
            Singleton<InstanceManager>.instance.SetName(this.m_InstanceID, newName);
        }

        private void OnSelectedItemChanged(UIComponent component, ushort item)
        {
            string name = this.IDToName(item);
            this.m_StopName.text = name;
            Singleton<InstanceManager>.instance.SetName(this.m_InstanceID, name);
        }

        private void OnReuseNameButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.ProcessNodes((System.Action<ushort>) (nodeID => Singleton<InstanceManager>.instance.SetName(
                new InstanceID()
                {
                    NetNode = nodeID
                }, this.m_StopName.text)));
        }

        private void OnCloseButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.Hide();
        }

        private void OnUnbunchingClick(UIComponent component, UIMouseEventParameter p)
        {
            CachedNodeData.m_cachedNodeData[(int) this.m_InstanceID.NetNode].Unbunching = !CachedNodeData
                .m_cachedNodeData[(int) this.m_InstanceID.NetNode]
                .Unbunching;
        }

        private void OnUpdateCloseStopsClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.ProcessNodes(
                (System.Action<ushort>) (nodeID => CachedNodeData.m_cachedNodeData[(int) nodeID].Unbunching =
                    CachedNodeData.m_cachedNodeData[(int) this.m_InstanceID.NetNode].Unbunching));
        }

        private void OnModifyLineClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (this.m_InstanceID.Type != InstanceType.NetNode || (int) this.m_InstanceID.NetNode == 0)
                return;
            ushort transportLine = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) this.m_InstanceID.NetNode]
                .m_transportLine;
            InstanceID instanceID = new InstanceID();
            instanceID.TransportLine = transportLine;
            this.Hide();
            WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(this.m_WorldMousePosition, instanceID);
        }

        private void OnPreviousStopClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.ChangeInstanceID(this.m_InstanceID, new InstanceID()
            {
                NetNode = TransportLine.GetPrevStop(this.m_InstanceID.NetNode)
            });
        }

        private void OnNextStopClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            this.ChangeInstanceID(this.m_InstanceID, new InstanceID()
            {
                NetNode = TransportLine.GetNextStop(this.m_InstanceID.NetNode)
            });
        }

        private void OnDeleteStopClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (this.m_InstanceID.Type != InstanceType.NetNode || (int) this.m_InstanceID.NetNode == 0)
                return;
            ushort transportLine = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) this.m_InstanceID.NetNode]
                .m_transportLine;
            if (!Singleton<TransportManager>.instance.m_lines.m_buffer[(int) transportLine]
                .RemoveStop(transportLine, this.m_StopIndex))
                return;
            this.Hide();
        }

        private void ProcessNodes(System.Action<ushort> processNode)
        {
            NetManager instance = Singleton<NetManager>.instance;
            bool flag = false;
            NetNode netNode1 = instance.m_nodes.m_buffer[(int) this.m_InstanceID.NetNode];
            Vector3 position = netNode1.m_position;
            ItemClass.SubService subService = netNode1.Info.m_class.m_subService;
            ushort building = Singleton<BuildingManager>.instance.FindBuilding(position, 100f,
                ItemClass.Service.PublicTransport, subService, Building.Flags.Active, Building.Flags.Untouchable);
            if ((int) building != 0)
            {
                ushort[] stationStops = PanelExtenderCityService.GetStationStops(building);
                if (stationStops.Length != 0 &&
                    ((IEnumerable<ushort>) stationStops).Contains<ushort>(this.m_InstanceID.NetNode))
                {
                    flag = true;
                    for (int index = 0; index < stationStops.Length; ++index)
                        processNode(stationStops[index]);
                }
            }
            if (flag)
                return;
            for (int index = 0; index < 32768; ++index)
            {
                NetNode netNode2 = instance.m_nodes.m_buffer[index];
                if (netNode2.m_flags != NetNode.Flags.None && (int) netNode2.m_transportLine != 0 &&
                    position == netNode2.m_position)
                    processNode((ushort) index);
            }
        }

        public void Show(Vector3 worldMousePosition, InstanceID instanceID)
        {
            this.m_WorldMousePosition = worldMousePosition;
            this.m_InstanceID = instanceID;
            if (InstanceManager.IsValid(this.m_InstanceID))
            {
                WorldInfoPanel.HideAllWorldInfoPanels();
                NetManager instance = Singleton<NetManager>.instance;
                ushort transportLine = instance.m_nodes.m_buffer[(int) this.m_InstanceID.NetNode].m_transportLine;
                this.m_VehicleType.spriteName = PublicTransportWorldInfoPanel.GetVehicleTypeIcon(
                    Singleton<TransportManager>.instance.m_lines.m_buffer[(int) transportLine].Info.m_transportType);
                this.m_StopIndex = TransportLineUtil.GetStopIndex(transportLine, this.m_InstanceID.NetNode);
                this.m_StopName.text = Singleton<InstanceManager>.instance.GetName(this.m_InstanceID) ??
                                       string.Format(Localization.Get("STOP_LIST_BOX_ROW_STOP"),
                                           (object) (this.m_StopIndex + 1));
                this.m_SuggestedNames.ClearItems();
                this.m_SuggestedNames.AddItems(
                    this.FindBuildings(instance.m_nodes.m_buffer[(int) this.m_InstanceID.NetNode].m_position),
                    new Func<ushort, string>(this.IDToName));
                this.m_Line.text = Singleton<TransportManager>.instance.GetLineName(transportLine);
                this.Show();
                this.LateUpdate();
            }
            else
            {
                this.Hide();
            }
        }

        private void ChangeInstanceID(InstanceID oldID, InstanceID newID)
        {
            if (this.m_InstanceID.IsEmpty || !(this.m_InstanceID == oldID))
                return;
            this.Show(Singleton<NetManager>.instance.m_nodes.m_buffer[(int) newID.NetNode].m_position, newID);
            ToolsModifierControl.cameraController.SetTarget(newID,
                ToolsModifierControl.cameraController.transform.position,
                Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift));
        }

        private void CheckForClose()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                this.Hide();
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !this.Raycast(this.GetCamera().ScreenPointToRay(Input.mousePosition)))
                {
                    //TODO(earalov): restore
                    //this.Hide();
                }
            }
        }

        private void CreateButton()
        {
            UIButton button = UIUtils.CreateButton(this.m_unbunching.parent);
            button.name = "UpdateCloseStops";
            button.autoSize = true;
            button.textPadding = new RectOffset(10, 10, 4, 2);
            button.anchor = UIAnchorStyle.Left | UIAnchorStyle.CenterVertical;
            button.text = Localization.Get("STOP_PANEL_UPDATE_CLOSE_STOPS");
            button.tooltip = Localization.Get("STOP_PANEL_UPDATE_CLOSE_STOPS_TOOLTIP");
            button.textScale = 0.75f;
            button.eventClick += new MouseEventHandler(this.OnUpdateCloseStopsClick);
            this.m_closeStopsUnbunching = button;
        }

        private void UpdatePosition()
        {
            if ((UnityEngine.Object) this.m_CameraTransform != (UnityEngine.Object) null)
            {
                Vector3 position;
                Quaternion rotation;
                Vector3 size;
                if (InstanceManager.GetPosition(this.m_InstanceID, out position, out rotation, out size))
                    position.y += size.y * 0.8f;
                else
                    position = this.m_WorldMousePosition;
                Vector3 vector3_1 = Camera.main.WorldToScreenPoint(position) *
                                    Mathf.Sign(Vector3.Dot(position - this.m_CameraTransform.position,
                                        this.m_CameraTransform.forward));
                Vector2 vector2 = !((UnityEngine.Object) this.m_FullscreenContainer != (UnityEngine.Object) null)
                    ? this.m_uiView.GetScreenResolution()
                    : this.m_FullscreenContainer.size;
                Vector3 vector3_2 = vector3_1 / this.m_uiView.inputScale;
                Vector3 transform = this.pivot.UpperLeftToTransform(this.size, this.arbitraryPivotOffset);
                Vector3 vector3_3 = (Vector3) (this.m_uiView.ScreenPointToGUI((Vector2) vector3_2) +
                                               new Vector2(transform.x, transform.y));
                if ((double) vector3_3.x < 0.0)
                    vector3_3.x = 0.0f;
                if ((double) vector3_3.y < 0.0)
                    vector3_3.y = 0.0f;
                if ((double) vector3_3.x + (double) this.width > (double) vector2.x)
                    vector3_3.x = vector2.x - this.width;
                if ((double) vector3_3.y + (double) this.height > (double) vector2.y)
                    vector3_3.y = vector2.y - this.height;
                this.relativePosition = vector3_3;
            }
            this.m_SuggestedNames.ListPosition = this.m_StopName.absolutePosition +
                                                 new Vector3(0.0f, this.m_StopName.height);
        }

        private void UpdateBindings()
        {
            ushort netNode = this.m_InstanceID.NetNode;
            int num1 = WaitingPassengerCountQuery.Query(netNode, out _, out var max);
            byte num2 = (byte) ((uint) byte.MaxValue - (uint) max);
            this.m_PassengerCount.text = string.Format(Localization.Get("STOP_PANEL_WAITING_PEOPLE"), (object) num1);
            this.m_BoredCountdown.text = string.Format(Localization.Get("STOP_PANEL_BORED_TIMER"),
                (object) ColorUtility.ToHtmlStringRGB(this.GetColor(num2)), (object) num2);
            this.m_passengersInCurrent.text = CachedNodeData.m_cachedNodeData[(int) netNode].PassengersIn.ToString();
            this.m_passengersInLast.text = CachedNodeData.m_cachedNodeData[(int) netNode]
                .LastWeekPassengersIn.ToString();
            this.m_passengersInAverage.text = CachedNodeData.m_cachedNodeData[(int) netNode]
                .AveragePassengersIn.ToString();
            this.m_passengersOutCurrent.text = CachedNodeData.m_cachedNodeData[(int) netNode].PassengersOut.ToString();
            this.m_passengersOutLast.text = CachedNodeData.m_cachedNodeData[(int) netNode]
                .LastWeekPassengersOut.ToString();
            this.m_passengersOutAverage.text = CachedNodeData.m_cachedNodeData[(int) netNode]
                .AveragePassengersOut.ToString();
            this.m_passengersTotalCurrent.text = CachedNodeData.m_cachedNodeData[(int) netNode]
                .PassengersTotal.ToString();
            this.m_passengersTotalLast.text = CachedNodeData.m_cachedNodeData[(int) netNode]
                .LastWeekPassengersTotal.ToString();
            this.m_passengersTotalAverage.text = CachedNodeData.m_cachedNodeData[(int) netNode]
                .AveragePassengersTotal.ToString();
            if ((int) OptionsWrapper<Settings.Settings>.Options.IntervalAggressionFactor == 0)
            {
                this.m_unbunching.Disable();
                this.m_unbunching.isChecked = false;
                this.m_unbunching.label.text = Localization.Get("UNBUNCHING_DISABLED");
                this.m_unbunching.size = new Vector2(this.m_unbunching.label.width + 22f, 16f);
                this.m_closeStopsUnbunching.Hide();
            }
            else
            {
                this.m_unbunching.Enable();
                this.m_unbunching.isChecked = CachedNodeData.m_cachedNodeData[(int) netNode].Unbunching;
                this.m_unbunching.label.text = Localization.Get("UNBUNCHING_ENABLED");
                this.m_unbunching.size = new Vector2(this.m_unbunching.label.width + 22f, 16f);
                this.m_closeStopsUnbunching.Show();
            }
        }

        private void CheckForAltKey()
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                this.m_DeleteStop.Enable();
            else
                this.m_DeleteStop.Disable();
        }

        private Color GetColor(byte value)
        {
            float num = (float) ((double) value / (double) byte.MaxValue * 100.0);
            if ((double) num >= 50.0)
                return Color.green;
            if ((double) num >= 25.0)
                return Color.yellow;
            return Color.red;
        }

        private ushort[] FindBuildings(Vector3 position)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            HashSet<ushort> source = new HashSet<ushort>();
            foreach (ItemClass.Service service in Enum.GetValues(typeof(ItemClass.Service)))
            {
                foreach (ItemClass.SubService subService in Enum.GetValues(typeof(ItemClass.SubService)))
                {
                    ushort building = instance.FindBuilding(position, 100f, service, subService, Building.Flags.Active,
                        Building.Flags.Untouchable);
                    if ((int) building != 0)
                        source.Add(building);
                }
            }
            return source.ToArray<ushort>();
        }

        private string IDToName(ushort ID)
        {
            return Singleton<BuildingManager>.instance.GetBuildingName(ID, InstanceID.Empty) ?? "";
        }
    }
}