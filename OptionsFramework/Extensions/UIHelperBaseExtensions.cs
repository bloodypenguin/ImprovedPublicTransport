using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ColossalFramework.UI;
using ImprovedPublicTransport2.OptionsFramework.Attibutes;
using ICities;
using UnityEngine;

namespace ImprovedPublicTransport2.OptionsFramework.Extensions
{
    public static class UIHelperBaseExtensions
    {
        public static IEnumerable<UIComponent> AddOptionsGroup<T>(this UIHelperBase helper, Func<string, string> translator = null)
        {
            var result = new List<UIComponent>();
            var properties = from property in typeof(T).GetProperties().Where(p =>
            {
                var attributes =
                    (AbstractOptionsAttribute[])p.GetCustomAttributes(typeof(AbstractOptionsAttribute), false);
                return attributes.Length > 0;
            }) select property.Name;
            var groups = new Dictionary<string, UIHelperBase>();
            foreach (var propertyName in properties.ToArray())
            {
                var description = OptionsWrapper<T>.Options.GetPropertyDescription(propertyName);
                var groupName = OptionsWrapper<T>.Options.GetPropertyGroup(propertyName);
                if (groupName == null)
                {
                    var component = helper.ProcessProperty<T>(propertyName, description, translator);
                    if (component != null)
                    {
                        result.Add(component);
                    }
                }
                else
                {
                    if (translator != null)
                    {
                        groupName = translator.Invoke(groupName);
                    }
                    if (!groups.ContainsKey(groupName))
                    {
                        groups[groupName] = helper.AddGroup(groupName);
                    }
                    var component = groups[groupName].ProcessProperty<T>(propertyName, description, translator);
                    if (component != null)
                    {
                        result.Add(component);
                    }
                }
            }
            return result;
        }

        private static UIComponent ProcessProperty<T>(this UIHelperBase group, string propertyName, string description, Func<string, string> translator = null)
        {
            if (translator != null)
            {
                description = translator.Invoke(description);
            }

            var checkboxAttribute = OptionsWrapper<T>.Options.GetAttribute<T, CheckboxAttribute>(propertyName);
            if (checkboxAttribute != null)
            {
                return group.AddCheckbox<T>(description, propertyName, checkboxAttribute);
            }
            var textfieldAttribute = OptionsWrapper<T>.Options.GetAttribute<T, TextfieldAttribute>(propertyName);
            if (textfieldAttribute != null)
            {
                return group.AddTextfield<T>(description, propertyName, textfieldAttribute);
            }
            var dropDownAttribute = OptionsWrapper<T>.Options.GetAttribute<T, DropDownAttribute>(propertyName);
            if (dropDownAttribute != null)
            {
                return group.AddDropdown<T>(description, propertyName, dropDownAttribute, translator);
            }
            var sliderAttribute = OptionsWrapper<T>.Options.GetAttribute<T, SliderAttribute>(propertyName);
            if (sliderAttribute != null)
            {
                return group.AddSlider<T>(description, propertyName, sliderAttribute);
            }
            var buttonAttribute = OptionsWrapper<T>.Options.GetAttribute<T, ButtonAttribute>(propertyName);
            if (buttonAttribute != null)
            {
                return group.AddButton<T>(description, propertyName, buttonAttribute);
            }
            //TODO: more control types
            return null;
        }

        private static UIDropDown AddDropdown<T>(this UIHelperBase group, string text, string propertyName, DropDownAttribute attr, Func<string, string> translator = null)
        {
            var property = typeof(T).GetProperty(propertyName);
            var defaultCode = (int)property.GetValue(OptionsWrapper<T>.Options, null);
            int defaultSelection;
            var items = attr.GetItems(translator);
            try
            {
                defaultSelection = items.First(kvp => kvp.Value == defaultCode).Value;
            }
            catch
            {
                defaultSelection = 0;
                property.SetValue(OptionsWrapper<T>.Options, items.First().Value, null);
            }
            return (UIDropDown)group.AddDropdown(text, items.Select(kvp => kvp.Key).ToArray(), defaultSelection, sel =>
           {
               var code = items[sel].Value;
               property.SetValue(OptionsWrapper<T>.Options, code, null);
               OptionsWrapper<T>.SaveOptions();
               attr.Action<int>().Invoke(code);
           });
        }

        private static UICheckBox AddCheckbox<T>(this UIHelperBase group, string text, string propertyName, CheckboxAttribute attr)
        {
            var property = typeof(T).GetProperty(propertyName);
            return (UICheckBox)group.AddCheckbox(text, (bool)property.GetValue(OptionsWrapper<T>.Options, null),
                b =>
                {
                    property.SetValue(OptionsWrapper<T>.Options, b, null);
                    OptionsWrapper<T>.SaveOptions();
                    attr.Action<bool>().Invoke(b);
                });
        }

        private static UIButton AddButton<T>(this UIHelperBase group, string text, string propertyName, ButtonAttribute attr)
        {
            return (UIButton)group.AddButton(text, ()=> 
                {
                    attr.Action().Invoke();
                });
        }

        private static UITextField AddTextfield<T>(this UIHelperBase group, string text, string propertyName, TextfieldAttribute attr)
        {
            var property = typeof(T).GetProperty(propertyName);
            var initialValue = Convert.ToString(property.GetValue(OptionsWrapper<T>.Options, null));
            return (UITextField)group.AddTextfield(text, initialValue, s => { },
                s =>
                {
                    object value;
                    if (property.PropertyType == typeof(int))
                    {
                        value = Convert.ToInt32(s);
                    }
                    else if (property.PropertyType == typeof(short))
                    {
                        value = Convert.ToInt16(s);
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        value = Convert.ToDouble(s);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        value = Convert.ToSingle(s);
                    }
                    else
                    {
                        value = s; //TODO: more types
                    }
                    property.SetValue(OptionsWrapper<T>.Options, value, null);
                    OptionsWrapper<T>.SaveOptions();
                    attr.Action<string>().Invoke(s);
                });
        }

        private static UISlider AddSlider<T>(this UIHelperBase group, string text, string propertyName, SliderAttribute attr)
        {
            var property = typeof(T).GetProperty(propertyName);
            UILabel valueLabel = null;

            var helper = group as UIHelper;
            if (helper != null)
            {
                var type = typeof(UIHelper).GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                if (type != null)
                {
                    var panel = type.GetValue(helper) as UIComponent;
                    valueLabel = panel?.AddUIComponent<UILabel>();
                }
            }


            var slider = (UISlider)group.AddSlider(text, attr.Min, attr.Max, attr.Step, (float)property.GetValue(OptionsWrapper<T>.Options, null),
                f =>
                {
                    property.SetValue(OptionsWrapper<T>.Options, f, null);
                    OptionsWrapper<T>.SaveOptions();
                    attr.Action<float>().Invoke(f);
                    if (valueLabel != null)
                    {
                        valueLabel.text = f.ToString(CultureInfo.InvariantCulture);
                    }
                });
            var nameLabel = slider.parent.Find<UILabel>("Label");
            if (nameLabel != null)
            {
                nameLabel.width = nameLabel.textScale * nameLabel.font.size * nameLabel.text.Length;
            }
            if (valueLabel == null)
            {
                return slider;
            }
            valueLabel.AlignTo(slider, UIAlignAnchor.TopLeft);
            valueLabel.relativePosition = new Vector3(240, 0, 0);
            valueLabel.text = property.GetValue(OptionsWrapper<T>.Options, null).ToString();
            return slider;
        }
    }
}