// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehiclePrefabs
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImprovedPublicTransport2
{
    public class VehiclePrefabs
    {
        public static VehiclePrefabs instance;
        private PrefabData[] _busPrefabData;
        private PrefabData[] _biofuelBusPrefabData;
        private PrefabData[] _metroPrefabData;
        private PrefabData[] _trainPrefabData;
        private PrefabData[] _shipPrefabData;
        private PrefabData[] _planePrefabData;
        private PrefabData[] _taxiPrefabData;
        private PrefabData[] _tramPrefabData;
        private PrefabData[] _evacuationBusPrefabData;
        private PrefabData[] _monorailPrefabData;
        private PrefabData[] _cablecarPrefabData;
        private PrefabData[] _blimpPrefabData;
        private PrefabData[] _ferryPrefabData;
        private PrefabData[] _sightseeingBusPrefabData;
        private PrefabData[] _trolleybusPrefabData;
        private PrefabData[] _helicopterPrefabData;

        public static void Init()
        {
            VehiclePrefabs.instance = new VehiclePrefabs();
            VehiclePrefabs.instance.FindAllPrefabs();
        }

        public static void Deinit()
        {
            VehiclePrefabs.instance = (VehiclePrefabs)null;
        }

        public PrefabData[] GetPrefabs(ItemClass.Service service,
            ItemClass.SubService subService, ItemClass.Level level)
        {
            var prefabs = subService == ItemClass.SubService.PublicTransportBus ?
                VehiclePrefabs.instance.GetPrefabsNoLogging(service, subService) :
                VehiclePrefabs.instance.GetPrefabsNoLogging(service, subService, level);
            if (prefabs.Length == 0)
            {
                UnityEngine.Debug.LogWarning("IPT: Vehicles of item class [serrvice: " + service + ", sub service: " +
                                             subService +
                                             ", level: " + level +
                                             "] were requested. None was found.");
            }
            return prefabs;
        }

        public PrefabData[] GetPrefabs(ItemClass.Service service, ItemClass.SubService subService)
        {
            var prefabs = VehiclePrefabs.instance.GetPrefabsNoLogging(service, subService);
            if (prefabs.Length == 0)
            {
                UnityEngine.Debug.LogWarning("IPT: Vehicles of item class [serrvice: " + service + ", sub service: " +
                                             subService +
                                             "] were requested. None was found.");
            }
            return prefabs;
        }

        private PrefabData[] GetPrefabsNoLogging(ItemClass.Service service,
            ItemClass.SubService subService)
        {
            var prefabs = VehiclePrefabs.instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level1).
                Concat(VehiclePrefabs.instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level2)).
                Concat(VehiclePrefabs.instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level3)).
                Concat(VehiclePrefabs.instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level4)).ToArray();
            return prefabs;
        }

        private PrefabData[] GetPrefabsNoLogging(ItemClass.Service service,
            ItemClass.SubService subService, ItemClass.Level level)
        {
            if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None &&
                level == ItemClass.Level.Level4)
            {
                return _evacuationBusPrefabData;
            }
            if (service == ItemClass.Service.PublicTransport)
            {
                if (level == ItemClass.Level.Level1)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportBus:
                            return this._busPrefabData;
                        case ItemClass.SubService.PublicTransportMetro:
                            return this._metroPrefabData;
                        case ItemClass.SubService.PublicTransportTrain:
                            return this._trainPrefabData;
                        case ItemClass.SubService.PublicTransportShip:
                            return this._shipPrefabData;
                        case ItemClass.SubService.PublicTransportPlane:
                            return this._planePrefabData;
                        case ItemClass.SubService.PublicTransportTaxi:
                            return this._taxiPrefabData;
                        case ItemClass.SubService.PublicTransportTram:
                            return this._tramPrefabData;
                        case ItemClass.SubService.PublicTransportMonorail:
                            return this._monorailPrefabData;
                        case ItemClass.SubService.PublicTransportCableCar:
                            return this._cablecarPrefabData;
                        case ItemClass.SubService.PublicTransportTrolleybus:
                            return this._trolleybusPrefabData;
                    }
                }
                else if (level == ItemClass.Level.Level2)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportBus:
                            return this._biofuelBusPrefabData;
                        case ItemClass.SubService.PublicTransportShip:
                            return this._ferryPrefabData;
                        case ItemClass.SubService.PublicTransportPlane:
                            return this._blimpPrefabData;
                    }
                }
                else if (level == ItemClass.Level.Level3)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportTours:
                            return this._sightseeingBusPrefabData;
                        case ItemClass.SubService.PublicTransportPlane:
                            return this._helicopterPrefabData;
                    }
                }
            }
            return new PrefabData[] { };
        }

        private void FindAllPrefabs()
        {
            List<PrefabData> busList = new List<PrefabData>();
            List<PrefabData> biofuelBusList = new List<PrefabData>();
            List<PrefabData> metroList = new List<PrefabData>();
            List<PrefabData> trainList = new List<PrefabData>();
            List<PrefabData> shipList = new List<PrefabData>();
            List<PrefabData> planeList = new List<PrefabData>();
            List<PrefabData> taxiList = new List<PrefabData>();
            List<PrefabData> tramList = new List<PrefabData>();
            List<PrefabData> monorailList = new List<PrefabData>();
            List<PrefabData> blimpList = new List<PrefabData>();
            List<PrefabData> evacuationBusList = new List<PrefabData>();
            List<PrefabData> cableCarList = new List<PrefabData>();
            List<PrefabData> ferryList = new List<PrefabData>();
            List<PrefabData> sightseeingBusList = new List<PrefabData>();
            List<PrefabData> trolleybusList = new List<PrefabData>();
            List<PrefabData> helicopterList = new List<PrefabData>();

            for (int index = 0; index < PrefabCollection<VehicleInfo>.PrefabCount(); ++index)
            {
                VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab((uint)index);
                if ((Object)prefab != (Object)null && !VehiclePrefabs.IsTrailer(prefab) && prefab.m_placementStyle != ItemClass.Placement.Procedural)
                {
                    var service = prefab.m_class.m_service;
                    var subService = prefab.m_class.m_subService;
                    var level = prefab.m_class.m_level;

                    if (service == ItemClass.Service.Disaster && subService == ItemClass.SubService.None &&
                        level == ItemClass.Level.Level4)
                    {
                        evacuationBusList.Add(new PrefabData(prefab));
                        continue;
                    }
                    if (service == ItemClass.Service.PublicTransport)
                    {
                        if (level == ItemClass.Level.Level1)
                        {
                            switch (subService)
                            {
                                case ItemClass.SubService.PublicTransportBus:
                                    busList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportMetro:
                                    metroList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportTrain:
                                    trainList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportShip:
                                    shipList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportPlane:
                                    planeList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportTaxi:
                                    taxiList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportTram:
                                    tramList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportMonorail:
                                    monorailList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportCableCar:
                                    cableCarList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportTrolleybus:
                                    trolleybusList.Add(new PrefabData(prefab));
                                    continue;
                                default:
                                    continue;
                            }
                        }
                        if (level == ItemClass.Level.Level2)
                        {
                            switch (subService)
                            {
                                case ItemClass.SubService.PublicTransportBus:
                                    biofuelBusList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportShip:
                                    ferryList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportPlane:
                                    blimpList.Add(new PrefabData(prefab));
                                    continue;
                                default:
                                    continue;
                            }
                        }
                        if (level == ItemClass.Level.Level3)
                        {
                            switch (subService)
                            {
                                case ItemClass.SubService.PublicTransportTours:
                                    sightseeingBusList.Add(new PrefabData(prefab));
                                    continue;
                                case ItemClass.SubService.PublicTransportPlane:
                                    helicopterList.Add(new PrefabData(prefab));
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            this._busPrefabData = busList.ToArray();
            this._biofuelBusPrefabData = biofuelBusList.ToArray();
            this._metroPrefabData = metroList.ToArray();
            this._trainPrefabData = trainList.ToArray();
            this._shipPrefabData = shipList.ToArray();
            this._planePrefabData = planeList.ToArray();
            this._taxiPrefabData = taxiList.ToArray();
            this._tramPrefabData = tramList.ToArray();
            this._evacuationBusPrefabData = evacuationBusList.ToArray();
            this._blimpPrefabData = blimpList.ToArray();
            this._monorailPrefabData = monorailList.ToArray();
            this._ferryPrefabData = ferryList.ToArray();
            this._cablecarPrefabData = cableCarList.ToArray();
            this._sightseeingBusPrefabData = sightseeingBusList.ToArray();
            this._trolleybusPrefabData = trolleybusList.ToArray();
            this._helicopterPrefabData = helicopterList.ToArray();
        }

        private static bool IsTrailer(VehicleInfo info)
        {
            string str = ColossalFramework.Globalization.Locale.GetUnchecked("VEHICLE_TITLE", info.name);
            if (!str.StartsWith("VEHICLE_TITLE"))
                return str.StartsWith("Trailer");
            return true;
        }
    }
}