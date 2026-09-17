using System.Collections.Generic;
using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(Chunk), "GetWallVolumes")]
internal static class Chunk_GetWallVolumes
{
    [HarmonyPostfix]
    public static void Postfix(ref List<int> __result)
    {
        if (ModSettings.AllowBehindTraderCounter && __result?.Count > 0)
            __result = [];
    }
}
