using ColossalFramework;
using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.OptionsFramework;

namespace ImprovedPublicTransport2
{
    public static class SettingsActions
    {
        public static void OnBudgetCheckChanged(bool isChecked)
        {
            if (!ImprovedPublicTransportMod.inGame)
            {
                return;
            }
            SimulationManager.instance.AddAction(() =>
            {
                TransportManager instance = Singleton<TransportManager>.instance;
                int length = instance.m_lines.m_buffer.Length;
                for (int index = 0; index < length; ++index)
                {
                    if (!instance.m_lines.m_buffer[index].Complete)
                        TransportLineMod.SetBudgetControlState((ushort)index, isChecked);
                }
            });
        }

        public static void OnUpdateButtonClick()
        {
            if (!ImprovedPublicTransportMod.inGame)
            {
                return;
            }
            SimulationManager.instance.AddAction(() =>
            {
                int length = Singleton<TransportManager>.instance.m_lines.m_buffer.Length;
                var budgetControl = OptionsWrapper<Settings>.Options.BudgetControl;
                for (int index = 0; index < length; ++index)
                {
                    TransportLineMod.SetBudgetControlState((ushort) index, budgetControl);
                    if (budgetControl)
                        TransportLineMod.ClearEnqueuedVehicles((ushort) index);
                }
            });
        }

        public static void OnDefaultVehicleCountSubmitted(int count)
        {
            if (!ImprovedPublicTransportMod.inGame)
            {
                return;
            }
            SimulationManager.instance.AddAction(() =>
            {
                TransportManager instance = Singleton<TransportManager>.instance;
                int length = instance.m_lines.m_buffer.Length;
                for (int index = 0; index < length; ++index)
                {
                    if (!instance.m_lines.m_buffer[index].Complete)
                        TransportLineMod.SetTargetVehicleCount((ushort) index, count);
                }
            });
        }


        public static void OnResetButtonClick()
        {
            if (!ImprovedPublicTransportMod.inGame)
            {
                return;
            }
            SimulationManager.instance.AddAction(() =>
            {
                int length = Singleton<TransportManager>.instance.m_lines.m_buffer.Length;
                for (int index = 0; index < length; ++index)
                    TransportLineMod.SetNextSpawnTime((ushort) index, 0.0f);
            });
        }
    }
}