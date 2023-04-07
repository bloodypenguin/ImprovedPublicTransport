// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.DropDown
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2.UI.DontCryJustDieCommons
{
  public class DropDown : UIPanel
  {
    protected int _itemHeight = 27;
    protected int _maxVisibleItems = 8;
    protected bool _showPanel = true;
    protected Vector3 _listPosition = Vector3.zero;
    protected DropDownRow[] _items = new DropDownRow[0];
    protected UIButton _triggerButton;
    protected UIPanel _dropDownPanel;
    protected UIComponent _dropDownPanelAlignParent;
    protected UIScrollablePanel _scrollablePanel;
    protected UIPanel _scrollbarPanel;
    protected UIScrollbar _scrollbar;

    public UIComponent DropDownPanelAlignParent
    {
      set
      {
        this._dropDownPanelAlignParent = value;
      }
    }

    public UIFont Font { get; set; }

    public float ListWidth { get; set; }

    public Vector3 ListPosition
    {
      get
      {
        return this._listPosition;
      }
      set
      {
        this._listPosition = value;
      }
    }

    public bool ShowPanel
    {
      get
      {
        return this._showPanel;
      }
      set
      {
        this._showPanel = value;
      }
    }

    public int MaxVisibleItems
    {
      get
      {
        return this._maxVisibleItems;
      }
      set
      {
        this._maxVisibleItems = Math.Max(1, value);
      }
    }

    public int SelectedIndex
    {
      get
      {
        int num = -1;
        if (this._items == null || this._items.Length == 0)
          return num;
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
      set
      {
        if (this._items == null || this._items.Length == 0)
          return;
        this._items[value].IsSelected = true;
      }
    }

    public ushort SelectedItem
    {
      get
      {
        ushort num = 0;
        if (this._items == null || this._items.Length == 0)
          return num;
        for (int index = 0; index < this._items.Length; ++index)
        {
          DropDownRow dropDownRow = this._items[index];
          if (dropDownRow.IsSelected)
            num = dropDownRow.ID;
        }
        return num;
      }
      set
      {
        if (this._items == null || this._items.Length == 0 || (int) this.SelectedItem == (int) value)
          return;
        this.Text = "Default";
        for (int index = 0; index < this._items.Length; ++index)
        {
          DropDownRow dropDownRow = this._items[index];
          if ((int) dropDownRow.ID == (int) value)
          {
            dropDownRow.IsSelected = true;
            this.Text = dropDownRow.Text;
          }
          else
            dropDownRow.IsSelected = false;
        }
        this.OnSelectedItemChanged();
      }
    }

    public string Text
    {
      get
      {
        return this._triggerButton.text;
      }
      set
      {
        if (!this._showPanel)
        {
          this._triggerButton.text = "";
        }
        else
        {
          if (!(value != this._triggerButton.text))
            return;
          this._triggerButton.text = value;
        }
      }
    }

    public DropDownRow[] Items
    {
      get
      {
        return this._items;
      }
    }

    public event PropertyChangedEventHandler<ushort> eventSelectedItemChanged;

    public void ClearItems()
    {
      if (this._items == null)
        return;
      for (int index = 0; index < this._items.Length; ++index)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._items[index].gameObject);
      this._items = (DropDownRow[]) null;
      this.Text = "Default";
      this._scrollablePanel.scrollPosition = new Vector2(0.0f, 0.0f);
    }

    public void AddItem(ushort ID, Func<ushort, string> func)
    {
      if (this._items == null)
        this._items = new DropDownRow[0];
      DropDownRow[] dropDownRowArray = new DropDownRow[this._items.Length + 1];
      Array.Copy((Array) this._items, (Array) dropDownRowArray, this._items.Length);
      DropDownRow dropDownRow = this._scrollablePanel.AddUIComponent<DropDownRow>();
      if ((UnityEngine.Object) this.Font != (UnityEngine.Object) null)
        dropDownRow.Font = this.Font;
      dropDownRow.ID = ID;
      dropDownRow.IDToNameFunc = func;
      dropDownRow.eventClick += new MouseEventHandler(this.OnRowClick);
      dropDownRowArray[this._items.Length] = dropDownRow;
      this._items = dropDownRowArray;
    }

    public void AddItems(ushort[] IDs, Func<ushort, string> func)
    {
      List<DropDownRow> dropDownRowList = new List<DropDownRow>();
      foreach (ushort id in IDs)
      {
        DropDownRow dropDownRow = this._scrollablePanel.AddUIComponent<DropDownRow>();
        if ((UnityEngine.Object) this.Font != (UnityEngine.Object) null)
          dropDownRow.Font = this.Font;
        dropDownRow.ID = id;
        dropDownRow.IDToNameFunc = func;
        dropDownRow.eventClick += new MouseEventHandler(this.OnRowClick);
        dropDownRowList.Add(dropDownRow);
      }
      this._items = dropDownRowList.ToArray();
    }

    private void OnRowClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      for (int index = 0; index < this._items.Length; ++index)
        this._items[index].IsSelected = false;
      DropDownRow dropDownRow = component as DropDownRow;
      dropDownRow.IsSelected = !dropDownRow.IsSelected;
      this.Text = dropDownRow.Text;
      this._dropDownPanel.isVisible = false;
      this.OnSelectedItemChanged();
    }

    private void OnButtonClick(UIComponent component, UIMouseEventParameter eventParam)
    {
      if (this._items == null || this._items.Length == 0)
        return;
      if (this._dropDownPanel.isVisible)
      {
        this._dropDownPanel.Hide();
      }
      else
      {
        this._dropDownPanel.Show();
        int num1;
        float num2 = (float) (num1 = Math.Min(this._items.Length, this._maxVisibleItems)) * (float) this._itemHeight;
        this._dropDownPanel.height = num2;
        this._scrollablePanel.height = num2;
        int maxVisibleItems = this._maxVisibleItems;
        if (num1 < maxVisibleItems)
        {
          this._scrollablePanel.width = (double) this.ListWidth > 0.0 ? this.ListWidth : this.width;
          this.UpdateRowWidth();
          this._scrollbarPanel.Hide();
        }
        else
        {
          this._scrollablePanel.width = (float) (((double) this.ListWidth > 0.0 ? (double) this.ListWidth : (double) this.width) - 10.0);
          this.UpdateRowWidth();
          this._scrollbarPanel.Show();
          this.EnsureVisible(this.SelectedIndex);
        }
      }
    }

    private void UpdateRowWidth()
    {
      for (int index = 0; index < this._items.Length; ++index)
        this._items[index].width = this._scrollablePanel.width;
    }

    public void EnsureVisible(int index)
    {
      int num = index * this._itemHeight;
      if ((double) this._scrollbar.value > (double) num)
        this._scrollbar.value = (float) num;
      if ((double) this._scrollbar.value + (double) this._itemHeight >= (double) (num + this._itemHeight))
        return;
      this._scrollbar.value = (float) num - (float) this._itemHeight + (float) this._itemHeight;
    }

    private void CheckForPopupClose()
    {
      if ((UnityEngine.Object) this._dropDownPanel == (UnityEngine.Object) null || !Input.GetMouseButtonDown(0))
        return;
      Ray ray = this.GetCamera().ScreenPointToRay(Input.mousePosition);
      if (this._dropDownPanel.Raycast(ray) || this._triggerButton.Raycast(ray))
        return;
      this._dropDownPanel.Hide();
    }

    protected internal virtual void OnSelectedItemChanged()
    {
      // ISSUE: reference to a compiler-generated field
      if (this.eventSelectedItemChanged != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.eventSelectedItemChanged((UIComponent) this, this.SelectedItem);
      }
      this.InvokeUpward("OnSelectedItemChanged", (object) this.SelectedItem);
    }

    public static DropDown Create(UIComponent parent)
    {
      return parent.AddUIComponent<DropDown>();
    }

    public override void Update()
    {
      base.Update();
      if (this.isVisible)
      {
        if (this.SelectedIndex > -1)
          this.Text = this._items[this.SelectedIndex].Text;
        if (this._listPosition == Vector3.zero)
          this._dropDownPanel.absolutePosition = this.absolutePosition + new Vector3(0.0f, this.height);
        else
          this._dropDownPanel.absolutePosition = this._listPosition;
      }
      this.CheckForPopupClose();
    }

    public override void Start()
    {
      base.Start();
      if (this._showPanel)
      {
        this.backgroundSprite = "ButtonMenu";
        this.eventMouseEnter += (MouseEventHandler) ((component, param) => (component as DropDown).backgroundSprite = "ButtonMenuHovered");
        this.eventMouseLeave += (MouseEventHandler) ((component, param) => (component as DropDown).backgroundSprite = "ButtonMenu");
      }
      this.zOrder = 1;
      UIButton uiButton = this.AddUIComponent<UIButton>();
      uiButton.width = this.width;
      uiButton.height = this.height;
      uiButton.relativePosition = new Vector3(0.0f, 0.0f);
      if ((UnityEngine.Object) this.Font != (UnityEngine.Object) null)
        uiButton.font = this.Font;
      uiButton.textScale = 0.8f;
      uiButton.textVerticalAlignment = UIVerticalAlignment.Middle;
      uiButton.textHorizontalAlignment = UIHorizontalAlignment.Left;
      uiButton.textPadding = new RectOffset(6, 0, 4, 0);
      uiButton.normalFgSprite = "IconDownArrow";
      uiButton.hoveredFgSprite = "IconDownArrowHovered";
      uiButton.pressedFgSprite = "IconDownArrowPressed";
      uiButton.focusedFgSprite = "IconDownArrow";
      uiButton.disabledFgSprite = "IconDownArrowDisabled";
      uiButton.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
      uiButton.horizontalAlignment = UIHorizontalAlignment.Right;
      uiButton.verticalAlignment = UIVerticalAlignment.Middle;
      uiButton.zOrder = 0;
      uiButton.eventClick += new MouseEventHandler(this.OnButtonClick);
      this._triggerButton = uiButton;
      this.Text = "Default";
      UIPanel uiPanel1 = this.AddUIComponent(typeof (UIPanel)) as UIPanel;
      uiPanel1.name = "PopUpPanel";
      uiPanel1.isVisible = false;
      uiPanel1.width = (double) this.ListWidth > 0.0 ? this.ListWidth : this.width;
      uiPanel1.height = (float) (this._itemHeight * this._maxVisibleItems);
      uiPanel1.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel1.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel1.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      uiPanel1.autoLayout = true;
      uiPanel1.backgroundSprite = "GenericPanelWhite";
      uiPanel1.color = (Color32) Color.black;
      uiPanel1.AlignTo(this._dropDownPanelAlignParent, UIAlignAnchor.TopLeft);
      this._dropDownPanel = uiPanel1;
      UIScrollablePanel uiScrollablePanel = uiPanel1.AddUIComponent<UIScrollablePanel>();
      uiScrollablePanel.width = uiPanel1.width - 10f;
      uiScrollablePanel.height = uiPanel1.height;
      uiScrollablePanel.autoLayoutDirection = LayoutDirection.Vertical;
      uiScrollablePanel.autoLayoutStart = LayoutStart.TopLeft;
      uiScrollablePanel.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      uiScrollablePanel.autoLayout = true;
      uiScrollablePanel.clipChildren = true;
      uiScrollablePanel.backgroundSprite = "GenericPanelWhite";
      uiScrollablePanel.color = (Color32) Color.black;
      this._scrollablePanel = uiScrollablePanel;
      UIPanel uiPanel2 = uiPanel1.AddUIComponent<UIPanel>();
      uiPanel2.width = 10f;
      uiPanel2.height = uiPanel2.parent.height;
      uiPanel2.autoLayoutDirection = LayoutDirection.Horizontal;
      uiPanel2.autoLayoutStart = LayoutStart.TopLeft;
      uiPanel2.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
      uiPanel2.autoLayout = true;
      this._scrollbarPanel = uiPanel2;
      UIScrollbar scrollbar = uiPanel2.AddUIComponent<UIScrollbar>();
      scrollbar.width = 10f;
      scrollbar.height = uiPanel2.height;
      scrollbar.orientation = UIOrientation.Vertical;
      scrollbar.pivot = UIPivotPoint.BottomLeft;
      scrollbar.AlignTo((UIComponent) uiPanel2, UIAlignAnchor.TopRight);
      scrollbar.minValue = 0.0f;
      scrollbar.value = 0.0f;
      scrollbar.incrementAmount = (float) this._itemHeight;
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

    public override void OnDestroy()
    {
      if ((UnityEngine.Object) this._triggerButton != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._triggerButton.gameObject);
      if ((UnityEngine.Object) this._dropDownPanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._dropDownPanel.gameObject);
      if ((UnityEngine.Object) this._scrollablePanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._scrollablePanel.gameObject);
      if ((UnityEngine.Object) this._scrollbarPanel != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._scrollbarPanel.gameObject);
      if ((UnityEngine.Object) this._scrollbar != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._scrollbar.gameObject);
      base.OnDestroy();
    }
  }
}
