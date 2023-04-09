// <copyright file="AssemblyUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Plugins;

namespace ImprovedPublicTransport2.UI.AlgernonCommons
{
    using static ColossalFramework.Plugins.PluginManager;

    /// <summary>
    /// Core assembly-relateed utilities.
    /// </summary>
    public static class AssemblyUtils
    {
        // Mod assembly path cache.
        private static string s_assemblyPath = null;

        /// <summary>
        /// Gets current mod assembly name.
        /// </summary>
        public static string Name => Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// Gets current mod assembly version.
        /// </summary>
        public static Version CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Gets the current mod assembly version as a string, leaving off any trailing zero versions for build and revision.
        /// </summary>
        public static string TrimmedCurrentVersion => TrimVersion(CurrentVersion);

        /// <summary>
        /// Gets the mod directory filepath of the currently executing mod assembly.
        /// </summary>
        public static string AssemblyPath
        {
            get
            {
                // Return cached path if it exists.
                if (s_assemblyPath != null)
                {
                    return s_assemblyPath;
                }

                // No path cached - get list of currently active plugins.
                Assembly thisAssembly = Assembly.GetExecutingAssembly();
                IEnumerable<PluginInfo> plugins = PluginManager.instance.GetPluginsInfo();

                // Iterate through list.
                foreach (PluginInfo plugin in plugins)
                {
                    try
                    {
                        // Iterate through each assembly in plugins
                        foreach (Assembly assembly in plugin.GetAssemblies())
                        {
                            if (assembly == thisAssembly)
                            {
                                // Found it! Cache result and return path.
                                s_assemblyPath = plugin.modPath;
                                return plugin.modPath;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Don't care; just don't let a single failure stop us iterating through the plugins.
                        Logging.LogException(e, "exception iterating through plugins");
                    }
                }

                // If we got here, then we didn't find the assembly.
                Logging.Error("assembly path not found");
                return null;
            }
        }

        /// <summary>
        /// Gets the PluginInfo of the current enabled mod assembly.
        /// </summary>
        public static PluginInfo ThisPlugin => Singleton<PluginManager>.instance.FindPluginInfo(Assembly.GetExecutingAssembly());

        /// <summary>
        /// Returns the provided version as a string, leaving off any trailing zeros versions for build and revision.
        /// </summary>
        /// <param name="version">Version to display.</param>
        /// <returns>Trimmed version text.</returns>
        public static string TrimVersion(Version version)
        {
            // Trim off trailing zeros.
            if (version.Revision > 0)
            {
                // If any revision other than zero, we return the full version.
                return version.ToString(4);
            }
            else if (version.Build > 0)
            {
                // Revision is zero; if build is nonzero, return major.minor.build.
                // 1.0.1.0 => 1.0.1
                return version.ToString(3);
            }
            else
            {
                // Revision and build are zero; return major.minor.
                // 1.0.0.0 => 1.0
                return version.ToString(2);
            }
        }

        /// <summary>
        /// Checks to see if another mod is installed and enabled, based on a provided assembly name, and if so, returns the assembly reference.
        /// Case-sensitive!  PloppableRICO is not the same as ploppablerico.
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly.</param>
        /// <returns>Assembly reference if target is found and enabled, null otherwise.</returns>
        public static Assembly GetEnabledAssembly(string assemblyName)
        {
            // Iterate through the full list of plugins.
            foreach (PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                // Only looking at enabled plugins.
                if (plugin.isEnabled)
                {
                    foreach (Assembly assembly in plugin.GetAssemblies())
                    {
                        if (assembly.GetName().Name.Equals(assemblyName))
                        {
                            Logging.Message("found enabled mod assembly ", assemblyName, ", version ", assembly.GetName().Version);
                            return assembly;
                        }
                    }
                }
            }

            // If we've made it here, then we haven't found a matching assembly.
            Logging.Message("didn't find enabled assembly ", assemblyName);
            return null;
        }

        /// <summary>
        /// Checks to see if another mod is installed, based on a provided assembly name.
        /// Case-sensitive!  PloppableRICO is not the same as ploppablerico.
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly.</param>
        /// <returns>True if target is found, null otherwise.</returns>
        public static bool IsAssemblyPresent(string assemblyName)
        {
            // Iterate through the full list of plugins.
            foreach (PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                // Look at each assembly in plugin.
                foreach (Assembly assembly in plugin.GetAssemblies())
                {
                    if (assembly.GetName().Name.Equals(assemblyName))
                    {
                        Logging.Message("found mod assembly ", assemblyName, ", version ", assembly.GetName().Version);
                        return true;
                    }
                }
            }

            // If we've made it here, then we haven't found a matching assembly.
            Logging.Message("didn't find assembly ", assemblyName);
            return false;
        }
    }
}
