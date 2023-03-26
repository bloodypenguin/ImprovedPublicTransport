// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.TransportLineMod
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using ColossalFramework;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.RedirectionFramework.Attributes;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.Detour
{
    //TODO: replace completely with patch
    [TargetType(typeof(TransportLine))]
    public struct TransportLineDetour
    {

        public const byte BoardingTime = 12; //from the original TransportLine time
        public const byte AirplaneBoardingTime = 200;
        public const byte MaxUnbunchingTime = byte.MaxValue - BoardingTime;

        [RedirectMethod]
        public static bool CanLeaveStop(ref TransportLine thisLine, ushort nextStop, int waitTime)
        {
            if ((int)nextStop == 0)
                return true;
            ushort prevSegment = TransportLine.GetPrevSegment(nextStop);

            // here has bug, but when you recompile it, it gone! lamo
            // bug: missing m_trafficLightState0
            if ((int)prevSegment == 0 || ((int)thisLine.m_averageInterval - (int)Singleton<NetManager>.instance.m_segments.m_buffer[(int)prevSegment].m_trafficLightState0 + 2) / 4 <= 0)
                return true;
            //begin mod(*): compare with interval aggression setup instead of default 64
            var targetWaitTime = BoardingTime + Mathf.Min(OptionsWrapper<Settings>.Options.IntervalAggressionFactor, MaxUnbunchingTime);
            return waitTime >= targetWaitTime; //4 * 16 = 64 is max waiting time in vanilla, 12 is min waiting time
            //end mod
        }

        [RedirectReverse]
        public static ushort GetActiveVehicle(ref TransportLine thisLine, int index)
        {
            UnityEngine.Debug.Log("GetActiveVehicle");
            return 0;
        }
    }
}