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
        private static float _simulationTime;
        
        public static void Awake()
        {
            _simulationTime = 0;
        }

        public static float SimulationTime => _simulationTime;

        private void Update()
        {
            _simulationTime = _simulationTime + Singleton<SimulationManager>.instance.m_simulationTimeDelta;
        }

        private void OnDestroy()
        {
            _simulationTime = 0;
        }
    }
}