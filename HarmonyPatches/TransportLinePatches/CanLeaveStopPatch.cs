using ColossalFramework;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Util;
using UnityEngine;

namespace ImprovedPublicTransport2.HarmonyPatches.TransportLinePatches
{
    public static class CanLeaveStopPatch
    {
        public const byte BoardingTime = 12; //from the original TransportLine time
        public const byte AirplaneBoardingTime = 200;
        public const byte MaxUnbunchingTime = byte.MaxValue - BoardingTime;
        
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(TransportLine),
                    nameof(TransportLine.CanLeaveStop)),
                new PatchUtil.MethodDefinition(typeof(CanLeaveStopPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(TransportLine),
                    nameof(TransportLine.CanLeaveStop))
            );
        }

        public static bool Prefix(ref TransportLine __instance, out bool __result, ushort nextStop, int waitTime)
        {
            if (nextStop == 0)
            {
                __result = true;
                return false;
            }

            var prevSegment = TransportLine.GetPrevSegment(nextStop);
            if (prevSegment == 0 || (__instance.m_averageInterval -
                Singleton<NetManager>.instance.m_segments.m_buffer[prevSegment].m_trafficLightState0 + 2) / 4 <= 0)
            {
                __result = true;
                return false;
            }

            //begin mod(*): compare with interval aggression setup instead of default 64
            var targetWaitTime = BoardingTime + Mathf.Min(OptionsWrapper<Settings.Settings>.Options.IntervalAggressionFactor, MaxUnbunchingTime);
            __result = waitTime >= targetWaitTime; //4 * 16 = 64 is max waiting time in vanilla, 12 is min waiting time
            //end mod
            return false;
        }
    }
}