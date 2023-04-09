using System.Collections.Generic;
using System.Linq;
using ImprovedPublicTransport2.Data;
using JetBrains.Annotations;

namespace ImprovedPublicTransport2.Query
{
    public static class SelectedVehicleTypesQuery
    {

        [NotNull]
        public static List<PrefabData> Query(ushort lineID)
        {
            var prefabs = CachedTransportLineData._lineData[lineID].Prefabs;
            if (prefabs == null)
            {
                return new List<PrefabData>();
            }
            return prefabs.Select(name => VehiclePrefabs.instance.FindByName(name)).Where(p => p != null)
                .ToList();
        }
    }
}