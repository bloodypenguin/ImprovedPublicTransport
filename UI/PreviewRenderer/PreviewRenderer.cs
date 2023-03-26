// <copyright file="PreviewRenderer.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VehicleSelector
{
    using ColossalFramework;
    using UnityEngine;

    /// <summary>
    /// Prop and tree preview renderer.
    /// </summary>
    internal class PreviewRenderer : MonoBehaviour
    {
        // Rendering components and parameters.
        private readonly Camera _renderCamera;
        private Mesh _mesh;
        private Material _material;
        private float _rotation = 35f;
        private float _zoom = 3f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewRenderer"/> class.
        /// </summary>
        internal PreviewRenderer()
        {
            // Set up camera.
            _renderCamera = new GameObject("Camera").AddComponent<Camera>();
            _renderCamera.transform.SetParent(transform);
            _renderCamera.targetTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            _renderCamera.allowHDR = true;
            _renderCamera.enabled = false;
            _renderCamera.clearFlags = CameraClearFlags.Color;

            // Basic defaults.
            _renderCamera.pixelRect = new Rect(0f, 0f, 512, 512);
            _renderCamera.backgroundColor = new Color(0, 0, 0, 0);
            _renderCamera.fieldOfView = 30f;
            _renderCamera.nearClipPlane = 1f;
            _renderCamera.farClipPlane = 1000f;
        }

        /// <summary>
        /// Sets the render image size.
        /// </summary>
        internal Vector2 Size
        {
            set
            {
                // New size; set camera output sizes accordingly.
                _renderCamera.targetTexture = new RenderTexture((int)value.x, (int)value.y, 24, RenderTextureFormat.ARGB32);
                _renderCamera.pixelRect = new Rect(0f, 0f, value.x, value.y);
            }
        }

        /// <summary>
        /// Sets the mesh to be rendered.
        /// </summary>
        internal Mesh Mesh { set => _mesh = value; }

        /// <summary>
        /// Sets the render material.
        /// </summary>
        internal Material Material { set => _material = value; }

        /// <summary>
        /// Gets the current render target textures.
        /// </summary>
        internal RenderTexture Texture => _renderCamera.targetTexture;

        /// <summary>
        /// Gets or sets the preveiew camera rotation (in degrees).
        /// </summary>
        internal float CameraRotation
        {
            get => _rotation;

            // Rotation in degrees is modulo 360.
            set => _rotation = value % 360f;
        }

        /// <summary>
        /// Gets or sets the preview zoom level.
        /// </summary>
        internal float Zoom
        {
            get => _zoom;
            set => _zoom = Mathf.Clamp(value, 0.5f, 5f);
        }

        /// <summary>
        /// Gets the default prop shader.
        /// </summary>
        internal Shader PropShader => Shader.Find("Custom/Props/Prop/Default");

        /// <summary>
        /// Gets the default prop fence shader.
        /// </summary>
        internal Shader PropFenceShader => Shader.Find("Custom/Props/Prop/Fence");

        /// <summary>
        /// Gets the default diffuse shader.
        /// </summary>
        internal Shader DiffuseShader => Shader.Find("Diffuse");

        /// <summary>
        /// Gets the default treee shader.
        /// </summary>
        internal Shader TreeShader => Shader.Find("Custom/Trees/Default");

        /// <summary>
        /// Render the current mesh.
        /// </summary>
        public void Render()
        {
            // If no mesh, don't do anything here.
            if (_mesh == null)
            {
                return;
            }

            // Set background.
            _renderCamera.clearFlags = CameraClearFlags.Color;
            _renderCamera.backgroundColor = new Color32(33, 151, 199, 255);

            // Back up current game InfoManager mode.
            InfoManager infoManager = Singleton<InfoManager>.instance;
            InfoManager.InfoMode currentMode = infoManager.CurrentMode;
            InfoManager.SubInfoMode currentSubMode = infoManager.CurrentSubMode;

            // Set current game InfoManager to default (don't want to render with an overlay mode).
            infoManager.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            infoManager.UpdateInfoMode();

            // Backup current exposure and sky tint.
            float gameExposure = DayNightProperties.instance.m_Exposure;
            Color gameSkyTint = DayNightProperties.instance.m_SkyTint;

            // Backup current game lighting.
            Light gameMainLight = RenderManager.instance.MainLight;

            // Set exposure and sky tint for render.
            DayNightProperties.instance.m_Exposure = 0.75f;
            DayNightProperties.instance.m_SkyTint = new Color(0, 0, 0);
            DayNightProperties.instance.Refresh();

            // Set up our render lighting settings.
            Light renderLight = DayNightProperties.instance.sunLightSource;
            RenderManager.instance.MainLight = renderLight;

            // Reset the bounding box to be the smallest that can encapsulate all verticies of the new mesh.
            // That way the preview image is the largest size that fits cleanly inside the preview size.
            Bounds currentBounds = new Bounds(Vector3.zero, Vector3.zero);
            Vector3[] vertices;

            // Is the mesh readable, i.e. not locked?
            if (_mesh.isReadable)
            {
                // Readable mesh - calculate our own bounds, as some preset bounds are unreliable.
                // Use separate verticies instance instead of accessing Mesh.vertices each time (which is slow).
                // >10x measured performance improvement by doing things this way instead.
                vertices = _mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    currentBounds.Encapsulate(vertices[i]);
                }
            }
            else
            {
                // Locked mesh - use default bounds.
                currentBounds = _mesh.bounds;
            }

            // Expand bounds slightly.
            currentBounds.Expand(1f);

            // Set our model rotation parameters, so we look at it obliquely.
            const float xRotation = 20f;

            // Apply model rotation with our camnera rotation into a quaternion.
            Quaternion modelRotation = Quaternion.Euler(xRotation, 0f, 0f) * Quaternion.Euler(0f, CameraRotation, 0f);

            // Set material to use when previewing.
            Material previewMaterial = _material;

            // Override material for mssing or non-standard shaders.
            if (_material?.shader != null && _material.shader != TreeShader && _material.shader != PropShader)
            {
                previewMaterial = new Material(DiffuseShader)
                {
                    mainTexture = _material.mainTexture,
                };
            }

            // Don't render anything if we don't have a material.
            if (_material != null)
            {
                // Calculate rendering matrix and add mesh to scene.
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, modelRotation, Vector3.one);
                Graphics.DrawMesh(_mesh, matrix, previewMaterial, 0, _renderCamera, 0, null, true, true);
            }

            // Set zoom to encapsulate entire model.
            float magnitude = currentBounds.extents.magnitude;
            float clipExtent = (magnitude + 16f) * 1.5f;
            float clipCenter = magnitude * Zoom;

            // Clip planes.
            _renderCamera.nearClipPlane = Mathf.Max(clipCenter - clipExtent, 0.01f);
            _renderCamera.farClipPlane = clipCenter + clipExtent;

            // Rotate our camera around the model according to our current rotation.
            _renderCamera.transform.position = Vector3.forward * clipCenter;

            // Aim camera at middle of bounds.
            _renderCamera.transform.LookAt(currentBounds.center);

            // If game is currently in nighttime, enable sun and disable moon lighting.
            if (gameMainLight == DayNightProperties.instance.moonLightSource)
            {
                DayNightProperties.instance.sunLightSource.enabled = true;
                DayNightProperties.instance.moonLightSource.enabled = false;
            }

            // Light settings.
            renderLight.transform.eulerAngles = new Vector3(45f, 180f, 0f);
            renderLight.intensity = 2f;
            renderLight.color = Color.white;

            // Render!
            _renderCamera.RenderWithShader(previewMaterial.shader, string.Empty);

            // Restore game lighting.
            RenderManager.instance.MainLight = gameMainLight;

            // Reset to moon lighting if the game is currently in nighttime.
            if (gameMainLight == DayNightProperties.instance.moonLightSource)
            {
                DayNightProperties.instance.sunLightSource.enabled = false;
                DayNightProperties.instance.moonLightSource.enabled = true;
            }

            // Restore game exposure and sky tint.
            DayNightProperties.instance.m_Exposure = gameExposure;
            DayNightProperties.instance.m_SkyTint = gameSkyTint;
            DayNightProperties.instance.Refresh();

            // Restore game InfoManager mode.
            infoManager.SetCurrentMode(currentMode, currentSubMode);
            infoManager.UpdateInfoMode();
        }
    }
}