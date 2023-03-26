// <copyright file="UITextures.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    /// <summary>
    /// Texture atlas and file management.
    /// </summary>
    public static class UITextures
    {
        // Dictionary to cache texture atlas lookups.
        private static readonly Dictionary<string, UITextureAtlas> TextureCache = new Dictionary<string, UITextureAtlas>();

        // Dictionary to cache texture file lookups.
        private static readonly Dictionary<string, UITextureAtlas> FileCache = new Dictionary<string, UITextureAtlas>();

        /// <summary>
        /// Gets the "ingame" atlas.
        /// </summary>
        public static UITextureAtlas InGameAtlas => GetTextureAtlas("Ingame");

        /// <summary>
        /// Loads a cursor texture.
        /// </summary>
        /// <param name="cursorName">Cursor texture file name.</param>
        /// <returns>New cursor with default hotspot at 5,0.</returns>
        public static CursorInfo LoadCursor(string cursorName)
        {
            CursorInfo cursor = ScriptableObject.CreateInstance<CursorInfo>();

            cursor.m_texture = LoadTexture(cursorName);
            cursor.m_hotspot = new Vector2(5f, 0f);

            return cursor;
        }

        /// <summary>
        /// Loads a single-sprite texture from a given .png file.
        /// </summary>
        /// <param name="fileName">Atlas file name (".png" will be appended to make the filename).</param>
        /// <returns>New texture atlas.</returns>
        public static UITextureAtlas LoadSprite(string fileName)
        {
            // Check if we've already cached this file.
            if (FileCache.ContainsKey(fileName))
            {
                // Cached - return cached result.
                return FileCache[fileName];
            }

            // Create new texture atlas for button.
            UITextureAtlas newAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            newAtlas.name = fileName;
            newAtlas.material = UnityEngine.Object.Instantiate(UIView.GetAView().defaultAtlas.material);

            // Load texture from file.
            Texture2D newTexture = LoadTexture(fileName + ".png");
            newAtlas.material.mainTexture = newTexture;

            // Setup sprite.
            newAtlas.AddSprite(new UITextureAtlas.SpriteInfo
            {
                name = "normal",
                texture = newTexture,
                region = new Rect(0f, 0f, 1f, 1f),
            });

            // Add atlas to cache and return.
            FileCache.Add(fileName, newAtlas);
            return newAtlas;
        }

        /// <summary>
        /// Loads a four-sprite texture atlas from a given .png file.
        /// Assumes the texture file is 1x4 sprites which will be given the spritenames (in order) of:
        /// disabled, normal, pressed, hovered.
        /// </summary>
        /// <param name="fileName">Atlas file name (".png" will be appended to make the filename).</param>
        /// <returns>New texture atlas containing four sprites (disabled, normal, pressed, hovered).</returns>
        public static UITextureAtlas LoadQuadSpriteAtlas(string fileName) => LoadSpriteAtlas(fileName, new string[] { "disabled", "normal", "pressed", "hovered" });

        /// <summary>
        /// Loads a single-sprite texture atlas from a given .png file.
        /// </summary>
        /// <param name="fileName">Atlas file name (".png" will be appended to make the filename).</param>
        /// <returns>New texture atlas.</returns>
        public static UITextureAtlas LoadSingleSpriteAtlas(string fileName) => LoadSpriteAtlas(fileName, new string[] { "normal" });

        /// <summary>
        /// Loads a texture atlas from a given .png file.
        /// </summary>
        /// <param name="fileName">Atlas file name (".png" will be appended to make the filename).</param>
        /// <param name="spriteNames">Array of sprite names (in single row, read in equally-spaced columns from left to right).</param>
        /// <returns>New texture atlas.</returns>
        public static UITextureAtlas LoadSpriteAtlas(string fileName, string[] spriteNames)
        {
            // Check if we've already cached this file.
            if (FileCache.ContainsKey(fileName))
            {
                // Cached - return cached result.
                return FileCache[fileName];
            }

            // Create new texture atlas for button.
            UITextureAtlas newAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            newAtlas.name = fileName;
            newAtlas.material = UnityEngine.Object.Instantiate(UIView.GetAView().defaultAtlas.material);

            // Load texture from file.
            Texture2D newTexture = LoadTexture(fileName + ".png");
            newAtlas.material.mainTexture = newTexture;

            // Setup sprites.
            int numSprites = spriteNames.Length;
            float spriteWidth = 1f / spriteNames.Length;

            // Iterate through each sprite (counter increment is in region setup).
            for (int i = 0; i < numSprites; ++i)
            {
                UITextureAtlas.SpriteInfo sprite = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = newTexture,

                    // Sprite regions are horizontally arranged, evenly spaced.
                    region = new Rect(i * spriteWidth, 0f, spriteWidth, 1f),
                };
                newAtlas.AddSprite(sprite);
            }

            // Add atlas to cache and return.
            FileCache.Add(fileName, newAtlas);
            return newAtlas;
        }

        /// <summary>
        /// Returns a reference to the specified named atlas.
        /// </summary>
        /// <param name="atlasName">Atlas name.</param>
        /// <returns>Atlas reference (null if not found).</returns>
        public static UITextureAtlas GetTextureAtlas(string atlasName)
        {
            // Check if we've already cached this atlas.
            if (TextureCache.ContainsKey(atlasName))
            {
                // Cached - return cached result.
                return TextureCache[atlasName];
            }

            // Selections.
            int selectedAtlas = -1;
            int selectedAtlasSpriteCount = 0;

            // No cache entry - get game atlases and iterate through, looking for a name match.
            UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
            for (int i = 0; i < atlases.Length; ++i)
            {
                if (atlases[i].name.Equals(atlasName))
                {
                    // Found a matching name - if the number of sprites of this atlas is greater than the last one found, use this.
                    if (atlases[i].spriteNames.Length > selectedAtlasSpriteCount)
                    {
                        selectedAtlas = i;
                        selectedAtlasSpriteCount = atlases[i].spriteNames.Length;
                    }
                }
            }

            // If we found a suitable atlas, add it to the cache and return it.
            if (selectedAtlas >= 0)
            {
                TextureCache.Add(atlasName, atlases[selectedAtlas]);
                return atlases[selectedAtlas];
            }

            // If we got here, we couldn't find the specified atlas.
            return null;
        }

        /// <summary>
        /// Loads a 2D texture from a file.
        /// </summary>
        /// <param name="fileName">Texture file to load.</param>
        /// <returns>New 2D texture.</returns>
        private static Texture2D LoadTexture(string fileName)
        {
            try
            {
                using (Stream stream = OpenResourceFile(fileName))
                {
                    // New texture.
                    Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                    {
                        filterMode = FilterMode.Bilinear,
                        wrapMode = TextureWrapMode.Clamp,
                    };

                    // Read texture as byte stream from file.
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    texture.LoadImage(array);
                    texture.Apply();

                    return texture;
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception reading texture file ", fileName);
                return null;
            }
        }

        /// <summary>
        /// Opens the named resource file for reading.
        /// </summary>
        /// <param name="fileName">File to open.</param>
        /// <returns>Read-only file stream.</returns>
        private static Stream OpenResourceFile(string fileName)
        {
            string path = Path.Combine(AssemblyUtils.AssemblyPath, "Resources");
            return File.OpenRead(Path.Combine(path, fileName));
        }
    }
}
