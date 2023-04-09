// <copyright file="BuildingPanelManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System;
using ColossalFramework.UI;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;

namespace ImprovedPublicTransport2.UI
{
    /// <summary>
    /// Static class to manage the mod's building info panel.
    /// </summary>
    internal static class PrefabPanelManager
    {
        // Instance references.
        private static GameObject s_gameObject;
        private static PrefabPanel s_panel;

        // InfoPanel buttons.
        private static UIButton s_transportLineButton;

        /// <summary>
        /// Gets the active panel instance.
        /// </summary>
        internal static PrefabPanel Panel => s_panel;

        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        internal static void Create()
        {
            try
            {
                // If no instance already set, create one.
                if (s_gameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    s_gameObject = new GameObject("IPTPrefabPanel");
                    s_gameObject.transform.parent = UIView.GetAView().transform;

                    // Add panel and set parent transform.
                    s_panel = s_gameObject.AddComponent<PrefabPanel>();
                    ImprovedPublicTransportMod._iptGameObject.GetComponent<VehicleEditor>().eventSettingsApplied +=
                        OnEventSettingsApplied;

                    // Show panel.
                    Panel.Show();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating IPTPrefabPanel");
            }
        }

        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        internal static void Close()
        {
            if (s_panel == null)
            {
                return;
            }

            ImprovedPublicTransportMod._iptGameObject.GetComponent<VehicleEditor>().eventSettingsApplied -=
                OnEventSettingsApplied;
            GameObject.Destroy(s_panel);
            GameObject.Destroy(s_gameObject);

            s_panel = null;
            s_gameObject = null;
        }

        /// <summary>
        /// Sets the target to the selected building, creating the panel if necessary.
        /// </summary>
        /// <param name="lineID">New building ID.</param>
        internal static void SetTarget(ushort lineID)
        {
            // If no existing panel, create it.
            if (Panel == null)
            {
                Create();
            }

            // Set the target.
            Panel.SetTarget(lineID);
        }
        
        private static void OnEventSettingsApplied()
        {
            s_panel.SetTarget(s_panel.CurrentLineID);
        }
    }
}