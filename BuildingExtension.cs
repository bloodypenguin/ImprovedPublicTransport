using System.Collections.Generic;
using System.Linq;
using ICities;

namespace ImprovedPublicTransport2
{
    public class BuildingExtension : BuildingExtensionBase
    {
        public static event BuildingExtension.DepotAdded OnDepotAdded;
        public static event BuildingExtension.DepotRemoved OnDepotRemoved;

        private static Dictionary<ItemClassTriplet, HashSet<ushort>> _depotMap;

        public static void Init()
        {
            _depotMap = new Dictionary<ItemClassTriplet, HashSet<ushort>>();
            for (ushort index = 0; index < BuildingManager.instance.m_buildings.m_buffer.Length; ++index)
            {
                ObserveBuilding(index);
            }
        }

        public static void Deinit()
        {
            _depotMap = new Dictionary<ItemClassTriplet, HashSet<ushort>>();
        }

        public override void OnBuildingCreated(ushort id)
        {
            base.OnBuildingCreated(id);
            if (!ImprovedPublicTransportMod.inGame)
            {
                return;
            }
            ObserveBuilding(id);
        }

        public override void OnBuildingReleased(ushort id)
        {
            base.OnBuildingReleased(id);
            if (!ImprovedPublicTransportMod.inGame)
            {
                return;
            }
            foreach (var depots in _depotMap)
            {
                if (!depots.Value.Remove(id))
                {
                    continue;
                }
                DepotUtil.GetStats(ref BuildingManager.instance.m_buildings.m_buffer[id],
                    out TransportInfo primaryInfo, out TransportInfo secondaryInfo);
                OnReleasedForInfo(id, primaryInfo);
                OnReleasedForInfo(id, secondaryInfo);
            }
        }

        private void OnReleasedForInfo(ushort id, TransportInfo transportInfo)
        {
            if (transportInfo == null)
            {
                return;
            }
            OnDepotRemoved?.Invoke(transportInfo.GetService(), transportInfo.GetSubService(), transportInfo.GetClassLevel());
        }

        private static void ObserveBuilding(ushort buildingId)
        {
            DepotUtil.GetStats(ref BuildingManager.instance.m_buildings.m_buffer[buildingId],
                out TransportInfo primaryInfo, out TransportInfo secondaryInfo);

            ObserveForInfo(buildingId, primaryInfo);
            ObserveForInfo(buildingId, secondaryInfo);
        }

        private static void ObserveForInfo(ushort buildingId, TransportInfo transportInfo)
        {
            if (!DepotUtil.IsValidDepot(ref BuildingManager.instance.m_buildings.m_buffer[buildingId], transportInfo))
            {
                return;
            }
            var itemClassTriplet = new ItemClassTriplet(transportInfo.GetService(), transportInfo.GetSubService(),
                transportInfo.GetClassLevel());
            if (!_depotMap.TryGetValue(itemClassTriplet, out HashSet<ushort> depots))
            {
                depots = new HashSet<ushort>();
                _depotMap.Add(itemClassTriplet, depots);
            }
            if (depots.Contains(buildingId))
            {
                return;
            }
            depots.Add(buildingId);
            OnDepotAdded?.Invoke(itemClassTriplet.Service, itemClassTriplet.SubService, itemClassTriplet.Level);
        }

        public static ushort[] GetDepots(TransportInfo transportInfo)
        {
            if (transportInfo == null)
            {
                return new ushort[0];
            }

            return _depotMap.TryGetValue(
                new ItemClassTriplet(transportInfo.GetService(), transportInfo.GetSubService(),
                    transportInfo.GetClassLevel()),
                out HashSet<ushort> source)
                ? source.Where(d => DepotUtil.IsValidDepot(ref BuildingManager.instance.m_buildings.m_buffer[d],
                        transportInfo))
                    .ToArray()
                : new ushort [0];
            //we validate here to be compatible with MOM (if MOM sets max vehicle count later than this mod loads)
        }

        public delegate void DepotAdded(ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level);

        public delegate void DepotRemoved(ItemClass.Service service, ItemClass.SubService subService,
            ItemClass.Level level);
    }
}