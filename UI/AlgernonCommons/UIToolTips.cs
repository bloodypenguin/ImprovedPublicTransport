// <copyright file="UIToolTips.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    /// <summary>
    /// Custom tooltips.
    /// </summary>
    public static class UIToolTips
    {
        // Cache.
        private static UILabel _wordWrapTooltip;

        /// <summary>
        /// Gets a word-wrapping text-label tooltip box.
        /// </summary>
        public static UILabel WordWrapToolTip
        {
            get
            {
                if (_wordWrapTooltip == null)
                {
                    _wordWrapTooltip = WordWrapToolTipBox();
                }

                return _wordWrapTooltip;
            }
        }

        /// <summary>
        /// Creates a word-wrapping tooltip box.
        /// </summary>
        /// <returns>New tooltip box.</returns>
        private static UILabel WordWrapToolTipBox()
        {
            // Create GameObject and attach new UILabel.
            GameObject tooltipGameObject = new GameObject("AlgernonWordWrapTooltip");
            tooltipGameObject.transform.parent = UIView.Find("DefaultTooltip").gameObject.transform.parent;
            UILabel tipBox = tooltipGameObject.AddComponent<UILabel>();

            // Mimic game's default tooltop.
            tipBox.padding = new RectOffset(23, 23, 5, 5);
            tipBox.verticalAlignment = UIVerticalAlignment.Middle;
            tipBox.pivot = UIPivotPoint.BottomLeft;
            tipBox.arbitraryPivotOffset = new Vector2(-3, 6);

            // Appearance.
            tipBox.atlas = UITextures.InGameAtlas;
            tipBox.backgroundSprite = "InfoDisplay";

            // Start hidden and off to the side.
            tipBox.transformPosition = new Vector2(-2f, -2f);
            tipBox.isVisible = false;

            // Calculate size.
            WordWrapToolTipBoxResize(tipBox);
            tipBox.eventTextChanged += (c, v) => WordWrapToolTipBoxResize(c as UILabel);

            return tipBox;
        }

        /// <summary>
        /// Calculates the size of a word-wrap tooltip box.
        /// </summary>
        /// <param name="label">Word-wrap tooltip box.</param>
        private static void WordWrapToolTipBoxResize(UILabel label)
        {
            // Maximum width.
            const float MaximumWidth = 700f;

            // Null check.
            if (label == null)
            {
                return;
            }

            // Set label autosize.
            label.wordWrap = false;
            label.autoSize = true;
            label.PerformLayout();

            // If autosized label is wider than the maximum, truncate the width and turn on word-wrapping.
            if (label.width > MaximumWidth)
            {
                label.autoSize = false;
                label.autoHeight = true;
                label.wordWrap = true;
                label.width = MaximumWidth;
            }
        }
    }
}