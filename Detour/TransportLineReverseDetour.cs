using System.Runtime.CompilerServices;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;

namespace ImprovedPublicTransport2.Detour
{
    //TODO: Only to access the private method. use a reversed patch instead
    [TargetType(typeof(TransportLine))]
    public struct TransportLineReverseDetour
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [RedirectReverse]
        public static ushort GetActiveVehicle(ref TransportLine thisLine, int index)
        {
            UnityEngine.Debug.Log("GetActiveVehicle");
            return 0;
        }
    }
}