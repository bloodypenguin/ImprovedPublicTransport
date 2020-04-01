using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using System.Reflection;
using Harmony;
using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.Detour.Vehicles;
using ImprovedPublicTransport2.HarmonyPatches;
using ImprovedPublicTransport2.OptionsFramework.Extensions;
using ImprovedPublicTransport2.RedirectionFramework;
using UnityEngine;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2
{
  public class ImprovedPublicTransportMod : LoadingExtensionBase, IUserMod
  {
    private const string HarmonyId = "github.com/bloodypenguin/ImprovedPublicTransport";
    
    public static bool inGame = false;
    private GameObject _iptGameObject;
    private GameObject _worldInfoPanel;
    private readonly string version = "5.0.2";
    private HarmonyInstance HarmonyInstance;

    public string Name => $"Improved Public Transport 2 [r{version}]";

    public string Description => Localization.Get("MOD_DESCRIPTION");

      public void OnSettingsUI(UIHelperBase helper)
      {
          helper.AddOptionsGroup<Settings>(Localization.Get);
      }
      
      public override void OnCreated(ILoading loading)
      {
        base.OnCreated(loading);
        HarmonyInstance = HarmonyInstance.Create(HarmonyId);
      }

    public override void OnLevelLoaded(LoadMode mode)
    {
            base.OnLevelLoaded(mode);
        if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.NewGameFromScenario)
        {
            return;
        }
        inGame = true;
        try
      {
        Utils.Log((object) $"IPT2: Begin init version: {version}");
        this.ReleaseUnusedCitizenUnits();
        UIView objectOfType = UnityEngine.Object.FindObjectOfType<UIView>();
        if ((UnityEngine.Object) objectOfType != (UnityEngine.Object) null)
        {
          this._iptGameObject = new GameObject("IptGameObject");
          this._iptGameObject.transform.parent = objectOfType.transform;
          this._iptGameObject.AddComponent<SimHelper>();
          this._iptGameObject.AddComponent<LineWatcher>();
          this._worldInfoPanel = new GameObject("PublicTransportStopWorldInfoPanel");
          this._worldInfoPanel.transform.parent = objectOfType.transform;
          this._worldInfoPanel.AddComponent<PublicTransportStopWorldInfoPanel>();
          
          CachedNodeData.Init();
          
          Patch(typeof(DepotAI), nameof(DepotAI.StartTransfer), DepotAIPatch.GetPrefix(), null);
          
          Patch(typeof(NetManager), nameof(NetManager.ReleaseNode), null,
            typeof(NetManagerPatch).GetMethod(nameof(NetManagerPatch.ReleaseNode))); //TODO: think, whether it should be pre or post

          int maxVehicleCount;
          if (Utils.IsModActive(1764208250))
          {
            UnityEngine.Debug.LogWarning("IPT2: More Vehicles is enabled, applying compatibility workaround");
            maxVehicleCount = ushort.MaxValue + 1;
          }
          else
          {
            UnityEngine.Debug.Log("IPT2: More Vehicles is not enabled");
            maxVehicleCount = VehicleManager.MAX_VEHICLE_COUNT;
          }
          
          CachedVehicleData.Init(maxVehicleCount);
          Patch(typeof(VehicleManager), "ReleaseWaterSource", null,
            typeof(VehicleManagerPatch).GetMethod(nameof(VehicleManagerPatch.ReleaseWaterSource))); //TODO: think, whether it should be pre or post
          
          Redirector<BusAIDetour>.Deploy();
          PatchCountPassengers(typeof(BusAI));
          
          Redirector<TrolleybusAIDetour>.Deploy();
          PatchCountPassengers(typeof(TrolleybusAI));
          
          Redirector<PassengerTrainAIDetour>.Deploy();
          PatchCountPassengers(typeof(PassengerTrainAI));
          
          Redirector<PassengerShipAIDetour>.Deploy(); 
          PatchCountPassengers(typeof(PassengerShipAI));
          
          Redirector<PassengerPlaneAIDetour>.Deploy();
          PatchCountPassengers(typeof(PassengerPlaneAI));
          
          Redirector<PassengerFerryAIDetour>.Deploy();
          PatchCountPassengers(typeof(PassengerFerryAI));

          Redirector<PassengerBlimpAIDetour>.Deploy();
          PatchCountPassengers(typeof(PassengerBlimpAI));
          
          Redirector<PassengerHelicopterAIDetour>.Deploy();
          PatchCountPassengers(typeof(PassengerHelicopterAI));
          
          Redirector<TramAIDetour>.Deploy();
          PatchCountPassengers(typeof(TramAI));
          
          Redirector<CommonBuildingAIReverseDetour>.Deploy();
          Redirector<PublicTransportStopButtonDetour>.Deploy();
          Redirector<PublicTransportVehicleButtonDetour>.Deploy();
          Redirector<PublicTransportWorldInfoPanelDetour>.Deploy();
          BuildingExtension.Init();
          LineWatcher.instance.Init();
          
          CachedTransportLineData.Init();
          Redirector<TransportLineDetour>.Deploy();
          
          VehiclePrefabs.Init();
          SerializableDataExtension.instance.Loaded = true;
          LocaleModifier.Init();
          this._iptGameObject.AddComponent<VehicleEditor>();
          this._iptGameObject.AddComponent<PanelExtenderLine>();
          this._iptGameObject.AddComponent<PanelExtenderVehicle>();
          this._iptGameObject.AddComponent<PanelExtenderCityService>();
          Utils.Log((object) "Loading done!");
        }
        else
          Utils.LogError((object) "UIView not found, aborting!");
      }
      catch (Exception ex)
      {
        Utils.LogError((object) ("IPT2: Error during initialization, IPT disables itself." + System.Environment.NewLine + "Please try again without any other mod." + System.Environment.NewLine + "Please upload your log file and post the link here if that didn't help:" + System.Environment.NewLine + "http://steamcommunity.com/workshop/filedetails/discussion/424106600/615086038663282271/" + System.Environment.NewLine + ex.Message + System.Environment.NewLine + (object) ex.InnerException + System.Environment.NewLine + ex.StackTrace));
        this.Deinit();
      }
    }

    public override void OnLevelUnloading()
    {
            base.OnLevelUnloading();
      if (!inGame)
        return;
        inGame = false;
      this.Deinit();
      Utils.Log((object) ("Unloading done!" + System.Environment.NewLine));
    }

    private void PatchCountPassengers(Type type)
    {
      Patch(type, "UnloadPassengers", 
        typeof(UnloadPassengersPatch).GetMethod(nameof(UnloadPassengersPatch.UnloadPassengersPre), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static),
        typeof(UnloadPassengersPatch).GetMethod(nameof(UnloadPassengersPatch.UnloadPassengersPost), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
      );
      Patch(type, "LoadPassengers", 
        typeof(LoadPassengersPatch).GetMethod(nameof(LoadPassengersPatch.LoadPassengersPre), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static),
        typeof(LoadPassengersPatch).GetMethod(nameof(LoadPassengersPatch.LoadPassengersPost), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
      );
    }
    
    private void Patch(Type type, string methodName, MethodInfo prePatch, MethodInfo postPatch)
    {
      try
      {
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public;
        bindingFlags |= BindingFlags.Instance;

        HarmonyInstance.Patch(type.GetMethod(methodName,
            bindingFlags),
          prefix: prePatch == null ? null : new HarmonyMethod(prePatch),
          postfix: postPatch == null ? null : new HarmonyMethod(postPatch));
      }
      catch (Exception e)
      {
        UnityEngine.Debug.LogError("Improved Public Tranport 2: Failed to patch method " + methodName);
        UnityEngine.Debug.LogException(e);
      }
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
      HarmonyInstance?.UnpatchAll(HarmonyId);
      Redirector<TramAIDetour>.Revert();
      Redirector<PassengerTrainAIDetour>.Revert();
      Redirector<PassengerShipAIDetour>.Revert();
      Redirector<PassengerPlaneAIDetour>.Revert();
      Redirector<PassengerFerryAIDetour>.Revert();
      Redirector<PassengerBlimpAIDetour>.Revert();
      Redirector<BusAIDetour>.Revert();
      Redirector<CommonBuildingAIReverseDetour>.Revert();
      Redirector<PublicTransportStopButtonDetour>.Revert();
      Redirector<PublicTransportVehicleButtonDetour>.Revert();
      Redirector<PublicTransportWorldInfoPanelDetour>.Revert();
      Redirector<TrolleybusAIDetour>.Revert();
      Redirector<PassengerHelicopterAIDetour>.Revert();

      Redirector<TransportLineDetour>.Revert();
      CachedTransportLineData.Deinit();
      
      BuildingExtension.Deinit();
      CachedNodeData.Deinit();
      CachedVehicleData.Deinit();
      VehiclePrefabs.Deinit();
      SerializableDataExtension.instance.Loaded = false;
      LocaleModifier.Deinit();

      if ((UnityEngine.Object) this._iptGameObject != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._iptGameObject);
      if (!((UnityEngine.Object) this._worldInfoPanel != (UnityEngine.Object) null))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this._worldInfoPanel);
    }
  }
}
