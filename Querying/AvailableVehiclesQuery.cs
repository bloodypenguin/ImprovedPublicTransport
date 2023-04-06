using System.Collections.Generic;
using System.Linq;

namespace ImprovedPublicTransport2.Querying
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