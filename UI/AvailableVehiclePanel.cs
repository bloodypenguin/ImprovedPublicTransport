﻿// <copyright file="AvailableVehiclePanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using ImprovedPublicTransport2.Data;
using ImprovedPublicTransport2.Query;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;

namespace ImprovedPublicTransport2.UI
{
    /// <summary>
    /// Vehicle selection panel main class.
    /// </summary>
    internal class AvailableVehiclePanel : UIPanel
    {
        /// <summary>
        /// Layout margin.
        /// </summary>
        protected const float Margin = 5f;

        // Vehicle selection list.
        private UIList _vehicleList;

        // Search panel.
        private UITextField _nameSearch;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableVehiclePanel"/> class.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            try
            {
                // Basic setup.
                name = "AvailableVehiclePanel";
                autoLayout = false;
                isVisible = true;
                canFocus = true;
                isInteractive = true;
                width = VehicleSelection.ListWidth;
                height = VehicleSelection.VehicleListHeight;

                // Vehicle selection list.
                _vehicleList = UIList.AddUIList<VehicleSelectionRow>(
                    this,
                    0f,
                    0f,
                    VehicleSelection.ListWidth,
                    VehicleSelection.VehicleListHeight,
                    VehicleSelectionRow.VehicleRowHeight);
                _vehicleList.EventSelectionChanged +=
                    (c, selectedItem) => SelectedVehicle = selectedItem as PrefabData;

                // Search field.
                _nameSearch = UITextFields.AddSmallTextField(_vehicleList, 25f, -23f, VehicleSelection.ListWidth - 25f);
                _nameSearch.eventTextChanged += (c, text) => PopulateList();
                UISprite searchSprite = _nameSearch.AddUIComponent<UISprite>();
                searchSprite.atlas = UITextures.InGameAtlas;
                searchSprite.spriteName = "LineDetailButtonHovered";
                searchSprite.relativePosition = new Vector2(-25f, 0f);
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception setting up vehicle selection panel");
            }
        }

        /// <summary>
        /// Gets or sets the parent reference.
        /// </summary>
        internal VehicleSelection ParentPanel { get; set; }

        /// <summary>
        /// Gets the vehicle selection list.
        /// </summary>
        internal UIList VehicleList => _vehicleList;

        /// <summary>
        /// Sets the currently selected vehicle.
        /// </summary>
        protected virtual PrefabData SelectedVehicle
        {
            set => ParentPanel.SelectedListVehicle = value;
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        internal void ClearSelection() => _vehicleList.SelectedIndex = -1;

        /// <summary>
        /// Refreshes the list with current information.
        /// </summary>
        internal void RefreshList()
        {
            // Clear selected index.
            _vehicleList.SelectedIndex = -1;

            // Repopulate the list.
            PopulateList();
        }

        /// <summary>
        /// Populates the list with available vehicles.
        /// </summary>
        protected virtual void PopulateList()
        {
            // Ensure valid building selection.
            ushort currentLine = ParentPanel.CurrentLine;
            if (currentLine == 0)
            {
                return;
            }

            // Generated lists.
            var items = new List<PrefabData>();

            TransportInfo info = TransportManager.instance.m_lines.m_buffer[currentLine].Info;
            ItemClass.SubService subService = info.GetSubService();
            ItemClass.Service service = info.GetService();
            ItemClass.Level level = info.GetClassLevel();
            ItemClassTriplet triplet = new ItemClassTriplet(service, subService, level);

            // Get list of already-selected vehicles.
            var selectedList = SelectedVehicleTypesQuery.Query(currentLine);
            var allAvailableVehicles = AvailableVehiclesQuery.Query(triplet);
            allAvailableVehicles.ForEach(data =>
            {
                if (selectedList.Contains(data))
                {
                    return;
                }
                
                if (!NameAndCapacityFilter(data))
                {
                    return;
                }
                items.Add(data);
            });

            // Set display list items, without changing the display.
            _vehicleList.Data = new FastList<object>
            {
                m_buffer = items.OrderBy(x => x.Name).Select(d => (object)d).ToArray(),
                m_size = items.Count,
            };
        }

        protected bool NameAndCapacityFilter(PrefabData prefabData) => _nameSearch.text.IsNullOrWhiteSpace() ||
                                                         prefabData.DisplayName.ToLower().Contains(_nameSearch.text.ToLower()) ||
                                                         prefabData.TotalCapacity.ToString().Contains(_nameSearch.text);
    }
}