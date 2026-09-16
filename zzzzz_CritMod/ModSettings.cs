namespace CritMod;

public static class ModSettings
{
    public static string LogPrefix = "[CritMod] ";

    public static bool EnableBodyBaseDamageMultiplier = true;
    public static float BodyBaseDamageMultiplier = 1f;
    public static bool EnableBodyCriticalHits = true;
    public static int BodyCritChancePercentage = 10;
    public static float BodyCritDamageMultiplier = 2f;
    public static int QualityBodyCritChanceBonusPercentage = 2;

    public static bool EnableHeadBaseDamageMultiplier = true;
    public static float HeadBaseDamageMultiplier = 1f;
    public static bool EnableHeadCriticalHits = true;
    public static int HeadCritChancePercentage = 20;
    public static float HeadCritDamageMultiplier = 3f;
    public static int QualityHeadCritChanceBonusPercentage = 3;

    public static bool VerboseLogging = true;
}
