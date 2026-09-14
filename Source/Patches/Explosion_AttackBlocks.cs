using HarmonyLib;
using UnityEngine;

namespace MoreBlockDamageOptions.Patches;

[HarmonyPatch(typeof(Explosion), nameof(Explosion.AttackBlocks))]
public static class Explosion_AttackBlocks
{
    public static void Prefix(ref ExplosionData ___explosionData)
    {
        ___explosionData.BlockDamage *= Mathf.Max(0f, ModSettings.ExplosionBlockDamagePercentage / 100f);
    }
}
