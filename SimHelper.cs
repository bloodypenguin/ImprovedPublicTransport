// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.SimHelper
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using UnityEngine;

namespace ImprovedPublicTransport2
{
  public class SimHelper : MonoBehaviour
  {
    public static SimHelper instance;
    private float _simulationTime;

    public static bool IsSimPaused
    {
      get
      {
        if (!Singleton<SimulationManager>.instance.SimulationPaused)
          return Singleton<SimulationManager>.instance.ForcedSimulationPaused;
        return true;
      }
    }

    public float SimulationTime
    {
      get
      {
        return this._simulationTime;
      }
    }

    private void Awake()
    {
      SimHelper.instance = this;
    }

    private void Update()
    {
      this._simulationTime = this._simulationTime + Singleton<SimulationManager>.instance.m_simulationTimeDelta;
    }

    private void OnDestroy()
    {
      SimHelper.instance = (SimHelper) null;
    }
  }
}
