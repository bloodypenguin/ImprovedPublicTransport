// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.VehicleListBoxRow
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using System;
using System.Text;
using ImprovedPublicTransport2.OptionsFramework;
using UnityEngine;

namespace ImprovedPublicTransport2
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
      stringBuilder.AppendLine(this._prefab.Title);
      ItemClass.SubService subService = this._prefab.Info.GetSubService();
      if (subService == ItemClass.SubService.PublicTransportTaxi)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_CAPACITY_TAXI") + ": " + (object) this._prefab.TotalCapacity);
      else
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_CAPACITY") + ": " + (object) this._prefab.TotalCapacity);
      float num = (float) this._prefab.MaintenanceCost * 0.01f;
      string str1 = num.ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
      if (this._prefab.MaintenanceCost > 0)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_MAINTENANCE") + ": " + (object) this._prefab.MaintenanceCost + " (" + str1 + ")");
      num = (float) this._prefab.TicketPrice * 0.01f;
      string str2 = num.ToString(ColossalFramework.Globalization.Locale.Get("MONEY_FORMAT"), (IFormatProvider) LocaleManager.cultureInfo);
      if (subService == ItemClass.SubService.PublicTransportTaxi)
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_PRICE_PER_KILOMETER") + ": " + (object) this._prefab.TicketPrice + " (" + str2 + ")");
      else
        stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_TICKET_PRICE") + ": " + (object) this._prefab.TicketPrice + " (" + str2 + ")");
      stringBuilder.AppendLine(Localization.Get("VEHICLE_EDITOR_MAX_SPEED") + ": " + (object) this._prefab.MaxSpeed + " (" + (object) (this._prefab.MaxSpeed * 5) + " " + OptionsWrapper<Settings>.Options.SpeedString + ")");
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
