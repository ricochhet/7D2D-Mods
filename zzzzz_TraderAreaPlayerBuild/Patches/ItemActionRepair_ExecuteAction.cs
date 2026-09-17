using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(ItemActionRepair), "ExecuteAction", [typeof(ItemActionData), typeof(bool)])]
internal static class ItemActionRepair_ExecuteAction
{
    [HarmonyPrefix]
    public static void Prefix(out bool __state)
    {
        __state = PlayerBuildContext.Active;
        if (ModSettings.AllowRepairUpgrade)
            PlayerBuildContext.Active = true;
    }

    [HarmonyFinalizer]
    public static void Finalizer(bool __state) => PlayerBuildContext.Active = __state;
}
