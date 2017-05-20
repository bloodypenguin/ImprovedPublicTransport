// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.Settings
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

namespace ImprovedPublicTransport2
{
  public class Settings
  {
    private bool _budgetControl = true;
    private int _spawnTimeInterval = 10;
    private string _speedString = "km/h";
    private bool _showLineInfo = true;
    private int _defaultVehicleCount = 1;
    private byte _intervalAggressionFactor = 13; //13*4+12=64s default, like in vanilla
    private bool _unbunching = true;
    private int _statisticWeeks = 10;
    private bool _compatibilityMode;
    private int _vehicleEditorPosition;
    private bool _hideVehicleEditor;
    private bool _useKoreanLocale;

    public bool BudgetControl
    {
      get
      {
        return this._budgetControl;
      }
      set
      {
        this._budgetControl = value;
      }
    }

    public bool CompatibilityMode //deprecated
    {
      get
      {
        return this._compatibilityMode;
      }
      set
      {
        this._compatibilityMode = value;
      }
    }

    public int SpawnTimeInterval
    {
      get
      {
        return this._spawnTimeInterval;
      }
      set
      {
        this._spawnTimeInterval = value;
      }
    }

    public string SpeedString
    {
      get
      {
        return this._speedString;
      }
      set
      {
        this._speedString = value;
      }
    }

    public bool ShowLineInfo
    {
      get
      {
        return this._showLineInfo;
      }
      set
      {
        this._showLineInfo = value;
      }
    }

    public int DefaultVehicleCount
    {
      get
      {
        return this._defaultVehicleCount;
      }
      set
      {
        this._defaultVehicleCount = value;
      }
    }

    public byte IntervalAggressionFactor
    {
      get
      {
        return this._intervalAggressionFactor;
      }
      set
      {
        this._intervalAggressionFactor = value;
      }
    }

    public bool Unbunching
    {
      get
      {
        return this._unbunching;
      }
      set
      {
        this._unbunching = value;
      }
    }

    public int StatisticWeeks
    {
      get
      {
        return this._statisticWeeks;
      }
      set
      {
        this._statisticWeeks = value;
      }
    }

    public int VehicleEditorPosition
    {
      get
      {
        return this._vehicleEditorPosition;
      }
      set
      {
        this._vehicleEditorPosition = value;
      }
    }

    public bool HideVehicleEditor
    {
      get
      {
        return this._hideVehicleEditor;
      }
      set
      {
        this._hideVehicleEditor = value;
      }
    }

    public bool UseKoreanLocale
    {
      get
      {
        return this._useKoreanLocale;
      }
      set
      {
        this._useKoreanLocale = value;
      }
    }

    public void LogSettings()
    {
      Utils.Log((object) "IPT Settings");
      Utils.Log((object) ("BudgetControl: " + this._budgetControl.ToString()));
      Utils.Log((object) ("CompatibilityMode: " + this._compatibilityMode.ToString()));
      Utils.Log((object) ("SpawnTimeInterval: " + (object) this._spawnTimeInterval));
      Utils.Log((object) ("SpeedString: " + this._speedString));
      Utils.Log((object) ("ShowLineInfo: " + this._showLineInfo.ToString()));
      Utils.Log((object) ("DefaultVehicleCount: " + (object) this._defaultVehicleCount));
      Utils.Log((object) ("IntervalAggressionFactor: " + (object) this._intervalAggressionFactor));
      Utils.Log((object) ("Unbunching: " + this._unbunching.ToString()));
      Utils.Log((object) ("StatisticWeeks: " + (object) this._statisticWeeks));
    }
  }
}
