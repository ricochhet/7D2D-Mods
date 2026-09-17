using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(World), "CanPlaceBlockAt", [typeof(Vector3i), typeof(PersistentPlayerData), typeof(bool)])]
internal static class World_CanPlaceBlockAt
{
    [HarmonyPrefix]
    public static void Prefix(ref bool traderAllowed)
    {
        if (ModSettings.AllowPlace)
            traderAllowed = true;
    }
}
