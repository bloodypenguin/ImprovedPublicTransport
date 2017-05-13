// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.StopListBox
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace ImprovedPublicTransport
{
  public class StopListBox : UIPanel
  {
    protected int _itemHeight = 27;
    protected float _nextKeyCombi = Time.time;
    protected StopListBoxRow[] _items = new StopListBoxRow[0];
    protected UIScrollablePanel _scrollablePanel;
    protected UIScrollbar _scrollbar;

    public UIFont Font { get; set; }

    public new float height
    {
      get
      {
        return this.m_Size.y;
      }
      set
      {
        this.size = new Vector2(this.m_Size.x, value);
        if ((UnityEngine.Object) this._scrollablePanel != (UnityEngine.Object) null)
          this._scrollablePanel.height = value;
        if (!((UnityEngine.Object) this._scrollbar != (UnityEngine.Object) null))
          return;
        this._scrollbar.parent.height = value;
        this._scrollbar.height = value;
        this._scrollbar.trackObject.height = value;
      }
    }

    public void ClearItems()
    {
      if (this._items.Length == 0)
        return;
      for (int index = 0; index < this._items.Length; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._items[index].gameObject);
      this._items = new StopListBoxRow[0];
      this._scrollablePanel.scrollPosition = new Vector2(0.0f, 0.0f);
    }

    public void AddItem(ushort stopID, int stopIndex)
    {
      StopListBoxRow[] stopListBoxRowArray = new StopListBoxRow[this._items.Length + 1];
      Array.Copy((Array) this._items, (Array) stopListBoxRowArray, this._items.Length);
      StopListBoxRow stopListBoxRow = this._scrollablePanel.AddUIComponent<StopListBoxRow>();
      if ((UnityEngine.Object) this.Font != (UnityEngine.Object) null)
        stopListBoxRow.Font = this.Font;
      stopListBoxRow.StopID = stopID;
      stopListBoxRow.StopIndex = stopIndex;
      stopListBoxRow.eventMouseDown += new MouseEventHandler(this.OnMouseDown);
      stopListBoxRowArray[this._items.Length] = stopListBoxRow;
      this._items = stopListBoxRowArray;
    }

    private void OnMouseDown(UIComponent component, UIMouseEventParameter p)
    {
      StopListBoxRow stopListBoxRow = component as StopListBoxRow;
      if (p.buttons == UIMouseButton.Left)
      {
        for (int index = 0; index < this._items.Length; ++index)
        {
          if ((UnityEngine.Object) this._items[index] != (UnityEngine.Object) stopListBoxRow)
            this._items[index].IsSelected = false;
          else
            stopListBoxRow.IsSelected = !stopListBoxRow.IsSelected;
        }
      }
      else
      {
        if (p.buttons != UIMouseButton.Right)
          return;
        InstanceID instanceId = new InstanceID();
        instanceId.NetNode = stopListBoxRow.StopID;
        ToolsModifierControl.cameraController.SetTarget(instanceId, ToolsModifierControl.cameraController.transform.position, Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift));
        NetNode netNode = Singleton<NetManager>.instance.m_nodes.m_buffer[(int) stopListBoxRow.StopID];
        PublicTransportStopWorldInfoPanel.instance.Show(netNode.m_position, instanceId);
        Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
      }
    }

    public static StopListBox Create(UIComponent parent)
    {
      return parent.AddUIComponent<StopListBox>();
    }

    public override void Start()
    {
      base.Start();
      this.autoLayoutDirection = LayoutDirection.Horizontal;
      this.autoLayoutStart = LayoutStart.TopLeft;
      this.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      this.autoLayout = true;
      this._scrollablePanel = this.AddUIComponent<UIScrollablePanel>();
      this._scrollablePanel.width = this.width - 10f;
      this._scrollablePanel.height = this.height;
      this._scrollablePanel.autoLayoutDirection = LayoutDirection.Vertical;
      this._scrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
      this._scrollablePanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      this._scrollablePanel.autoLayout = true;
      this._scrollablePanel.clipChildren = true;
      this._scrollablePanel.backgroundSprite = "UnlockingPanel";
      this._scrollablePanel.color = (Color32) Color.black;
      UIPanel uiPanel = this.AddUIComponent<UIPanel>();
      uiPanel.width = 10f;
      uiPanel.height = this.height;
      uiPanel.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      uiPanel.autoLayout = true;
      UIScrollbar scrollbar = uiPanel.AddUIComponent<UIScrollbar>();
      scrollbar.width = 10f;
      scrollbar.height = scrollbar.parent.height;
      scrollbar.orientation = UIOrientation.Vertical;
      scrollbar.pivot = UIPivotPoint.BottomLeft;
      scrollbar.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopRight);
      scrollbar.minValue = 0.0f;
      scrollbar.value = 0.0f;
      scrollbar.incrementAmount = 27f;
      this._scrollbar = scrollbar;
      UISlicedSprite uiSlicedSprite1 = scrollbar.AddUIComponent<UISlicedSprite>();
      uiSlicedSprite1.relativePosition = (Vector3) Vector2.zero;
      uiSlicedSprite1.autoSize = true;
      uiSlicedSprite1.size = uiSlicedSprite1.parent.size;
      uiSlicedSprite1.fillDirection = UIFillDirection.Vertical;
      uiSlicedSprite1.spriteName = "ScrollbarTrack";
      scrollbar.trackObject = (UIComponent) uiSlicedSprite1;
      UISlicedSprite uiSlicedSprite2 = uiSlicedSprite1.AddUIComponent<UISlicedSprite>();
      uiSlicedSprite2.relativePosition = (Vector3) Vector2.zero;
      uiSlicedSprite2.fillDirection = UIFillDirection.Vertical;
      uiSlicedSprite2.autoSize = true;
      uiSlicedSprite2.width = uiSlicedSprite2.parent.width - 4f;
      uiSlicedSprite2.spriteName = "ScrollbarThumb";
      scrollbar.thumbObject = (UIComponent) uiSlicedSprite2;
      this._scrollablePanel.verticalScrollbar = scrollbar;
      this._scrollablePanel.eventMouseWheel += (MouseEventHandler) ((component, param) => this._scrollablePanel.scrollPosition += new Vector2(0.0f, Mathf.Sign(param.wheelDelta) * -1f * scrollbar.incrementAmount));
      this.eventVisibilityChanged += (PropertyChangedEventHandler<bool>) ((component, visible) =>
      {
        if (visible)
          return;
        for (int index = 0; index < this._items.Length; ++index)
          this._items[index].IsSelected = false;
      });
    }
  }
}
