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
        if (modSettings == null)
            return;

        IGlobalModSettingsCategory zombies = modSettings.GetCategory("Zombies");
        Common.Gears_ReadGlobalEnum(
            zombies,
            "ZombieBlockDamageMode",
            ref ModSettings.ZombieBlockDamageMode,
            value => ModSettings.ZombieBlockDamageMode = value
        );

        Common.Gears_ReadGlobalInt(
            zombies,
            "ZombieBlockDamageDistance",
            ref ModSettings.ZombieBlockDamageDistance,
            value => ModSettings.ZombieBlockDamageDistance = value
        );

        Common.Gears_ReadGlobalBool(
            zombies,
            "AlwaysAllowDoorDamage",
            ref ModSettings.AlwaysAllowDoorDamage,
            value => ModSettings.AlwaysAllowDoorDamage = value
        );

        Common.Gears_ReadGlobalBool(
            zombies,
            "EnableDestroyAreaMode",
            ref ModSettings.EnableDestroyAreaMode,
            value => ModSettings.EnableDestroyAreaMode = value
        );

        IGlobalModSettingsCategory explosions = modSettings.GetCategory("Explosions");
        Common.Gears_ReadGlobalBool(
            explosions,
            "EnableExplosionBlockDamage",
            ref ModSettings.EnableExplosionBlockDamage,
            value => ModSettings.EnableExplosionBlockDamage = value
        );

        Common.Gears_ReadGlobalInt(
            explosions,
            "ExplosionBlockDamagePercentage",
            ref ModSettings.ExplosionBlockDamagePercentage,
            value => ModSettings.ExplosionBlockDamagePercentage = value
        );

        IGlobalModSettingsCategory misc = modSettings.GetCategory("Misc");
        Common.Gears_ReadGlobalBool(
            misc,
            "VerboseLogging",
            ref ModSettings.VerboseLogging,
            value => ModSettings.VerboseLogging = value
        );
    }

    public void OnWorldSettingsLoaded(IModWorldSettings worldSettings) { }
}
