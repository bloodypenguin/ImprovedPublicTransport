using System.Collections.Generic;

namespace ImprovedPublicTransport2.Querying
{
    public static class QueuedVehicleQuery
    {
        public static List<PrefabData> Query(ushort lineID, ItemClassTriplet classTriplet)
        {
            var result = new List<PrefabData>();
            var enqueuedVehicles = CachedTransportLineData.GetEnqueuedVehicles(lineID);
            var prefabs = VehiclePrefabs.instance.GetPrefabs(classTriplet.Service, classTriplet.SubService, classTriplet.Level);
            foreach (var str in enqueuedVehicles)
            {
                foreach (var data in prefabs)
                {
                    if (data.ObjectName != str)
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