using ColossalFramework.UI;
using ImprovedPublicTransport2.HarmonyPatches.PublicTransportWorldInfoPanelPatches;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches.PublicTransportVehicleButtonPatches
{

    public static class OnMouseDownPatch
    {
        
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportVehicleButton), "OnMouseDown"),
                new PatchUtil.MethodDefinition(typeof(OnMouseDownPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportVehicleButton), "OnMouseDown")
            );
        }

        private static bool Prefix(UIComponent component, UIMouseEventParameter eventParam)
        {
            ushort objectUserData = (ushort)(component as UIButton).objectUserData;
            InstanceID empty = InstanceID.Empty;
            empty.Vehicle = objectUserData;
            Vector3 position;
            Quaternion rotation;
            Vector3 size;
            InstanceManager.GetPosition(empty, out position, out rotation, out size);
            if ((Object) PublicTransportVehicleButton.cameraController != (Object) null)
            {
                //begin mod: zoom on shift pressed
                PublicTransportVehicleButton.cameraController.SetTarget(empty, position, Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift));
                //end mod
            }
            PublicTransportWorldInfoPanel.ResetScrollPosition();
            UIView.SetFocus((UIComponent)null);

            return false;
        }
    }
}