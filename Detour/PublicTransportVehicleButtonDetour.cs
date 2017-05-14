using ColossalFramework.UI;
using ImprovedPublicTransport.Redirection.Attributes;
using UnityEngine;

namespace ImprovedPublicTransport.Detour
{
    [TargetType(typeof(PublicTransportVehicleButton))]
    public class PublicTransportVehicleButtonDetour : PublicTransportVehicleButton
    {
        [RedirectMethod]
        private void OnMouseDown(UIComponent component, UIMouseEventParameter eventParam)
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
        }
    }
}