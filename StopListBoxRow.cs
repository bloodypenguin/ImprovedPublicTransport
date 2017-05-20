// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.StopListBoxRow
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.UI;
using ImprovedPublicTransport2.Detour;
using UnityEngine;

namespace ImprovedPublicTransport2
{
  public class StopListBoxRow : UIPanel
  {
    private float _delta = 1f;
    private string _cachedName = "";
    private UILabel _stopName;
    private UILabel _stopCount;
    private bool _isSelected;
    private InstanceID _instanceID;

    public UIFont Font { get; set; }

    public ushort StopID
    {
      get
      {
        return this._instanceID.NetNode;
      }
      set
      {
        this._instanceID.NetNode = value;
      }
    }

    public int StopIndex { get; set; }

    private ushort NextStopID
    {
      get
      {
        if ((int) this.StopID != 0)
          return TransportLine.GetNextStop(this.StopID);
        return 0;
      }
    }

    public bool IsSelected
    {
      get
      {
        return this._isSelected;
      }
      set
      {
        this._isSelected = value;
        if (this._isSelected)
          this.backgroundSprite = "ListItemHighlight";
        else
          this.backgroundSprite = "";
      }
    }

    public static string GenerateStopName(string name, ushort node, int stopIndex)
    {
      string str;
      if (string.IsNullOrEmpty(name))
      {
        if (stopIndex == -1)
        {
          ushort transportLine = Singleton<NetManager>.instance.m_nodes.m_buffer[node].m_transportLine;
          str = string.Format("{0} #{1}", (object) Singleton<TransportManager>.instance.GetLineName(transportLine), (object) (TransportLineMod.GetStopIndex(transportLine, node) + 1));
        }
        else
          str = string.Format(Localization.Get("STOP_LIST_BOX_ROW_STOP"), (object)stopIndex);
      }
      else
        str = name;
      return str;
    }

    protected override void OnMouseEnter(UIMouseEventParameter p)
    {
      byte max;
      this.tooltip = string.Format(Localization.Get("STOP_LIST_BOX_ROW_TOOLTIP"), (object) GenerateStopName(Singleton<InstanceManager>.instance.GetName(this._instanceID), this._instanceID.NetNode, this.StopIndex), (object) PanelExtenderLine.CountWaitingPassengers(this.StopID, this.NextStopID, out max));
      if (!this.IsSelected)
        this.backgroundSprite = "ListItemHover";
      base.OnMouseEnter(p);
    }

    protected override void OnMouseLeave(UIMouseEventParameter p)
    {
      if (!this.IsSelected)
        this.backgroundSprite = "";
      base.OnMouseLeave(p);
    }

    public override void Update()
    {
      base.Update();
      if (!this.isVisible || (int) this.StopID == 0)
        return;
      string name = Singleton<InstanceManager>.instance.GetName(this._instanceID);
      if (this._cachedName != name)
      {
        this._cachedName = name;
        Utils.Truncate(this._stopName, GenerateStopName(name, this._instanceID.NetNode, this.StopIndex));
      }
      if ((double) this._delta >= 1.0)
      {
        this._delta = 0.0f;
        byte max;
        this._stopCount.text = PanelExtenderLine.CountWaitingPassengers(this.StopID, this.NextStopID, out max).ToString();
      }
      this._delta = this._delta + Singleton<SimulationManager>.instance.m_simulationTimeDelta;
    }

    public override void Start()
    {
      base.Start();
      this.width = this.parent.width;
      this.height = 27f;
      this.autoLayoutDirection = LayoutDirection.Horizontal;
      this.autoLayoutStart = LayoutStart.TopLeft;
      this.autoLayoutPadding = new RectOffset(4, 0, 0, 0);
      this.autoLayout = true;
      this._stopName = this.AddUIComponent<UILabel>();
      this._stopName.textScale = 0.8f;
      this._stopName.font = this.Font;
      this._stopName.autoSize = false;
      this._stopName.height = this.height;
      this._stopName.width = this.width * 0.7f - (float) (this.autoLayoutPadding.left * 2);
      this._stopName.verticalAlignment = UIVerticalAlignment.Middle;
      this._stopCount = this.AddUIComponent<UILabel>();
      this._stopCount.textScale = 0.8f;
      this._stopCount.font = this.Font;
      this._stopCount.autoSize = false;
      this._stopCount.height = this.height;
      this._stopCount.width = this.width * 0.3f - (float) (this.autoLayoutPadding.left * 2);
      this._stopCount.verticalAlignment = UIVerticalAlignment.Middle;
      this._stopCount.textAlignment = UIHorizontalAlignment.Right;
    }

    public override void OnDestroy()
    {
      if ((Object) this._stopName != (Object) null)
        Object.Destroy((Object) this._stopName.gameObject);
      if ((Object) this._stopCount != (Object) null)
        Object.Destroy((Object) this._stopCount.gameObject);
      base.OnDestroy();
    }
  }
}
