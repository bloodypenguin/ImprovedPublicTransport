// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Serializer
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.IO;
using System.Xml.Serialization;

namespace ImprovedPublicTransport2
{
  public static class Serializer
  {
    private static readonly string _fileName = "ImprovedPublicTransportSettings.xml";

    public static Settings LoadSettings()
    {
      Settings settings = new Settings();
      try
      {
        using (StreamReader streamReader = new StreamReader(Serializer._fileName))
          settings = (Settings) new XmlSerializer(typeof (Settings)).Deserialize((TextReader) streamReader);
      }
      catch (Exception ex)
      {
        Utils.LogError((object) ex.Message);
      }
      if (settings != null)
        return settings;
      Utils.LogWarning((object) "Error while loading settings, will use default settings instead.");
      return new Settings();
    }

    public static void SaveSettings(Settings settings)
    {
      try
      {
        using (StreamWriter streamWriter = new StreamWriter(Serializer._fileName))
          new XmlSerializer(typeof (Settings)).Serialize((TextWriter) streamWriter, (object) settings);
      }
      catch (Exception ex)
      {
        Utils.LogError((object) ex.Message);
      }
    }
  }
}
