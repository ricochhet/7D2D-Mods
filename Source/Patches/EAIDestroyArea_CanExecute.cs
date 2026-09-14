using HarmonyLib;

namespace MoreBlockDamageOptions.Patches;

[HarmonyPatch(typeof(EAIDestroyArea), nameof(EAIDestroyArea.CanExecute))]
public static class EAIDestroyArea_CanExecute
{
    public static bool Prefix(ref bool __result)
    {
        if (!ModSettings.EnableDestroyAreaMode)
        {
            __result = false;
            return false;
        }

        return true;
    }
}
