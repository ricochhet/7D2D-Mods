using System;
using System.Reflection;
using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using HarmonyLib;

namespace CritMod;

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

        IGlobalModSettingsCategory body = GetGlobalModSettingsCategory(modSettings, "Body");
        ReadGlobalInt(
            body,
            "BodyCritChancePercentage",
            ref ModSettings.BodyCritChancePercentage,
            value => ModSettings.BodyCritChancePercentage = value
        );

        ReadGlobalFloat(
            body,
            "BodyCritDamageMultiplier",
            ref ModSettings.BodyCritDamageMultiplier,
            value => ModSettings.BodyCritDamageMultiplier = value
        );

        ReadGlobalInt(
            body,
            "QualityBodyCritChanceBonusPercentage",
            ref ModSettings.QualityBodyCritChanceBonusPercentage,
            value => ModSettings.QualityBodyCritChanceBonusPercentage = value
        );

        IGlobalModSettingsCategory head = GetGlobalModSettingsCategory(modSettings, "Head");
        ReadGlobalInt(
            body,
            "HeadCritChancePercentage",
            ref ModSettings.HeadCritChancePercentage,
            value => ModSettings.HeadCritChancePercentage = value
        );

        ReadGlobalFloat(
            body,
            "HeadCritDamageMultiplier",
            ref ModSettings.HeadCritDamageMultiplier,
            value => ModSettings.HeadCritDamageMultiplier = value
        );

        ReadGlobalInt(
            body,
            "QualityHeadCritChanceBonusPercentage",
            ref ModSettings.QualityHeadCritChanceBonusPercentage,
            value => ModSettings.QualityHeadCritChanceBonusPercentage = value
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

    private static void ReadGlobalFloat(
        IGlobalModSettingsCategory category,
        string name,
        ref float value,
        Action<float> onValueChanged
    )
    {
        if (category.GetSetting(name) is not IGlobalValueSetting globalValueSetting)
            return;

        if (float.TryParse(globalValueSetting.CurrentValue, out float initialResult))
            value = initialResult;

        float current = value;
        globalValueSetting.OnSettingChanged += (_, rawNewValue) =>
        {
            if (float.TryParse(rawNewValue, out float parsed))
            {
                string previous = current.ToString();
                current = parsed;
                onValueChanged(parsed);
                LogTryParse(previous, parsed.ToString(), name);
            }
        };
    }

    private static void ReadGlobalInt(
        IGlobalModSettingsCategory category,
        string name,
        ref int value,
        Action<int> onValueChanged
    )
    {
        if (category.GetSetting(name) is not IGlobalValueSetting globalValueSetting)
            return;

        if (int.TryParse(globalValueSetting.CurrentValue, out int initialResult))
            value = initialResult;

        int current = value;
        globalValueSetting.OnSettingChanged += (_, rawNewValue) =>
        {
            if (int.TryParse(rawNewValue, out int parsed))
            {
                string previous = current.ToString();
                current = parsed;
                onValueChanged(parsed);
                LogTryParse(previous, parsed.ToString(), name);
            }
        };
    }

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
