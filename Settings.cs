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
        private const string SETTINGS_BUDGET = "SETTINGS_ENABLE_BUDGET_CONTROL";
        private const string SETTINGS_UNBUNCHING = "UNBUNCHING_ENABLED";

        [BudgetDescription]
        [Checkbox(SETTINGS_BUDGET, SETTINGS_BUDGET, nameof(SettingsActions), nameof(SettingsActions.OnBudgetCheckChanged))]  //TODO(earalov): add new locale?
        public bool BudgetControl { get; set; } = true;

        [BudgetDescription]
        [Button("SETTINGS_UPDATE", SETTINGS_BUDGET, nameof(SettingsActions), nameof(SettingsActions.OnUpdateButtonClick))]
        [XmlIgnore]
        public object BudgetControlUpdateButton { get; } = null;

        public bool CompatibilityMode { get; set; } //deprecated

        public int SpawnTimeInterval { get; set; } = 10;

        public string SpeedString { get; set; } = "km/h";

        public bool ShowLineInfo { get; set; } = true;

        public int DefaultVehicleCount { get; set; } = (int)VehicleEditorPositions.Bottom;

        [Slider("SETTINGS_UNBUNCHING_AGGRESSION", 1.0f, 60.0f, 1.0f, SETTINGS_UNBUNCHING)] //TODO(earalov): action and tooltip
        public byte IntervalAggressionFactor { get; set; } = 13; //TODO(earalov): convert into max seconds at stop

        public bool Unbunching { get; } = true;

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