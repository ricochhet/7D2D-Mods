using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(ItemActionAttack), "Hit")]
internal static class ItemActionAttack_Hit
{
    [HarmonyPrefix]
    public static void Prefix(int _attackerEntityId, out bool __state)
    {
        __state = PlayerBuildContext.Active;
        if (ModSettings.AllowMineBreak)
        {
            GameManager instance = GameManager.Instance;
            bool flag = instance?.World?.GetEntity(_attackerEntityId) is EntityPlayer;
            PlayerBuildContext.Active = __state || flag;
        }
    }

    [HarmonyFinalizer]
    public static void Finalizer(bool __state) => PlayerBuildContext.Active = __state;
}
