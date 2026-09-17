using System;
using System.Reflection;
using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using HarmonyLib;

namespace TraderAreaPlayerBuild;

public class ModApi : IModApi, IGearsModApi
{
    public void InitMod(IGearsMod modInstance) { }

    public void InitMod(Mod _modInstance)
    {
        Log.Out(ModSettings.LogPrefix + "Init starting.");
        Harmony harmony = new("com.ricochet.critmod");
        try
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Out(ModSettings.LogPrefix + "Init completed.");
        }
        catch (Exception ex)
        {
            Log.Error(ModSettings.LogPrefix + "patching failed: " + ex.GetBaseException().Message);
            Log.Exception(ex);
        }
    }

    public void OnGlobalSettingsLoaded(IModGlobalSettings modSettings)
    {
        if (modSettings == null)
            return;

        IGlobalModSettingsCategory general = GetGlobalModSettingsCategory(modSettings, "General");
        ReadGlobalBool(general, "AllowPlace", ref ModSettings.AllowPlace, value => ModSettings.AllowPlace = value);
        ReadGlobalBool(general, "AllowPickup", ref ModSettings.AllowPickup, value => ModSettings.AllowPickup = value);

        ReadGlobalBool(
            general,
            "AllowMineBreak",
            ref ModSettings.AllowMineBreak,
            value => ModSettings.AllowMineBreak = value
        );

        ReadGlobalBool(
            general,
            "AllowRepairUpgrade",
            ref ModSettings.AllowRepairUpgrade,
            value => ModSettings.AllowRepairUpgrade = value
        );

        ReadGlobalBool(
            general,
            "AllowPourWater",
            ref ModSettings.AllowPourWater,
            value => ModSettings.AllowPourWater = value
        );

        ReadGlobalBool(
            general,
            "AllowBehindTraderCounter",
            ref ModSettings.AllowBehindTraderCounter,
            value => ModSettings.AllowBehindTraderCounter = value
        );

        ReadGlobalBool(
            general,
            "AllowLandClaim",
            ref ModSettings.AllowLandClaim,
            value => ModSettings.AllowLandClaim = value
        );

        IGlobalModSettingsCategory misc = GetGlobalModSettingsCategory(modSettings, "Misc");
        ReadGlobalBool(
            misc,
            "VerboseLogging",
            ref ModSettings.VerboseLogging,
            value => ModSettings.VerboseLogging = value
        );
    }

    public void OnWorldSettingsLoaded(IModWorldSettings worldSettings) { }

    private static IGlobalModSettingsCategory GetGlobalModSettingsCategory(
        IModGlobalSettings modSetting,
        string name
    ) => modSetting.GetTab(name).GetCategory(name);

    private static void ReadGlobalBool(
        IGlobalModSettingsCategory category,
        string name,
        ref bool value,
        Action<bool> onValueChanged
    )
    {
        if (category.GetSetting(name) is not IGlobalValueSetting globalValueSetting)
            return;

        if (TryParseBool(globalValueSetting.CurrentValue, out bool initialResult))
            value = initialResult;

        bool current = value;
        globalValueSetting.OnSettingChanged += (_, rawNewValue) =>
        {
            if (TryParseBool(rawNewValue, out bool parsed))
            {
                string previous = current.ToString();
                current = parsed;
                onValueChanged(parsed);
                LogTryParse(previous, parsed.ToString(), name);
            }
        };
    }

    private static bool TryParseBool(string value, out bool result)
    {
        result = false;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim();

        if (bool.TryParse(value, out result))
            return true;

        if (
            value.Equals("Yes", StringComparison.OrdinalIgnoreCase)
            || value.Equals("1", StringComparison.OrdinalIgnoreCase)
        )
        {
            result = true;
            return true;
        }

        if (
            value.Equals("No", StringComparison.OrdinalIgnoreCase)
            || value.Equals("0", StringComparison.OrdinalIgnoreCase)
        )
        {
            result = false;
            return true;
        }

        return false;
    }

    private static void LogTryParse(string initialValue, string newValue, string valueName)
    {
        if (ModSettings.VerboseLogging)
            Log.Out(ModSettings.LogPrefix + $"{valueName}={initialValue} --> {valueName}={newValue}");
    }
}
