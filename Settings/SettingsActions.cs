using ColossalFramework;
using ImprovedPublicTransport2.OptionsFramework;

namespace ImprovedPublicTransport2.Settings
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
                        CachedTransportLineData.SetBudgetControlState((ushort) index, isChecked);
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
                var budgetControl = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.BudgetControl;
                for (int index = 0; index < length; ++index)
                {
                    CachedTransportLineData.SetBudgetControlState((ushort) index, budgetControl);
                    if (budgetControl)
                        CachedTransportLineData.ClearEnqueuedVehicles((ushort) index);
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
                        CachedTransportLineData.SetTargetVehicleCount((ushort) index, count);
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
                    CachedTransportLineData.SetNextSpawnTime((ushort) index, 0.0f);
            });
        }


        public static void OnDeleteLinesClick()
        {
            if (!ImprovedPublicTransportMod.inGame)
            {
                return;
            }
            if (!OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteBusLines &&
                !OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteTramLines &&
                !OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteTrainLines &&
                !OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteMetroLines &&
                !OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteMonorailLines &&
                !OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteShipLines &&
                !OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeletePlaneLines)
            {
                return;
            }
            WorldInfoPanel.Hide<PublicTransportWorldInfoPanel>();
            ConfirmPanel.ShowModal(Localization.Get("SETTINGS_LINE_DELETION_TOOL_CONFIRM_TITLE"),
                Localization.Get("SETTINGS_LINE_DELETION_TOOL_CONFIRM_MSG"), (s, r) =>
                {
                    if (r != 1)
                        return;
                    Singleton<SimulationManager>.instance.AddAction(() =>
                    {
                        SimulationManager.instance.AddAction(DeleteLines);
                    });
                });
        }

        private static void DeleteLines()
        {
            TransportManager instance = Singleton<TransportManager>.instance;
            int length = instance.m_lines.m_buffer.Length;
            for (int index = 0; index < length; ++index)
            {
                TransportInfo info = instance.m_lines.m_buffer[index].Info;
                if (info == null || instance.m_lines.m_buffer[index].m_flags == TransportLine.Flags.None)
                {
                    continue;
                }
                bool flag = false;
                var subService = info.GetSubService();
                var service = info.GetService();
                var level = info.GetClassLevel();
                if (service == ItemClass.Service.PublicTransport) //TODO(earalov): handle evacuation buses
                {
                    if (level == ItemClass.Level.Level1)
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportBus:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteBusLines;
                                break;
                            case ItemClass.SubService.PublicTransportMetro:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteMetroLines;
                                break;
                            case ItemClass.SubService.PublicTransportTrain:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteTrainLines;
                                break;
                            case ItemClass.SubService.PublicTransportShip:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteShipLines;
                                break;
                            case ItemClass.SubService.PublicTransportPlane:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeletePlaneLines;
                                break;
                            case ItemClass.SubService.PublicTransportTram:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteTramLines;
                                break;
                            case ItemClass.SubService.PublicTransportMonorail:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteMonorailLines;
                                break;
                            case ItemClass.SubService.PublicTransportTrolleybus:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteBusLines;
                                break;
                        }
                    }
                    else if (level == ItemClass.Level.Level2)
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportBus:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteBusLines;
                                break;
                            case ItemClass.SubService.PublicTransportShip:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteShipLines;
                                break;
                            case ItemClass.SubService.PublicTransportPlane:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeletePlaneLines;
                                break;
                            case ItemClass.SubService.PublicTransportTrain:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteTrainLines;
                                break;
                        }
                    }
                    else if (level == ItemClass.Level.Level3)
                    {
                        switch (subService)
                        {
                            case ItemClass.SubService.PublicTransportTours:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeleteBusLines;
                                break;
                            case ItemClass.SubService.PublicTransportPlane:
                                flag = OptionsWrapper<ImprovedPublicTransport2.Settings.Settings>.Options.DeletePlaneLines;
                                break;
                        }
                    }
                    if (flag)
                    {
                        instance.ReleaseLine((ushort) index); //TODO(earalov): make sure that outside connection lines don't get deleted
                    }
                }
            }
        }
    }
}