using System;
using GearsAPI.Settings.Global;

namespace MoreBlockDamageOptions;

public static class Common
{
    public static IGlobalModSettingsCategory GetCategory(this IModGlobalSettings modSetting, string name) =>
        modSetting.GetTab(name).GetCategory(name);

    public static void Gears_ReadGlobalEnum<TEnum>(
        IGlobalModSettingsCategory category,
        string name,
        ref TEnum value,
        Action<TEnum> onValueChanged
    )
        where TEnum : struct, Enum
    {
        if (category.GetSetting(name) is not IGlobalValueSetting globalValueSetting)
            return;

        if (Enum.TryParse<TEnum>(globalValueSetting.CurrentValue, true, out var initialResult))
            value = initialResult;

        TEnum current = value;
        globalValueSetting.OnSettingChanged += (_, rawNewValue) =>
        {
            if (Enum.TryParse<TEnum>(rawNewValue, true, out var parsed))
            {
                string previous = current.ToString();
                current = parsed;
                onValueChanged(parsed);
                VerboseLogging_TryParse(previous, parsed.ToString(), name);
            }
        };
    }

    public static void Gears_ReadGlobalInt(
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
                VerboseLogging_TryParse(previous, parsed.ToString(), name);
            }
        };
    }

    public static void Gears_ReadGlobalFloat(
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
                VerboseLogging_TryParse(previous, parsed.ToString(), name);
            }
        };
    }

    public static void Gears_ReadGlobalBool(
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
                VerboseLogging_TryParse(previous, parsed.ToString(), name);
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

    private static void VerboseLogging_TryParse(string initialValue, string newValue, string valueName)
    {
        if (ModSettings.VerboseLogging)
            Log.Out(ModSettings.LogPrefix + $"{valueName}={initialValue} --> {valueName}={newValue}");
    }
}
