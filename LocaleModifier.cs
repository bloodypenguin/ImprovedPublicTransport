// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.LocaleModifier
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.Globalization;
using System;
using System.Collections.Generic;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2
{
  public static class LocaleModifier
  {
    public static void Init()
    {
      LocaleManager.eventLocaleChanged += new LocaleManager.LocaleChangedHandler(LocaleModifier.Modify);
      LocaleModifier.Modify();
    }

    public static void Deinit()
    {
      LocaleManager.eventLocaleChanged -= new LocaleManager.LocaleChangedHandler(LocaleModifier.Modify);
      LocaleModifier.Modify("AIINFO_BUSDEPOT_BUSCOUNT", "{0} / {1}", "{0}");
      LocaleModifier.Modify("AIINFO_TRAMDEPOT_TRAMCOUNT", "{0} / {1}", "{0}");
    }

    private static void Modify()
    {
      LocaleModifier.Modify("AIINFO_BUSDEPOT_BUSCOUNT", "{0}", "{0} / {1}");
      LocaleModifier.Modify("AIINFO_TRAMDEPOT_TRAMCOUNT", "{0}", "{0} / {1}");
    }

    private static void Modify(string id, string oldValue, string newValue)
    {
      try
      {
        Dictionary<Locale.Key, string> dictionary = Utils.GetPrivate<Dictionary<Locale.Key, string>>((object) Utils.GetPrivate<Locale>((object) SingletonLite<LocaleManager>.instance, "m_Locale"), "m_LocalizedStrings");
        Locale.Key key = new Locale.Key()
        {
          m_Identifier = id
        };
        string str;
        if (!dictionary.TryGetValue(key, out str))
          return;
        dictionary[key] = str.Replace(oldValue, newValue);
      }
      catch (Exception ex)
      {
        Utils.LogWarning((object) ("Unexpected " + ex.GetType().Name + " updating localization: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine));
      }
    }
  }
}
