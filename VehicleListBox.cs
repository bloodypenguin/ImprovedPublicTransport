// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleListBox
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImprovedPublicTransport2
{
  public class VehicleListBox : UIPanel
  {
    protected int _itemHeight = 27;
    protected float _nextKeyCombi = Time.time;
    protected VehicleListBoxRow[] _items = new VehicleListBoxRow[0];
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

    public int SelectedIndex
    {
      get
      {
        int num = -1;
        for (int index = 0; index < this._items.Length; ++index)
        {
          if (this._items[index].IsSelected)
          {
            num = index;
            break;
          }
        }
        return num;
      }
    }

    public int[] SelectedIndexes
    {
      get
      {
        List<int> intList = new List<int>();
        for (int index = 0; index < this._items.Length; ++index)
        {
          if (this._items[index].IsSelected)
            intList.Add(index);
        }
        return intList.ToArray();
      }
    }

    public HashSet<ushort> SelectedVehicles
    {
      get
      {
        HashSet<ushort> ushortSet = new HashSet<ushort>();
        for (int index = 0; index < this._items.Length; ++index)
        {
          VehicleListBoxRow vehicleListBoxRow = this._items[index];
          if (vehicleListBoxRow.IsSelected)
            ushortSet.Add(vehicleListBoxRow.VehicleID);
        }
        return ushortSet;
      }
    }

    public HashSet<string> SelectedItems
    {
      get
      {
        HashSet<string> stringSet = new HashSet<string>();
        for (int index = 0; index < this._items.Length; ++index)
        {
          VehicleListBoxRow vehicleListBoxRow = this._items[index];
          if (vehicleListBoxRow.IsSelected)
            stringSet.Add(vehicleListBoxRow.Prefab.ObjectName);
        }
        return stringSet;
      }
      set
      {
        if (value == null || this.SelectedItems == value)
          return;
        for (int index = 0; index < this._items.Length; ++index)
        {
          VehicleListBoxRow vehicleListBoxRow = this._items[index];
          if (value.Contains(vehicleListBoxRow.Prefab.ObjectName))
            vehicleListBoxRow.IsSelected = true;
        }
      }
    }

    public HashSet<string> Items
    {
      get
      {
        HashSet<string> stringSet = new HashSet<string>();
        for (int index = 0; index < this._items.Length; ++index)
        {
          VehicleListBoxRow vehicleListBoxRow = this._items[index];
          stringSet.Add(vehicleListBoxRow.Prefab.ObjectName);
        }
        return stringSet;
      }
    }

    public event MouseEventHandler eventRowShiftClick;

    public event PropertyChangedEventHandler<HashSet<string>> eventSelectedItemsChanged;

    private bool AllSelected()
    {
      for (int index = 0; index < this._items.Length; ++index)
      {
        if (!this._items[index].IsSelected)
          return false;
      }
      return true;
    }

    public void SetSelectionStateToAll(bool state, bool triggerItemsChangedEvent = false)
    {
      for (int index = 0; index < this._items.Length; ++index)
        this._items[index].IsSelected = state;
      if (!triggerItemsChangedEvent)
        return;
      this.OnSelectedItemsChanged();
    }

    public void ClearItems()
    {
      if (this._items.Length == 0)
        return;
      for (int index = 0; index < this._items.Length; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._items[index].gameObject);
      this._items = new VehicleListBoxRow[0];
      this._scrollablePanel.scrollPosition = new Vector2(0.0f, 0.0f);
    }

    public void AddItem(PrefabData data, ushort vehicleID = 0)
    {
      VehicleListBoxRow[] vehicleListBoxRowArray = new VehicleListBoxRow[this._items.Length + 1];
      Array.Copy((Array) this._items, (Array) vehicleListBoxRowArray, this._items.Length);
      VehicleListBoxRow vehicleListBoxRow = this._scrollablePanel.AddUIComponent<VehicleListBoxRow>();
      if ((UnityEngine.Object) this.Font != (UnityEngine.Object) null)
        vehicleListBoxRow.Font = this.Font;
      vehicleListBoxRow.Prefab = data;
      vehicleListBoxRow.VehicleID = vehicleID;
      vehicleListBoxRow.eventMouseDown += new MouseEventHandler(this.OnMouseDown);
      vehicleListBoxRowArray[this._items.Length] = vehicleListBoxRow;
      this._items = vehicleListBoxRowArray;
    }

    private void OnMouseDown(UIComponent component, UIMouseEventParameter p)
    {
      VehicleListBoxRow vehicleListBoxRow = component as VehicleListBoxRow;
      bool zoomIn = Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift);
      if ((int) vehicleListBoxRow.VehicleID != 0 && p.buttons == UIMouseButton.Right)
      {
        InstanceID id = new InstanceID();
        id.Vehicle = vehicleListBoxRow.VehicleID;
        ToolsModifierControl.cameraController.SetTarget(id, ToolsModifierControl.cameraController.transform.position, zoomIn);
        DefaultTool.OpenWorldInfoPanel(id, ToolsModifierControl.cameraController.transform.position);
      }
      else
      {
        if (p.buttons != UIMouseButton.Left)
          return;
        // ISSUE: reference to a compiler-generated field
        if (this.eventRowShiftClick != null && zoomIn)
        {
          this.PlayClickSound(p.source);
          // ISSUE: reference to a compiler-generated field
          this.eventRowShiftClick(component, p);
          if (vehicleListBoxRow.IsSelected)
            return;
        }
        vehicleListBoxRow.IsSelected = !vehicleListBoxRow.IsSelected;
        this.OnSelectedItemsChanged();
      }
    }

    protected override void OnMouseHover(UIMouseEventParameter p)
    {
      base.OnMouseHover(p);
      if (!((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) & Input.GetKey(KeyCode.A)) || (double) this._nextKeyCombi >= (double) Time.time)
        return;
      this._nextKeyCombi = Time.time + 0.25f;
      if (this.AllSelected())
        this.SetSelectionStateToAll(false, true);
      else
        this.SetSelectionStateToAll(true, true);
    }

    protected internal virtual void OnSelectedItemsChanged()
    {
      // ISSUE: reference to a compiler-generated field
      if (this.eventSelectedItemsChanged != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.eventSelectedItemsChanged((UIComponent) this, this.SelectedItems);
      }
      this.InvokeUpward("OnSelectedItemsChanged", (object) this.SelectedItems);
    }

    public static VehicleListBox Create(UIComponent parent)
    {
      return parent.AddUIComponent<VehicleListBox>();
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
    }
  }
}
