// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.BusAIMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using ColossalFramework;
using UnityEngine;

namespace ImprovedPublicTransport.Detour
{
  public class BusAIMod : BusAI
  {
    public const byte MIN_WAIT_COUNTER = 12;
    private const byte MAX_BOARDING_COUNTER = 60;
    private static bool _isDeployed;
    private static BusAIMod.LoadPassengersCallback LoadPassengers;
    private static BusAIMod.UnloadPassengersCallback UnloadPassengers;
    private static Redirection<BusAI, BusAIMod> _redirectionArriveAtTarget;
    private static Redirection<BusAI, BusAIMod> _redirectionCanLeave;

    public static void Init()
    {
      if (BusAIMod._isDeployed)
        return;
      BusAIMod.LoadPassengers = (BusAIMod.LoadPassengersCallback) Utils.CreateDelegate<BusAI, BusAIMod.LoadPassengersCallback>("LoadPassengers", (object) null);
      BusAIMod.UnloadPassengers = (BusAIMod.UnloadPassengersCallback) Utils.CreateDelegate<BusAI, BusAIMod.UnloadPassengersCallback>("UnloadPassengers", (object) null);
      BusAIMod._redirectionArriveAtTarget = new Redirection<BusAI, BusAIMod>("ArriveAtTarget");
      BusAIMod._redirectionCanLeave = new Redirection<BusAI, BusAIMod>("CanLeave");
      BusAIMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!BusAIMod._isDeployed)
        return;
      BusAIMod.LoadPassengers = (BusAIMod.LoadPassengersCallback) null;
      BusAIMod.UnloadPassengers = (BusAIMod.UnloadPassengersCallback) null;
      BusAIMod._redirectionArriveAtTarget.Revert();
      BusAIMod._redirectionArriveAtTarget = (Redirection<BusAI, BusAIMod>) null;
      BusAIMod._redirectionCanLeave.Revert();
      BusAIMod._redirectionCanLeave = (Redirection<BusAI, BusAIMod>) null;
      BusAIMod._isDeployed = false;
    }

    private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
    {
      if ((int) data.m_targetBuilding == 0)
      {
        Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
        return true;
      }
      ushort nextStop = 0;
      if ((int) data.m_transportLine != 0)
        nextStop = TransportLine.GetNextStop(data.m_targetBuilding);
      ushort targetBuilding = data.m_targetBuilding;
      ushort transferSize1 = data.m_transferSize;
      BusAIMod.UnloadPassengers((BusAI) this, vehicleID, ref data, targetBuilding, nextStop);
      ushort num1 = (ushort) ((uint) transferSize1 - (uint) data.m_transferSize);
      VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].LastStopGonePassengers = (int) num1;
      VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].CurrentStop = targetBuilding;
      NetManagerMod.m_cachedNodeData[(int) targetBuilding].PassengersOut += (int) num1;
      if ((int) nextStop == 0)
      {
        data.m_flags |= Vehicle.Flags.GoingBack;
        if (!this.StartPathFind(vehicleID, ref data))
          return true;
        data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
        data.m_flags |= Vehicle.Flags.Stopped;
        data.m_waitCounter = (byte) 0;
      }
      else
      {
        data.m_targetBuilding = nextStop;
        if (!this.StartPathFind(vehicleID, ref data))
          return true;
        ushort transferSize2 = data.m_transferSize;
        BusAIMod.LoadPassengers((BusAI) this, vehicleID, ref data, targetBuilding, nextStop);
        ushort num2 = (ushort) ((uint) data.m_transferSize - (uint) transferSize2);
        int ticketPrice = data.Info.m_vehicleAI.GetTicketPrice(vehicleID, ref data);
        VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].Add((int) num2, ticketPrice);
        NetManagerMod.m_cachedNodeData[(int) targetBuilding].PassengersIn += (int) num2;
        data.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
        data.m_flags |= Vehicle.Flags.Stopped;
        data.m_waitCounter = (byte) 0;
      }
      return false;
    }

    public new bool CanLeave(ushort vehicleID, ref Vehicle vehicleData)
    {
      if ((int) vehicleData.m_leadingVehicle != 0)
        return true;
      if ((int) vehicleData.m_waitCounter >= 12 && BusAIMod.IsBoardingDone(vehicleID, ref vehicleData))
        return BusAIMod.IsUnbunchingDone(vehicleID, ref vehicleData);
      return false;
    }

    private static bool IsBoardingDone(ushort vehicleID, ref Vehicle vehicleData)
    {
      CitizenManager instance1 = Singleton<CitizenManager>.instance;
      bool flag = false;
      ushort firstVehicle = vehicleData.GetFirstVehicle(vehicleID);
      if ((int) Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int) firstVehicle].m_waitCounter >= 60)
        flag = true;
      uint num1 = vehicleData.m_citizenUnits;
      int num2 = 0;
      while ((int) num1 != 0)
      {
        uint nextUnit = instance1.m_units.m_buffer[(int) num1].m_nextUnit;
        for (int index = 0; index < 5; ++index)
        {
          uint citizen = instance1.m_units.m_buffer[(int) num1].GetCitizen(index);
          if ((int) citizen != 0)
          {
            ushort instance2 = instance1.m_citizens.m_buffer[(int) citizen].m_instance;
            if ((int) instance2 != 0 && (instance1.m_instances.m_buffer[(int) instance2].m_flags & CitizenInstance.Flags.EnteringVehicle) != CitizenInstance.Flags.None)
            {
              if (!flag)
                return false;
              BusAIMod.EnterVehicle(firstVehicle, instance2, ref instance1.m_instances.m_buffer[(int) instance2]);
            }
          }
        }
        num1 = nextUnit;
        if (++num2 > 524288)
        {
          CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
          break;
        }
      }
      return true;
    }

    private static bool IsUnbunchingDone(ushort vehicleID, ref Vehicle vehicleData)
    {
      bool flag;
      try
      {
        if ((int) vehicleData.m_transportLine == 0 || (int) ImprovedPublicTransportMod.Settings.IntervalAggressionFactor == 0 || (!TransportLineMod.GetUnbunchingState(vehicleData.m_transportLine) || (int) vehicleData.m_waitCounter >= (int) (byte) Mathf.Min((int) ImprovedPublicTransportMod.Settings.IntervalAggressionFactor * 6 + 12, (int) byte.MaxValue)))
        {
          flag = true;
        }
        else
        {
          TransportManager instance1 = Singleton<TransportManager>.instance;
          int length = instance1.m_lines.m_buffer[(int) vehicleData.m_transportLine].CountVehicles(vehicleData.m_transportLine);
          if (length == 1)
          {
            flag = true;
          }
          else
          {
            ushort currentStop = VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].CurrentStop;
            if ((int) currentStop != 0 && !NetManagerMod.m_cachedNodeData[(int) currentStop].Unbunching)
              flag = true;
            else if ((int) vehicleData.m_lastFrame != 0)
            {
              flag = false;
            }
            else
            {
              ushort vehicleID1 = instance1.m_lines.m_buffer[(int) vehicleData.m_transportLine].m_vehicles;
              VehicleManager instance2 = Singleton<VehicleManager>.instance;
              float max = 0.0f;
              ushort[] numArray = new ushort[length];
              float[] keys = new float[length];
              for (int index = 0; index < length; ++index)
              {
                float current;
                BusAIMod.GetProgressStatus(vehicleID1, ref instance2.m_vehicles.m_buffer[(int) vehicleID1], out current, out max);
                numArray[index] = vehicleID1;
                keys[index] = current;
                vehicleID1 = instance2.m_vehicles.m_buffer[(int) vehicleID1].m_nextLineVehicle;
              }
              Array.Sort<float, ushort>(keys, numArray);
              int index1 = Array.IndexOf<ushort>(numArray, vehicleID);
              if (index1 == -1)
              {
                flag = true;
              }
              else
              {
                int index2 = index1 + 1;
                if (index2 == length)
                  index2 = 0;
                float num = keys[index2] - keys[index1];
                if ((double) num < 0.0)
                  num += max;
                flag = (double) num > (double) max / (double) length * 0.899999976158142;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (BusAIMod._isDeployed)
          Utils.Log((object) (ex.Message + System.Environment.NewLine + System.Environment.StackTrace));
        flag = true;
      }
      VehicleManagerMod.m_cachedVehicleData[(int) vehicleID].IsUnbunchingInProgress = !flag;
      return flag;
    }

    private static void EnterVehicle(ushort firstVehicle, ushort instanceID, ref CitizenInstance citizenData)
    {
      citizenData.m_flags &= ~CitizenInstance.Flags.EnteringVehicle;
      citizenData.Unspawn(instanceID);
      if ((int) citizenData.m_citizen == 0)
        return;
      VehicleManager instance = Singleton<VehicleManager>.instance;
      if ((int) firstVehicle == 0)
        return;
      VehicleInfo info = instance.m_vehicles.m_buffer[(int) firstVehicle].Info;
      int ticketPrice = info.m_vehicleAI.GetTicketPrice(firstVehicle, ref instance.m_vehicles.m_buffer[(int) firstVehicle]);
      if (ticketPrice == 0)
        return;
      Singleton<EconomyManager>.instance.AddResource(EconomyManager.Resource.PublicIncome, ticketPrice, info.m_class);
    }

    public static bool GetProgressStatus(ushort vehicleID, ref Vehicle data, out float current, out float max)
    {
      ushort transportLine = data.m_transportLine;
      ushort targetBuilding = data.m_targetBuilding;
      if ((int) transportLine != 0 && (int) targetBuilding != 0)
      {
        float min;
        float max1;
        float total;
        Singleton<TransportManager>.instance.m_lines.m_buffer[(int) transportLine].GetStopProgress(targetBuilding, out min, out max1, out total);
        uint path = data.m_path;
        bool valid;
        if ((int) path == 0 || (data.m_flags & Vehicle.Flags.WaitingPath) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        {
          current = min;
          valid = false;
        }
        else
          current = BusAI.GetPathProgress(path, (int) data.m_pathPositionIndex, min, max1, out valid);
        max = total;
        return valid;
      }
      current = 0.0f;
      max = 0.0f;
      return true;
    }

    private delegate void LoadPassengersCallback(BusAI ai, ushort vehicleID, ref Vehicle data, ushort currentStop, ushort nextStop);

    private delegate void UnloadPassengersCallback(BusAI ai, ushort vehicleID, ref Vehicle data, ushort currentStop, ushort nextStop);
  }
}
