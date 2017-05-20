using ImprovedPublicTransport2.OptionsFramework.Attibutes;

namespace ImprovedPublicTransport2
{
    [Options("ImprovedPublicTransport2", "ImprovedPublicTransportSettings")]
    public class Settings
    {
        private const string SETTINGS_UI = "SETTINGS_UI";

        public bool BudgetControl { get; set; } = true;

        public bool CompatibilityMode { get; set; } //deprecated

        public int SpawnTimeInterval { get; set; } = 10;

        public string SpeedString { get; set; } = "km/h";

        public bool ShowLineInfo { get; set; } = true;

        public int DefaultVehicleCount { get; set; } = (int)VehicleEditorPositions.Bottom;

        public byte IntervalAggressionFactor { get; set; } = 13;

        public bool Unbunching { get; set; } = true;

        public int StatisticWeeks { get; set; } = 10;

        [DropDown("SETTINGS_VEHICLE_EDITOR_POSITION", nameof(VehicleEditorPositions), SETTINGS_UI)]
        public int VehicleEditorPosition { get; set; }

        [Checkbox("SETTINGS_VEHICLE_EDITOR_HIDE", SETTINGS_UI)]
        public bool HideVehicleEditor { get; set; }
    }
}