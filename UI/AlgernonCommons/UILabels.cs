// <copyright file="UILabels.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    /// <summary>
    /// UI labels.
    /// </summary>
    public static class UILabels
    {
        /// <summary>
        /// Adds a plain text label to the specified UI component.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="xPos">Relative x position (top-left corner; top-right corner if alignment is set to UIHorizontalAlignment.Right).</param>
        /// <param name="yPos">Relative y position.</param>
        /// <param name="text">Label text.</param>
        /// <param name="width">Label width (-1 (default) for autosize).</param>
        /// <param name="textScale">Text scale (default 1.0).</param>
        /// <param name="alignment">Text alignment (default left).</param>
        /// <returns>New text label.</returns>
        public static UILabel AddLabel(UIComponent parent, float xPos, float yPos, string text, float width = -1f, float textScale = 1.0f, UIHorizontalAlignment alignment = UIHorizontalAlignment.Left)
        {
            // Add label.
            UILabel label = parent.AddUIComponent<UILabel>();

            // Set sizing options.
            if (width > 0f)
            {
                // Fixed width.
                label.autoSize = false;
                label.width = width;
                label.autoHeight = true;
                label.wordWrap = true;
            }
            else
            {
                // Autosize.
                label.autoSize = true;
                label.autoHeight = false;
                label.wordWrap = false;
            }

            // Alignment.
            label.textAlignment = alignment;

            // Text.
            label.textScale = textScale;
            label.text = text;

            // Position (aligned to right if text alignment is set to right).
            label.relativePosition = new Vector2(alignment == UIHorizontalAlignment.Right ? xPos - label.width : xPos, yPos);

            return label;
        }

        /// <summary>
        /// Dynamically resizes a text label by shrinking the text scale until it fits within the desired maximum width.
        /// </summary>
        /// <param name="label">Label to resize.</param>
        /// <param name="maxWidth">Maximum acceptable label width.</param>
        /// <param name="minScale">Minimum acceptible label scale (to nearest increment of 0.05f).</param>
        public static void ResizeLabel(UILabel label, float maxWidth, float minScale)
        {
            // Don't do anything with negative widths or scales.
            if (maxWidth < 10f || minScale < 0.5f)
            {
                return;
            }

            // Make sure label is autosizeable and up-to-date.
            label.autoSize = true;
            label.PerformLayout();

            // Iterate through text scales until acceptible width is reached, or we reach the minimum permitted scale.
            while (label.width > maxWidth && label.textScale > minScale)
            {
                label.textScale -= 0.01f;
                label.PerformLayout();
            }

            // Finally, clamp label size.
            label.autoSize = false;
            if (label.width > maxWidth)
            {
                label.width = maxWidth;
            }
        }

        /// <summary>
        /// Truncates a text label by reducing the number of characters displayed and appending an ellipsis.
        /// </summary>
        /// <param name="label">Label to truncate.</param>
        /// <param name="maxWidth">Maximum acceptable label width (minimum 20).</param>
        public static void TruncateLabel(UILabel label, float maxWidth)
        {
            // Don't do anything with maximum widths below 20f.
            if (maxWidth < 20f)
            {
                return;
            }

            // Make sure label is autosizeable and up-to-date.
            label.autoSize = true;
            label.PerformLayout();

            // Iterativly remove the last remaining letter through text scales until acceptible width is reached.
            while (label.width > maxWidth)
            {
                label.text = label.text.Substring(0, label.text.Length - 4) + "...";
                label.PerformLayout();
            }
        }
    }
}