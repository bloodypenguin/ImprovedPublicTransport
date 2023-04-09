using ColossalFramework;
using ColossalFramework.UI;
using ImprovedPublicTransport2.UI;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches.PublicTransportStopButtonPatches
{
    public class OnMouseDownPatch
    {

        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportStopButton), "OnMouseDown"),
                new PatchUtil.MethodDefinition(typeof(OnMouseDownPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportStopButton), "OnMouseDown")
            );
        }
        
        private static bool Prefix(UIComponent component, UIMouseEventParameter eventParam)
        {
            ushort objectUserData = (ushort)(component as UIButton).objectUserData;
            Vector3 position = Singleton<NetManager>.instance.m_nodes.m_buffer[(int)objectUserData].m_position;
            InstanceID empty = InstanceID.Empty;
            empty.NetNode = objectUserData;
            if ((Object)PublicTransportStopButton.cameraController != (Object)null)
                //begin mod: zoom on shift pressed
                ToolsModifierControl.cameraController.SetTarget(empty, position, Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift));
                //end mod

            PublicTransportWorldInfoPanel.ResetScrollPosition();
            UIView.SetFocus((UIComponent)null);

            //begin mod: show PublicTransportStopWorldInfoPanel
            PublicTransportStopWorldInfoPanel.instance.Show(position, empty);
            //end mod

            return false;
        }
    }
}