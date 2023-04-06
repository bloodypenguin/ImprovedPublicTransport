// <copyright file="SelectedVehiclePanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;
using VehicleSelector;

namespace ImprovedPublicTransport2.UI
{
    /// <summary>
    /// Selected vehicle panel.
    /// </summary>
    internal class SelectedVehiclePanel : AvailableVehiclePanel
    {
        // Panel to display when no item is selected.
        private readonly UIPanel _randomPanel;
        private readonly UILabel _randomLabel;

        public Func<ushort, List<VehicleInfo>> _vehicleGetter;
        public bool _showRandomIfNoVehicles;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedVehiclePanel"/> class.
        /// </summary>
        internal SelectedVehiclePanel()
        {

            // Panel setup.
            _randomPanel = VehicleList.AddUIComponent<UIPanel>();
            _randomPanel.width = VehicleList.width;
            _randomPanel.height = VehicleList.height;
            _randomPanel.relativePosition = new Vector2(0f, 0f);

            // Random sprite.
            UISprite randomSprite = _randomPanel.AddUIComponent<UISprite>();
            randomSprite.atlas = UITextures.InGameAtlas;
            randomSprite.spriteName = "Random";

            // Label.
            _randomLabel = UILabels.AddLabel(_randomPanel, 0f, 0f, Localization.Get("ANY_VEHICLE"), VehicleList.width, 0.8f);

            // Size is 56x33, so offset -8 from left and 3.5 from top to match normal row sizing.
            randomSprite.size = new Vector2(56f, 33f);
            randomSprite.relativePosition = new Vector2(-8, (40f - randomSprite.height) / 2f);
            _randomLabel.relativePosition = new Vector2(48f, (randomSprite.height - _randomLabel.height) / 2f);
        }

        /// <summary>
        /// Sets the currently selected vehicle.
        /// </summary>
        protected override VehicleInfo SelectedVehicle { set => ParentPanel.SelectedLineVehicle = value; }

        /// <summary>
        /// Populates the list.
        /// </summary>
        protected override void PopulateList()
        {
            List<VehicleItem> items = new List<VehicleItem>();
            List<VehicleInfo> lineVehicles = _vehicleGetter.Invoke(ParentPanel.CurrentLine);

            // Any selected vehicles?
            if (lineVehicles != null && lineVehicles.Count > 0)
            {
                // Yes - hide random panel.
                _randomPanel.Hide();

                // Generate filtered display list.
                foreach (VehicleInfo vehicle in lineVehicles)
                {
                    // Generate vehicle record for name filtering.
                    VehicleItem thisItem = new VehicleItem(vehicle);

                    // Apply name filter.
                    if (!NameFilter(thisItem.Name))
                    {
                        continue;
                    }

                    // Name filter passed - add to available list.
                    items.Add(thisItem);
                }
            }
            else if (_showRandomIfNoVehicles)
            {
                // No selected vehicles available - show random item panel.
                _randomPanel.Show();

                // Check for TLM override.
                _randomLabel.text = Localization.Get("ANY_VEHICLE");
            }
            else
            {
                _randomLabel.Hide();
            }

            // Set display list items, without changing the display.
            VehicleList.Data = new FastList<VehicleItem>
            {
                m_buffer = items.OrderBy(x => x.Name).ToArray(),
                m_size = items.Count,
            };
        }
    }
}