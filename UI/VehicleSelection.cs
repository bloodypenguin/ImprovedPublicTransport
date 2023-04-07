// <copyright file="VehicleSelection.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using ColossalFramework.UI;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using ImprovedPublicTransport2.UI.PreviewRenderer;
using JetBrains.Annotations;
using UnityEngine;

namespace ImprovedPublicTransport2.UI
{
    /// <summary>
    /// Warehouse vehicle controls.
    /// </summary>
    internal class VehicleSelection : UIPanel
    {
        /// <summary>
        /// Panel height.
        /// </summary>
        internal const float PanelHeight = VehicleListY + VehicleListHeight + Margin;

        /// <summary>
        /// List height.
        /// </summary>
        internal const float VehicleListHeight = 200f;

        /// <summary>
        /// Selection list column width.
        /// </summary>
        internal const float ListWidth = 340f;

        /// <summary>
        /// Preview column width.
        /// </summary>
        internal const float PreviewWidth = 150f;

        /// <summary>
        /// Panel width.
        /// </summary>
        internal const float PanelWidth = RightColumnX + ListWidth + Margin;

        // Layout constants - private.
        private const float Margin = 5f;
        private const float TitleOffsetY = 40f;
        private const float VehicleListY = 70f;
        private const float ArrowSize = 32f;
        private const float MidControlX = Margin + ListWidth + Margin;
        private const float RightColumnX = MidControlX + PreviewWidth + Margin;

        // Panel components.
        private UILabel _titleLabel;
        private UIButton _addButton;
        private UIButton _removeButton;
        private UIButton _addAllButton;
        private UIButton _removeAllButton;
        private AvailableVehiclePanel _availableVehiclePanel;
        private SelectedVehiclePanel _selectedVehiclePanel;
        private PreviewPanel _previewPanel;

        // Currently selected vehicles.
        private VehicleInfo _selectedLineVehicle;
        private VehicleInfo _selectedAvailableVehicle;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleSelection"/> class.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            // Set size.
            height = PanelHeight;
            width = PanelWidth;

            // Appearance.
            atlas = UITextures.InGameAtlas;
            backgroundSprite = "GenericPanelLight";
            color = new Color32(160, 160, 160, 255);

            // Title.
            _titleLabel = UILabels.AddLabel(this, 0f, 10f, "Select vehicle", PanelWidth, 1f,
                UIHorizontalAlignment.Center);

            // 'Add vehicle' button.
            _addButton = UIButtons.AddIconButton(
                this,
                RightColumnX - ArrowSize - Margin,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("IPT2-Add"),
                Localization.Get("LINE_PANEL_ADD_VEHICLE"));
            _addButton.isEnabled = false;
            _addButton.eventClicked += (c, p) => AddVehicle(_selectedAvailableVehicle);

            // 'Add all vehicles' button.
            _addAllButton = UIButtons.AddIconButton(
                this,
                RightColumnX - ArrowSize - Margin - ArrowSize - Margin,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("IPT2-AddAll"),
                Localization.Get("ADD_ALL_TIP"));
            _addAllButton.isEnabled = false;
            _addAllButton.eventClicked += (c, p) => AddAllVehicles();

            // Remove vehicle button.
            _removeButton = UIButtons.AddIconButton(
                this,
                MidControlX,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("IPT2-Remove"),
                Localization.Get("LINE_PANEL_REMOVE_VEHICLE"));
            _removeButton.isEnabled = false;
            _removeButton.eventClicked += (c, p) => RemoveVehicle();

            // 'Remove all vehicles' button.
            _removeAllButton = UIButtons.AddIconButton(
                this,
                MidControlX + ArrowSize + Margin,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("IPT2-RemoveAll"),
                Localization.Get("REMOVE_ALL_TIP"));
            _removeAllButton.isEnabled = false;
            _removeAllButton.eventClicked += (c, p) => RemoveAllVehicles();

            // Vehicle selection panels.
            _selectedVehiclePanel = this.AddUIComponent<SelectedVehiclePanel>();
            _selectedVehiclePanel.relativePosition = new Vector2(Margin, VehicleListY);
            _selectedVehiclePanel.ParentPanel = this;
            _availableVehiclePanel = this.AddUIComponent<AvailableVehiclePanel>();
            _availableVehiclePanel.ParentPanel = this;
            _availableVehiclePanel.relativePosition = new Vector2(RightColumnX, VehicleListY);

            // Vehicle selection list labels.
            UILabels.AddLabel(_availableVehiclePanel.VehicleList, 0f, -TitleOffsetY,
                Localization.Get("AVAILABLE_VEHICLES"), ListWidth, 0.8f, UIHorizontalAlignment.Center);
            UILabels.AddLabel(_selectedVehiclePanel.VehicleList, 0f, -TitleOffsetY,
                Localization.Get("SELECTED_VEHICLES"), ListWidth, 0.8f, UIHorizontalAlignment.Center);

            // Preview panel.
            _previewPanel = AddUIComponent<PreviewPanel>();
            _previewPanel.relativePosition = new Vector2(MidControlX, VehicleListY + ArrowSize + Margin);
        }

        /// <summary>
        /// Sets the currently selected vehicle from the list of currently selected vehicles.
        /// </summary>
        internal VehicleInfo SelectedLineVehicle
        {
            set
            {
                _selectedLineVehicle = value;

                if (value != null)
                {
                    // Clear other vehicle list selection if this is active.
                    _availableVehiclePanel.ClearSelection();
                    _previewPanel.lineColor = TransportManager.instance.m_lines.m_buffer[CurrentLine].m_color;
                    _previewPanel.SetTarget(value);
                }
                else
                {
                    // Null value set; clear list selection.
                    _selectedVehiclePanel.ClearSelection();
                }

                // Update button states.
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// Sets the currently selected vehicle from the list of all currently unselected vehicles.
        /// </summary>
        internal VehicleInfo SelectedListVehicle
        {
            set
            {
                _selectedAvailableVehicle = value;

                if (value != null)
                {
                    // Clear other vehicle list selection if this is active.
                    _selectedVehiclePanel.ClearSelection();
                    _previewPanel.lineColor = TransportManager.instance.m_lines.m_buffer[CurrentLine].m_color;
                    _previewPanel.SetTarget(value);
                }
                else
                {
                    // Null value set; clear list selection.
                    _availableVehiclePanel.ClearSelection();
                }

                // Update button states.
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// Gets or sets the parent tab reference.
        /// </summary>
        internal PrefabPanel ParentPanel { get; set; }

        /// <summary>
        /// Gets the currently selected line.
        /// </summary>
        internal ushort CurrentLine { get; private set; }

        /// <summary>
        /// Sets/changes the currently selected line.
        /// </summary>
        /// <param name="lineID">New line ID.</param>
        /// <param name="title">Selection list title string.</param>
        internal void SetTarget(ushort lineID, string title)
        {
            // Ensure valid line.
            if (lineID != 0)
            {
                CurrentLine = lineID;
                _titleLabel.text = title;

                // Regenerate lists and set button states..
                Refresh();
            }
        }

        /// <summary>
        /// Refreshes list contents, clears the preview display, and updates button states.
        /// </summary>
        private void Refresh()
        {
            // Clear preview.
            _previewPanel.SetTarget(null);

            _selectedVehiclePanel.RefreshList();
            _availableVehiclePanel.RefreshList();

            UpdateButtonStates();
        }

        /// <summary>
        /// Updates button states according to the current state.
        /// </summary>
        private void UpdateButtonStates()
        {
            _addButton.isEnabled = _selectedAvailableVehicle != null;
            _addAllButton.isEnabled = _availableVehiclePanel.VehicleList.Data.m_size > 0;
            _removeButton.isEnabled = _selectedLineVehicle != null;
            _removeAllButton.isEnabled = _selectedVehiclePanel.VehicleList.Data.m_size > 0;
        }

        /// <summary>
        /// Adds a vehicle to the list for this line.
        /// </summary>
        /// <param name="vehicle">Vehicle prefab to add.</param>
        private void AddVehicle([CanBeNull] VehicleInfo vehicle)
        {
            if (vehicle == null)
            {
                return;
            }
            var vehicleInfos = Query.SelectedVehicleTypesQuery.Query(CurrentLine) ?? new List<VehicleInfo>();
            vehicleInfos.Add(vehicle);
            Command.SelectVehicleTypesCommand.Execute(vehicleInfos);
            Refresh();
        }

        /// <summary>
        /// Removes the currently selected vehicle from the list for this line.
        /// </summary>
        private void RemoveVehicle()
        {
            if (_selectedLineVehicle == null)
            {
                return;
            }
            var vehicleInfos = Query.SelectedVehicleTypesQuery.Query(CurrentLine) ?? new List<VehicleInfo>();
            vehicleInfos.Remove(_selectedLineVehicle);
            Command.SelectVehicleTypesCommand.Execute(vehicleInfos);
            Refresh();
        }

        /// <summary>
        /// Adds all vehicle types in the available vehicle list to this line.
        /// </summary>
        private void AddAllVehicles()
        {
            var vehicleInfos = Query.SelectedVehicleTypesQuery.Query(CurrentLine) ?? new List<VehicleInfo>();
            foreach (VehicleItem item in _availableVehiclePanel.VehicleList.Data)
            {
                vehicleInfos.Add(item.Info);
            }
            Command.SelectVehicleTypesCommand.Execute(vehicleInfos);
            Refresh();
        }

        /// <summary>
        /// Removes all vehicle types from this line.
        /// </summary>
        private void RemoveAllVehicles()
        {
            Command.SelectVehicleTypesCommand.Execute(new List<VehicleInfo>());
            Refresh();
        }
    }
}