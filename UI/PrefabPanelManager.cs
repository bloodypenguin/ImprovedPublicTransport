// <copyright file="BuildingPanelManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System;
using ColossalFramework.UI;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;
using VehicleSelector;

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
            if (s_panel != null)
            {
                GameObject.Destroy(s_panel);
                GameObject.Destroy(s_gameObject);

                s_panel = null;
                s_gameObject = null;
            }
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

        /// <summary>
        /// Adds the building buttons to game building info panels.
        /// </summary>
        internal static void AddInfoPanelButtons()
        {
            // try
            // {
            //     s_transportLineButton =
            //         AddInfoPanelButton(
            //             UIView.library.Get<PublicTransportWorldInfoPanel>(typeof(PublicTransportWorldInfoPanel).Name));
            // }
            // catch (Exception e)
            // {
            //     Logging.LogException(e, "exception adding building info panel buttons");
            // }
        }

        //TODO: implement
        /// <summary>
        /// Handles building info world target building changes.
        /// </summary>
        internal static void TargetChanged()
        {
            // ushort lineID = WorldInfoPanel.GetCurrentInstanceID().TransportLine;
            // bool supportedBuilding = Transfers.BuildingEligibility(lineID);
            // s_transportLineButton.isVisible = supportedBuilding;
            //
            // // Don't do anything if panel isn't open.
            // if (s_panel != null)
            // {
            //     if (supportedBuilding)
            //     {
            //         SetTarget(lineID);
            //     }
            //     else
            //     {
            //         Close();
            //     }
            // }
        }

        /// <summary>
        /// Adds a Transfer Controller button to a building info panel to directly access that building's.
        /// </summary>
        /// <param name="infoPanel">Infopanel to apply the button to.</param>
        /// <returns>New UIButton.</returns>
        private static UIButton AddInfoPanelButton(WorldInfoPanel infoPanel)
        {
            const float ButtonHeight = 42f;
            const float ButtonWidth = 42f;

            // Targets.
            UIComponent parent = null;
            float relativeX = 0f;
            float relativeY = 0f;

            // Player info panels have wrappers, warehouse and zoned ones don't.
            UIComponent wrapper = infoPanel.Find("Wrapper");
            if (wrapper == null)
            {
                if (infoPanel.Find("ActionPanel") is UIPanel actionPanel)
                {
                    Logging.Message("adding info panel button to warehouse/shelter");

                    // Warehouse or shelter.
                    relativeX = 47f;
                    parent = actionPanel;
                }
                else if (infoPanel.Find("Misc") is UIPanel miscPanel)
                {
                    Logging.Message("adding info panel button to unique factories");

                    // Unique factory.
                    relativeX = 18f;
                    parent = miscPanel;
                }
                else if (infoPanel.Find("ZoneTypeInfo") is UITabContainer zoneTypeInfoPanel)
                {
                    // Zoned building.
                    Logging.Message("adding info panel button to zoned building");
                    relativeX = 10f;
                    relativeY = zoneTypeInfoPanel.relativePosition.y;
                    parent = infoPanel.component;
                }
            }
            else
            {
                // City service panel.
                relativeX = 94f;
                UIComponent buttonPanels = wrapper.Find("MainSectionPanel")?.Find("MainBottom")?.Find("ButtonPanels");
                if (buttonPanels != null)
                {
                    parent = buttonPanels.Find("ActionButtons")?.Find("ActionPanelPanel")?.Find("ActionPanel");

                    // Send park button panels to back.
                    buttonPanels.Find("ParkButtons")?.SendToBack();
                }
            }

            if (parent == null)
            {
                Logging.Error("couldn't place panel button for ", infoPanel.name);
                return null;
            }

            UIButton panelButton = parent.AddUIComponent<UIButton>();

            // Basic button setup.
            panelButton.atlas = UITextures.InGameAtlas;
            panelButton.height = ButtonHeight;
            panelButton.width = ButtonWidth;
            panelButton.normalBgSprite = "GenericPanelLight";
            panelButton.focusedBgSprite = "GenericPanelLight";
            panelButton.hoveredBgSprite = "GenericPanelWhite";
            panelButton.pressedBgSprite = "GenericPanelLight";
            panelButton.disabledBgSprite = "ButtonMenuDisabled";
            panelButton.color = new Color32(219, 219, 219, 255);
            panelButton.focusedColor = Color.white;
            panelButton.hoveredColor = Color.white;
            panelButton.disabledColor = Color.white;
            panelButton.name = "VehicleSelectorButton";
            panelButton.tooltip = Localization.Get("MOD_NAME");
            panelButton.tooltipBox = UIToolTips.WordWrapToolTip;

            UISprite buttonSprite = panelButton.AddUIComponent<UISprite>();
            buttonSprite.size = panelButton.size;
            buttonSprite.atlas = UITextures.LoadSingleSpriteAtlas("IPT2-Icon");
            buttonSprite.relativePosition = new Vector2(1f, 1f);
            buttonSprite.spriteName = "normal";
            buttonSprite.tooltip = Localization.Get("MOD_NAME");

            // Set position.
            panelButton.relativePosition = new Vector2(relativeX, relativeY);
            panelButton.BringToFront();

            // Event handler.
            panelButton.eventClick += (c, p) =>
            {
                // Select current building in the building details panel and show.
                SetTarget(WorldInfoPanel.GetCurrentInstanceID().Building);

                // Manually unfocus control, otherwise it can stay focused until next UI event (looks untidy).
                c.Unfocus();
            };

            return panelButton;
        }
    }
}