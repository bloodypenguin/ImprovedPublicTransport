using System.Collections.Generic;
using System.Linq;
using ICities;

namespace ImprovedPublicTransport2
{
    public class BuildingExtension : BuildingExtensionBase
    {
        public static BuildingExtension instance;
        public event BuildingExtension.DepotAdded OnDepotAdded;
        public event BuildingExtension.DepotRemoved OnDepotRemoved;


        private Dictionary<ItemClassTriplet, HashSet<ushort>> _depotMap;
        private bool _initialized;

        public override void OnCreated(IBuilding building)
        {
            base.OnCreated(building);
            instance = this;
            this._depotMap = new Dictionary<ItemClassTriplet, HashSet<ushort>>();
            this._initialized = false;
        }

        public override void OnReleased()
        {
            base.OnReleased();
            BuildingExtension.instance = null;
        }

        public void Init()
        {
            for (ushort index = 0; index < BuildingManager.instance.m_buildings.m_buffer.Length; ++index)
            {
                ObserveBuilding(index);
            }
            this._initialized = true;
        }

        public void Deinit()
        {
            this._initialized = false;
        }

        public override void OnBuildingCreated(ushort id)
        {
            base.OnBuildingCreated(id);
            if (!ImprovedPublicTransportMod.inGame || !_initialized)
            {
                return;
            }
            ObserveBuilding(id);
        }

        public override void OnBuildingReleased(ushort id)
        {
            base.OnBuildingReleased(id);
            if (!ImprovedPublicTransportMod.inGame || !_initialized)
            {
                return;
            }
            foreach (KeyValuePair<ItemClassTriplet, HashSet<ushort>> depots in this._depotMap)
            {
                if (depots.Value.Remove(id))
                {
                    TransportInfo transportInfo = null;
                    DepotUtil.IsValidDepot(ref BuildingManager.instance.m_buildings.m_buffer[id], ref transportInfo,
                        out ItemClass.Service service, out ItemClass.SubService subService, out ItemClass.Level level);
                    this.OnDepotRemoved?.Invoke(service, subService, level);
                }
            }
        }

        private void ObserveBuilding(ushort buildingId)
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
            this.OnDepotAdded?.Invoke(service, subService, level);
        }

        public ushort[] GetDepots(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            HashSet<ushort> source;
            return _depotMap.TryGetValue(new ItemClassTriplet(service, subService, level), out source) ? source.ToArray() : new ushort[]{};
        }

        public delegate void DepotAdded(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level);

        public delegate void DepotRemoved(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level);
    }
}