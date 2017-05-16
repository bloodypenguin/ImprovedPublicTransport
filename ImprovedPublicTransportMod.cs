// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.ImprovedPublicTransportMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using ImprovedPublicTransport.Detour;
using ImprovedPublicTransport.Redirection;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public class ImprovedPublicTransportMod : IUserMod, ILoadingExtension
  {
    public static Settings Settings = Serializer.LoadSettings();
    private LoadMode _loadMode;
    private GameObject _iptGameObject;
    private GameObject _worldInfoPanel;

    public string Name => "Improved Public Transport 2";

    public string Description
    {
      get
      {
        Localization.Load();
        return Localization.Get("MOD_DESCRIPTION");
      }
    }

    public void OnSettingsUI(UIHelperBase helper)
    {
      ImprovedPublicTransportMod.Settings = Serializer.LoadSettings();
      Localization.Load();
      UIHelperBase uiHelperBase = helper.AddGroup(Localization.Get("SETTINGS_UI"));
      uiHelperBase.AddDropdown(Localization.Get("SETTINGS_VEHICLE_EDITOR_POSITION"), new string[2]
      {
        Localization.Get("SETTINGS_VEHICLE_EDITOR_POSITION_BOTTOM"),
        Localization.Get("SETTINGS_VEHICLE_EDITOR_POSITION_RIGHT")
      }, ImprovedPublicTransportMod.Settings.VehicleEditorPosition, new OnDropdownSelectionChanged(this.SelectedIndexChanged));
      uiHelperBase.AddCheckbox(Localization.Get("SETTINGS_VEHICLE_EDITOR_HIDE"), ImprovedPublicTransportMod.Settings.HideVehicleEditor, new OnCheckChanged(this.CheckChanged));
    }

    private void SelectedIndexChanged(int selectedIndex)
    {
      if (selectedIndex > 1)
        throw new ArgumentOutOfRangeException("selectedIndex");
      ImprovedPublicTransportMod.Settings.VehicleEditorPosition = selectedIndex;
      Serializer.SaveSettings(ImprovedPublicTransportMod.Settings);
    }

    private void CheckChanged(bool isChecked)
    {
      ImprovedPublicTransportMod.Settings.HideVehicleEditor = isChecked;
      Serializer.SaveSettings(ImprovedPublicTransportMod.Settings);
    }

    public void OnCreated(ILoading loading)
    {
    }

    public void OnLevelLoaded(LoadMode mode)
    {
      this._loadMode = mode;
      if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
        return;
      try
      {
        Utils.Log((object) "Begin init version: 3.8.10");
        ImprovedPublicTransportMod.Settings.LogSettings();
        this.ReleaseUnusedCitizenUnits();
        UIView objectOfType = UnityEngine.Object.FindObjectOfType<UIView>();
        if ((UnityEngine.Object) objectOfType != (UnityEngine.Object) null)
        {
          this._iptGameObject = new GameObject("IptGameObject");
          this._iptGameObject.transform.parent = objectOfType.transform;
          this._iptGameObject.AddComponent<VehicleEditor>();
          this._iptGameObject.AddComponent<PanelExtenderLine>();
          this._iptGameObject.AddComponent<PanelExtenderVehicle>();
          this._iptGameObject.AddComponent<PanelExtenderCityService>();
          this._iptGameObject.AddComponent<SimHelper>();
          this._iptGameObject.AddComponent<LineWatcher>();
          this._worldInfoPanel = new GameObject("PublicTransportStopWorldInfoPanel");
          this._worldInfoPanel.transform.parent = objectOfType.transform;
          this._worldInfoPanel.AddComponent<PublicTransportStopWorldInfoPanel>().Show();
          NetManagerMod.Init();
          VehicleManagerMod.Init();
          Redirector<BusAIDetour>.Deploy();
          Redirector<PassengerTrainAIDetour>.Deploy();
          Redirector<PassengerShipAIDetour>.Deploy(); 
          Redirector<PassengerPlaneAIDetour>.Deploy();
          Redirector<TramAIDetour>.Deploy();
          Redirector<CommonBuildingAIReverseDetour>.Deploy();  
          BuildingWatcher.instance.Init();
          LineWatcher.instance.Init();
          TransportLineMod.Init();
          VehiclePrefabs.Init();
          SerializableDataExtension.instance.Loaded = true;
          LocaleModifier.Init();
          Redirector<PublicTransportStopButtonDetour>.Deploy();
          Redirector<PublicTransportVehicleButtonDetour>.Deploy();
          Redirector<PublicTransportWorldInfoPanelDetour>.Deploy();
          Utils.Log((object) "Loading done!");
        }
        else
          Utils.LogError((object) "UIView not found, aborting!");
      }
      catch (Exception ex)
      {
        Utils.LogError((object) ("Error during initialization, IPT disables itself." + System.Environment.NewLine + "Please try again without any other mod." + System.Environment.NewLine + "Please upload your log file and post the link here if that didn't help:" + System.Environment.NewLine + "http://steamcommunity.com/workshop/filedetails/discussion/424106600/615086038663282271/" + System.Environment.NewLine + ex.Message + System.Environment.NewLine + (object) ex.InnerException + System.Environment.NewLine + ex.StackTrace));
        this.Deinit();
      }
    }

    public void OnLevelUnloading()
    {
      if (this._loadMode != LoadMode.LoadGame && this._loadMode != LoadMode.NewGame && this._loadMode == LoadMode.NewGameFromScenario)
        return;
      Serializer.SaveSettings(ImprovedPublicTransportMod.Settings);
      this.Deinit();
      Utils.Log((object) ("Unloading done!" + System.Environment.NewLine));
    }

    public void OnReleased()
    {
    }

    private void ReleaseUnusedCitizenUnits()
    {
      Utils.Log((object) "Find and clear unused citizen units.");
      CitizenManager instance = Singleton<CitizenManager>.instance;
      int num = 0;
      for (int index = 0; index < instance.m_units.m_buffer.Length; ++index)
      {
        CitizenUnit citizenUnit = instance.m_units.m_buffer[index];
        if (citizenUnit.m_flags != CitizenUnit.Flags.None && (int) citizenUnit.m_building == 0 && ((int) citizenUnit.m_vehicle == 0 && (int) citizenUnit.m_goods == 0))
        {
          ++num;
          instance.m_units.m_buffer[index] = new CitizenUnit();
          instance.m_units.ReleaseItem((uint) index);
          Utils.LogToTxt((object) string.Format("CitizenUnit #{0} - Flags: {1} - Citizens: #{2} | #{3} | #{4} | #{5} | #{6}", (object) index, (object) citizenUnit.m_flags, (object) citizenUnit.m_citizen0, (object) citizenUnit.m_citizen1, (object) citizenUnit.m_citizen2, (object) citizenUnit.m_citizen3, (object) citizenUnit.m_citizen4));
        }
      }
      Utils.Log((object) ("Cleared " + (object) num + " unused citizen units."));
    }

    private void Deinit()
    {
      Redirector<TramAIDetour>.Revert();
      Redirector<PassengerTrainAIDetour>.Revert();
      Redirector<PassengerShipAIDetour>.Revert();
      Redirector<PassengerPlaneAIDetour>.Revert();
      Redirector<BusAIDetour>.Revert();
      Redirector<CommonBuildingAIReverseDetour>.Revert();
      TransportLineMod.Deinit();
      BuildingWatcher.instance.Deinit();
      NetManagerMod.Deinit();
      VehicleManagerMod.Deinit();
      VehiclePrefabs.Deinit();
      SerializableDataExtension.instance.Loaded = false;
      LocaleModifier.Deinit();
      Redirector<PublicTransportStopButtonDetour>.Revert();
      Redirector<PublicTransportVehicleButtonDetour>.Revert();
      Redirector<PublicTransportWorldInfoPanelDetour>.Revert();

      if ((UnityEngine.Object) this._iptGameObject != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._iptGameObject);
      if (!((UnityEngine.Object) this._worldInfoPanel != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this._worldInfoPanel);
    }
  }
}
