using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace TraderAreaPlayerBuild.Patches;

[HarmonyPatch(typeof(World), nameof(World.CanPlaceLandProtectionBlockAt))]
public static class World_CanPlaceLandProtectionBlockAt_AllowTrader
{
    private static readonly MethodInfo IsWithinTraderAreaTwoPointMethod = AccessTools.Method(
        typeof(World),
        nameof(World.IsWithinTraderArea),
        [typeof(Vector3i), typeof(Vector3i)]
    );

    private static readonly FieldInfo EnabledField = AccessTools.Field(
        typeof(ModSettings),
        nameof(ModSettings.AllowLandClaim)
    );

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new CodeMatcher(instructions).MatchForward(
            false,
            new CodeMatch(OpCodes.Call, IsWithinTraderAreaTwoPointMethod)
        );

        matcher.Advance(1);
        object allowLabel = matcher.Operand;

        matcher
            .Advance(1)
            .InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldsfld, EnabledField),
                new CodeInstruction(OpCodes.Brtrue, allowLabel)
            );

        return matcher.InstructionEnumeration();
    }
}
