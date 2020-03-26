using ImprovedPublicTransport2.Detour;

namespace ImprovedPublicTransport2.HarmonyPatches
{
    public class NetManagerPatch
    {
        public static void ReleaseNode(ushort node)
        {
            if (CachedNodeData.m_cachedNodeData[node].IsEmpty)
            {
                return;
            }
            CachedNodeData.m_cachedNodeData[node] = new NodeData();
        }
    }
}