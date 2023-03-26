// <copyright file="VehicleItem.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ImprovedPublicTransport2.UI.AlgernonCommons;

namespace VehicleSelector
{
    /// <summary>
    /// Vehicle list item record.
    /// </summary>
    public class VehicleItem
    {
        // Vehicle prefab.
        private VehicleInfo _vehicleInfo;
        private string _vehicleName;

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleItem"/> class.
        /// </summary>
        /// <param name="prefab">Vehicle prefab for this item.</param>
        public VehicleItem(VehicleInfo prefab)
        {
            Info = prefab;
        }

        /// <summary>
        /// Gets the vehicles's name (empty string if none).
        /// </summary>
        public string Name => _vehicleName;

        /// <summary>
        /// Gets or sets the vehicle prefab for this record.
        /// </summary>
        public VehicleInfo Info
        {
            get => _vehicleInfo;

            set
            {
                _vehicleInfo = value;

                // Set display name.
                _vehicleName = PrefabUtils.GetDisplayName(value);
            }
        }
    }
}