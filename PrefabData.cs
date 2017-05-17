// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.PrefabData
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public class PrefabData
  {
    private bool _saveToXml = true;
    private VehicleInfo _info;
    private VehicleInfo.VehicleTrailer _lastTrailer;
    private PrefabData[] _trailerData;
    private PrefabData.DefaultPrefabData Defaults;
    private bool _changeFlag;
    private int _maintenanceCost;

    public int PrefabDataIndex
    {
      get
      {
        return this._info.m_prefabDataIndex;
      }
    }

    public VehicleInfo Info
    {
      get
      {
        return this._info;
      }
    }

    public string ObjectName
    {
      get
      {
        return this._info.name;
      }
    }

    public string Title
    {
      get
      {
        return ColossalFramework.Globalization.Locale.Get("VEHICLE_TITLE", PrefabCollection<VehicleInfo>.PrefabName((uint) this.PrefabDataIndex));
      }
    }

    public int TotalCapacity
    {
      get
      {
        int num = 0;
        if (this._trailerData != null)
        {
          for (int index = 0; index < this._trailerData.Length; ++index)
            num += this._trailerData[index].Capacity;
        }
        return num + PrefabData.GetCapacity(this._info.m_class.m_service, this._info.m_class.m_subService, this._info.m_class.m_level, this._info.m_vehicleAI);
      }
    }

    public int Capacity
    {
      get
      {
        return PrefabData.GetCapacity(this._info.m_class.m_service, this._info.m_class.m_subService, this._info.m_class.m_level, this._info.m_vehicleAI);
      }
      set
      {
        if (this.Capacity == value)
          return;
        if (this._trailerData != null)
        {
          for (int index = 0; index < this._trailerData.Length; ++index)
            this._trailerData[index].Capacity = value;
        }
        PrefabData.SetCapacity(this._info.m_class.m_service, this._info.m_class.m_subService, this._info.m_class.m_level, this._info.m_vehicleAI, value);
        if (this._info.m_class.m_subService != ItemClass.SubService.PublicTransportTaxi)
          this.EnsureCitizenUnits();
        this._changeFlag = true;
      }
    }

    public int CarCount
    {
      get
      {
        int num = 1;
        if (this._trailerData != null)
        {
          for (int index = 0; index < this._trailerData.Length; ++index)
          {
            if (this._trailerData[index].Info.GetSubService() == this._info.GetSubService())
              ++num;
          }
        }
        return num;
      }
    }

    public int MaxSpeed
    {
      get
      {
        return (int) this._info.m_maxSpeed;
      }
      set
      {
        if (this.MaxSpeed == value)
          return;
        this._info.m_maxSpeed = (float) value;
        this._changeFlag = true;
      }
    }

    public int MaintenanceCost
    {
      get
      {
        ItemClass.Service service = this._info.GetService();
        ItemClass.SubService subService = this._info.GetSubService();
        ItemClass.Level level = this._info.GetClassLevel();
        switch (subService)
        {
          case ItemClass.SubService.PublicTransportBus:
          case ItemClass.SubService.PublicTransportMetro:
          case ItemClass.SubService.PublicTransportTrain:
          case ItemClass.SubService.PublicTransportTram:
          case ItemClass.SubService.PublicTransportShip:
          case ItemClass.SubService.PublicTransportPlane:
          case ItemClass.SubService.PublicTransportMonorail:
          case ItemClass.SubService.PublicTransportCableCar:
            if (this._maintenanceCost == 0)
            {
              float num = (float) this.TotalCapacity / (float) this.CarCount / (float) GameDefault.GetCapacity(service, subService, level);
              this.MaintenanceCost = Mathf.RoundToInt((float) (PrefabData.GetMaintenanceCost(service, subService, level, this._info.m_vehicleAI) * 16) * num);
            }
            return this._maintenanceCost;
          default:
            return 0;
        }
      }
      set
      {
        if (this._maintenanceCost == value || value <= 0)
          return;
        this._maintenanceCost = value;
        this._changeFlag = true;
      }
    }

    public int TicketPrice
    {
      get
      {
        return PrefabData.GetTicketPrice(this._info.m_class.m_service, this._info.m_class.m_subService, this._info.m_class.m_level, this._info.m_vehicleAI);
      }
      set
      {
        if (this.TicketPrice == value)
          return;
        PrefabData.SetTicketPrice(this._info.m_class.m_service, this._info.m_class.m_subService, this._info.m_class.m_level, this._info.m_vehicleAI, value);
        this._changeFlag = true;
      }
    }

    public bool EngineOnBothEnds
    {
      get
      {
        if (this._info.GetSubService() != ItemClass.SubService.PublicTransportTrain || this._info.m_trailers == null)
          return false;
        int length = this._info.m_trailers.Length;
        return length > 0 && (UnityEngine.Object) this._info.m_trailers[length - 1].m_info == (UnityEngine.Object) this._info;
      }
      set
      {
        if (this._info.m_trailers == null || this._info.GetSubService() != ItemClass.SubService.PublicTransportTrain || this.EngineOnBothEnds == value)
          return;
        int length = this._info.m_trailers.Length;
        if (length <= 1)
          return;
        if (value && (UnityEngine.Object) this._info.m_trailers[length - 1].m_info != (UnityEngine.Object) this._info)
        {
          Utils.Log((object) ("Replacing last trailer with engine and inverting it for " + this.ObjectName));
          this._info.m_trailers[length - 1].m_info = this._info;
          this._info.m_trailers[length - 1].m_invertProbability = 100;
          this.ApplyBackEngine(this._info, 100);
          this._changeFlag = true;
        }
        else
        {
          if (value)
            return;
          Utils.Log((object) ("Reverting last trailer setting for " + this.ObjectName));
          this._info.m_trailers[length - 1].m_info = this._lastTrailer.m_info;
          this._info.m_trailers[length - 1].m_invertProbability = this._lastTrailer.m_invertProbability;
          this.ApplyBackEngine(this._lastTrailer.m_info, this._lastTrailer.m_invertProbability);
          this._changeFlag = true;
        }
      }
    }

    public PrefabData(VehicleInfo info)
    {
      this._info = info;
      Utils.Log((object) ("Creating PrefabData for " + this.ObjectName));
      if (this.ObjectName == "451494281.London 1992 Stock (4 car)_Data")
      {
        int length = 3;
        this._info.m_trailers = new VehicleInfo.VehicleTrailer[length];
        for (int index = 0; index < length; ++index)
        {
          this._info.m_trailers[index].m_info = this._info;
          this._info.m_trailers[index].m_invertProbability = 50;
          this._info.m_trailers[index].m_probability = 100;
        }
      }
      else if (this.ObjectName.Contains("D3S Solaris Urbino 24 '15") && this._info.m_trailers != null && this._info.m_trailers.Length != 0)
      {
        this._info.m_dampers = 0.6f;
        VehicleInfo loaded = PrefabCollection<VehicleInfo>.FindLoaded(this.ObjectName.Substring(0, this.ObjectName.IndexOf(".")) + ".D3S Solaris Urbino 24 '15 (II)_Data");
        if ((UnityEngine.Object) loaded != (UnityEngine.Object) null)
        {
          Utils.Log((object) ("Fixing " + this.ObjectName));
          this._info.m_trailers[0].m_info.m_dampers = 0.6f;
          loaded.m_attachOffsetFront = 1.07f;
          loaded.m_dampers = 0.6f;
          this._info.m_trailers[1].m_info = loaded;
          this._info.m_trailers[1].m_invertProbability = 0;
          this._info.m_trailers[1].m_probability = 100;
          this.ApplyBackEngine(loaded, 0);
        }
      }
      if (this._info.m_trailers != null)
      {
        int length = this._info.m_trailers.Length;
        if (length > 0)
        {
          this._lastTrailer = this._info.m_trailers[length - 1];
          this._trailerData = new PrefabData[length];
          for (int index = 0; index < length; ++index)
          {
            VehicleInfo info1 = this._info.m_trailers[index].m_info;
            if ((UnityEngine.Object) info1 != (UnityEngine.Object) null)
              this._trailerData[index] = PrefabData.CreateTrailerData(info1);
          }
        }
      }
      this.CacheDefaults();
      this.LoadPrefabData();
    }

    private PrefabData()
    {
    }

    public void SetValues(int capacity, int maintenanceCost, int ticketPrice, int maxSpeed, bool engineOnBothEnds)
    {
      this.Capacity = capacity;
      this.MaintenanceCost = maintenanceCost;
      this.TicketPrice = ticketPrice;
      this.MaxSpeed = maxSpeed;
      this.EngineOnBothEnds = engineOnBothEnds;
      if (!this._changeFlag)
        return;
      this.SavePrefabData();
      this._changeFlag = false;
    }

    public void SetDefaults()
    {
      this.Capacity = this.Defaults.Capacity;
      this.MaintenanceCost = this.Defaults.MaintenanceCost;
      this.TicketPrice = this.Defaults.TicketPrice;
      this.MaxSpeed = this.Defaults.MaxSpeed;
      this.EngineOnBothEnds = this.Defaults.EngineOnBothEnds;
      if (this._trailerData != null)
      {
        for (int index = 0; index < this._trailerData.Length; ++index)
          this._trailerData[index].SetDefaults();
      }
      if (!this._changeFlag)
        return;
      this.SavePrefabData();
      this._changeFlag = false;
    }

    private static int GetCapacity(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai)
    {
      int num = 0;
        if (service == ItemClass.Service.PublicTransport)
        {
            if (level == ItemClass.Level.Level1)
            {
                if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
                    num = (ai as BusAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportMetro && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerShipAI)
                    num = (ai as PassengerShipAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                    num = (ai as PassengerPlaneAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportTaxi && ai is TaxiAI)
                    num = (ai as TaxiAI).m_travelCapacity;
                else if (subService == ItemClass.SubService.PublicTransportTram && ai is TramAI)
                    num = (ai as TramAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportMonorail && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportCableCar && ai is CableCarAI)
                    num = (ai as CableCarAI).m_passengerCapacity;
            }
            else if (level == ItemClass.Level.Level2)
            {
                if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
                    num = (ai as PassengerFerryAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
                    num = (ai as PassengerBlimpAI).m_passengerCapacity;
            }

        }
        else if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI)
        {
            num = (ai as BusAI).m_passengerCapacity;
        }
      return num;
    }

    private static void SetCapacity(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai, int capacity)
    {
        if (service == ItemClass.Service.PublicTransport)
        {
            if (level == ItemClass.Level.Level1)
            {
                if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
                    (ai as BusAI).m_passengerCapacity = capacity;
                else if (subService == ItemClass.SubService.PublicTransportMetro && ai is PassengerTrainAI)
                    (ai as PassengerTrainAI).m_passengerCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
                    (ai as PassengerTrainAI).m_passengerCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerShipAI)
                    (ai as PassengerShipAI).m_passengerCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                    (ai as PassengerPlaneAI).m_passengerCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportTaxi && ai is TaxiAI)
                    (ai as TaxiAI).m_travelCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportTram && ai is TramAI)
                    (ai as TramAI).m_passengerCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportMonorail && ai is PassengerTrainAI)
                    (ai as PassengerTrainAI).m_passengerCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportCableCar && ai is CableCarAI)
                    (ai as CableCarAI).m_passengerCapacity = capacity;
                }
            else if (level == ItemClass.Level.Level2)
            {
                if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
                    (ai as PassengerFerryAI).m_passengerCapacity = capacity;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
                    (ai as PassengerBlimpAI).m_passengerCapacity = capacity;
                }

        }
        else if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI)
        {
            (ai as BusAI).m_passengerCapacity = capacity;
            }
        }

    public static int GetMaintenanceCost(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai)
    {
        int num = 0;
        if (service == ItemClass.Service.PublicTransport)
        {
            if (level == ItemClass.Level.Level1)
            {
                if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
                    num = (ai as BusAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportMetro && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerShipAI)
                    num = (ai as PassengerShipAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                    num = (ai as PassengerPlaneAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportTaxi && ai is TaxiAI)
                    num = (ai as TaxiAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportTram && ai is TramAI)
                    num = (ai as TramAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportMonorail && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportCableCar && ai is CableCarAI)
                    num = (ai as CableCarAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                }
            else if (level == ItemClass.Level.Level2)
            {
                if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
                    num = (ai as PassengerFerryAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
                    num = (ai as PassengerBlimpAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
                }

        }
        else if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI)
        {
            num = (ai as BusAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
        }
        return num;
        }

    private static int GetTicketPrice(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai)
    {
        int num = 0;
        if (service == ItemClass.Service.PublicTransport)
        {
            if (level == ItemClass.Level.Level1)
            {
                if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
                    num = (ai as BusAI).m_ticketPrice;
                else if (subService == ItemClass.SubService.PublicTransportMetro && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerShipAI)
                    num = (ai as PassengerShipAI).m_ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                    num = (ai as PassengerPlaneAI).m_ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportTaxi && ai is TaxiAI)
                    num = (ai as TaxiAI).m_pricePerKilometer;
                    else if (subService == ItemClass.SubService.PublicTransportTram && ai is TramAI)
                    num = (ai as TramAI).m_ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportMonorail && ai is PassengerTrainAI)
                    num = (ai as PassengerTrainAI).m_ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportCableCar && ai is CableCarAI)
                    num = (ai as CableCarAI).m_ticketPrice;
                }
            else if (level == ItemClass.Level.Level2)
            {
                if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
                    num = (ai as PassengerFerryAI).m_ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
                    num = (ai as PassengerBlimpAI).m_ticketPrice;
                }

        }
        else if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI)
        {
            num = (ai as BusAI).m_ticketPrice;
            }
        return num;
        }

    private static void SetTicketPrice(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai, int ticketPrice)
    {
        if (service == ItemClass.Service.PublicTransport)
        {
            if (level == ItemClass.Level.Level1)
            {
                if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
                    (ai as BusAI).m_ticketPrice = ticketPrice;
                else if (subService == ItemClass.SubService.PublicTransportMetro && ai is PassengerTrainAI)
                    (ai as PassengerTrainAI).m_ticketPrice = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
                    (ai as PassengerTrainAI).m_ticketPrice = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerShipAI)
                    (ai as PassengerShipAI).m_ticketPrice = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                    (ai as PassengerPlaneAI).m_ticketPrice = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportTaxi && ai is TaxiAI)
                    (ai as TaxiAI).m_pricePerKilometer = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportTram && ai is TramAI)
                    (ai as TramAI).m_ticketPrice = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportMonorail && ai is PassengerTrainAI)
                    (ai as PassengerTrainAI).m_ticketPrice = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportCableCar && ai is CableCarAI)
                    (ai as CableCarAI).m_ticketPrice = ticketPrice;
                }
            else if (level == ItemClass.Level.Level2)
            {
                if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
                    (ai as PassengerFerryAI).m_ticketPrice = ticketPrice;
                    else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
                    (ai as PassengerBlimpAI).m_ticketPrice = ticketPrice;
                }

        }
        else if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI)
        {
            (ai as BusAI).m_ticketPrice = ticketPrice;
            }
        }

    private void LoadPrefabData()
    {
      try
      {
        string str = "IptVehicleData";
        if (!Directory.Exists(str))
          Directory.CreateDirectory(str);
        string path2 = Utils.RemoveInvalidFileNameChars(this.ObjectName + ".xml");
        string path = System.IO.Path.Combine(str, path2);
        if (!File.Exists(path))
        {
          Utils.Log((object) ("No stored data found for " + this.ObjectName));
        }
        else
        {
          Utils.Log((object) ("Found stored data for " + this.ObjectName));
          using (StreamReader streamReader = new StreamReader(path))
          {
            PrefabData.DefaultPrefabData defaultPrefabData = (PrefabData.DefaultPrefabData) new XmlSerializer(typeof (PrefabData.DefaultPrefabData)).Deserialize((TextReader) streamReader);
            this.Capacity = defaultPrefabData.Capacity;
            this.MaintenanceCost = defaultPrefabData.MaintenanceCost;
            this.TicketPrice = defaultPrefabData.TicketPrice;
            this.MaxSpeed = defaultPrefabData.MaxSpeed;
            this.EngineOnBothEnds = defaultPrefabData.EngineOnBothEnds;
            this._changeFlag = false;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.LogError((object) (ex.Message + System.Environment.NewLine + ex.StackTrace));
      }
    }

    private void SavePrefabData()
    {
      if (!this._saveToXml)
        return;
      try
      {
        string str = "IptVehicleData";
        if (!Directory.Exists(str))
          Directory.CreateDirectory(str);
        string path2 = Utils.RemoveInvalidFileNameChars(this.ObjectName + ".xml");
        string path = System.IO.Path.Combine(str, path2);
        PrefabData.DefaultPrefabData defaultPrefabData = new PrefabData.DefaultPrefabData();
        defaultPrefabData.Capacity = this.Capacity;
        defaultPrefabData.MaintenanceCost = this.MaintenanceCost;
        defaultPrefabData.TicketPrice = this.TicketPrice;
        defaultPrefabData.MaxSpeed = this.MaxSpeed;
        defaultPrefabData.EngineOnBothEnds = this.EngineOnBothEnds;
        using (StreamWriter streamWriter = new StreamWriter(path))
          new XmlSerializer(typeof (PrefabData.DefaultPrefabData)).Serialize((TextWriter) streamWriter, (object) defaultPrefabData);
      }
      catch (Exception ex)
      {
        Utils.LogError((object) (ex.Message + System.Environment.NewLine + ex.StackTrace));
      }
    }

    private void CacheDefaults()
    {
      this.Defaults = new PrefabData.DefaultPrefabData();
      this.Defaults.Capacity = this.Capacity;
      this.Defaults.MaxSpeed = this.MaxSpeed;
      this.Defaults.MaintenanceCost = 800;
      this.Defaults.TicketPrice = this.TicketPrice;
      this.Defaults.EngineOnBothEnds = this.EngineOnBothEnds;
    }

    private void ApplyBackEngine(VehicleInfo newInfo, int invertProbability)
    {
      Vehicle[] buffer = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
      for (int index = 0; index < buffer.Length; ++index)
      {
        VehicleInfo info = buffer[index].Info;
        if (!((UnityEngine.Object) info == (UnityEngine.Object) null) && info.m_prefabDataIndex == this.PrefabDataIndex && ((int) buffer[index].m_leadingVehicle == 0 && info.m_trailers != null) && info.m_trailers.Length != 0)
        {
          ushort lastVehicle = buffer[index].GetLastVehicle((ushort) index);
          if (invertProbability == 100)
            buffer[(int) lastVehicle].m_flags |= Vehicle.Flags.Inverted;
          else
            buffer[(int) lastVehicle].m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
          buffer[(int) lastVehicle].Info = newInfo;
        }
      }
    }

    private void EnsureCitizenUnits()
    {
      Vehicle[] buffer = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
      for (int index = 0; index < buffer.Length; ++index)
      {
        if (buffer[index].m_flags != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        {
          VehicleInfo info = buffer[index].Info;
          if (!((UnityEngine.Object) info == (UnityEngine.Object) null) && info.m_prefabDataIndex == this.PrefabDataIndex)
            PrefabData.EnsureCitizenUnits((ushort) index, ref buffer[index], this.Capacity);
        }
      }
    }

    private static void EnsureCitizenUnits(ushort vehicleID, ref Vehicle data, int passengerCount)
    {
      CitizenManager instance = Singleton<CitizenManager>.instance;
      uint num = 0;
      for (uint firstUnit = data.m_citizenUnits; (int) firstUnit != 0; firstUnit = instance.m_units.m_buffer[(int) firstUnit].m_nextUnit)
      {
        if ((instance.m_units.m_buffer[(int) firstUnit].m_flags & CitizenUnit.Flags.Vehicle) != CitizenUnit.Flags.None)
          passengerCount -= 5;
        if (passengerCount < 0)
        {
          Utils.LogToTxt((object) string.Format("ReleaseUnits for #{0}", (object) vehicleID));
          instance.m_units.m_buffer[(int) num].m_nextUnit = 0U;
          instance.ReleaseUnits(firstUnit);
          return;
        }
        num = firstUnit;
      }
      if (passengerCount <= 0)
        return;
      Utils.LogToTxt((object) string.Format("CreateUnits for #{0}", (object) vehicleID));
      uint firstUnit1 = 0;
      if (!instance.CreateUnits(out firstUnit1, ref Singleton<SimulationManager>.instance.m_randomizer, (ushort) 0, vehicleID, 0, 0, 0, passengerCount, 0))
        return;
      if ((int) num != 0)
        instance.m_units.m_buffer[(int) num].m_nextUnit = firstUnit1;
      else
        data.m_citizenUnits = firstUnit1;
    }

    private static PrefabData CreateTrailerData(VehicleInfo info)
    {
      PrefabData prefabData = new PrefabData();
      prefabData._info = info;
      prefabData._saveToXml = false;
      prefabData.CacheDefaults();
      return prefabData;
    }

    [XmlRoot("PrefabData")]
    public struct DefaultPrefabData
    {
      public int Capacity;
      public int MaxSpeed;
      public int MaintenanceCost;
      public int TicketPrice;
      public bool EngineOnBothEnds;
    }
  }
}
