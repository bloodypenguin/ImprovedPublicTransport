// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Utils
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ImprovedPublicTransport.Detour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public static class Utils
  {
    private static readonly string _fileName = "ImprovedPublicTransport.log";
    private static readonly string _logPrefix = "ImprovedPublicTransport: ";
    private static string _modPath;

    public static string ModPath
    {
      get
      {
        if (Utils._modPath == null)
          Utils._modPath = Utils.GetModPath(typeof (ImprovedPublicTransportMod).Assembly.GetName().Name, 424106600UL);
        return Utils._modPath;
      }
    }

    public static void ClearLogFile()
    {
      try
      {
        File.WriteAllText(Utils._fileName, string.Empty);
      }
      catch
      {
        Debug.LogWarning((object) ("Error while clearing log file: " + Utils._fileName));
      }
    }

    public static void LogToTxt(object o)
    {
      try
      {
        File.AppendAllText(Utils._fileName, DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy ") + o + Environment.NewLine);
      }
      catch
      {
        Debug.LogWarning((object) ("Error while writing to log file: " + Utils._fileName));
      }
    }

    public static void Log(object o)
    {
      Utils.Log(PluginManager.MessageType.Message, o);
    }

    public static void LogError(object o)
    {
      Utils.Log(PluginManager.MessageType.Error, o);
    }

    public static void LogWarning(object o)
    {
      Utils.Log(PluginManager.MessageType.Warning, o);
    }

    private static void Log(PluginManager.MessageType type, object o)
    {
      string str = Utils._logPrefix + o;
      switch (type)
      {
        case PluginManager.MessageType.Error:
          Debug.LogError((object) str);
          break;
        case PluginManager.MessageType.Warning:
          Debug.LogWarning((object) str);
          break;
        case PluginManager.MessageType.Message:
          Debug.Log((object) str);
          break;
      }
    }

    public static Q GetPrivate<Q>(object o, string fieldName)
    {
      FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      FieldInfo fieldInfo1 = (FieldInfo) null;
      foreach (FieldInfo fieldInfo2 in fields)
      {
        if (fieldInfo2.Name == fieldName)
        {
          fieldInfo1 = fieldInfo2;
          break;
        }
      }
      return (Q) fieldInfo1.GetValue(o);
    }

    public static float ToSingle(string value)
    {
      float result = 0.0f;
      float.TryParse(value, out result);
      return result;
    }

    public static int ToInt32(string value)
    {
      int result = 0;
      int.TryParse(value, out result);
      return result;
    }

    public static byte ToByte(string value)
    {
      byte result = 0;
      byte.TryParse(value, out result);
      return result;
    }

    public static bool Truncate(UILabel label, string text, string suffix = "…")
    {
      bool flag = false;
      try
      {
        using (UIFontRenderer renderer = label.ObtainRenderer())
        {
          float units = label.GetUIView().PixelsToUnits();
          float[] characterWidths = renderer.GetCharacterWidths(text);
          float num1 = 0.0f;
          float num2 = (float) ((double) label.width - (double) label.padding.horizontal - 2.0);
          for (int index = 0; index < characterWidths.Length; ++index)
          {
            num1 += characterWidths[index] / units;
            if ((double) num1 > (double) num2)
            {
              flag = true;
              text = text.Substring(0, index - 3) + suffix;
              break;
            }
          }
        }
        label.text = text;
      }
      catch
      {
        flag = false;
      }
      return flag;
    }

    public static string RemoveInvalidFileNameChars(string fileName)
    {
      return ((IEnumerable<char>) Path.GetInvalidFileNameChars()).Aggregate<char, string>(fileName, (Func<string, char, string>) ((current, c) => current.Replace(c.ToString(), string.Empty)));
    }

    public static int RoundToNearest(float value, int nearest)
    {
      return Mathf.RoundToInt(value / (float) nearest) * nearest;
    }

    public static bool AreParametersEqual(ParameterInfo[] sourceParameters, ParameterInfo[] destinationParameters)
    {
      if (sourceParameters.Length != destinationParameters.Length)
        return false;
      for (int index = 0; index < sourceParameters.Length; ++index)
      {
        if (!sourceParameters[index].ParameterType.IsAssignableFrom(destinationParameters[index].ParameterType))
          return false;
      }
      return true;
    }

    public static string GetModPath(string assemblyName, ulong workshopId)
    {
      foreach (PluginManager.PluginInfo pluginInfo in Singleton<PluginManager>.instance.GetPluginsInfo())
      {
        if (pluginInfo.name == assemblyName || (long) pluginInfo.publishedFileID.AsUInt64 == (long) workshopId)
          return pluginInfo.modPath;
      }
      return (string) null;
    }
  }
}
