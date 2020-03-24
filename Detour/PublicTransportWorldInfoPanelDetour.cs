using System;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ImprovedPublicTransport2.Detour
{
    [TargetType(typeof(PublicTransportWorldInfoPanel))]
    public class PublicTransportWorldInfoPanelDetour
    {
        [RedirectMethod]
        private void RefreshVehicleButtons(ushort lineID)
        {
            this.m_vehicleButtons.SetItemCount(Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineID].CountVehicles(lineID));
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineID].m_vehicles;
            int index = 0;
            while ((int)num != 0)
            {
                Vector3 relativePosition = this.m_vehicleButtons.items[index].relativePosition;
                relativePosition.x = this.kvehiclesX;
                this.m_vehicleButtons.items[index].relativePosition = relativePosition;
                this.m_vehicleButtons.items[index].objectUserData = (object)num;
                //begin mod(+): add tooltip with asset name
                this.m_vehicleButtons.items[index].tooltip = Singleton<VehicleManager>.instance.GetVehicleName(num);
                //end mod
                num = instance.m_vehicles.m_buffer[(int)num].m_nextLineVehicle;
                if (++index >= CachedVehicleData.MaxVehicleCount)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + System.Environment.StackTrace);
                    break;
                }
            }
        }

        [RedirectMethod]
        private void UpdateStopButtons(ushort lineID)
        {
            ushort stop = Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineID].m_stops;
            foreach (UIComponent uiComponent in this.m_stopButtons.items)
            {
                uiComponent.Find<UILabel>("PassengerCount").text = Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineID].CalculatePassengerCount(stop).ToString();
                //begin mod(+): add tooltip with stop name
                var id = InstanceID.Empty;
                id.NetNode = stop;
                var name = Singleton<InstanceManager>.instance.GetName(id) ?? string.Empty;
                if (string.Empty == name)
                {
                    name = StopListBoxRow.GenerateStopName(name, stop, -1);
                }
                uiComponent.tooltip = name;
                //end mod

                stop = TransportLine.GetNextStop(stop);
            }
        }

        private UITemplateList<UIButton> m_vehicleButtons => (UITemplateList<UIButton>)
            typeof(PublicTransportWorldInfoPanel)
                .GetField("m_vehicleButtons", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(this);
        private UITemplateList<UIButton> m_stopButtons => (UITemplateList<UIButton>)
            typeof(PublicTransportWorldInfoPanel)
                .GetField("m_stopButtons", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(this);

        private float kvehiclesX => (float)
            typeof(PublicTransportWorldInfoPanel)
                .GetField("kvehiclesX", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(this);
    }
}