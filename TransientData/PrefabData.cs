// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.PrefabData
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ColossalFramework;
using ColossalFramework.Globalization;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2.TransientData
{
  public class PrefabData
  {
    private bool _saveToXml = true;
    private VehicleInfo.VehicleTrailer _lastTrailer;
    private PrefabData[] _trailerData;
    private DefaultPrefabData Defaults;
    private bool _changeFlag;
    private int _maintenanceCost;

    public int PrefabDataIndex => Info.m_prefabDataIndex;

    public VehicleInfo Info { get; private set; }

    public string Name => Info.name;

    public string DisplayName { get; }

    public string Title => Locale.Get("VEHICLE_TITLE", PrefabCollection<VehicleInfo>.PrefabName((uint) this.PrefabDataIndex));

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
        return num + PrefabData.GetCapacity(this.Info.m_class.m_service, this.Info.m_class.m_subService, this.Info.m_class.m_level, this.Info.m_vehicleAI);
      }
    }

    public int Capacity
    {
      get
      {
        return PrefabData.GetCapacity(this.Info.m_class.m_service, this.Info.m_class.m_subService, this.Info.m_class.m_level, this.Info.m_vehicleAI);
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
        PrefabData.SetCapacity(this.Info.m_class.m_service, this.Info.m_class.m_subService, this.Info.m_class.m_level, this.Info.m_vehicleAI, value);
        if (this.Info.m_class.m_subService != ItemClass.SubService.PublicTransportTaxi)
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
            if (this._trailerData[index].Info.GetSubService() == this.Info.GetSubService())
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
        return (int) this.Info.m_maxSpeed;
      }
      set
      {
        if (this.MaxSpeed == value)
          return;
        this.Info.m_maxSpeed = (float) value;
        this._changeFlag = true;
      }
    }

    public int MaintenanceCost
    {
      get
      {
        ItemClass.Service service = this.Info.GetService();
        ItemClass.SubService subService = this.Info.GetSubService();
        ItemClass.Level level = this.Info.GetClassLevel();
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
          case ItemClass.SubService.PublicTransportTrolleybus:
            if (this._maintenanceCost == 0)
            {
              float num = (float) this.TotalCapacity / (float) this.CarCount / (float) GameDefault.GetCapacity(service, subService, level, Info.m_vehicleType);
              this.MaintenanceCost = Mathf.RoundToInt((float) (PrefabData.GetMaintenanceCost(service, subService, level, this.Info.m_vehicleAI) * 16) * num);
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

    public bool EngineOnBothEnds
    {
      get
      {
        if (this.Info.GetSubService() != ItemClass.SubService.PublicTransportTrain || this.Info.m_trailers == null)
          return false;
        int length = this.Info.m_trailers.Length;
        return length > 0 && (UnityEngine.Object) this.Info.m_trailers[length - 1].m_info == (UnityEngine.Object) this.Info;
      }
      set
      {
        if (this.Info.m_trailers == null || this.Info.GetSubService() != ItemClass.SubService.PublicTransportTrain || this.EngineOnBothEnds == value)
          return;
        int length = this.Info.m_trailers.Length;
        if (length <= 1)
          return;
        if (value && (UnityEngine.Object) this.Info.m_trailers[length - 1].m_info != (UnityEngine.Object) this.Info)
        {
          Utils.Log((object) ("Replacing last trailer with engine and inverting it for " + this.Name));
          this.Info.m_trailers[length - 1].m_info = this.Info;
          this.Info.m_trailers[length - 1].m_invertProbability = 100;
          this.ApplyBackEngine(this.Info, 100);
          this._changeFlag = true;
        }
        else
        {
          if (value)
            return;
          Utils.Log((object) ("Reverting last trailer setting for " + this.Name));
          this.Info.m_trailers[length - 1].m_info = this._lastTrailer.m_info;
          this.Info.m_trailers[length - 1].m_invertProbability = this._lastTrailer.m_invertProbability;
          this.ApplyBackEngine(this._lastTrailer.m_info, this._lastTrailer.m_invertProbability);
          this._changeFlag = true;
        }
      }
    }

    public PrefabData(VehicleInfo info)
    {
      this.Info = info;
      DisplayName = PrefabUtils.GetDisplayName(info);
      Utils.Log((object) ("Creating PrefabData for " + this.Name));
      if (this.Name == "451494281.London 1992 Stock (4 car)_Data")
      {
        int length = 3;
        this.Info.m_trailers = new VehicleInfo.VehicleTrailer[length];
        for (int index = 0; index < length; ++index)
        {
          this.Info.m_trailers[index].m_info = this.Info;
          this.Info.m_trailers[index].m_invertProbability = 50;
          this.Info.m_trailers[index].m_probability = 100;
        }
      }
      else if (this.Name.Contains("D3S Solaris Urbino 24 '15") && this.Info.m_trailers != null && this.Info.m_trailers.Length != 0)
      {
        this.Info.m_dampers = 0.6f;
        VehicleInfo loaded = PrefabCollection<VehicleInfo>.FindLoaded(this.Name.Substring(0, this.Name.IndexOf(".")) + ".D3S Solaris Urbino 24 '15 (II)_Data");
        if ((UnityEngine.Object) loaded != (UnityEngine.Object) null)
        {
          Utils.Log((object) ("Fixing " + this.Name));
          this.Info.m_trailers[0].m_info.m_dampers = 0.6f;
          loaded.m_attachOffsetFront = 1.07f;
          loaded.m_dampers = 0.6f;
          this.Info.m_trailers[1].m_info = loaded;
          this.Info.m_trailers[1].m_invertProbability = 0;
          this.Info.m_trailers[1].m_probability = 100;
          this.ApplyBackEngine(loaded, 0);
        }
      }
      if (this.Info.m_trailers != null)
      {
        int length = this.Info.m_trailers.Length;
        if (length > 0)
        {
          this._lastTrailer = this.Info.m_trailers[length - 1];
          this._trailerData = new PrefabData[length];
          for (int index = 0; index < length; ++index)
          {
            VehicleInfo info1 = this.Info.m_trailers[index].m_info;
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

    public void SetValues(int capacity, int maintenanceCost, int maxSpeed, bool engineOnBothEnds)
    {
      this.Capacity = capacity;
      this.MaintenanceCost = maintenanceCost;
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
                else if (subService == ItemClass.SubService.PublicTransportTrolleybus && ai is TrolleybusAI)
                  num = (ai as TrolleybusAI).m_passengerCapacity;
            }
            else if (level == ItemClass.Level.Level2)
            {
                if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
                    num = (ai as BusAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
                    num = (ai as PassengerFerryAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
                    num = (ai as PassengerBlimpAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                  num = (ai as PassengerPlaneAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
                  num = (ai as PassengerTrainAI).m_passengerCapacity;
            }
            else if (level == ItemClass.Level.Level3)
            {
                if (subService == ItemClass.SubService.PublicTransportTours && ai is BusAI)
                    num = (ai as BusAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerHelicopterAI)
                  num = (ai as PassengerHelicopterAI).m_passengerCapacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                  num = (ai as PassengerPlaneAI).m_passengerCapacity;
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
                    else if (subService == ItemClass.SubService.PublicTransportTrolleybus && ai is TrolleybusAI)
                      (ai as TrolleybusAI).m_passengerCapacity = capacity;
                }
            else if (level == ItemClass.Level.Level2)
            {
                if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
                    (ai as BusAI).m_passengerCapacity = capacity;
                else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
                    (ai as PassengerFerryAI).m_passengerCapacity = capacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
                    (ai as PassengerBlimpAI).m_passengerCapacity = capacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                  (ai as PassengerPlaneAI).m_passengerCapacity = capacity;
                else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
                  (ai as PassengerTrainAI).m_passengerCapacity = capacity;
            }
            else if (level == ItemClass.Level.Level3)
            {
                if (subService == ItemClass.SubService.PublicTransportTours && ai is BusAI)
                    (ai as BusAI).m_passengerCapacity = capacity;
                if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerHelicopterAI)
                  (ai as PassengerHelicopterAI).m_passengerCapacity = capacity;
                else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
                  (ai as PassengerPlaneAI).m_passengerCapacity = capacity;
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
              {
                num = (ai as BusAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportMetro && ai is PassengerTrainAI)
              {
                num = (ai as PassengerTrainAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
              {
                num = (ai as PassengerTrainAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerShipAI)
              {
                num = (ai as PassengerShipAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
              {
                num = (ai as PassengerPlaneAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportTaxi && ai is TaxiAI)
              {
                num = (ai as TaxiAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportTram && ai is TramAI)
              {
                num = (ai as TramAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportMonorail && ai is PassengerTrainAI)
              {
                num = (ai as PassengerTrainAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportCableCar && ai is CableCarAI)
              {
                num = (ai as CableCarAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportTrolleybus && ai is TrolleybusAI)
              {
                num = (ai as TrolleybusAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;                
              }
            }
            else if (level == ItemClass.Level.Level2)
            {
              if (subService == ItemClass.SubService.PublicTransportBus && ai is BusAI)
              {
                num = (ai as BusAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportShip && ai is PassengerFerryAI)
              {
                num = (ai as PassengerFerryAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerBlimpAI)
              {
                num = (ai as PassengerBlimpAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
              {
                num = (ai as PassengerPlaneAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportTrain && ai is PassengerTrainAI)
              {
                num = (ai as PassengerTrainAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
            }
            else if (level == ItemClass.Level.Level3)
            {
              if (subService == ItemClass.SubService.PublicTransportTours && ai is BusAI)
              {
                num = (ai as BusAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              } else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerHelicopterAI)
              {
                num = (ai as PassengerHelicopterAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
              else if (subService == ItemClass.SubService.PublicTransportPlane && ai is PassengerPlaneAI)
              {
                num = (ai as PassengerPlaneAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
              }
            }

        }
        else if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI)
        {
            num = (ai as BusAI).m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
        }
        return num;
        }

    private void LoadPrefabData()
    {
      try
      {
        string str = "IptVehicleData";
        if (!Directory.Exists(str))
          Directory.CreateDirectory(str);
        string path2 = Utils.RemoveInvalidFileNameChars(this.Name + ".xml");
        string path = System.IO.Path.Combine(str, path2);
        if (!File.Exists(path))
        {
          Utils.Log((object) ("No stored data found for " + this.Name));
        }
        else
        {
          Utils.Log((object) ("Found stored data for " + this.Name));
          using (StreamReader streamReader = new StreamReader(path))
          {
            PrefabData.DefaultPrefabData defaultPrefabData = (PrefabData.DefaultPrefabData) new XmlSerializer(typeof (PrefabData.DefaultPrefabData)).Deserialize((TextReader) streamReader);
            this.Capacity = defaultPrefabData.Capacity;
            this.MaintenanceCost = defaultPrefabData.MaintenanceCost;
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
        string path2 = Utils.RemoveInvalidFileNameChars(this.Name + ".xml");
        string path = System.IO.Path.Combine(str, path2);
        PrefabData.DefaultPrefabData defaultPrefabData = new PrefabData.DefaultPrefabData();
        defaultPrefabData.Capacity = this.Capacity;
        defaultPrefabData.MaintenanceCost = this.MaintenanceCost;
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
    
    public string GetDescription()
    {
      var stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(Title);
      ItemClass.SubService subService = Info.GetSubService();
      if (subService == ItemClass.SubService.PublicTransportTaxi)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_CAPACITY_TAXI") + ": " + (object) TotalCapacity);
      else
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_CAPACITY") + ": " + (object) TotalCapacity);
      float num = (float) MaintenanceCost * 0.01f;
      string str1 = num.ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
      if (MaintenanceCost > 0)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_MAINTENANCE") + ": " + (object) MaintenanceCost + " (" + str1 + ")");
      stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_MAX_SPEED") + ": " + (object) MaxSpeed + " (" + (object) (MaxSpeed * 5) + " " + OptionsWrapper<Settings.Settings>.Options.SpeedString + ")");
      return stringBuilder.ToString();
    }

    private static PrefabData CreateTrailerData(VehicleInfo info)
    {
      PrefabData prefabData = new PrefabData();
      prefabData.Info = info;
      prefabData._saveToXml = false;
      prefabData.CacheDefaults();
      return prefabData;
    }

    
    public static bool operator ==(PrefabData obj1, PrefabData obj2)
    {
      if (ReferenceEquals(obj1, obj2)) 
        return true;
      if (ReferenceEquals(obj1, null)) 
        return false;
      if (ReferenceEquals(obj2, null))
        return false;
      return obj1.Equals(obj2);
    }
    
    public static bool operator !=(PrefabData obj1, PrefabData obj2) => !(obj1 == obj2);
    public bool Equals(PrefabData other)
    {
      return Equals(Info, other.Info);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((PrefabData)obj);
    }

    public override int GetHashCode()
    {
      return (Info != null ? Info.GetHashCode() : 0);
    }

    [XmlRoot("PrefabData")]
    public struct DefaultPrefabData
    {
      public int Capacity;
      public int MaxSpeed;
      public int MaintenanceCost;
      public bool EngineOnBothEnds;
    }
  }
}
