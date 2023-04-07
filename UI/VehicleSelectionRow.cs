// <copyright file="VehicleSelectionRow.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ColossalFramework.UI;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;
using VehicleSelector;

namespace ImprovedPublicTransport2.UI
{
    /// <summary>
    /// UIList row item for vehicle prefabs.
    /// </summary>
    public class VehicleSelectionRow : UIListRow
    {
        /// <summary>
        /// Row height.
        /// </summary>
        public const float VehicleRowHeight = 40f;

        // Layout constants - private.
        private const float VehicleSpriteSize = 40f;
        private const float SteamSpriteWidth = 26f;
        private const float SteamSpriteHeight = 16f;
        private const float ScrollMargin = 10f;

        // Vehicle name label.
        private UILabel _vehicleNameLabel;

        // Preview image.
        private UISprite _vehicleSprite;

        // Steam icon.
        private UISprite _steamSprite;

        /// <summary>
        /// Vehicle prefab.
        /// </summary>
        private VehicleInfo _info;

        /// <summary>
        /// Gets the height for this row.
        /// </summary>
        public override float RowHeight => VehicleRowHeight;

        /// <summary>
        /// Generates and displays a row.
        /// </summary>
        /// <param name="data">Object data to display.</param>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public override void Display(object data, int rowIndex)
        {
            // Perform initial setup for new rows.
            if (_vehicleNameLabel == null)
            {
                // Add object name label.
                _vehicleNameLabel = AddLabel(VehicleSpriteSize + Margin, width - Margin - VehicleSpriteSize - Margin - SteamSpriteWidth - ScrollMargin - Margin, wordWrap: true);

                // Add preview sprite image.
                _vehicleSprite = AddUIComponent<UISprite>();
                _vehicleSprite.height = VehicleSpriteSize;
                _vehicleSprite.width = VehicleSpriteSize;
                _vehicleSprite.relativePosition = Vector2.zero;

                // Add setam sprite.
                _steamSprite = AddUIComponent<UISprite>();
                _steamSprite.width = SteamSpriteWidth;
                _steamSprite.height = SteamSpriteHeight;
                _steamSprite.atlas = UITextures.InGameAtlas;
                _steamSprite.spriteName = "SteamWorkshop";
                _steamSprite.relativePosition = new Vector2(width - Margin - ScrollMargin - SteamSpriteWidth, (height - SteamSpriteHeight) / 2f);
            }

            // Get building ID and set name label.
            if (data is VehicleItem thisItem)
            {
                _info = thisItem.Info;
                var capacity = _info.m_vehicleAI.GetPassengerCapacity(true);
                var allowedNameLength = 15;
                string trimmedName;
                if (thisItem.Name.Length <= allowedNameLength)
                {
                    trimmedName = thisItem.Name;
                }
                else
                {
                    trimmedName = thisItem.Name.Substring(0, 15) + "…"; 
                }
                var capacityText =  $"({Localization.Get("VEHICLE_SELECTION_CAPACITY")}: {capacity})";
                _vehicleNameLabel.text = $"{trimmedName} {capacityText}";
                _vehicleNameLabel.tooltip = $"{thisItem.Name} {capacityText}";

                _vehicleSprite.atlas = _info?.m_Atlas;
                _vehicleSprite.spriteName = _info?.m_Thumbnail;

                _steamSprite.isVisible = PrefabUtils.IsWorkshopAsset(_info);
            }
            else
            {
                // Just in case (no valid vehicle record).
                _vehicleNameLabel.text = string.Empty;
            }

            // Set initial background as deselected state.
            Deselect(rowIndex);
        }
    }
}