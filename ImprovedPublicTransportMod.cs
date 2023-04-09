using System;
using CitiesHarmony.API;
using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using ImprovedPublicTransport2.HarmonyPatches.BuildingManagerPatches;
using ImprovedPublicTransport2.HarmonyPatches.DepotAIPatches;
using ImprovedPublicTransport2.HarmonyPatches.NetManagerPatches;
using ImprovedPublicTransport2.HarmonyPatches.PublicTransportLineVehicleSelectorPatches;
using ImprovedPublicTransport2.HarmonyPatches.TransportLinePatches;
using ImprovedPublicTransport2.HarmonyPatches.TransportManagerPatches;
using ImprovedPublicTransport2.HarmonyPatches.VehicleManagerPatches;
using ImprovedPublicTransport2.HarmonyPatches.XYZVehicleAIPatches;
using ImprovedPublicTransport2.OptionsFramework.Extensions;
using ImprovedPublicTransport2.RedirectionFramework;
using ImprovedPublicTransport2.Data;
using ImprovedPublicTransport2.HarmonyPatches.PublicTransportVehicleButtonPatches;
using ImprovedPublicTransport2.HarmonyPatches.PublicTransportWorldInfoPanelPatches;
using ImprovedPublicTransport2.ReverseDetours;
using ImprovedPublicTransport2.UI;
using ImprovedPublicTransport2.UI.PanelExtenders;
using UnityEngine;
using Object = UnityEngine.Object;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2
{
    public class ImprovedPublicTransportMod : LoadingExtensionBase, IUserMod
    {
        public const string BaseModName = "Improved Public Transport 2";
        public const string ShortModName = "IPT2";

        public static bool inGame;
        public static GameObject _iptGameObject;
        private GameObject _worldInfoPanel;
        private readonly string version = "7.0.0-preview4";

        public string Name => $"{BaseModName} [r{version}]";

        public string Description => Localization.Get("MOD_DESCRIPTION");

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddOptionsGroup<Settings.Settings>(Localization.Get);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }

            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
            {
                return;
            }

            inGame = true;
            try
            {
                Utils.Log($"{ShortModName}: Begin init version: {version}");
                ReleaseUnusedCitizenUnits();
                UIView objectOfType = Object.FindObjectOfType<UIView>();
                if (objectOfType != null)
                {
                    _iptGameObject = new GameObject("IptGameObject");
                    _iptGameObject.transform.parent = objectOfType.transform;
                    _iptGameObject.AddComponent<SimHelper>();
                    _iptGameObject.AddComponent<LineWatcher>();
                    _worldInfoPanel = new GameObject("PublicTransportStopWorldInfoPanel");
                    _worldInfoPanel.transform.parent = objectOfType.transform;
                    _worldInfoPanel.AddComponent<PublicTransportStopWorldInfoPanel>();

                    CachedNodeData.Init();

                    int maxVehicleCount;
                    if (Utils.IsModActive(1764208250)) // More Vehicles
                    {
                        Debug.LogWarning(
                            $"{ShortModName}: More Vehicles is enabled, applying compatibility workaround");
                        maxVehicleCount = ushort.MaxValue + 1;
                    }
                    else
                    {
                        Debug.Log($"{ShortModName}: More Vehicles is not enabled");
                        maxVehicleCount = VehicleManager.MAX_VEHICLE_COUNT;
                    }

                    CachedVehicleData.Init(maxVehicleCount);

                    LoadPassengersPatch.Apply();
                    UnloadPassengersPatch.Apply();
                    StartTransferPatch.Apply();
                    ReleaseNodePatch.Apply();
                    ReleaseWaterSourcePatch.Apply();
                    GetVehicleInfoPatch.Apply();
                    CheckTransportLineVehiclesPatch.Apply();
                    ClassMatchesPatch.Apply();
                    CanLeavePatch.Apply();

                    Redirector<CommonBuildingAIReverseDetour>.Deploy();
                    HarmonyPatches.PublicTransportStopButtonPatches.OnMouseDownPatch.Apply();
                    HarmonyPatches.PublicTransportVehicleButtonPatches.OnMouseDownPatch.Apply();
                    RefreshVehicleButtonsPatch.Apply();
                    UpdateStopButtonsPatch.Apply();
                    BuildingExtension.Init();
                    LineWatcher.instance.Init();

                    CachedTransportLineData.Init();
                    Redirector<TransportLineReverseDetour>.Deploy();
                    SimulationStepPatch.Apply();
                    GetLineVehiclePatch.Apply();
                    CanLeaveStopPatch.Apply();

                    VehiclePrefabs.Init();
                    SerializableDataExtension.instance.Loaded = true;
                    LocaleModifier.Init();
                    _iptGameObject.AddComponent<VehicleEditor>();
                    _iptGameObject.AddComponent<PanelExtenderLine>();
                    _iptGameObject.AddComponent<PanelExtenderVehicle>();
                    _iptGameObject.AddComponent<PanelExtenderCityService>();
                    Utils.Log("Loading done!");
                }
                else
                    Utils.LogError("UIView not found, aborting!");
            }
            catch (Exception ex)
            {
                Utils.LogError(
                    $"{ShortModName}: Error during initialization, IPT disables itself.{Environment.NewLine}" +
                    $"Please try again without any other mod.{Environment.NewLine}" +
                    $"Please upload your log file and post the link here if that didn't help:{Environment.NewLine}" +
                    $"http://steamcommunity.com/workshop/filedetails/discussion/424106600/615086038663282271/{Environment.NewLine}" +
                    $"{ex.Message}{Environment.NewLine}" +
                    $"{ex.InnerException}{Environment.NewLine}" +
                    $"{ex.StackTrace}");
                Deinit();
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }

            if (!inGame)
                return;
            inGame = false;
            Deinit();
            Utils.Log("Unloading done!" + Environment.NewLine);
        }


        private void ReleaseUnusedCitizenUnits()
        {
            Utils.Log("Find and clear unused citizen units.");
            CitizenManager instance = Singleton<CitizenManager>.instance;
            int num = 0;
            for (int index = 0; index < instance.m_units.m_buffer.Length; ++index)
            {
                CitizenUnit citizenUnit = instance.m_units.m_buffer[index];
                if (citizenUnit.m_flags != CitizenUnit.Flags.None && citizenUnit.m_building == 0 &&
                    (citizenUnit.m_vehicle == 0 && citizenUnit.m_goods == 0))
                {
                    ++num;
                    instance.m_units.m_buffer[index] = new CitizenUnit();
                    instance.m_units.ReleaseItem((uint)index);
                    Utils.LogToTxt(
                        $"CitizenUnit #{index} - Flags: {citizenUnit.m_flags} - Citizens: #{citizenUnit.m_citizen0} | #{citizenUnit.m_citizen1} | #{citizenUnit.m_citizen2} | #{citizenUnit.m_citizen3} | #{citizenUnit.m_citizen4}");
                }
            }

            Utils.Log("Cleared " + num + " unused citizen units.");
        }

        private void Deinit()
        {
            LoadPassengersPatch.Undo();
            UnloadPassengersPatch.Undo();
            StartTransferPatch.Undo();
            ReleaseNodePatch.Undo();
            ReleaseWaterSourcePatch.Undo();
            GetVehicleInfoPatch.Undo();
            ClassMatchesPatch.Undo();
            CheckTransportLineVehiclesPatch.Undo();
            GetDepotLevelsPatch.Undo();
            CanLeavePatch.Undo();

            Redirector<CommonBuildingAIReverseDetour>.Revert();
            HarmonyPatches.PublicTransportStopButtonPatches.OnMouseDownPatch.Undo();
            HarmonyPatches.PublicTransportVehicleButtonPatches.OnMouseDownPatch.Undo();
            RefreshVehicleButtonsPatch.Undo();
            UpdateStopButtonsPatch.Undo();

            Redirector<TransportLineReverseDetour>.Revert();
            SimulationStepPatch.Undo();
            GetLineVehiclePatch.Undo();
            CanLeaveStopPatch.Undo();
            CachedTransportLineData.Deinit();

            BuildingExtension.Deinit();
            CachedNodeData.Deinit();
            CachedVehicleData.Deinit();
            SerializableDataExtension.instance.Loaded = false;
            LocaleModifier.Deinit();

            if (_iptGameObject != null)
                Object.Destroy(_iptGameObject);
            if (!(_worldInfoPanel != null))
                return;
            Object.Destroy(_worldInfoPanel);
        }

        public void OnEnabled()
        {
            HarmonyHelper.EnsureHarmonyInstalled();
        }
    }
}