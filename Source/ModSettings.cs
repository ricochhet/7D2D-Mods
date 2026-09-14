namespace MoreBlockDamageOptions;

public static class ModSettings
{
    public const string LogPrefix = "[MoreBlockDamageOptions] ";
    public static ZombieBlockDamageMode ZombieBlockDamageMode = ZombieBlockDamageMode.None;
    public static int ZombieBlockDamageDistance = 5;
    public static bool AlwaysAllowDoorDamage = false;
    public static float ExplosionBlockDamagePercentage = 25f;
    public static bool VerboseLogging = true;
}

public enum ZombieBlockDamageMode
{
    None,
    Distance,
    Vanilla,
}
