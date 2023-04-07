// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.NodeData
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ImprovedPublicTransport2.OptionsFramework;
using UnityEngine;

namespace ImprovedPublicTransport2
{
  public struct NodeData
  {
    private bool _noUnbunching;
    private int _averagePassengersIn;
    private int _averagePassengersOut;
    private MovingAverage _passengerInData;
    private MovingAverage _passengerOutData;

    public bool IsEmpty
    {
      get
      {
        if (this.PassengersTotal == 0 && this._passengerInData == null)
          return !this._noUnbunching;
        return false;
      }
    }

    public bool Unbunching
    {
      get
      {
        return !this._noUnbunching;
      }
      set
      {
        this._noUnbunching = !value;
      }
    }

    public float[] PassengerInData
    {
      get
      {
        if (this._passengerInData == null)
          return new float[0];
        return this._passengerInData.ToArray();
      }
      set
      {
        this._passengerInData = new MovingAverage(value, OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
        this._averagePassengersIn = Mathf.RoundToInt(this._passengerInData.Average);
      }
    }

    public float[] PassengerOutData
    {
      get
      {
        if (this._passengerOutData == null)
          return new float[0];
        return this._passengerOutData.ToArray();
      }
      set
      {
        this._passengerOutData = new MovingAverage(value, OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
        this._averagePassengersOut = Mathf.RoundToInt(this._passengerOutData.Average);
      }
    }

    public int PassengersIn { get; set; }

    public int PassengersOut { get; set; }

    public int PassengersTotal
    {
      get
      {
        return this.PassengersIn + this.PassengersOut;
      }
    }

    public int LastWeekPassengersIn { get; set; }

    public int LastWeekPassengersOut { get; set; }

    public int LastWeekPassengersTotal
    {
      get
      {
        return this.LastWeekPassengersIn + this.LastWeekPassengersOut;
      }
    }

    public int AveragePassengersIn
    {
      get
      {
        return this._averagePassengersIn;
      }
    }

    public int AveragePassengersOut
    {
      get
      {
        return this._averagePassengersOut;
      }
    }

    public int AveragePassengersTotal
    {
      get
      {
        return this._averagePassengersIn + this._averagePassengersOut;
      }
    }

    public void StartNewWeek()
    {
      if (this._passengerInData == null)
        this._passengerInData = new MovingAverage(OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
      this._passengerInData.Push((float) this.PassengersIn);
      this._averagePassengersIn = Mathf.RoundToInt(this._passengerInData.Average);
      this.LastWeekPassengersIn = this.PassengersIn;
      this.PassengersIn = 0;
      if (this._passengerOutData == null)
        this._passengerOutData = new MovingAverage(OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
      this._passengerOutData.Push((float) this.PassengersOut);
      this._averagePassengersOut = Mathf.RoundToInt(this._passengerOutData.Average);
      this.LastWeekPassengersOut = this.PassengersOut;
      this.PassengersOut = 0;
    }
  }
}
