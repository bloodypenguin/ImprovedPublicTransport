using System.Collections.Generic;
using System.Linq;
using ImprovedPublicTransport2.TransientData;

namespace ImprovedPublicTransport2.Query
{
    public static class AvailableVehiclesQuery
    {
        public static List<PrefabData> Query(ItemClassTriplet classTriplet)
        {
            var prefabs = VehiclePrefabs.instance.GetPrefabs(classTriplet.Service, classTriplet.SubService, classTriplet.Level);
            return prefabs.ToList();
        }
    }
}