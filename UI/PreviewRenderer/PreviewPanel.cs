// <copyright file="PreviewPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2.UI.PreviewRenderer
{
    /// <summary>
    /// Panel that contains the building preview image.
    /// </summary>
    public class PreviewPanel : UIPanel
    {
        // Panel components.
        private readonly UITextureSprite _previewSprite;
        private readonly UISprite _noPreviewSprite;
        private readonly UISprite _thumbnailSprite;
        private readonly PreviewRenderer _renderer;

        // Currently selected prefab.
        private PrefabInfo _renderPrefab;
        public Color32 lineColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewPanel"/> class.
        /// </summary>
        internal PreviewPanel()
        {
            // Size.
            width = VehicleSelection.PreviewWidth;
            height = VehicleSelection.PreviewWidth;

            // Appearance.
            opacity = 1.0f;

            _previewSprite = AddUIComponent<UITextureSprite>();
            _previewSprite.size = size;
            _previewSprite.relativePosition = Vector2.zero;

            _noPreviewSprite = AddUIComponent<UISprite>();
            _noPreviewSprite.size = size;
            _noPreviewSprite.relativePosition = Vector2.zero;

            _thumbnailSprite = AddUIComponent<UISprite>();
            _thumbnailSprite.size = size;
            _thumbnailSprite.relativePosition = Vector2.zero;

            // Initialise renderer; use double size for anti-aliasing.
            _renderer = gameObject.AddComponent<PreviewRenderer>();
            _renderer.Size = _previewSprite.size * 2;

            // Click-and-drag rotation.
            eventMouseDown += (component, mouseEvent) =>
            {
                eventMouseMove += RotateCamera;
            };

            eventMouseUp += (component, mouseEvent) =>
            {
                eventMouseMove -= RotateCamera;
            };

            // Zoom with mouse wheel.
            eventMouseWheel += (component, mouseEvent) =>
            {
                _renderer.Zoom -= Mathf.Sign(mouseEvent.wheelDelta) * 0.25f;

                // Render updated image.
                RenderPreview();
            };
        }

        /// <summary>
        /// Sets the prefab to render.
        /// </summary>
        /// <param name="prefab">Prefab to render.</param>
        public void SetTarget(PrefabInfo prefab)
        {
            // Update current selection to the new prefab.
            _renderPrefab = prefab;

            // Show the updated render.
            RenderPreview();
        }

        /// <summary>
        /// Render and show a preview of a building.
        /// </summary>
        public void RenderPreview()
        {
            bool validRender = false;

            if (_renderPrefab is VehicleInfo vehicle)
            {
                // Don't render anything without a mesh or material.
                if (vehicle?.m_mesh != null && vehicle.m_material != null)
                {
                    // Set mesh and material for render.
                    _renderer.Mesh = vehicle.m_mesh;
                    _renderer.Material = vehicle.m_material;
                    _renderer.MaterialColor = lineColor;

                    // Render.
                    _renderer.Render();

                    // We got a valid render; ensure preview sprite is square (decal previews can change width), set display texture, and set status flag.
                    _previewSprite.relativePosition = Vector2.zero;
                    _previewSprite.size = size;
                    _previewSprite.texture = _renderer.Texture;
                    validRender = true;
                }

                // If not a valid render, try to show thumbnail instead.
                else if (vehicle.m_Atlas != null && !string.IsNullOrEmpty(vehicle.m_Thumbnail))
                {
                    // Show thumbnail.
                    ShowThumbnail(vehicle.m_Atlas, vehicle.m_Thumbnail);

                    // All done here.
                    return;
                }
            }

            // Reset background if we didn't get a valid render.
            if (!validRender)
            {
                _previewSprite.Hide();
                _thumbnailSprite.Hide();
                _noPreviewSprite.Show();
                return;
            }

            // If we got here, we should have a render; show it.
            _noPreviewSprite.Hide();
            _thumbnailSprite.Hide();
            _previewSprite.Show();
        }

        /// <summary>
        /// Rotates the preview camera (model rotation) in accordance with mouse movement.
        /// </summary>
        /// <param name="c">Calling component.</param>
        /// <param name="p">Mouse event pareameter.</param>
        private void RotateCamera(UIComponent c, UIMouseEventParameter p)
        {
            // Change rotation.
            _renderer.CameraRotation -= p.moveDelta.x / _previewSprite.width * 360f;

            // Render updated image.
            RenderPreview();
        }

        /// <summary>
        /// Displays a prefab's UI thumbnail (instead of a render or blank panel).
        /// </summary>
        /// <param name="atlas">Thumbnail atlas.</param>
        /// <param name="thumbnail">Thumbnail sprite name.</param>
        private void ShowThumbnail(UITextureAtlas atlas, string thumbnail)
        {
            // Set thumbnail.
            _thumbnailSprite.atlas = atlas;
            _thumbnailSprite.spriteName = thumbnail;

            // Show thumbnail sprite and hide others.
            _noPreviewSprite.Hide();
            _previewSprite.Hide();
            _thumbnailSprite.Show();
        }
    }
}