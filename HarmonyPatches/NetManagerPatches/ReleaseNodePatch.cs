using ImprovedPublicTransport2.PersistentData;
using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.NetManagerPatches
{
    public class ReleaseNodePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(NetManager), nameof(NetManager.ReleaseNode)),
                null,
                new PatchUtil.MethodDefinition(typeof(ReleaseNodePatch), nameof(ReleaseNodePost))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(NetManager), nameof(NetManager.ReleaseNode))
            );
        }

        public static void ReleaseNodePost(ushort node)
        {
            if (CachedNodeData.m_cachedNodeData[node].IsEmpty)
            {
                return;
            }

            CachedNodeData.m_cachedNodeData[node] = new NodeData();
        }
    }
}