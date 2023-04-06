using System.Collections.Generic;
using ColossalFramework;
using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.Query
{
    public static class ActiveVehiclesQuery
    {
        public static List<VehicleQueryResult> Query(ushort lineID, ItemClassTriplet classTriplet)
        {
            var results = new List<VehicleQueryResult>();
            var transportLine = Singleton<TransportManager>.instance.m_lines.m_buffer[lineID];
            var activeVehicleCount = TransportLineUtil.CountLineActiveVehicles(lineID, out _);
            var prefabs =
                VehiclePrefabs.instance.GetPrefabs(classTriplet.Service, classTriplet.SubService, classTriplet.Level);

            for (var index1 = 0; index1 < activeVehicleCount; ++index1)
            {
                var vehicle = transportLine.GetVehicle(index1);
                if (vehicle == 0)
                {
                    continue;
                }
                //based on beginning of TransportLine.SimulationStep
                if ((VehicleManager.instance.m_vehicles.m_buffer[vehicle].m_flags & Vehicle.Flags.GoingBack) !=
                    ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned |
                      Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource |
                      Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath |
                      Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving |
                      Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying |
                      Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo |
                      Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing |
                      Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName |
                      Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion |
                      Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition |
                      Vehicle.Flags.InsideBuilding |
                      Vehicle.Flags.LeftHandDrive))
                {
                    continue;
                }
                var info = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[vehicle].Info;
                foreach (var data in prefabs)
                {
                    if (info.name != data.ObjectName)
                    {
                        continue;
                    }
                    results.Add(new VehicleQueryResult {PrefabData = data, VehicleID = vehicle});
                    break;
                }
            }

            return results;
        }
        
        public class VehicleQueryResult
        {
            public ushort VehicleID;
            public PrefabData PrefabData;
        }
    }
}