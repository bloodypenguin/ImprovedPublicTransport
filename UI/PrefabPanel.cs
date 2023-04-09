// <copyright file="BuildingPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ImprovedPublicTransport2.UI.AlgernonCommons;

namespace ImprovedPublicTransport2.UI
{
    using System;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Building info panel.
    /// </summary>
    internal class PrefabPanel : UIPanel
    {
        /// <summary>
        /// Layout margin.
        /// </summary>
        protected const float Margin = 5f;

        // Layout constants - private.
        private const float TitleHeight = 40f;
        private const float NameLabelY = TitleHeight + Margin;
        private const float NameLabelHeight = 30f;
        private const float AreaLabelHeight = 20f;
        private const float AreaLabel1Y = TitleHeight + NameLabelHeight;
        private const float AreaLabel2Y = AreaLabel1Y + AreaLabelHeight;
        private const float ListY = AreaLabel2Y + AreaLabelHeight + Margin;
        private const float VehicleSelectionHeight = VehicleSelection.PanelHeight + Margin;
        private const float NoPanelHeight = ListY + VehicleSelectionHeight +Margin;
        private const float IconButtonSize = 40f;
        private const float IconButtonY = ListY - IconButtonSize - Margin;
        private const float PasteButtonX = PanelWidth - IconButtonSize - Margin;
        private const float CopyButtonX = PasteButtonX - IconButtonSize - Margin;
        private const float CopyBuildingButtonX = CopyButtonX - IconButtonSize - Margin;
        private const float CopyDistrictButtonX = CopyBuildingButtonX - IconButtonSize - Margin;
        private const float PanelWidth = VehicleSelection.PanelWidth + Margin + Margin;

        // Panel components.
        private UILabel _buildingLabel;
        private UIButton _copyButton;
        private UIButton _pasteButton;
        private UIButton _copyBuildingButton;
        private UIButton _copyDistrictButton;

        // Sub-panels.
        private VehicleSelection _vehicleSelection;

        // Status flag.
        private bool _panelReady = false;

        // Current selections.
        private ushort _currentLineID;
        private TransportLine _thisLine;
        private byte _currentDistrict;
        private byte _currentPark;

        // Event handling.
        private bool _copyProcessing = false;
        private bool _pasteProcessing = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrefabPanel"/> class.
        /// </summary>
        public PrefabPanel()
        {
            base.Awake();
            try
            {
                // Basic setup.
                autoLayout = false;
                backgroundSprite = "UnlockingPanel2";
                isVisible = true;
                canFocus = true;
                isInteractive = true;
                width = PanelWidth;
                height = NoPanelHeight;

                // Default position - centre in screen.
                relativePosition = new Vector2(Mathf.Floor((GetUIView().fixedWidth - PanelWidth) / 2),
                    (GetUIView().fixedHeight - NoPanelHeight) / 2);

                // Title label.
                UILabel titleLabel = UILabels.AddLabel(this, 0f, 10f, ImprovedPublicTransportMod.BaseModName, PanelWidth, 1.2f);
                titleLabel.textAlignment = UIHorizontalAlignment.Center;

                // Building label.
                _buildingLabel = UILabels.AddLabel(this, 0f, NameLabelY, string.Empty, PanelWidth);
                _buildingLabel.textAlignment = UIHorizontalAlignment.Center;

                // Drag handle.
                UIDragHandle dragHandle = this.AddUIComponent<UIDragHandle>();
                dragHandle.relativePosition = Vector3.zero;
                dragHandle.width = PanelWidth - 35f;
                dragHandle.height = TitleHeight;

                // Close button.
                UIButton closeButton = AddUIComponent<UIButton>();
                closeButton.relativePosition = new Vector2(width - 35f, 2f);
                closeButton.normalBgSprite = "buttonclose";
                closeButton.hoveredBgSprite = "buttonclosehover";
                closeButton.pressedBgSprite = "buttonclosepressed";

                // Close button event handler.
                closeButton.eventClick += (component, clickEvent) => { PrefabPanelManager.Close(); };

                // Zoom to building button.
                UIButton zoomButton = AddZoomButton(this, Margin, Margin, 30f, "ZOOM_BUILDING");
                zoomButton.eventClicked += (c, p) => ZoomToLine(_currentLineID);

                // Copy/paste buttons.
                _copyButton = UIButtons.AddIconButton(this, CopyButtonX, IconButtonY, IconButtonSize,
                    UITextures.LoadQuadSpriteAtlas("IPT2-Copy"), Localization.Get("COPY_TIP"));
                _copyButton.Hide(); //TODO: restore
                _copyButton.eventClicked += (c, p) => CopyPaste.Instance.Copy(_currentLineID);
                _pasteButton = UIButtons.AddIconButton(this, PasteButtonX, IconButtonY, IconButtonSize,
                    UITextures.LoadQuadSpriteAtlas("IPT2-Paste"), Localization.Get("PASTE_TIP"));
                _pasteButton.Hide(); //TODO: restore
                _pasteButton.eventClicked += (c, p) => Paste();

                // Copy to buttons.
                _copyBuildingButton = UIButtons.AddIconButton(
                    this,
                    CopyBuildingButtonX,
                    IconButtonY,
                    IconButtonSize,
                    UITextures.LoadQuadSpriteAtlas("IPT2-CopyBuilding"),
                    Localization.Get("COPY_BUILDING_TIP"));
                _copyBuildingButton.eventClicked += (c, p) => CopyPaste.Instance.CopyToBuildings(_currentLineID, 0, 0);
                _copyBuildingButton.Hide(); //TODO: restore
                _copyDistrictButton = UIButtons.AddIconButton(
                    this,
                    CopyDistrictButtonX,
                    IconButtonY,
                    IconButtonSize,
                    UITextures.LoadQuadSpriteAtlas("IPT2-CopyDistrict"),
                    Localization.Get("COPY_DISTRICT_TIP"));
                _copyDistrictButton.eventClicked += (c, p) =>
                    CopyPaste.Instance.CopyToBuildings(_currentLineID, _currentDistrict, _currentPark);
                _copyDistrictButton.Hide(); //TODO: restore

                _vehicleSelection = AddUIComponent<VehicleSelection>();
                _vehicleSelection.ParentPanel = this;
                _vehicleSelection.relativePosition = new Vector3(Margin, ListY);

                // Enable events.
                _panelReady = true;
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception setting up building panel");
            }
        }

        /// <summary>
        /// Gets the current building ID.
        /// </summary>
        internal ushort CurrentLineID => _currentLineID;

        /// <summary>
        /// Gets or sets a value indicating whether this is an incoming (true) or outgoing (false) transfer.
        /// </summary>
        internal bool IsIncoming { get; set; }

        /// <summary>
        /// Gets or sets the current transfer reason.
        /// </summary>
        internal TransferManager.TransferReason TransferReason { get; set; }

        /// <summary>
        /// Called by Unity every update.
        /// Used to check for copy/paste keypress.
        /// </summary>
        public override void Update()
        {
            // Don't do anything if not set up yet.
            if (!_panelReady)
            {
                return;
            }

            //TODO: restore
            // Copy key processing - use event flag to avoid repeated triggering.
            // if (ModSettings.KeyCopy.IsPressed())
            // {
            //     if (!_copyProcessing)
            //     {
            //         CopyPaste.Instance.Copy(CurrentLineID);
            //         _copyProcessing = true;
            //
            //         // Update paste button state.
            //         _pasteButton.isEnabled = CopyPaste.Instance.IsValidTarget(CurrentLineID);
            //     }
            // }
            // else
            // {
            //     // Key no longer down - resume processing of events.
            //     _copyProcessing = false;
            // }

            // Paste key processing - use event flag to avoid repeated triggering.
            // if (ModSettings.KeyPaste.IsPressed())
            // {
            //     if (!_pasteProcessing)
            //     {
            //         Paste();
            //         _pasteProcessing = true;
            //     }
            // }
            // else
            // {
            //     // Key no longer down - resume processing of events.
            //     _pasteProcessing = false;
            // }

            base.Update();
        }

        /// <summary>
        /// Adds an zoom icon button.
        /// </summary>
        /// <param name="parent">Parent UIComponent.</param>
        /// <param name="xPos">Relative X position.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="size">Button size.</param>
        /// <param name="tooltipKey">Tooltip translation key.</param>
        /// <returns>New UIButton.</returns>
        internal static UIButton AddZoomButton(UIComponent parent, float xPos, float yPos, float size,
            string tooltipKey)
        {
            UIButton newButton = parent.AddUIComponent<UIButton>();

            // Size and position.
            newButton.relativePosition = new Vector2(xPos, yPos);
            newButton.height = size;
            newButton.width = size;

            // Appearance.
            newButton.atlas = UITextures.InGameAtlas;
            newButton.normalFgSprite = "LineDetailButtonHovered";
            newButton.focusedFgSprite = "LineDetailButtonFocused";
            newButton.hoveredFgSprite = "LineDetailButton";
            newButton.disabledFgSprite = "LineDetailButtonDisabled";
            newButton.pressedFgSprite = "LineDetailButtonPressed";

            // Tooltip.
            newButton.tooltip = Localization.Get(tooltipKey);

            return newButton;
        }

        /// <summary>
        /// Zooms to the specified building.
        /// </summary>
        /// <param name="lineID">Target line ID.</param>
        internal static void ZoomToLine(ushort lineID)
        {
            // Go to target line if available.
            if (lineID != 0)
            {
                // Clear existing target fist to force a re-zoom-in if required.
                ToolsModifierControl.cameraController.ClearTarget();

                InstanceID instance = default;
                instance.TransportLine = lineID;
                var vehicles = Singleton<TransportManager>.instance.m_lines.m_buffer[lineID].m_vehicles;
                ToolsModifierControl.cameraController.SetTarget(instance,
                    VehicleManager.instance.m_vehicles.m_buffer[vehicles].GetLastFramePosition(), zoomIn: true);
            }
        }

        /// <summary>
        /// Sets/changes the currently selected building.
        /// </summary>
        /// <param name="lineID">New building ID.</param>
        internal void SetTarget(ushort lineID)
        {
            // Local references.
            TransportManager transportManager = Singleton<TransportManager>.instance;

            // Update selected building ID.
            _currentLineID = lineID;
            _thisLine = transportManager.m_lines.m_buffer[_currentLineID];

            // Set up used panels.
            _vehicleSelection.SetTarget(lineID, Localization.Get("LINE_PANEL_SELECT_TYPES"));
            _vehicleSelection.Show();

            // Set panel height.
            height = NoPanelHeight;

            // Set name.
            _buildingLabel.text = transportManager.GetLineName(_currentLineID);
            
            // Make sure we're fully visible on-screen.
            if (absolutePosition.y + height > Screen.height - 120)
            {
                absolutePosition = new Vector2(absolutePosition.x, Screen.height - 120 - height);
            }

            if (absolutePosition.x + width > Screen.width - 20)
            {
                absolutePosition = new Vector2(Screen.width - 20 - width, absolutePosition.y);
            }

            if (absolutePosition.y < 20f)
            {
                absolutePosition = new Vector2(absolutePosition.x, 20f);
            }

            if (absolutePosition.x < 20f)
            {
                absolutePosition = new Vector2(20f, absolutePosition.y);
            }

            // Update button states.
            _pasteButton.isEnabled = CopyPaste.Instance.IsValidTarget(CurrentLineID);
            _copyDistrictButton.isEnabled = _currentDistrict != 0 | _currentPark != 0;

            // Make sure we're visible if we're not already.
            Show();
        }

        /// <summary>
        /// Paste data action.
        /// </summary>
        private void Paste()
        {
            //TODO: restore
            // // Paste data.
            // CopyPaste.Instance.Paste(CurrentLineID);
            //
            // // Update lists.
            // foreach (VehicleSelection vehicleSelection in _vehicleSelection)
            // {
            //     vehicleSelection.Refresh();
            // }
        }
    }
}