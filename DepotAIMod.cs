// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.DepotAIMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public static class DepotAIMod
  {
    public static ushort StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer, string prefabName)
    {
      SimulationManager instance1 = Singleton<SimulationManager>.instance;
      VehicleManager instance2 = Singleton<VehicleManager>.instance;
      ushort vehicle = 0;
      DepotAI buildingAi = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) buildingID].Info.m_buildingAI as DepotAI;
      if (reason == buildingAi.m_transportInfo.m_vehicleReason)
      {
        VehicleInfo vehicleInfo = VehicleManagerMod.GetVehicleInfo(ref instance1.m_randomizer, buildingAi.m_info.m_class, offer.TransportLine, prefabName);
        if ((Object) vehicleInfo != (Object) null)
        {
          Vector3 position;
          Vector3 target;
          buildingAi.CalculateSpawnPosition(buildingID, ref data, ref instance1.m_randomizer, vehicleInfo, out position, out target);
          if (instance2.CreateVehicle(out vehicle, ref instance1.m_randomizer, vehicleInfo, position, reason, false, true))
          {
            vehicleInfo.m_vehicleAI.SetSource(vehicle, ref instance2.m_vehicles.m_buffer[(int) vehicle], buildingID);
            vehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref instance2.m_vehicles.m_buffer[(int) vehicle], reason, offer);
          }
        }
      }
      return vehicle;
    }
  }
}
