using System.Collections.Generic;
using System.Linq;

namespace ImprovedPublicTransport2.Query
{
    public static class SelectedVehicleTypesQuery
    {

        // When returns null, it means random
        public static List<VehicleInfo> Query(ushort lineID)
        {
            var prefabs = CachedTransportLineData._lineData[lineID].Prefabs;
            return prefabs?.Select(PrefabCollection<VehicleInfo>.FindLoaded).Where(p => p != null)
                .ToList();
        }
    }
}