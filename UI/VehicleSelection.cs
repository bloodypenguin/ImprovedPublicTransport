// <copyright file="VehicleSelection.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ColossalFramework.UI;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using ImprovedPublicTransport2.UI.PreviewRenderer;
using UnityEngine;
using VehicleSelector;

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
        private readonly UILabel _titleLabel;
        private readonly UIButton _addButton;
        private readonly UIButton _removeButton;
        private readonly UIButton _addAllButton;
        private readonly UIButton _removeAllButton;
        private readonly VehicleSelectionPanel _vehicleSelectionPanel;
        private readonly SelectedVehiclePanel _selectedVehiclePanel;
        private readonly PreviewPanel _previewPanel;

        // Currently selected vehicles.
        private VehicleInfo _selectedBuildingVehicle;
        private VehicleInfo _selectedListVehicle;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleSelection"/> class.
        /// </summary>
        internal VehicleSelection()
        {
            // Set size.
            height = PanelHeight;
            width = PanelWidth;

            // Appearance.
            atlas = UITextures.InGameAtlas;
            backgroundSprite = "GenericPanelLight";
            color = new Color32(160, 160, 160, 255);

            // Title.
            _titleLabel = UILabels.AddLabel(this, 0f, 10f, "Select vehicle", PanelWidth, 1f, UIHorizontalAlignment.Center);

            // 'Add vehicle' button.
            _addButton = UIButtons.AddIconButton(
                this,
                RightColumnX - ArrowSize - Margin,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("VS-Add"),
                Localization.Get("ADD_VEHICLE_TIP"));
            _addButton.isEnabled = false;
            _addButton.eventClicked += (c, p) => AddVehicle(_selectedListVehicle);

            // 'Add all vehicles' button.
            _addAllButton = UIButtons.AddIconButton(
                this,
                RightColumnX - ArrowSize - Margin - ArrowSize - Margin,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("VS-AddAll"),
                Localization.Get("ADD_ALL_TIP"));
            _addAllButton.isEnabled = false;
            _addAllButton.eventClicked += (c, p) => AddAllVehicles();

            // Remove vehicle button.
            _removeButton = UIButtons.AddIconButton(
                this,
                MidControlX,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("VS-Remove"),
                Localization.Get("REMOVE_VEHICLE_TIP"));
            _removeButton.isEnabled = false;
            _removeButton.eventClicked += (c, p) => RemoveVehicle();

            // 'Remove all vehicles' button.
            _removeAllButton = UIButtons.AddIconButton(
                this,
                MidControlX + ArrowSize + Margin,
                VehicleListY,
                ArrowSize,
                UITextures.LoadQuadSpriteAtlas("VS-RemoveAll"),
                Localization.Get("REMOVE_ALL_TIP"));
            _removeAllButton.isEnabled = false;
            _removeAllButton.eventClicked += (c, p) => RemoveAllVehicles();

            // Vehicle selection panels.
            _selectedVehiclePanel = this.AddUIComponent<SelectedVehiclePanel>();
            _selectedVehiclePanel.relativePosition = new Vector2(Margin, VehicleListY);
            _selectedVehiclePanel.ParentPanel = this;
            _vehicleSelectionPanel = this.AddUIComponent<VehicleSelectionPanel>();
            _vehicleSelectionPanel.ParentPanel = this;
            _vehicleSelectionPanel.relativePosition = new Vector2(RightColumnX, VehicleListY);

            // Vehicle selection list labels.
            UILabels.AddLabel(_vehicleSelectionPanel.VehicleList, 0f, -TitleOffsetY, Localization.Get("AVAILABLE_VEHICLES"), ListWidth, 0.8f, UIHorizontalAlignment.Center);
            UILabels.AddLabel(_selectedVehiclePanel.VehicleList, 0f, -TitleOffsetY, Localization.Get("SELECTED_VEHICLES"), ListWidth, 0.8f, UIHorizontalAlignment.Center);

            // Preview panel.
            _previewPanel = AddUIComponent<PreviewPanel>();
            _previewPanel.relativePosition = new Vector2(MidControlX, VehicleListY + ArrowSize + Margin);
        }

        /// <summary>
        /// Sets the currently selected vehicle from the list of currently selected vehicles.
        /// </summary>
        internal VehicleInfo SelectedBuildingVehicle
        {
            set
            {
                _selectedBuildingVehicle = value;

                if (value != null)
                {
                    // Clear other vehicle list selection if this is active.
                    _vehicleSelectionPanel.ClearSelection();
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
                _selectedListVehicle = value;

                if (value != null)
                {
                    // Clear other vehicle list selection if this is active.
                    _selectedVehiclePanel.ClearSelection();
                    _previewPanel.SetTarget(value);
                }
                else
                {
                    // Null value set; clear list selection.
                    _vehicleSelectionPanel.ClearSelection();
                }

                // Update button states.
                UpdateButtonStates();
            }
        }

        /// <summary>
        /// Gets or sets the parent tab reference.
        /// </summary>
        internal BuildingPanel ParentPanel { get; set; }

        /// <summary>
        /// Gets the current transfer reason.
        /// </summary>
        internal TransferManager.TransferReason TransferReason { get; private set; }

        /// <summary>
        /// Gets the currently selected building.
        /// </summary>
        internal ushort CurrentBuilding { get; private set; }

        /// <summary>
        /// Sets/changes the currently selected building.
        /// </summary>
        /// <param name="buildingID">New building ID.</param>
        /// <param name="title">Selection list title string.</param>
        /// <param name="reason">Transfer reason for this vehicle selection.</param>
        internal void SetTarget(ushort buildingID, string title, TransferManager.TransferReason reason)
        {
            // Ensure valid building.
            if (buildingID != 0)
            {
                CurrentBuilding = buildingID;
                TransferReason = reason;
                _titleLabel.text = title;

                // Regenerate lists and set button states..
                Refresh();
            }
        }

        /// <summary>
        /// Refreshes list contents, clears the preview display, and updates button states.
        /// </summary>
        internal void Refresh()
        {
            // Clear preview.
            _previewPanel.SetTarget(null);

            _selectedVehiclePanel.RefreshList();
            _vehicleSelectionPanel.RefreshList();

            UpdateButtonStates();
        }

        /// <summary>
        /// Updates button states according to the current state.
        /// </summary>
        internal void UpdateButtonStates()
        {
            _addButton.isEnabled = _selectedListVehicle != null;
            _addAllButton.isEnabled = _vehicleSelectionPanel.VehicleList.Data.m_size > 0;
            _removeButton.isEnabled = _selectedBuildingVehicle != null;
            _removeAllButton.isEnabled = _selectedVehiclePanel.VehicleList.Data.m_size > 0;
        }

        /// <summary>
        /// Adds a vehicle to the list for this transfer.
        /// </summary>
        /// <param name="vehicle">Vehicle prefab to add.</param>
        private void AddVehicle(VehicleInfo vehicle)
        {
            // Add vehicle to building.
            VehicleControl.AddVehicle(CurrentBuilding, TransferReason, vehicle);

            // Update lists.
            Refresh();
        }

        /// <summary>
        /// Removes the currently selected vehicle from the list for this building.
        /// </summary>
        private void RemoveVehicle()
        {
            // Remove selected vehicle from building.
            VehicleControl.RemoveVehicle(CurrentBuilding, TransferReason, _selectedBuildingVehicle);

            // Update lists.
            Refresh();
        }

        /// <summary>
        /// Adds all vehicles in the available vehicle list to this building.
        /// </summary>
        private void AddAllVehicles()
        {
            // Add all vehicles in target list to bulding.
            foreach (VehicleItem item in _vehicleSelectionPanel.VehicleList.Data)
            {
                VehicleControl.AddVehicle(CurrentBuilding, TransferReason, item.Info);
            }

            // Update lists.
            Refresh();
        }

        /// <summary>
        /// Adds all vehicles in the available vehicle list to this building.
        /// </summary>
        private void RemoveAllVehicles()
        {
            // Add all vehicles in target list to bulding.
            foreach (VehicleItem item in _selectedVehiclePanel.VehicleList.Data)
            {
                VehicleControl.RemoveVehicle(CurrentBuilding, TransferReason, item.Info);
            }

            // Update lists.
            Refresh();
        }
    }
}