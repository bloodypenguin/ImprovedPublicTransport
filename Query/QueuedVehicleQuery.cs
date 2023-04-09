using System.Collections.Generic;
using ImprovedPublicTransport2.Data;
using JetBrains.Annotations;

namespace ImprovedPublicTransport2.Query
{
    public static class QueuedVehicleQuery
    {
        [NotNull]
        public static List<PrefabData> Query(ushort lineID, ItemClassTriplet classTriplet)
        {
            var result = new List<PrefabData>();
            var enqueuedVehicles = CachedTransportLineData.GetEnqueuedVehicles(lineID);
            var prefabs = VehiclePrefabs.instance.GetPrefabs(classTriplet.Service, classTriplet.SubService, classTriplet.Level);
            foreach (var str in enqueuedVehicles)
            {
                foreach (var data in prefabs)
                {
                    if (data.Name != str)
                    {
                        continue;
                    }
                    result.Add(data);
                    break;
                }
            }

            return result;
        }
    }
}