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
            var objectUserData = (ushort)(component as UIButton).objectUserData;
            var position = Singleton<NetManager>.instance.m_nodes.m_buffer[objectUserData].m_position;
            var instanceID = InstanceID.Empty;
            instanceID.NetNode = objectUserData;
            if (PublicTransportStopButton.cameraController != null)
                //begin mod: zoom on shift pressed
                ToolsModifierControl.cameraController.SetTarget(instanceID, position, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                //end mod

            PublicTransportWorldInfoPanel.ResetScrollPosition();
            UIView.SetFocus(null);


            //begin mod: show PublicTransportStopWorldInfoPanel
            if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
            {
                PublicTransportStopWorldInfoPanel.instance.Show(position, instanceID);
            }
            //end mod

            return false;
        }
    }
}