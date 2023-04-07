// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleData
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ImprovedPublicTransport2.OptionsFramework;
using UnityEngine;

namespace ImprovedPublicTransport2
{
  public struct VehicleData
  {
    private int _lastStopNewPassengers;
    private int _averagePassengers;
    private int _averageIncome;
    private MovingAverage _passengerData;
    private MovingAverage _incomeData;

    public bool IsEmpty
    {
      get
      {
        if (this.PassengersThisWeek == 0)
          return this._passengerData == null;
        return false;
      }
    }

    public float[] PassengerData
    {
      get
      {
        if (this._passengerData == null)
          return new float[0];
        return this._passengerData.ToArray();
      }
      set
      {
        this._passengerData = new MovingAverage(value, OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
        this._averagePassengers = Mathf.RoundToInt(this._passengerData.Average);
      }
    }

    public float[] IncomeData
    {
      get
      {
        if (this._incomeData == null)
          return new float[0];
        return this._incomeData.ToArray();
      }
      set
      {
        this._incomeData = new MovingAverage(value, OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
        this._averageIncome = Mathf.RoundToInt(this._incomeData.Average);
      }
    }

    public ushort CurrentStop { get; set; }

    public bool IsUnbunchingInProgress { get; set; }

    public int LastStopGonePassengers { get; set; }

    public int LastStopNewPassengers
    {
      get
      {
        return this._lastStopNewPassengers;
      }
      set
      {
        this.PassengersThisWeek = this.PassengersThisWeek + value;
        this._lastStopNewPassengers = value;
      }
    }

    public int PassengersThisWeek { get; set; }

    public int PassengersLastWeek { get; set; }

    public int PassengersAverage
    {
      get
      {
        return this._averagePassengers;
      }
    }

    public int IncomeThisWeek { get; set; }

    public int IncomeLastWeek { get; set; }

    public int IncomeAverage
    {
      get
      {
        return this._averageIncome;
      }
    }

    public void BoardPassengers(int newPassengers, int ticketPrice, ushort stop)
    {
      this.LastStopNewPassengers = newPassengers;
      this.IncomeThisWeek += newPassengers * ticketPrice;
      this.CurrentStop = stop;
    }
    
    public void DisembarkPassengers(int passengersOut, ushort stop)
    {
      this.LastStopGonePassengers = passengersOut;
      this.CurrentStop = stop;
    }

    public void StartNewWeek(int maintenanceCost)
    {
      if (this._passengerData == null)
        this._passengerData = new MovingAverage(OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
      this._passengerData.Push((float) this.PassengersThisWeek);
      this._averagePassengers = Mathf.RoundToInt(this._passengerData.Average);
      this.PassengersLastWeek = this.PassengersThisWeek;
      this.PassengersThisWeek = 0;
      this.IncomeThisWeek -= maintenanceCost;
      if (this._incomeData == null)
        this._incomeData = new MovingAverage(OptionsWrapper<Settings.Settings>.Options.StatisticWeeks);
      this._incomeData.Push((float) this.IncomeThisWeek);
      this._averageIncome = Mathf.RoundToInt(this._incomeData.Average);
      this.IncomeLastWeek = this.IncomeThisWeek;
      this.IncomeThisWeek = 0;
    }
  }
}
