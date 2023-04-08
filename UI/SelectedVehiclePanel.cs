// <copyright file="SelectedVehiclePanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using ImprovedPublicTransport2.Query;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;

namespace ImprovedPublicTransport2.UI
{
    /// <summary>
    /// Selected vehicle panel.
    /// </summary>
    internal class SelectedVehiclePanel : AvailableVehiclePanel
    {
        // Panel to display when no item is selected.
        private UIPanel _randomPanel;
        private UILabel _randomLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedVehiclePanel"/> class.
        /// </summary>
        public override void Awake()
        {
            base.Awake();
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
            _randomLabel = UILabels.AddLabel(_randomPanel, 0f, 0f, Localization.Get("ANY_VEHICLE"), VehicleList.width,
                0.8f);

            // Size is 56x33, so offset -8 from left and 3.5 from top to match normal row sizing.
            randomSprite.size = new Vector2(56f, 33f);
            randomSprite.relativePosition = new Vector2(-8, (40f - randomSprite.height) / 2f);
            _randomLabel.relativePosition = new Vector2(48f, (randomSprite.height - _randomLabel.height) / 2f);
        }

        /// <summary>
        /// Sets the currently selected vehicle.
        /// </summary>
        protected override PrefabData SelectedVehicle
        {
            set => ParentPanel.SelectedLineVehicle = value;
        }

        /// <summary>
        /// Populates the list.
        /// </summary>
        protected override void PopulateList()
        {
            var items = new List<PrefabData>();
            var lineVehicles = SelectedVehicleTypesQuery.Query(ParentPanel.CurrentLine);

            // Any selected vehicles?
            if (lineVehicles is { Count: > 0 })
            {
                // Yes - hide random panel.
                _randomPanel.Hide();

                // Generate filtered display list.
                foreach (var vehicle in lineVehicles)
                {

                    // Apply name filter.
                    if (!NameFilter(vehicle.DisplayName))
                    {
                        continue;
                    }

                    // Name filter passed - add to available list.
                    items.Add(vehicle);
                }
            }
            else
            {
                // No selected vehicles available - show random item panel.
                _randomPanel.Show();

                // Check for TLM override.
                _randomLabel.text = Localization.Get("ANY_VEHICLE");
            }

            // Set display list items, without changing the display.
            VehicleList.Data = new FastList<object>
            {
                m_buffer = items.OrderBy(x => x.DisplayName).ToArray(),
                m_size = items.Count,
            };
        }
    }
}