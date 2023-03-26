// <copyright file="ModBase.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    /// <summary>
    /// Base mod class, with settings file and optional what's new notifications.
    /// </summary>
    public abstract class ModBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModBase"/> class.
        /// </summary>
        public ModBase()
        {
            // Set base instance reference.
            Instance = this;
        }

        /// <summary>
        /// Gets the active instance reference.
        /// </summary>
        public static ModBase Instance { get; private set; }

        /// <summary>
        /// Gets the mod's base display name (name only).
        /// </summary>
        public abstract string BaseName { get; }

        /// <summary>
        /// Gets the mod's full display name, including version.
        /// </summary>
        public virtual string Name => BaseName + ' ' + AssemblyUtils.TrimmedCurrentVersion;

        /// <summary>
        /// Gets the mod's name for logging purposes.
        /// </summary>
        public virtual string LogName => BaseName;

        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public virtual void OnEnabled() => LoadSettings();

        /// <summary>
        /// Load mod settings.
        /// </summary>
        public abstract void LoadSettings();

        /// <summary>
        /// Saves mod settings.
        /// </summary>
        public abstract void SaveSettings();
    }
}