using System;
using System.Reflection;
using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using HarmonyLib;

namespace MoreBlockDamageOptions;

public class ModApi : IModApi, IGearsModApi
{
    public void InitMod(IGearsMod _modInstance) { }

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
        if (modSettings is null)
            return;

        IGlobalModSettingsCategory main = modSettings.GetTab("MoreBlockDamageOptions").GetCategory("Main");
        Common.Gears_ReadGlobalInt(
            main,
            "ZombieBlockBreakDistance",
            ref ModSettings.ZombieBlockBreakDistance,
            value => ModSettings.ZombieBlockBreakDistance = value
        );

        Common.Gears_ReadGlobalBool(
            main,
            "AlwaysAllowDoorBreaking",
            ref ModSettings.AlwaysAllowDoorBreaking,
            value => ModSettings.AlwaysAllowDoorBreaking = value
        );

        Common.Gears_ReadGlobalFloat(
            main,
            "ExplosionBlockDamagePercentage",
            ref ModSettings.ExplosionBlockDamagePercentage,
            value => ModSettings.ExplosionBlockDamagePercentage = value
        );

        Common.Gears_ReadGlobalBool(
            main,
            "VerboseLogging",
            ref ModSettings.VerboseLogging,
            value => ModSettings.VerboseLogging = value
        );
    }

    public void OnWorldSettingsLoaded(IModWorldSettings worldSettings) { }
}
