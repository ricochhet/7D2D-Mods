using System;
using System.Reflection;
using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using HarmonyLib;

namespace MoreBlockDamageOptions;

public class ModApi : IModApi, IGearsModApi
{
    public void InitMod(IGearsMod modInstance) { }

    public void InitMod(Mod _modInstance)
    {
        Log.Out(ModSettings.LogPrefix + "Init starting.");
        Harmony harmony = new("com.ricochet.moreblockdamageoptions");
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

        IGlobalModSettingsCategory zombies = GetGlobalModSettingsCategory(modSettings, "Zombies");
        ReadGlobalEnum(
            zombies,
            "ZombieBlockDamageMode",
            ref ModSettings.ZombieBlockDamageMode,
            value => ModSettings.ZombieBlockDamageMode = value
        );

        ReadGlobalInt(
            zombies,
            "ZombieBlockDamageDistance",
            ref ModSettings.ZombieBlockDamageDistance,
            value => ModSettings.ZombieBlockDamageDistance = value
        );

        ReadGlobalBool(
            zombies,
            "AlwaysAllowDoorDamage",
            ref ModSettings.AlwaysAllowDoorDamage,
            value => ModSettings.AlwaysAllowDoorDamage = value
        );

        ReadGlobalBool(
            zombies,
            "EnableDestroyAreaMode",
            ref ModSettings.EnableDestroyAreaMode,
            value => ModSettings.EnableDestroyAreaMode = value
        );

        IGlobalModSettingsCategory explosions = GetGlobalModSettingsCategory(modSettings, "Explosions");
        ReadGlobalBool(
            explosions,
            "EnableExplosionBlockDamage",
            ref ModSettings.EnableExplosionBlockDamage,
            value => ModSettings.EnableExplosionBlockDamage = value
        );

        ReadGlobalInt(
            explosions,
            "ExplosionBlockDamagePercentage",
            ref ModSettings.ExplosionBlockDamagePercentage,
            value => ModSettings.ExplosionBlockDamagePercentage = value
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

    private static void ReadGlobalEnum<TEnum>(
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
