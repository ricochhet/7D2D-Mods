using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CritMod.Patches;

[HarmonyPatch(typeof(EntityAlive), nameof(EntityAlive.DamageEntity))]
public static class EntityAlive_DamageEntity
{
    public static void Prefix(
        EntityAlive __instance,
        DamageSource _damageSource,
        ref int _strength,
        ref bool _criticalHit
    )
    {
        if (_strength <= 0 || __instance?.world == null)
            return;

        if (__instance.world.GetEntity(_damageSource.getEntityId()) is not EntityPlayer)
            return;

        bool isHeadshot = _damageSource.GetEntityDamageBodyPart(__instance) == EnumBodyPartHit.Head;
        int chance = isHeadshot ? ModSettings.HeadCritChancePercentage : ModSettings.BodyCritChancePercentage;
        float multiplier = isHeadshot ? ModSettings.HeadCritDamageMultiplier : ModSettings.BodyCritDamageMultiplier;

        ItemValue itemValue = _damageSource.AttackingItem;
        if (itemValue?.HasQuality != null && itemValue.HasQuality)
        {
            int qualityBonus = isHeadshot
                ? ModSettings.QualityHeadCritChanceBonusPercentage
                : ModSettings.QualityBodyCritChanceBonusPercentage;
            chance += (itemValue.Quality - 1) * qualityBonus;
        }

        if (Random.value < chance / 100f)
        {
            _strength = (int)Mathf.Round(_strength * multiplier);
            _criticalHit = true;
            LogCriticalHit(_strength, chance, multiplier, isHeadshot);
        }
    }

    private static void LogCriticalHit(int strength, float chance, float multiplier, bool isHeadshot)
    {
        if (ModSettings.VerboseLogging)
        {
            Log.Out(
                ModSettings.LogPrefix
                    + $"Critical hit -- Strength: {strength}, Chance: {chance}, Multiplier: {multiplier}, Headshot: {isHeadshot}"
            );
        }
    }
}
