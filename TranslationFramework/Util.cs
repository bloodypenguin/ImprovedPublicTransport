using System;
using System.Linq;
using ColossalFramework.Plugins;
using ICities;

namespace ImprovedPublicTransport2.TranslationFramework
{
    public static class Util
    {
        public static string AssemblyPath(Type modType)
        {
            return PluginInfo(modType).modPath;
        }

        private static PluginManager.PluginInfo PluginInfo(Type modType)
        {
            var pluginManager = PluginManager.instance;
            var plugins = pluginManager.GetPluginsInfo();
            try
            {
                foreach (var item in plugins)
                {
                    try
                    {
                        var instances = item.GetInstances<IUserMod>();
                        if (modType != instances.FirstOrDefault()?.GetType())
                        {
                            continue;
                        }
                        return item;
                    }
                    catch
                    {
                        //do nothing
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to find assembly of type" + modType, e);
            }
            throw new Exception("Failed to find assembly of type" + modType);
        }
    }
}