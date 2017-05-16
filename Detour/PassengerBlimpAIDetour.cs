using ColossalFramework;
using ImprovedPublicTransport.Redirection.Attributes;

namespace ImprovedPublicTransport.Detour
{
    [TargetType(typeof(PassengerBlimpAI))]
    public class PassengerBlimpAIDetour : BlimpAI
    {
        [RedirectMethod]
        public override bool CanLeave(ushort vehicleID, ref Vehicle vehicleData)
        {
            if ((int)vehicleData.m_leadingVehicle == 0 && (int)vehicleData.m_waitCounter < 12 ||
                !base.CanLeave(vehicleID, ref vehicleData))
            {
                //begin mod(+): track if unbunching happens
                VehicleManagerMod.m_cachedVehicleData[(int)vehicleID].IsUnbunchingInProgress = false;
                //end mod
                return false;
            }

            if ((int)vehicleData.m_leadingVehicle == 0 && (int)vehicleData.m_transportLine != 0)
            {
                //begin mod(+): Check if unbunching enabled for this line & stop. track if unbunching happens
                ushort currentStop = VehicleManagerMod.m_cachedVehicleData[vehicleID].CurrentStop;
                if (currentStop != 0 && NetManagerMod.m_cachedNodeData[currentStop].Unbunching &&
                    TransportLineMod.GetUnbunchingState(vehicleData.m_transportLine))
                {
                    var canLeaveStop = Singleton<TransportManager>.instance.m_lines
                        .m_buffer[(int)vehicleData.m_transportLine]
                        .CanLeaveStop(vehicleData.m_targetBuilding, (int)vehicleData.m_waitCounter >> 4);
                    VehicleManagerMod.m_cachedVehicleData[vehicleID].IsUnbunchingInProgress = !canLeaveStop;
                    return canLeaveStop;
                }
                //end mod
            }
            //begin mod(+): track if unbunching happens
            VehicleManagerMod.m_cachedVehicleData[vehicleID].IsUnbunchingInProgress = false;
            //end mod
            return true;
        }

        [RedirectMethod]
        private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
        {
            if ((int)data.m_targetBuilding == 0)
            {
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
                return true;
            }
            ushort nextStop = 0;
            if ((int)data.m_transportLine != 0)
                nextStop = TransportLine.GetNextStop(data.m_targetBuilding);
            ushort targetBuilding = data.m_targetBuilding;
            //begin mod(+): track stats
            ushort transferSize1 = data.m_transferSize;
            //end mod
            PassengerBlimpAIDetour.UnloadPassengers(data.Info.m_vehicleAI as PassengerBlimpAI, vehicleID, ref data, targetBuilding, nextStop);
            //begin mod(+): track stats 
            ushort num1 = (ushort)((uint)transferSize1 - (uint)data.m_transferSize);
            VehicleManagerMod.m_cachedVehicleData[(int)vehicleID].LastStopGonePassengers = (int)num1;
            VehicleManagerMod.m_cachedVehicleData[(int)vehicleID].CurrentStop = targetBuilding;
            NetManagerMod.m_cachedNodeData[(int)targetBuilding].PassengersOut += (int)num1;
            //end mod
            if ((int)nextStop == 0)
            {
                data.m_flags |= Vehicle.Flags.GoingBack;
                if (!this.StartPathFind(vehicleID, ref data))
                    return true;
                data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
                data.m_flags |= Vehicle.Flags.Stopped;
                data.m_waitCounter = (byte)0;
            }
            else
            {
                data.m_targetBuilding = nextStop;
                if (!this.StartPathFind(vehicleID, ref data))
                    return true;
                //begin mod(+): track stats
                ushort transferSize2 = data.m_transferSize;
                //end mod
                PassengerBlimpAIDetour.LoadPassengers(data.Info.m_vehicleAI as PassengerBlimpAI, vehicleID, ref data, targetBuilding, nextStop);
                //begin mod(+): track stats
                ushort num2 = (ushort)((uint)data.m_transferSize - (uint)transferSize2);
                int ticketPrice = data.Info.m_vehicleAI.GetTicketPrice(vehicleID, ref data);
                VehicleManagerMod.m_cachedVehicleData[(int)vehicleID].Add((int)num2, ticketPrice);
                NetManagerMod.m_cachedNodeData[(int)targetBuilding].PassengersIn += (int)num2;
                //end mod
                data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
                data.m_flags |= Vehicle.Flags.Stopped;
                data.m_waitCounter = (byte)0;
            }
            return false;
        }


        [RedirectReverse]
        private static void LoadPassengers(PassengerBlimpAI ai, ushort vehicleID, ref Vehicle data, ushort currentStop,
            ushort nextStop)
        {
            UnityEngine.Debug.Log("LoadPassengers");
        }

        [RedirectReverse]
        private static void UnloadPassengers(PassengerBlimpAI ai, ushort vehicleID, ref Vehicle data, ushort currentStop,
            ushort nextStop)
        {
            UnityEngine.Debug.Log("UnloadPassengers");
        }
    }
}