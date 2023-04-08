using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.TransportLinePatches
{
    public class GetLineVehiclePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(TransportLine),
                    nameof(TransportLine.GetLineVehicle)),
                new PatchUtil.MethodDefinition(typeof(GetLineVehiclePatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(TransportLine),
                    nameof(TransportLine.GetLineVehicle))
            );
        }

        public static bool Prefix(ushort lineID, ref VehicleInfo __result)
        {
            var info = TransportManager.instance.m_lines.m_buffer[lineID].Info;
            if (lineID <= 0 || info?.m_class == null || info.m_class.m_service == ItemClass.Service.Disaster)
            {
                return true; //if it's not a proper transport line, let's not modify the behavior
            }

            var dequeuedVehicle = CachedTransportLineData.Dequeue(lineID);
            var name =  dequeuedVehicle ?? CachedTransportLineData.GetRandomPrefab(lineID);
            __result = string.IsNullOrEmpty(name) ? null : PrefabCollection<VehicleInfo>.FindLoaded(name);
            return false;
        }
    }
}