using HarmonyLib;

namespace MoreBlockDamageOptions.Patches;

[HarmonyPatch(typeof(Explosion), nameof(Explosion.AttackBlocks))]
public static class Explosion_AttackBlocks
{
    public static void Prefix(ref ExplosionData ___explosionData)
    {
        if (ModSettings.ExplosionBlockDamagePercentage <= 0 || !ModSettings.EnableExplosionBlockDamage)
        {
            ___explosionData.BlockDamage = 0f;
            return;
        }

        ___explosionData.BlockDamage *= ModSettings.ExplosionBlockDamagePercentage / 100f;
    }
}
