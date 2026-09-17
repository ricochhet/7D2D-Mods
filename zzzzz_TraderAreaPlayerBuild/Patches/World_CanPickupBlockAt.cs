using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(World), "CanPickupBlockAt", [typeof(Vector3i), typeof(PersistentPlayerData)])]
internal static class World_CanPickupBlockAt
{
    [HarmonyPrefix]
    public static bool Prefix(World __instance, Vector3i blockPos, PersistentPlayerData lpRelative, ref bool __result)
    {
        if (
            ModSettings.AllowPickup
            && World.SandboxUseTraderArea == TraderAreaStates.Default
            && __instance.IsWithinTraderArea(blockPos)
        )
        {
            __result = __instance.CanPlaceBlockAt(blockPos, lpRelative, traderAllowed: true);
            return false;
        }
        return true;
    }
}
