using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(World), "IsWithinTraderArea", [typeof(Vector3i)])]
internal static class World_IsWithinTraderArea_Single
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result)
    {
        if (PlayerBuildContext.Active)
            __result = false;
    }
}
