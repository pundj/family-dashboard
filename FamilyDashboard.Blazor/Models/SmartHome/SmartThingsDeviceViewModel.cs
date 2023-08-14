using FamilyDashboard.Blazor.Helpers;

namespace FamilyDashboard.Blazor.Models.SmartHome;

public class SmartThingsDeviceViewModel
{
    public string? DeviceId { get; set; }
    public SmartHomeDeviceType Type { get; set; }
    public string? Label { get; set; }
    public SmartHomeSwitch SwitchValue { get; set; }
    public SmartHomeSwitch AlarmValue { get; set; }
    public SmartHomeOnlineState OnlineState { get; set; }
    public int? Level { get; set; }
    public int? Hue { get; set; }
    public int? Saturation { get; set; }
    public int? Temperature { get; set; }
    public SmartHomeSmokeCODetectorState SmokeDetectorState { get; set; }
    public SmartHomeSmokeCODetectorState CarbonMonoxideDetectorState { get; set; }
    public string? BatteryLevel { get; set; }
    public string? TemperatureMeasurement { get; set; }
    public string? ThermostatHumidityMeasurement { get; set; }
    public string? ThermostatHeatingSetpoint { get; set; }
    public string? ThermostatCoolingSetpoint { get; set; }
    public SmartHomeThermostatMode ThermostatMode { get; set; }
    public SmartHomeThermostatOperatingState ThermostatOperatingState { get; set; }
    public SmartHomeOpenState ValveState { get; set; }
    public SmartHomeOpenState ContactState { get; set; }
    public SmartHomePresenceState PresenceState { get; set; }
    public SmartHomeWaterSensorState WaterSensorState { get; set; }
    public string? Status => GetStatus();
    public string? TemperatureText => GetTemperature();
    public string? IconClassName => GetIconClassName();
    public string? ColorClassName => GetColorClassName();

    private string GetStatus()
    {
        if (OnlineState == SmartHomeOnlineState.Offline)
            return OnlineState.GetDisplayName();
        else if (SwitchValue != SmartHomeSwitch.Unknown)
            return SwitchValue.GetDisplayName();
        else if (AlarmValue != SmartHomeSwitch.Unknown)
            return AlarmValue.GetDisplayName();
        else if (PresenceState != SmartHomePresenceState.Unknown)
            return PresenceState.GetDisplayName();
        else if (WaterSensorState != SmartHomeWaterSensorState.Unknown)
            return WaterSensorState.GetDisplayName();
        else if (ContactState != SmartHomeOpenState.Unknown)
            return ContactState.GetDisplayName();
        else if (CarbonMonoxideDetectorState != SmartHomeSmokeCODetectorState.Unknown)
            return CarbonMonoxideDetectorState.GetDisplayName();
        else if (SmokeDetectorState != SmartHomeSmokeCODetectorState.Unknown)
            return SmokeDetectorState.GetDisplayName();
        else if (ValveState != SmartHomeOpenState.Unknown)
            return ValveState.GetDisplayName();
        else

        return "";
    }

    private string GetTemperature() => 
        TemperatureMeasurement != default
            ? $"{TemperatureMeasurement}{(ThermostatOperatingState != SmartHomeThermostatOperatingState.Unknown ? " - " + ThermostatOperatingState + " to " + (ThermostatOperatingState == SmartHomeThermostatOperatingState.Cooling ? ThermostatCoolingSetpoint : ThermostatHeatingSetpoint) : ThermostatMode != SmartHomeThermostatMode.Unknown ? ThermostatMode : "")}"
            : "";

    private string GetIconClassName()
    {
        return Type switch
        {
            SmartHomeDeviceType.Light => "lightbulb",
            SmartHomeDeviceType.Switch => "light-switch",
            SmartHomeDeviceType.Thermostat => "thermometer-half",
            SmartHomeDeviceType.SmartPlug => "plug",
            SmartHomeDeviceType.Lock => "lock",
            SmartHomeDeviceType.MultiFunctionalSensor or SmartHomeDeviceType.ContactSensor => "sensor-on",
            SmartHomeDeviceType.MotionSensor => "person-running-fast",
            SmartHomeDeviceType.LeakSensor => "droplet",
            SmartHomeDeviceType.SmokeDetector => "fire-smoke",
            SmartHomeDeviceType.MobilePresence => "location-dot",
            SmartHomeDeviceType.RemoteController => "gamepad",
            SmartHomeDeviceType.Siren => "siren-on",
            SmartHomeDeviceType.WaterValve => "pipe-valve",
            SmartHomeDeviceType.Hub => "router",
            _ => "question",
        };
    }

    private string GetColorClassName()
    {
        if (WaterSensorState == SmartHomeWaterSensorState.Wet ||
            CarbonMonoxideDetectorState == SmartHomeSmokeCODetectorState.SmokeDetected ||
            SmokeDetectorState == SmartHomeSmokeCODetectorState.SmokeDetected)
            return "danger";

        if (OnlineState == SmartHomeOnlineState.Offline)
            return "secondary";

        return SwitchValue switch
        {
            SmartHomeSwitch.On => "success",
            _ => "white",
        };
    }
}
