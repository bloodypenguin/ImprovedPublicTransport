using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ColossalFramework;
using ColossalFramework.Globalization;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.UI.AlgernonCommons;
using UnityEngine;
using static System.Int32;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2.Data
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

    public int TotalCapacity
    {
      get
      {
        var totalCapacity = GetCapacity(Info.m_class.m_service, Info.m_class.m_subService,
          Info.m_class.m_level, Info.m_vehicleAI);
        if (_trailerData != null)
        {
          totalCapacity += _trailerData.Select((t, index) => _trailerData[index].Capacity).Sum();
        }


        return totalCapacity;
      }
    }

    public int Capacity
    {
      get => GetCapacity(Info.m_class.m_service, Info.m_class.m_subService, Info.m_class.m_level, Info.m_vehicleAI);
      set
      {
        if (Capacity == value)
          return;
        if (_trailerData != null)
        {
          foreach (var trailer in _trailerData)
          {
            trailer.Capacity = value;
          }
        }
        SetCapacity(Info.m_class.m_service, Info.m_class.m_subService, Info.m_class.m_level, Info.m_vehicleAI, value);
        if (Info.m_class.m_subService != ItemClass.SubService.PublicTransportTaxi)
        {
          EnsureCitizenUnits();
        }

        _changeFlag = true;
      }
    }

    public int CarCount
    {
      get
      {
        var num = 1;
        if (_trailerData != null)
        {
          num += _trailerData.Count(t => t.Info.GetSubService() == Info.GetSubService());
        }
        return num;
      }
    }

    public int MaxSpeed
    {
      get => (int) Info.m_maxSpeed;
      set
      {
        if (MaxSpeed == value)
          return;
        Info.m_maxSpeed = value;
        _changeFlag = true;
      }
    }

    public int MaintenanceCost
    {
      get
      {
        var service = Info.GetService();
        var subService = Info.GetSubService();
        var level = Info.GetClassLevel();
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
            if (_maintenanceCost == 0)
            {
              float num = TotalCapacity / (float) CarCount / GameDefault.GetCapacity(service, subService, level, Info.m_vehicleType);
              MaintenanceCost = Mathf.RoundToInt(GetMaintenanceCost(service, subService, level, Info.m_vehicleAI) * 16 * num);
            }
            return _maintenanceCost;
          default:
            return 0;
        }
      }
      set
      {
        if (_maintenanceCost == value || value <= 0)
        {
          return;
        }

        _maintenanceCost = value;
        _changeFlag = true;
      }
    }

    public bool EngineOnBothEnds
    {
      get
      {
        if (Info.GetSubService() != ItemClass.SubService.PublicTransportTrain || Info.m_trailers == null)
        {
          return false;
        }

        var length = Info.m_trailers.Length;
        return length > 0 && Info.m_trailers[length - 1].m_info == Info;
      }
      
      set
      {
        if (Info.m_trailers == null || Info.GetSubService() != ItemClass.SubService.PublicTransportTrain ||
            EngineOnBothEnds == value)
        {
          return;
        }

        var length = Info.m_trailers.Length;
        if (length <= 1)
        {
          return;
        }

        if (value && Info.m_trailers[length - 1].m_info != Info)
        {
          Utils.Log("Replacing last trailer with engine and inverting it for " + Name);
          Info.m_trailers[length - 1].m_info = Info;
          Info.m_trailers[length - 1].m_invertProbability = 100;
          ApplyBackEngine(Info, 100);
          _changeFlag = true;
        }
        else
        {
          if (value)
          {
            return;
          }

          Utils.Log("Reverting last trailer setting for " + Name);
          Info.m_trailers[length - 1].m_info = _lastTrailer.m_info;
          Info.m_trailers[length - 1].m_invertProbability = _lastTrailer.m_invertProbability;
          ApplyBackEngine(_lastTrailer.m_info, _lastTrailer.m_invertProbability);
          _changeFlag = true;
        }
      }
    }

    public PrefabData(VehicleInfo info)
    {
      Info = info;
      DisplayName = GetDisplayName(info);
      Utils.Log("Creating PrefabData for " + Name);
      if (Name == "451494281.London 1992 Stock (4 car)_Data")
      {
        const int length = 3;
        Info.m_trailers = new VehicleInfo.VehicleTrailer[length];
        for (var index = 0; index < length; ++index)
        {
          Info.m_trailers[index].m_info = Info;
          Info.m_trailers[index].m_invertProbability = 50;
          Info.m_trailers[index].m_probability = 100;
        }
      }
      else if (Name.Contains("D3S Solaris Urbino 24 '15") && Info.m_trailers != null && Info.m_trailers.Length != 0)
      {
        Info.m_dampers = 0.6f;
        var loaded = PrefabCollection<VehicleInfo>.FindLoaded(Name.Substring(0, Name.IndexOf(".", StringComparison.Ordinal)) + ".D3S Solaris Urbino 24 '15 (II)_Data");
        if (loaded != null)
        {
          Utils.Log("Fixing " + Name);
          Info.m_trailers[0].m_info.m_dampers = 0.6f;
          loaded.m_attachOffsetFront = 1.07f;
          loaded.m_dampers = 0.6f;
          Info.m_trailers[1].m_info = loaded;
          Info.m_trailers[1].m_invertProbability = 0;
          Info.m_trailers[1].m_probability = 100;
          ApplyBackEngine(loaded, 0);
        }
      }
      if (Info.m_trailers != null)
      {
        var length = Info.m_trailers.Length;
        if (length > 0)
        {
          _lastTrailer = Info.m_trailers[length - 1];
          _trailerData = new PrefabData[length];
          for (var index = 0; index < length; ++index)
          {
            var info1 = Info.m_trailers[index].m_info;
            if (info1 != null)
            {
              _trailerData[index] = CreateTrailerData(info1);
            }
          }
        }
      }
      CacheDefaults();
      LoadPrefabData();
    }

    private static string GetDisplayName(VehicleInfo info)
    {
      var infoName = info?.name;
      if (infoName != null)
      {
        if(infoName.StartsWith("Bus") ||
           infoName.StartsWith("DoubleDeckerBus") ||
           infoName.StartsWith("ArticulatedBus") ||
           infoName.StartsWith("Small Aircraft Passenger") ||
           infoName.StartsWith("Medium Aircraft Passenger") ||
           infoName.StartsWith("Large Aircraft Passenger") ||
           infoName.StartsWith("Metro") ||
           infoName.StartsWith("Tram") ||
           infoName.StartsWith("Train")||
           infoName.StartsWith("Ferry")||
           infoName.StartsWith("Monorail")
           )
        {
          if (infoName.StartsWith("Metro"))
          {
            var indexOfUnderscore = infoName.IndexOf("_");
            if (indexOfUnderscore > 0)
            {
              infoName = infoName.Remove(indexOfUnderscore);
            }
          }
          
          var hasNumberOnEnd = TryParse(infoName.Substring(infoName.Length - 3), out var result);
          if (!hasNumberOnEnd)
          {
            hasNumberOnEnd = TryParse(infoName.Substring(infoName.Length - 2), out result);
          }
          if (!hasNumberOnEnd)
          {
            hasNumberOnEnd = TryParse(infoName.Substring(infoName.Length - 1), out result);
          }

          if (hasNumberOnEnd)
          {
            return $"{PrefabUtils.GetDisplayName(info)} {result}";
          }
        }
      }
      return PrefabUtils.GetDisplayName(info);
    }

    private PrefabData()
    {
    }

    public void SetValues(int capacity, int maintenanceCost, int maxSpeed, bool engineOnBothEnds)
    {
      Capacity = capacity;
      MaintenanceCost = maintenanceCost;
      MaxSpeed = maxSpeed;
      EngineOnBothEnds = engineOnBothEnds;
      if (!_changeFlag)
      {
        return;
      }

      SavePrefabData();
      _changeFlag = false;
    }

    public void SetDefaults()
    {
      Capacity = Defaults.Capacity;
      MaintenanceCost = Defaults.MaintenanceCost;
      MaxSpeed = Defaults.MaxSpeed;
      EngineOnBothEnds = Defaults.EngineOnBothEnds;
      if (_trailerData != null)
      {
        foreach (var trailer in _trailerData)
        {
          trailer.SetDefaults();
        }
      }
      if (!_changeFlag)
        return;
      SavePrefabData();
      _changeFlag = false;
    }

    private static int GetCapacity(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai)
    {
      var num = 0;
        switch (service)
        {
          case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level1:
          {
            num = subService switch
            {
              ItemClass.SubService.PublicTransportBus when ai is BusAI busAI => busAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportMetro when ai is MetroTrainAI metroTrainAI => metroTrainAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI => trainAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportShip when ai is PassengerShipAI shipAI => shipAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportTaxi when ai is TaxiAI taxiAI => taxiAI.m_travelCapacity,
              ItemClass.SubService.PublicTransportTram when ai is TramAI tramAI => tramAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportMonorail when ai is PassengerTrainAI trainAI => trainAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportCableCar when ai is CableCarAI carAI => carAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportTrolleybus when ai is TrolleybusAI trolleybusAI => trolleybusAI.m_passengerCapacity,
              _ => num
            };

            break;
          }
          case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level2:
          {
            num = subService switch
            {
              ItemClass.SubService.PublicTransportBus when ai is BusAI busAI => busAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportShip when ai is PassengerFerryAI ferryAI => ferryAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportPlane when ai is PassengerBlimpAI blimpAI => blimpAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI.m_passengerCapacity,
              ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI => trainAI.m_passengerCapacity,
              _ => num
            };
            break;
          }
          case ItemClass.Service.PublicTransport:
          {
            if (level == ItemClass.Level.Level3)
            {
              num = subService switch
              {
                ItemClass.SubService.PublicTransportTours when ai is BusAI busAI => busAI.m_passengerCapacity,
                ItemClass.SubService.PublicTransportPlane when ai is PassengerHelicopterAI helicopterAI => helicopterAI.m_passengerCapacity,
                ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI.m_passengerCapacity,
                _ => num
              };
            }

            break;
          }
          case ItemClass.Service.Disaster when subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI busAI:
            num = busAI.m_passengerCapacity;
            break;
        }
      return num;
    }

    private static void SetCapacity(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai, int capacity)
    {
      switch (service)
      {
        case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level1:
        {
          switch (subService)
          {
            case ItemClass.SubService.PublicTransportBus when ai is BusAI busAI:
              busAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportMetro when ai is MetroTrainAI metroTrainAI:
              metroTrainAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI:
              trainAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportShip when ai is PassengerShipAI shipAI:
              shipAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI:
              planeAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportTaxi when ai is TaxiAI taxiAI:
              taxiAI.m_travelCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportTram when ai is TramAI tramAI:
              tramAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportMonorail when ai is PassengerTrainAI trainAI:
              trainAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportCableCar when ai is CableCarAI carAI:
              carAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportTrolleybus when ai is TrolleybusAI trolleybusAI:
              trolleybusAI.m_passengerCapacity = capacity;
              break;
          }

          break;
        }
        case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level2:
        {
          switch (subService)
          {
            case ItemClass.SubService.PublicTransportBus when ai is BusAI busAI:
              busAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportShip when ai is PassengerFerryAI ferryAI:
              ferryAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportPlane when ai is PassengerBlimpAI blimpAI:
              blimpAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI:
              planeAI.m_passengerCapacity = capacity;
              break;
            case ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI:
              trainAI.m_passengerCapacity = capacity;
              break;
          }

          break;
        }
        case ItemClass.Service.PublicTransport:
        {
          if (level == ItemClass.Level.Level3)
          {
            switch (subService)
            {
              case ItemClass.SubService.PublicTransportTours when ai is BusAI busAI:
                busAI.m_passengerCapacity = capacity;
                break;
              case ItemClass.SubService.PublicTransportPlane when ai is PassengerHelicopterAI helicopterAI:
                helicopterAI.m_passengerCapacity = capacity;
                break;
              case ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI:
                planeAI.m_passengerCapacity = capacity;
                break;
            }
          }

          break;
        }
        case ItemClass.Service.Disaster when subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI busAI:
          busAI.m_passengerCapacity = capacity;
          break;
      }
    }

    public static int GetMaintenanceCost(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleAI ai)
    {
        var num = 0;
        switch (service)
        {
          case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level1:
          {
            num = subService switch
            {
              ItemClass.SubService.PublicTransportBus when ai is BusAI busAI => busAI.m_transportInfo
                ?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportMetro when ai is MetroTrainAI metroTrainAI => metroTrainAI
                .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI => trainAI.m_transportInfo
                ?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportShip when ai is PassengerShipAI shipAI => shipAI.m_transportInfo
                ?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI.m_transportInfo
                ?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportTaxi when ai is TaxiAI taxiAI => taxiAI.m_transportInfo
                ?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportTram when ai is TramAI tramAI => tramAI.m_transportInfo
                ?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportMonorail when ai is PassengerTrainAI trainAI => trainAI
                .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportCableCar when ai is CableCarAI carAI => carAI.m_transportInfo
                ?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportTrolleybus when ai is TrolleybusAI trolleybusAI => trolleybusAI
                .m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              _ => 0
            };

            break;
          }
          case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level2:
          {
            num = subService switch
            {
              ItemClass.SubService.PublicTransportBus when ai is BusAI busAI => busAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportShip when ai is PassengerFerryAI ferryAI => ferryAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportPlane when ai is PassengerBlimpAI blimpAI => blimpAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              ItemClass.SubService.PublicTransportTrain when ai is PassengerTrainAI trainAI => trainAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
              _ => 0
            };

            break;
          }
          case ItemClass.Service.PublicTransport:
          {
            if (level == ItemClass.Level.Level3)
            {
              num = subService switch
              {
                ItemClass.SubService.PublicTransportTours when ai is BusAI busAI => busAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                ItemClass.SubService.PublicTransportPlane when ai is PassengerHelicopterAI helicopterAI => helicopterAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                ItemClass.SubService.PublicTransportPlane when ai is PassengerPlaneAI planeAI => planeAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0,
                _ => 0
              };
            }

            break;
          }
          case ItemClass.Service.Disaster when subService == ItemClass.SubService.None && level == ItemClass.Level.Level4 && ai is BusAI busAI:
            num = busAI.m_transportInfo?.m_maintenanceCostPerVehicle ?? 0;
            break;
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
        string path2 = Utils.RemoveInvalidFileNameChars(Name + ".xml");
        string path = Path.Combine(str, path2);
        if (!File.Exists(path))
        {
          Utils.Log("No stored data found for " + Name);
        }
        else
        {
          Utils.Log("Found stored data for " + Name);
          using (StreamReader streamReader = new StreamReader(path))
          {
            DefaultPrefabData defaultPrefabData = (DefaultPrefabData) new XmlSerializer(typeof (DefaultPrefabData)).Deserialize(streamReader);
            Capacity = defaultPrefabData.Capacity;
            MaintenanceCost = defaultPrefabData.MaintenanceCost;
            MaxSpeed = defaultPrefabData.MaxSpeed;
            EngineOnBothEnds = defaultPrefabData.EngineOnBothEnds;
            _changeFlag = false;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
      }
    }

    private void SavePrefabData()
    {
      if (!_saveToXml)
      {
        return;
      }

      try
      {
        const string str = "IptVehicleData";
        if (!Directory.Exists(str))
        {
          Directory.CreateDirectory(str);
        }

        var path2 = Utils.RemoveInvalidFileNameChars(Name + ".xml");
        var path = Path.Combine(str, path2);
        var defaultPrefabData = new DefaultPrefabData();
        defaultPrefabData.Capacity = Capacity;
        defaultPrefabData.MaintenanceCost = MaintenanceCost;
        defaultPrefabData.MaxSpeed = MaxSpeed;
        defaultPrefabData.EngineOnBothEnds = EngineOnBothEnds;
        using var streamWriter = new StreamWriter(path);
        new XmlSerializer(typeof(DefaultPrefabData)).Serialize(streamWriter, defaultPrefabData);
      }
      catch (Exception ex)
      {
        Utils.LogError(ex.Message + Environment.NewLine + ex.StackTrace);
      }
    }

    private void CacheDefaults()
    {
      Defaults = new DefaultPrefabData
      {
        Capacity = Capacity,
        MaxSpeed = MaxSpeed,
        MaintenanceCost = 800,
        EngineOnBothEnds = EngineOnBothEnds
      };
    }

    private void ApplyBackEngine(VehicleInfo newInfo, int invertProbability)
    {
      var buffer = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
      for (var index = 0; index < buffer.Length; ++index)
      {
        var info = buffer[index].Info;
        if (info == null || info.m_prefabDataIndex != PrefabDataIndex || buffer[index].m_leadingVehicle != 0 || info.m_trailers == null || info.m_trailers.Length == 0)
        {
          continue;
        }
        var lastVehicle = buffer[index].GetLastVehicle((ushort) index);
        if (invertProbability == 100)
        {
          buffer[lastVehicle].m_flags |= Vehicle.Flags.Inverted;
        }
        else
        {
          buffer[lastVehicle].m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                         Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource |
                                         Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 |
                                         Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving |
                                         Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff |
                                         Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace |
                                         Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack |
                                         Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                         Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                         Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                         Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                                         Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                         Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
        }

        buffer[lastVehicle].Info = newInfo;
      }
    }

    private void EnsureCitizenUnits()
    {
      var buffer = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
      for (var index = 0; index < buffer.Length; ++index)
      {
        if (buffer[index].m_flags == ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                                       Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget |
                                       Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 |
                                       Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped |
                                       Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed |
                                       Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing |
                                       Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                                       Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                                       Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                                       Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading |
                                       Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic |
                                       Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                                       Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        {
          continue;
        }
        var info = buffer[index].Info;
        if (info != null && info.m_prefabDataIndex == PrefabDataIndex)
        {
          EnsureCitizenUnits((ushort)index, ref buffer[index], Capacity);
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
          Utils.LogToTxt($"ReleaseUnits for #{vehicleID}");
          instance.m_units.m_buffer[(int) num].m_nextUnit = 0U;
          instance.ReleaseUnits(firstUnit);
          return;
        }
        num = firstUnit;
      }
      if (passengerCount <= 0)
        return;
      Utils.LogToTxt($"CreateUnits for #{vehicleID}");
      uint firstUnit1 = 0;
      if (!instance.CreateUnits(out firstUnit1, ref Singleton<SimulationManager>.instance.m_randomizer, 0, vehicleID, 0, 0, 0, passengerCount, 0))
        return;
      if ((int) num != 0)
        instance.m_units.m_buffer[(int) num].m_nextUnit = firstUnit1;
      else
        data.m_citizenUnits = firstUnit1;
    }
    
    public string GetDescription()
    {
      var stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(DisplayName);
      ItemClass.SubService subService = Info.GetSubService();
      if (subService == ItemClass.SubService.PublicTransportTaxi)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_CAPACITY_TAXI") + ": " + TotalCapacity);
      else
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_CAPACITY") + ": " + TotalCapacity);
      float num = MaintenanceCost * 0.01f;
      string str1 = num.ToString(Locale.Get("MONEY_FORMAT"), LocaleManager.cultureInfo);
      if (MaintenanceCost > 0)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_MAINTENANCE") + ": " + MaintenanceCost + " (" + str1 + ")");
      stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_MAX_SPEED") + ": " + MaxSpeed + " (" + MaxSpeed * 5 + " " + OptionsWrapper<Settings.Settings>.Options.SpeedString + ")");
      return stringBuilder.ToString();
    }

    private static PrefabData CreateTrailerData(VehicleInfo info)
    {
      var prefabData = new PrefabData
      {
        Info = info,
        _saveToXml = false
      };
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
      if (obj.GetType() != GetType()) return false;
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
