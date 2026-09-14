using System;
using System.Reflection;
using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using HarmonyLib;

namespace MoreBlockDamageOptions;

public class Init : IModApi, IGearsModApi
{
    public void InitMod(IGearsMod _modInstance) { }

    public void InitMod(Mod _modInstance)
    {
        Log.Out(ModSettings.LogPrefix + "Init starting.");
        Harmony harmony = new("com.ricochet.moreblockdamageoptions");
        try
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            Log.Error(ModSettings.LogPrefix + "patching failed: " + ex.GetBaseException().Message);
            Log.Exception(ex);
        }
    }

    public void OnGlobalSettingsLoaded(IModGlobalSettings modSettings)
    {
        ISliderGlobalSetting zombieBlockBreakDistance =
            modSettings.GetTab("MoreBlockDamageOptions").GetCategory("Main").GetSetting("ZombieBlockBreakDistance")
            as ISliderGlobalSetting;
        Common.TryParseInt(
            zombieBlockBreakDistance.CurrentValue,
            ref ModSettings.ZombieBlockBreakDistance,
            nameof(ModSettings.ZombieBlockBreakDistance)
        );
        zombieBlockBreakDistance.OnSettingChanged += static (_, value) =>
        {
            Common.TryParseInt(
                value,
                ref ModSettings.ZombieBlockBreakDistance,
                nameof(ModSettings.ZombieBlockBreakDistance)
            );
        };

        ISwitchGlobalSetting alwaysAllowDoorBreaking =
            modSettings.GetTab("MoreBlockDamageOptions").GetCategory("Main").GetSetting("AlwaysAllowDoorBreaking")
            as ISwitchGlobalSetting;
        Common.TryParseBool(
            alwaysAllowDoorBreaking.CurrentValue,
            ref ModSettings.AlwaysAllowDoorBreaking,
            nameof(ModSettings.AlwaysAllowDoorBreaking)
        );
        alwaysAllowDoorBreaking.OnSettingChanged += static (_, value) =>
        {
            Common.TryParseBool(
                value,
                ref ModSettings.AlwaysAllowDoorBreaking,
                nameof(ModSettings.AlwaysAllowDoorBreaking)
            );
        };

        ISliderGlobalSetting explosionBlockDamagePercentage =
            modSettings
                .GetTab("MoreBlockDamageOptions")
                .GetCategory("Main")
                .GetSetting("ExplosionBlockDamagePercentage") as ISliderGlobalSetting;
        Common.TryParseFloat(
            explosionBlockDamagePercentage.CurrentValue,
            ref ModSettings.ExplosionBlockDamagePercentage,
            nameof(ModSettings.ExplosionBlockDamagePercentage)
        );
        explosionBlockDamagePercentage.OnSettingChanged += static (_, value) =>
        {
            Common.TryParseFloat(
                value,
                ref ModSettings.ExplosionBlockDamagePercentage,
                nameof(ModSettings.ExplosionBlockDamagePercentage)
            );
        };

        ISwitchGlobalSetting verboseLogging =
            modSettings.GetTab("MoreBlockDamageOptions").GetCategory("Main").GetSetting("VerboseLogging")
            as ISwitchGlobalSetting;
        Common.TryParseBool(
            verboseLogging.CurrentValue,
            ref ModSettings.VerboseLogging,
            nameof(ModSettings.VerboseLogging)
        );
        verboseLogging.OnSettingChanged += static (_, value) =>
        {
            Common.TryParseBool(value, ref ModSettings.VerboseLogging, nameof(ModSettings.VerboseLogging));
        };
    }

    public void OnWorldSettingsLoaded(IModWorldSettings worldSettings) { }
}
