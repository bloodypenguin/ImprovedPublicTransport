// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Localization
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.Globalization;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImprovedPublicTransport2
{
  public class Localization
  {
    private static string _selectedLanguage = string.Empty;
    private static Dictionary<string, string> _defaultLocale;
    private static Dictionary<string, string> _selectedLocale;

    public static string Get(string id)
    {
      string str1;
      try
      {
        string str2;
        if (Localization._selectedLocale.TryGetValue(id, out str2))
          str1 = str2;
        else if (Localization._defaultLocale.TryGetValue(id, out str2))
        {
          str1 = str2;
        }
        else
        {
          Utils.Log((object) ("Error with " + id + ": The id was not found in the localization files."));
          str1 = id.ToString();
        }
      }
      catch (Exception ex)
      {
        Utils.Log((object) ("Error with " + id + ": " + (object) ex.GetType() + ": " + ex.Message));
        str1 = id.ToString();
      }
      return str1;
    }

    public static void Load()
    {
      try
      {
        if (Localization._defaultLocale == null)
          Localization._defaultLocale = Localization.Load(Path.Combine(Path.Combine(Utils.AssemblyPath, "Locale"), "en.txt"));
        string language = SingletonLite<LocaleManager>.instance.language;
        if (ImprovedPublicTransportMod.Settings.UseKoreanLocale)
        {
          Localization._selectedLocale = Localization.Load(Path.Combine(Path.Combine(Utils.AssemblyPath, "Locale"), "kr.txt"));
          Localization._selectedLanguage = "kr";
        }
        else
        {
          if (Localization._selectedLanguage.Equals(language))
            return;
          Localization._selectedLocale = Localization.Load(Path.Combine(Path.Combine(Utils.AssemblyPath, "Locale"), language + ".txt"));
          Localization._selectedLanguage = language;
        }
      }
      catch (Exception ex)
      {
        Utils.Log((object) ("Unexpected " + ex.GetType().Name + " loading localization: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine));
      }
    }

    private static Dictionary<string, string> Load(string path)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (File.Exists(path))
      {
        Utils.Log((object) ("Loading localization file: " + path));
        foreach (string readAllLine in File.ReadAllLines(path))
        {
          if (readAllLine != null)
          {
            string str = readAllLine.Trim();
            if (str.Length != 0)
            {
              int length = str.IndexOf(' ');
              if (length > 0)
                dictionary.Add(str.Substring(0, length), str.Substring(length + 1).Replace("\\n", "\n"));
            }
          }
        }
      }
      else
        Utils.Log((object) ("Localization file: " + path + " doesn't exists!"));
      return dictionary;
    }
  }
}
