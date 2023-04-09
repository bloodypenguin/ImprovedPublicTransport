// <copyright file="UIButtons.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    /// <summary>
    /// UI buttons.
    /// </summary>
    public static class UIButtons
    {
        /// <summary>
        /// Adds a simple pushbutton.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="text">Button text.</param>
        /// <param name="width">Button width (default 200).</param>
        /// <param name="height">Button height (default 30).</param>
        /// <param name="scale">Text scale (default 0.9).</param>
        /// <param name="vertPad">Vertical text padding within button (default 4).</param>
        /// <param name="tooltip">Tooltip, if any.</param>
        /// <returns>New pushbutton.</returns>
        public static UIButton AddButton(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 30f, float scale = 0.9f, int vertPad = 4, string tooltip = null)
        {
            UIButton button = parent.AddUIComponent<UIButton>();

            // Size and position.
            button.size = new Vector2(width, height);
            button.relativePosition = new Vector2(posX, posY);

            // Appearance.
            button.font = UIFonts.SemiBold;
            button.textScale = scale;
            button.atlas = UITextures.InGameAtlas;
            button.normalBgSprite = "ButtonWhite";
            button.hoveredBgSprite = "ButtonWhite";
            button.focusedBgSprite = "ButtonWhite";
            button.pressedBgSprite = "ButtonWhitePressed";
            button.disabledBgSprite = "ButtonWhiteDisabled";
            button.color = Color.white;
            button.focusedColor = Color.white;
            button.hoveredColor = Color.white;
            button.pressedColor = Color.white;
            button.textColor = Color.black;
            button.pressedTextColor = Color.black;
            button.focusedTextColor = Color.black;
            button.hoveredTextColor = Color.blue;
            button.disabledTextColor = Color.grey;
            button.canFocus = false;

            // Add tooltip.
            if (tooltip != null)
            {
                button.tooltip = tooltip;
            }

            // Text.
            button.textScale = scale;
            button.textPadding = new RectOffset(0, 0, vertPad, 0);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Center;
            button.text = text;

            return button;
        }

        /// <summary>
        /// Adds a simple pushbutton.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="position">Relative postion.</param>
        /// <param name="text">Button text.</param>
        /// <param name="width">Button width (default 200).</param>
        /// <param name="height">Button height (default 30).</param>
        /// <param name="scale">Text scale (default 0.9).</param>
        /// <param name="vertPad">Vertical text padding within button (default 4).</param>
        /// <param name="tooltip">Tooltip, if any.</param>
        /// <returns>New pushbutton.</returns>
        public static UIButton AddButton(UIComponent parent, Vector3 position, string text, float width = 200f, float height = 30f, float scale = 0.9f, int vertPad = 4, string tooltip = null) =>
            AddButton(parent, position.x, position.y, text, width, height, scale, vertPad, tooltip);

        /// <summary>
        /// Adds a simple pushbutton, slightly smaller than the standard.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="text">Button text.</param>
        /// <param name="width">Button width (default 200).</param>
        /// <param name="height">Button height (default 30).</param>
        /// <param name="scale">Text scale (default 0.9).</param>
        /// <returns>New UIButton.</returns>
        public static UIButton AddSmallerButton(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 28f, float scale = 0.8f)
        {
            UIButton button = AddButton(parent, posX, posY, text, width, height, scale);

            // Adjust bounding box to center 0.8 text in a 28-high button.
            button.textPadding = new RectOffset(4, 4, 4, 0);

            return button;
        }

        /// <summary>
        /// Adds a simple pushbutton, even smaller than the others.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="text">Button text.</param>
        /// <param name="width">Button width (default 200).</param>
        /// <param name="height">Button height (default 20).</param>
        /// <param name="scale">Text scale (default 0.7).</param>
        /// <returns>New UIButton.</returns>
        public static UIButton AddEvenSmallerButton(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 20f, float scale = 0.7f)
        {
            UIButton button = AddButton(parent, posX, posY, text, width, height, scale);

            // Adjust bounding box to center 0.7 text in a 20-high button.
            button.textPadding = new RectOffset(3, 3, 3, 0);

            return button;
        }

        /// <summary>
        /// Adds an icon-style button to the specified component at the specified coordinates.
        /// </summary>
        /// <param name="parent">Parent UIComponent.</param>
        /// <param name="xPos">Relative X position.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="size">Button size (square).</param>
        /// <param name="atlas">Icon atlas.</param>
        /// <param name="tooltip">Tooltip (null for none).</param>
        /// <returns>New UIButton of the given type.</returns>
        public static UIButton AddIconButton(UIComponent parent, float xPos, float yPos, float size, UITextureAtlas atlas, string tooltip = null) =>
            AddIconButton<UIButton>(parent, xPos, yPos, size, atlas, tooltip);

        /// <summary>
        /// Adds an icon-style button of a custom type to the specified component at the specified coordinates.
        /// </summary>
        /// <typeparam name="TButton">Button type.</typeparam>
        /// <param name="parent">Parent UIComponent.</param>
        /// <param name="xPos">Relative X position.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="size">Button size (square).</param>
        /// <param name="atlas">Icon atlas.</param>
        /// <param name="tooltip">Tooltip (null for none).</param>
        /// <returns>New UIButton of the given type.</returns>
        public static TButton AddIconButton<TButton>(UIComponent parent, float xPos, float yPos, float size, UITextureAtlas atlas, string tooltip = null)
            where TButton : UIButton
        {
            TButton newButton = parent.AddUIComponent<TButton>();

            // Size and position.
            newButton.relativePosition = new Vector2(xPos, yPos);
            newButton.height = size;
            newButton.width = size;

            // Appearance.
            newButton.atlas = atlas;
            newButton.normalFgSprite = "normal";
            newButton.focusedFgSprite = "normal";
            newButton.hoveredFgSprite = "hovered";
            newButton.disabledFgSprite = "disabled";
            newButton.pressedFgSprite = "pressed";

            // Tooltip.
            if (tooltip != null)
            {
                newButton.tooltip = tooltip;
            }

            return newButton;
        }
    }
}