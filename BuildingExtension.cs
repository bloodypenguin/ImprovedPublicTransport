using System.Collections.Generic;
using System.Linq;
using ICities;

namespace ImprovedPublicTransport2
{
    public class BuildingExtension : BuildingExtensionBase
    {
        public static event BuildingExtension.DepotAdded OnDepotAdded;
        public static event BuildingExtension.DepotRemoved OnDepotRemoved;

        private static Dictionary<ItemClassTriplet, HashSet<ushort>> _depotMap = new Dictionary<ItemClassTriplet, HashSet<ushort>>();

        public static void Init()
        {
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
            foreach (KeyValuePair<ItemClassTriplet, HashSet<ushort>> depots in _depotMap)
            {
                if (depots.Value.Remove(id))
                {
                    TransportInfo transportInfo = null;
                    DepotUtil.IsValidDepot(ref BuildingManager.instance.m_buildings.m_buffer[id], ref transportInfo,
                        out ItemClass.Service service, out ItemClass.SubService subService, out ItemClass.Level level);
                    OnDepotRemoved?.Invoke(service, subService, level);
                }
            }
        }

        private static void ObserveBuilding(ushort buildingId)
        {
            TransportInfo transportInfo = null;
            if (!DepotUtil.IsValidDepot(ref BuildingManager.instance.m_buildings.m_buffer[buildingId],
                ref transportInfo,
                out ItemClass.Service service, out ItemClass.SubService subService, out ItemClass.Level level))
            {
                return;
            }
            var itemClassTriplet = new ItemClassTriplet(service, subService, level);
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
            OnDepotAdded?.Invoke(service, subService, level);
        }

        public static ushort[] GetDepots(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            HashSet<ushort> source;
            TransportInfo info = null;
            return _depotMap.TryGetValue(new ItemClassTriplet(service, subService, level), out source) ? 
                source.Where(d => DepotUtil.IsValidDepot(ref BuildingManager.instance.m_buildings.m_buffer[d], ref info, out _, out _ ,out _)).ToArray() : 
                new ushort[]{}; //we validate here to be compatible with MOM (if MOM sets max vehicle count later than this mod loads)
        }

        public delegate void DepotAdded(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level);

        public delegate void DepotRemoved(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level);
    }
}