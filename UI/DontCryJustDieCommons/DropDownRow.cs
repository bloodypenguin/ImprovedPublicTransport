// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.DropDownRow
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using ColossalFramework.UI;
using ImprovedPublicTransport2.Util;
using UnityEngine;
using UIUtils = ImprovedPublicTransport2.Util.UIUtils;

namespace ImprovedPublicTransport2.UI.DontCryJustDieCommons
{
  public class DropDownRow : UIPanel
  {
    private bool _init;
    private UILabel _label;
    private ushort _ID;
    private bool _isSelected;
    private Func<ushort, string> _idToNameFunc;

    public UIFont Font { get; set; }

    public Func<ushort, string> IDToNameFunc
    {
      set
      {
        this._idToNameFunc = value;
      }
    }

    public ushort ID
    {
      get
      {
        return this._ID;
      }
      set
      {
        this._ID = value;
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

    public string Text
    {
      get
      {
        if (!this._init)
          this.Start();
        string text = this._idToNameFunc(this._ID);
        if (Utils.Truncate(this._label, text, "…"))
          this.tooltip = text;
        else
          this.tooltip = "";
        return this._label.text;
      }
    }

    protected override void OnMouseEnter(UIMouseEventParameter p)
    {
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
      if (this.isVisible && !UIUtils.IsFullyClippedFromParent((UIComponent) this))
        this._label.text = this.Text;
      base.Update();
    }

    public override void Start()
    {
      if (this._init)
        return;
      this._init = true;
      base.Start();
      this.width = this.parent.width - 10f;
      this.height = 27f;
      this.autoLayoutDirection = LayoutDirection.Horizontal;
      this.autoLayoutStart = LayoutStart.TopLeft;
      this.autoLayoutPadding = new RectOffset(4, 0, 0, 0);
      this.autoLayout = true;
      this._label = this.AddUIComponent<UILabel>();
      this._label.text = "";
      this._label.textScale = 0.8f;
      this._label.font = this.Font;
      this._label.autoSize = false;
      this._label.height = this.height;
      this._label.width = this.width - (float) this.autoLayoutPadding.left;
      this._label.verticalAlignment = UIVerticalAlignment.Middle;
    }

    public override void OnDestroy()
    {
      if ((UnityEngine.Object) this._label != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._label.gameObject);
      base.OnDestroy();
    }
  }
}
