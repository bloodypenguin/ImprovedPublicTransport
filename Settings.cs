using System;
using System.ComponentModel;
using System.Xml.Serialization;
using ImprovedPublicTransport2.OptionsFramework.Attibutes;

namespace ImprovedPublicTransport2
{
    [Options("ImprovedPublicTransport2", "ImprovedPublicTransportSettings")]
    public class Settings
    {
        private const string UI = "SETTINGS_UI";
        private const string SETTINGS_BUDGET = "SETTINGS_ENABLE_BUDGET_CONTROL"; //TODO(earalov): add new locale

        [BudgetDescription]
        [Checkbox("SETTINGS_ENABLE_BUDGET_CONTROL", SETTINGS_BUDGET, nameof(SettingsActions), nameof(SettingsActions.OnBudgetCheckChanged))]
        public bool BudgetControl { get; set; } = true;

        [BudgetDescription]
        [Button("SETTINGS_UPDATE", SETTINGS_BUDGET, nameof(SettingsActions), nameof(SettingsActions.OnUpdateButtonClick))]
        [XmlIgnore]
        public object BudgetControlUpdateButton { get; set; }

        public bool CompatibilityMode { get; set; } //deprecated

        public int SpawnTimeInterval { get; set; } = 10;

        public string SpeedString { get; set; } = "km/h";

        public bool ShowLineInfo { get; set; } = true;

        public int DefaultVehicleCount { get; set; } = (int)VehicleEditorPositions.Bottom;

        public byte IntervalAggressionFactor { get; set; } = 13;

        public bool Unbunching { get; set; } = true;

        public int StatisticWeeks { get; set; } = 10;

        [DropDown("SETTINGS_VEHICLE_EDITOR_POSITION", nameof(VehicleEditorPositions), UI)]
        public int VehicleEditorPosition { get; set; }

        [Checkbox("SETTINGS_VEHICLE_EDITOR_HIDE", UI)]
        public bool HideVehicleEditor { get; set; }

        [AttributeUsage(AttributeTargets.All)]
        public class BudgetDescriptionAttribute : DescriptionAttribute
        {
            public BudgetDescriptionAttribute() : 
                base(Localization.Get("SETTINGS_BUDGET_CONTROL_TOOLTIP") + System.Environment.NewLine + Localization.Get("EXPLANATION_BUDGET_CONTROL"))
            {
                
            }
        }
    }


}