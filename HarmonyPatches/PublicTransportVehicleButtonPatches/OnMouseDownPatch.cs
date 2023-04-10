using ColossalFramework.UI;
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
            var objectUserData = (ushort)(component as UIButton).objectUserData;
            var instanceID = InstanceID.Empty;
            instanceID.Vehicle = objectUserData;
            InstanceManager.GetPosition(instanceID, out var position, out _, out _);
            if (PublicTransportVehicleButton.cameraController != null)
            {
                //begin mod: zoom on shift pressed
                PublicTransportVehicleButton.cameraController.SetTarget(instanceID, position, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                //end mod
            }
            PublicTransportWorldInfoPanel.ResetScrollPosition();
            UIView.SetFocus(null);
            //begin mod: show PublicTransportVehicleWorldInfoPanel
            if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
            {
                WorldInfoPanel.Show<PublicTransportVehicleWorldInfoPanel>(position, instanceID);
            }
            //end mod

            return false;
        }
    }
}