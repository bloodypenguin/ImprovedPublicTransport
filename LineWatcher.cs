// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.LineWatcher
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using System.Collections.Generic;
using ImprovedPublicTransport2.Detour;
using ImprovedPublicTransport2.OptionsFramework;
using UnityEngine;

namespace ImprovedPublicTransport2
{
  public class LineWatcher : MonoBehaviour
  {
    private HashSet<ushort> _knownLines = new HashSet<ushort>();
    public static LineWatcher instance;
    private bool _initialized;

    public int KnownLineCount
    {
      get
      {
        return this._knownLines.Count;
      }
    }

    public int SimulationLineCount
    {
      get
      {
        return Mathf.Max(0, Singleton<TransportManager>.instance.m_lineCount - 1);
      }
    }

    private void Awake()
    {
      LineWatcher.instance = this;
    }

    private void Update()
    {
      if (!this._initialized)
        return;
      if (this.SimulationLineCount > this.KnownLineCount)
      {
        Array16<TransportLine> lines = Singleton<TransportManager>.instance.m_lines;
        for (ushort lineID = 0; (uint) lineID < lines.m_size; ++lineID)
        {
          if (LineWatcher.IsValid(ref lines.m_buffer[(int) lineID]) && this._knownLines.Add(lineID))
          {
            TransportLineMod.SetLineDefaults(lineID);
            Vector3 position = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) lines.m_buffer[(int) lineID].GetStop(0)].m_position;
            ushort closestDepot = TransportLineMod.GetClosestDepot(lineID, position);
            if ((int) closestDepot != 0)
              TransportLineMod.SetDepot(lineID, closestDepot);
            if (OptionsWrapper<Settings>.Options.ShowLineInfo && lines.m_buffer[(int)lineID].Info?.m_class?.m_service != ItemClass.Service.Disaster)
              WorldInfoPanel.Show<PublicTransportWorldInfoPanel>(position, new InstanceID()
              {
                TransportLine = lineID
              });
          }
        }
      }
      else
      {
        if (this.SimulationLineCount >= this.KnownLineCount)
          return;
        Array16<TransportLine> lines = Singleton<TransportManager>.instance.m_lines;
        for (ushort index = 0; (uint) index < lines.m_size; ++index)
        {
          if (!LineWatcher.IsValid(ref lines.m_buffer[(int) index]))
            this._knownLines.Remove(index);
        }
      }
    }

    private void OnDestroy()
    {
      LineWatcher.instance = (LineWatcher) null;
    }

    public void Init()
    {
      Array16<TransportLine> lines = Singleton<TransportManager>.instance.m_lines;
      for (ushort index = 0; (uint) index < lines.m_size; ++index)
      {
        if (LineWatcher.IsValid(ref lines.m_buffer[(int) index]))
          this._knownLines.Add(index);
      }
      this._initialized = true;
    }

    private static bool IsValid(ref TransportLine line)
    {
      if ((line.m_flags & TransportLine.Flags.Complete) != TransportLine.Flags.None)
        return (line.m_flags & TransportLine.Flags.Temporary) == TransportLine.Flags.None;
      return false;
    }
  }
}
