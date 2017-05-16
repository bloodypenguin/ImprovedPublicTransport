using ImprovedPublicTransport.Redirection.Attributes;

namespace ImprovedPublicTransport.Detour
{
    [TargetType(typeof(CommonBuildingAI))]
    public class CommonBuildingAIReverseDetour : ShelterAI
    {
        [RedirectReverse]
        public static void CalculateOwnVehicles(CommonBuildingAI ai, ushort buildingID, ref Building data,
            TransferManager.TransferReason material, ref int count, ref int cargo, ref int capacity, ref int outside)
        {
            UnityEngine.Debug.Log("1");
        }
    }
}