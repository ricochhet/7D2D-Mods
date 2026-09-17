using System;

namespace TraderAreaPlayerBuild.Patches;

internal static class PlayerBuildContext
{
    [ThreadStatic]
    public static bool Active;
}
