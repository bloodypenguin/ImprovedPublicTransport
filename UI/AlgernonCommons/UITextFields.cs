// <copyright file="UITextFields.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    /// <summary>
    /// UI textfields.
    /// </summary>
    public static class UITextFields
    {
        /// <summary>
        /// Adds a tiny input text field at the specified coordinates.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="width">Textfield width (default 200).</param>
        /// <returns>New large textfield with attached label.</returns>
        public static UITextField AddTinyTextField(UIComponent parent, float posX, float posY, float width = 200f) => AddTextField(parent, posX, posY, width, 16f, 0.8f, 3);

        /// <summary>
        /// Adds a small textfield with an attached label to the left.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="text">Label text.</param>
        /// <param name="width">Textfield width (default 200).</param>
        /// <returns>New large textfield with attached label.</returns>
        public static UITextField AddSmallLabelledTextField(UIComponent parent, float posX, float posY, string text, float width = 200f) => AddLabelledTextField(parent, posX, posY, text, width, 18f, 0.8f, 3);

        /// <summary>
        /// Adds a large textfield with an attached label to the left.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="text">Label text.</param>
        /// <param name="width">Textfield width (default 200).</param>
        /// <returns>New large textfield with attached label.</returns>
        public static UITextField AddBigLabelledTextField(UIComponent parent, float posX, float posY, string text, float width = 200f) => AddLabelledTextField(parent, posX, posY, text, width, 30f, 1.2f, 6);

        /// <summary>
        /// Adds a textfield with an attached label to the left.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="text">Label text.</param>
        /// <param name="width">Textfield width (default 200).</param>
        /// <param name="height">Textfield height (default 22.)</param>
        /// <param name="scale">Text scale (default 1.0).</param>
        /// <param name="vertPad">Vertical text padding within textfield box (default 4).</param>
        /// <param name="tooltip">Tooltip, if any.</param>
        /// <returns>New textfield with attached label.</returns>
        public static UITextField AddLabelledTextField(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 22f, float scale = 1.0f, int vertPad = 4, string tooltip = null)
        {
            UITextField textField = AddTextField(parent, posX, posY, width, height, scale, vertPad, tooltip);

            // Label.
            UILabel label = textField.AddUIComponent<UILabel>();
            label.name = "label";
            label.textScale = scale;
            label.autoSize = true;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.wordWrap = false;

            // Event handler to set position on text change.
            label.eventTextChanged += (c, newText) => label.relativePosition = new Vector2(-(label.width + 5f), ((height - label.height) / 2f) + 2f);

            // Set text.
            label.text = text;

            return textField;
        }

        /// <summary>
        /// Adds a small input text field at the specified coordinates.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="width">Textfield width (default 200).</param>
        /// <returns>New large textfield with attached label.</returns>
        public static UITextField AddSmallTextField(UIComponent parent, float posX, float posY, float width = 200f) => AddTextField(parent, posX, posY, width, 18f, 0.9f, 3);

        /// <summary>
        /// Adds a large input text field at the specified coordinates.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="width">Textfield width (default 200).</param>
        /// <returns>New large textfield with attached label.</returns>
        public static UITextField AddBigTextField(UIComponent parent, float posX, float posY, float width = 200f) => AddTextField(parent, posX, posY, width, 30f, 1.2f, 6);

        /// <summary>
        /// Adds an input text field at the specified coordinates.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="posX">Relative X postion.</param>
        /// <param name="posY">Relative Y position.</param>
        /// <param name="width">Textfield width (default 200).</param>
        /// <param name="height">Textfield height (default 22).</param>
        /// <param name="scale">Text scale (default 1.0).</param>
        /// <param name="vertPad">Vertical text padding within textfield box (default 4).</param>
        /// <param name="tooltip">Tooltip, if any.</param>
        /// <returns>New textfield *without* attached label.</returns>
        public static UITextField AddTextField(UIComponent parent, float posX, float posY, float width = 200f, float height = 22f, float scale = 1f, int vertPad = 4, string tooltip = null)
        {
            UITextField textField = parent.AddUIComponent<UITextField>();

            // Size and position.
            textField.size = new Vector2(width, height);
            textField.relativePosition = new Vector2(posX, posY);

            // Text settings.
            textField.textScale = scale;
            textField.padding = new RectOffset(6, 6, vertPad, 1);
            textField.horizontalAlignment = UIHorizontalAlignment.Center;

            // Behaviour.
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;

            // Appearance.
            textField.color = new Color32(255, 255, 255, 255);
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(0, 0, 0, 128);
            textField.atlas = UITextures.InGameAtlas;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanel";

            // Add tooltip.
            if (tooltip != null)
            {
                textField.tooltip = tooltip;
            }

            return textField;
        }

        /// <summary>
        /// Creates a plain textfield using the game's option panel checkbox template.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="text">Descriptive label text.</param>
        /// <returns>New checkbox using the game's option panel template.</returns>
        public static UITextField AddPlainTextfield(UIComponent parent, string text)
        {
            UIPanel textFieldPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("IPT2_OptionsTextfieldTemplate")) as UIPanel;

            // Set text label.
            textFieldPanel.Find<UILabel>("Label").text = text;
            return textFieldPanel.Find<UITextField>("Text Field");
        }
    }
}