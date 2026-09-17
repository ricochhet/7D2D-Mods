using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(ItemActionDumpWater), "ExecuteAction", [typeof(ItemActionData), typeof(bool)])]
internal static class ItemActionDumpWater_ExecuteAction
{
    [HarmonyPrefix]
    public static void Prefix(out bool __state)
    {
        __state = PlayerBuildContext.Active;
        if (ModSettings.AllowPourWater)
            PlayerBuildContext.Active = true;
    }

    [HarmonyFinalizer]
    public static void Finalizer(bool __state) => PlayerBuildContext.Active = __state;
}
