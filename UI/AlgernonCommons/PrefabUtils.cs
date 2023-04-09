// <copyright file="PrefabUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    /// <summary>
    /// Prefab-related utilities.
    /// </summary>
    public static class PrefabUtils
    {
        /// <summary>
        /// Returns a cleaned-up display name for the given prefab.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <returns>Cleaned display name.</returns>
        public static string GetDisplayName(PrefabInfo prefab)
        {
            // Null check.
            if (prefab?.name == null)
            {
                return "null";
            }

            // Try getting any localized name first.
            string localizedName = prefab.GetUncheckedLocalizedTitle();

            // Perform cleanup.
            return GetDisplayName(localizedName);
        }

        /// <summary>
        /// Sanitises a raw prefab name for display.
        /// </summary>
        /// <param name="prefabName">Prefab name.</param>
        /// <returns>Cleaned display name.</returns>
        public static string GetDisplayName(string prefabName)
        {
            // Omit any package number, and trim off any trailing _Data.
            int index = prefabName.IndexOf('.');
            return prefabName.Substring(index + 1).Replace("_Data", string.Empty);
        }

        /// <summary>
        /// Checks if this asset is a workshop asset (ie. has a workshop ID associated with it).
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <returns>True if this is a workshop asset, false otherwise.</returns>
        public static bool IsWorkshopAsset(PrefabInfo prefab)
        {
            // Null check.
            if (prefab?.name == null)
            {
                return false;
            }

            // Check for a package number (name contains period).
            return prefab.name.IndexOf('.') >= 0;
        }
    }
}