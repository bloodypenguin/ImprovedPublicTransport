using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ImprovedPublicTransport2.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace ImprovedPublicTransport2
{
    public class VehiclePrefabs : Singleton<VehiclePrefabs>
    {

        private PrefabData[] _busPrefabData;
        private PrefabData[] _biofuelBusPrefabData;
        private PrefabData[] _metroPrefabData;
        private PrefabData[] _trainPrefabData;
        private PrefabData[] _airportTrainPrefabData;
        private PrefabData[] _shipPrefabData;
        private PrefabData[] _planePrefabData;
        private PrefabData[] _taxiPrefabData;
        private PrefabData[] _tramPrefabData;
        private PrefabData[] _evacuationBusPrefabData;
        private PrefabData[] _monorailPrefabData;
        private PrefabData[] _cableCarPrefabData;
        private PrefabData[] _blimpPrefabData;
        private PrefabData[] _ferryPrefabData;
        private PrefabData[] _sightseeingBusPrefabData;
        private PrefabData[] _trolleybusPrefabData;
        private PrefabData[] _helicopterPrefabData;

        private Dictionary<string, PrefabData> _allPrefabData;

        public static void Init()
        {
            instance.RegisterPrefabs();
        }

        [CanBeNull]
        public PrefabData FindByName([NotNull] string prefabName)
        {
            return _allPrefabData[prefabName];
        }

        public PrefabData[] GetPrefabs(ItemClass.Service service,
            ItemClass.SubService subService, ItemClass.Level level)
        {
            var prefabs = VehicleUtil.AllowAllVehicleLevelsOnLine(subService)
                ? instance.GetPrefabsNoLogging(service, subService)
                : instance.GetPrefabsNoLogging(service, subService, level);
            if (prefabs.Length == 0)
            {
                Debug.LogWarning("IPT: Vehicles of item class [serrvice: " + service + ", sub service: " +
                                             subService +
                                             ", level: " + level +
                                             "] were requested. None was found.");
            }

            return prefabs;
        }

        public PrefabData[] GetPrefabs(ItemClass.Service service, ItemClass.SubService subService)
        {
            var prefabs = instance.GetPrefabsNoLogging(service, subService);
            if (prefabs.Length == 0)
            {
                Debug.LogWarning("IPT: Vehicles of item class [serrvice: " + service + ", sub service: " +
                                             subService +
                                             "] were requested. None was found.");
            }

            return prefabs;
        }

        private PrefabData[] GetPrefabsNoLogging(ItemClass.Service service,
            ItemClass.SubService subService)
        {
            var prefabs = instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level1)
                .Concat(instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level2))
                .Concat(instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level3))
                .Concat(instance.GetPrefabsNoLogging(service, subService, ItemClass.Level.Level4))
                .ToArray();
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
                            return _busPrefabData;
                        case ItemClass.SubService.PublicTransportMetro:
                            return _metroPrefabData;
                        case ItemClass.SubService.PublicTransportTrain:
                            return _trainPrefabData;
                        case ItemClass.SubService.PublicTransportShip:
                            return _shipPrefabData;
                        case ItemClass.SubService.PublicTransportPlane:
                            return _planePrefabData;
                        case ItemClass.SubService.PublicTransportTaxi:
                            return _taxiPrefabData;
                        case ItemClass.SubService.PublicTransportTram:
                            return _tramPrefabData;
                        case ItemClass.SubService.PublicTransportMonorail:
                            return _monorailPrefabData;
                        case ItemClass.SubService.PublicTransportCableCar:
                            return _cableCarPrefabData;
                        case ItemClass.SubService.PublicTransportTrolleybus:
                            return _trolleybusPrefabData;
                    }
                }
                else if (level == ItemClass.Level.Level2)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportBus:
                            return _biofuelBusPrefabData;
                        case ItemClass.SubService.PublicTransportShip:
                            return _ferryPrefabData;
                        case ItemClass.SubService.PublicTransportPlane:
                            return _blimpPrefabData;
                        case ItemClass.SubService.PublicTransportTrain:
                            return _airportTrainPrefabData;
                    }
                }
                else if (level == ItemClass.Level.Level3)
                {
                    switch (subService)
                    {
                        case ItemClass.SubService.PublicTransportTours:
                            return _sightseeingBusPrefabData;
                        case ItemClass.SubService.PublicTransportPlane:
                            return _helicopterPrefabData;
                    }
                }
            }

            return new PrefabData[] { };
        }

        private void RegisterPrefabs()
        {
            _allPrefabData = new Dictionary<string, PrefabData>();
            var busList = new List<PrefabData>();
            var biofuelBusList = new List<PrefabData>();
            var metroList = new List<PrefabData>();
            var trainList = new List<PrefabData>();
            var airportTrainList = new List<PrefabData>();
            var shipList = new List<PrefabData>();
            var planeList = new List<PrefabData>();
            var taxiList = new List<PrefabData>();
            var tramList = new List<PrefabData>();
            var monorailList = new List<PrefabData>();
            var blimpList = new List<PrefabData>();
            var evacuationBusList = new List<PrefabData>();
            var cableCarList = new List<PrefabData>();
            var ferryList = new List<PrefabData>();
            var sightseeingBusList = new List<PrefabData>();
            var trolleybusList = new List<PrefabData>();
            var helicopterList = new List<PrefabData>();

            for (var index = 0; index < PrefabCollection<VehicleInfo>.PrefabCount(); ++index)
            {
                var prefab = PrefabCollection<VehicleInfo>.GetPrefab((uint)index);
                if (prefab == null || prefab.m_placementStyle == ItemClass.Placement.Procedural)
                {
                    continue;
                }

                var service = prefab.m_class.m_service;
                var subService = prefab.m_class.m_subService;
                var level = prefab.m_class.m_level;


                switch (service)
                {
                    case ItemClass.Service.Disaster
                        when subService == ItemClass.SubService.None && level == ItemClass.Level.Level4:
                    {
                        evacuationBusList.Add(RegisterPrefab(prefab));
                        continue;
                    }

                    case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level1:
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportBus:
                            {
                                busList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportMetro:
                            {
                                metroList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportTrain:
                            {
                                trainList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportShip:
                            {
                                shipList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportPlane:
                            {
                                planeList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportTaxi:
                            {
                                taxiList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportTram:
                            {
                                tramList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportMonorail:
                            {
                                monorailList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportCableCar:
                            {
                                cableCarList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportTrolleybus:
                            {
                                trolleybusList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                    }

                    case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level2:
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportBus:
                            {
                                biofuelBusList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportShip:
                            {
                                ferryList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportPlane:
                            {
                                blimpList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportTrain:
                            {
                                airportTrainList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                    }
                    case ItemClass.Service.PublicTransport when level == ItemClass.Level.Level3:
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportTours:
                            {
                                sightseeingBusList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            case ItemClass.SubService.PublicTransportPlane:
                            {
                                helicopterList.Add(RegisterPrefab(prefab));
                                continue;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                    }
                    default:
                    {
                        continue;
                    }
                }
            }

            _busPrefabData = busList.ToArray();
            _biofuelBusPrefabData = biofuelBusList.ToArray();
            _metroPrefabData = metroList.ToArray();
            _trainPrefabData = trainList.ToArray();
            _airportTrainPrefabData = airportTrainList.ToArray();
            _shipPrefabData = shipList.ToArray();
            _planePrefabData = planeList.ToArray();
            _taxiPrefabData = taxiList.ToArray();
            _tramPrefabData = tramList.ToArray();
            _evacuationBusPrefabData = evacuationBusList.ToArray();
            _blimpPrefabData = blimpList.ToArray();
            _monorailPrefabData = monorailList.ToArray();
            _ferryPrefabData = ferryList.ToArray();
            _cableCarPrefabData = cableCarList.ToArray();
            _sightseeingBusPrefabData = sightseeingBusList.ToArray();
            _trolleybusPrefabData = trolleybusList.ToArray();
            _helicopterPrefabData = helicopterList.ToArray();
        }

        [NotNull]
        private PrefabData RegisterPrefab([NotNull] VehicleInfo prefab)
        {
            var prefabData = new PrefabData(prefab);
            _allPrefabData.Add(prefab.name, prefabData);
            return prefabData;
        }
    }
}