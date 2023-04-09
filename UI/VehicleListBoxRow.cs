﻿// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleListBoxRow
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using System;
using System.Text;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ImprovedPublicTransport2.OptionsFramework;
using ImprovedPublicTransport2.Data;
using UnityEngine;
using Utils = ImprovedPublicTransport2.Util.Utils;

namespace ImprovedPublicTransport2.UI
{
  public class VehicleListBoxRow : UIPanel
  {
    private UILabel _label;
    private PrefabData _prefab;
    private string _cachedName;
    private bool _isSelected;

    public UIFont Font { get; set; }

    public PrefabData Prefab
    {
      get
      {
        return this._prefab;
      }
      set
      {
        this._prefab = value;
      }
    }

    public ushort VehicleID { get; set; }

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

    protected override void OnMouseEnter(UIMouseEventParameter p)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(_prefab.GetDescription());
      stringBuilder.AppendLine();
      if ((int) this.VehicleID != 0)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_LIST_BOX_ROW_TOOLTIP1"));
      else
        stringBuilder.AppendLine(Localization.Get("VEHICLE_LIST_BOX_ROW_TOOLTIP2"));
      this.tooltip = stringBuilder.ToString();
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
      if (!this.isVisible || (int) this.VehicleID == 0)
        return;
      string vehicleName = Singleton<VehicleManager>.instance.GetVehicleName(this.VehicleID);
      if (!(this._cachedName != vehicleName))
        return;
      this._cachedName = vehicleName;
      Utils.Truncate(this._label, vehicleName, "…");
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
      this._label = this.AddUIComponent<UILabel>();
      this._label.textScale = 0.8f;
      this._label.font = this.Font;
      this._label.autoSize = false;
      this._label.height = this.height;
      this._label.width = this.width - (float) this.autoLayoutPadding.left;
      this._label.verticalAlignment = UIVerticalAlignment.Middle;
      Utils.Truncate(this._label, this._prefab.Title, "…");
    }

    public override void OnDestroy()
    {
      if ((UnityEngine.Object) this._label != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this._label.gameObject);
      base.OnDestroy();
    }
  }
}
