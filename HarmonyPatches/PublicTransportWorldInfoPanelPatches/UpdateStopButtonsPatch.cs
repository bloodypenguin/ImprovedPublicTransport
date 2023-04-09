using ColossalFramework;
using ColossalFramework.UI;
using ImprovedPublicTransport2.UI;
using ImprovedPublicTransport2.Util;

namespace ImprovedPublicTransport2.HarmonyPatches.PublicTransportWorldInfoPanelPatches
{
    public static class UpdateStopButtonsPatch
    {

        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportWorldInfoPanel), "UpdateStopButtons"),
                new PatchUtil.MethodDefinition(typeof(UpdateStopButtonsPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PublicTransportWorldInfoPanel), "UpdateStopButtons")
            );
        }
        
        public static bool Prefix(UITemplateList<UIButton> ___m_stopButtons, ushort lineID)
        {
            ushort stop = Singleton<TransportManager>.instance.m_lines.m_buffer[(int)lineID].m_stops;
            foreach (UIComponent uiComponent in ___m_stopButtons.items)
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

            return false;
        }
    }
}