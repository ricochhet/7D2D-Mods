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

        if (Enum.TryParse<TEnum>(globalValueSetting.CurrentValue, true, out var result))
            value = result;

        globalValueSetting.OnSettingChanged += (_, newValue) =>
        {
            VerboseLogging_TryParse(globalValueSetting.CurrentValue, newValue, name);
            if (Enum.TryParse<TEnum>(newValue, true, out var parsed))
                onValueChanged(parsed);
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

        if (int.TryParse(globalValueSetting.CurrentValue, out int result))
            value = result;

        globalValueSetting.OnSettingChanged += (_, value) =>
        {
            VerboseLogging_TryParse(result.ToString(), value, name);
            if (int.TryParse(value, out int parsed))
                onValueChanged(parsed);
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

        if (float.TryParse(globalValueSetting.CurrentValue, out float result))
            value = result;

        globalValueSetting.OnSettingChanged += (_, value) =>
        {
            VerboseLogging_TryParse(result.ToString(), value, name);
            if (float.TryParse(value, out float parsed))
                onValueChanged(parsed);
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

        if (bool.TryParse(globalValueSetting.CurrentValue, out bool result))
            value = result;

        globalValueSetting.OnSettingChanged += (_, value) =>
        {
            VerboseLogging_TryParse(result.ToString(), value, name);
            if (bool.TryParse(value, out bool parsed))
                onValueChanged(parsed);
        };
    }

    private static void VerboseLogging_TryParse(string initialValue, string newValue, string valueName)
    {
        if (ModSettings.VerboseLogging)
            Log.Out(ModSettings.LogPrefix + $"{valueName}={initialValue} --> {valueName}={newValue}");
    }
}
