using HarmonyLib;
using UnityEngine;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(World), "IsWithinTraderPlacingProtection", [typeof(Bounds)])]
internal static class World_IsWithinTraderPlacingProtection_Bounds
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
