using System;
using ColossalFramework;
using UnityEngine;

namespace ImprovedPublicTransport2.Query
{
    public static class WaitingPassengerCountQuery
    {
        public static int Query(ushort currentStop, out ushort nextStop, out byte max)
        {
            nextStop = TransportLine.GetNextStop(currentStop);
            max = 0;
            if (currentStop == 0 || nextStop == 0)
            {
                return 0;
            }

            var instance1 = Singleton<CitizenManager>.instance;
            var instance2 = Singleton<NetManager>.instance;
            var position1 = instance2.m_nodes.m_buffer[currentStop].m_position;
            var position2 = instance2.m_nodes.m_buffer[nextStop].m_position;
            var num1 = Mathf.Max((int)((position1.x - 64.0) / 8.0 + 1080.0), 0);
            var num2 = Mathf.Max((int)((position1.z - 64.0) / 8.0 + 1080.0), 0);
            var num3 = Mathf.Min((int)((position1.x + 64.0) / 8.0 + 1080.0), 2159);
            var num4 = Mathf.Min((int)((position1.z + 64.0) / 8.0 + 1080.0), 2159);
            var num6 = 0;
            for (var index1 = num2; index1 <= num4; ++index1)
            {
                for (var index2 = num1; index2 <= num3; ++index2)
                {
                    var instanceID = instance1.m_citizenGrid[index1 * 2160 + index2];
                    var num7 = 0;
                    while (instanceID != 0)
                    {
                        int nextGridInstance = instance1.m_instances.m_buffer[instanceID].m_nextGridInstance;
                        if (
                            (instance1.m_instances.m_buffer[instanceID].m_flags &
                             CitizenInstance.Flags.WaitingTransport) != CitizenInstance.Flags.None &&
                            Vector3.SqrMagnitude((Vector3)instance1.m_instances.m_buffer[instanceID].m_targetPos -
                                                 position1) < 4096.0 && instance1.m_instances.m_buffer[instanceID].Info
                                .m_citizenAI.TransportArriveAtSource(instanceID,
                                    ref instance1.m_instances.m_buffer[instanceID], position1, position2))
                        {
                            var waitCounter = instance1.m_instances.m_buffer[instanceID].m_waitCounter;
                            max = Math.Max(waitCounter, max);
                            ++num6;
                        }

                        instanceID = (ushort)nextGridInstance;
                        if (++num7 > 65536)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core,
                                "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }

            return num6;
        }
    }
}