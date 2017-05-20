// Decompiled with JetBrains decompiler
// Type: ImprovedPublicTransport.UIHelper
// Assembly: ImprovedPublicTransport, Version=1.0.6177.17409, Culture=neutral, PublicKeyToken=null
// MVID: 76F370C5-F40B-41AE-AA9D-1E3F87E934D3
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\424106600\ImprovedPublicTransport.dll

using ColossalFramework.UI;
using UnityEngine;

namespace ImprovedPublicTransport2
{
  public static class UIUtils
  {
    private static UIFont _font;

    public static UIFont Font
    {
      get
      {
        if ((Object) UIUtils._font == (Object) null)
          UIUtils._font = GameObject.Find("(Library) PublicTransportInfoViewPanel").GetComponent<PublicTransportInfoViewPanel>().Find<UILabel>("Label").font;
        return UIUtils._font;
      }
    }

    public static bool IsFullyClippedFromParent(UIComponent component)
    {
      if ((Object) component.parent == (Object) null || (Object) component.parent == (Object) component)
        return false;
      UIScrollablePanel parent = component.parent as UIScrollablePanel;
      return (Object) parent != (Object) null && parent.clipChildren && ((double) component.relativePosition.x < 0.0 - (double) component.size.x - 1.0 || (double) component.relativePosition.x + (double) component.size.x > (double) component.parent.size.x + (double) component.size.x + 1.0 || ((double) component.relativePosition.y < 0.0 - (double) component.size.y - 1.0 || (double) component.relativePosition.y + (double) component.size.y > (double) component.parent.size.y + (double) component.size.y + 1.0));
    }

    public static UIButton CreateButton(UIComponent parent)
    {
      UIButton uiButton = parent.AddUIComponent<UIButton>();
      UIFont font = UIUtils.Font;
      uiButton.font = font;
      RectOffset rectOffset = new RectOffset(0, 0, 4, 0);
      uiButton.textPadding = rectOffset;
      string str1 = "ButtonMenu";
      uiButton.normalBgSprite = str1;
      string str2 = "ButtonMenuDisabled";
      uiButton.disabledBgSprite = str2;
      string str3 = "ButtonMenuHovered";
      uiButton.hoveredBgSprite = str3;
      string str4 = "ButtonMenu";
      uiButton.focusedBgSprite = str4;
      string str5 = "ButtonMenuPressed";
      uiButton.pressedBgSprite = str5;
      Color32 color32_1 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      uiButton.textColor = color32_1;
      Color32 color32_2 = new Color32((byte) 7, (byte) 7, (byte) 7, byte.MaxValue);
      uiButton.disabledTextColor = color32_2;
      Color32 color32_3 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      uiButton.hoveredTextColor = color32_3;
      Color32 color32_4 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      uiButton.focusedTextColor = color32_4;
      Color32 color32_5 = new Color32((byte) 30, (byte) 30, (byte) 44, byte.MaxValue);
      uiButton.pressedTextColor = color32_5;
      return uiButton;
    }

    public static UICheckBox CreateCheckBox(UIComponent parent)
    {
      UICheckBox uiCheckBox = parent.AddUIComponent<UICheckBox>();
      Vector2 size = parent.size;
      uiCheckBox.size = size;
      int num = 1;
      uiCheckBox.clipChildren = num != 0;
      UISprite uiSprite1 = uiCheckBox.AddUIComponent<UISprite>();
      uiSprite1.spriteName = "check-unchecked";
      uiSprite1.size = new Vector2(16f, 16f);
      uiSprite1.relativePosition = Vector3.zero;
      UISprite uiSprite2 = uiSprite1.AddUIComponent<UISprite>();
      uiCheckBox.checkedBoxObject = (UIComponent) uiSprite2;
      ((UISprite) uiCheckBox.checkedBoxObject).spriteName = "check-checked";
      uiCheckBox.checkedBoxObject.size = new Vector2(16f, 16f);
      uiCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
      UILabel uiLabel = uiCheckBox.AddUIComponent<UILabel>();
      uiCheckBox.label = uiLabel;
      uiCheckBox.label.font = UIUtils.Font;
      uiCheckBox.label.textColor = (Color32) Color.white;
      uiCheckBox.label.textScale = 0.8f;
      uiCheckBox.label.relativePosition = new Vector3(22f, 2f);
      return uiCheckBox;
    }
  }
}
