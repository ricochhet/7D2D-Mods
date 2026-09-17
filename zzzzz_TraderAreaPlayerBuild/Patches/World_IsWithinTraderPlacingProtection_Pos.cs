using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(World), "IsWithinTraderPlacingProtection", [typeof(Vector3i)])]
internal static class World_IsWithinTraderPlacingProtection_Pos
{
    [HarmonyPrefix]
    public static bool Prefix(ref bool __result)
    {
        if (!ModSettings.AllowPlace)
            return true;

        __result = false;
        return false;
    }
}
