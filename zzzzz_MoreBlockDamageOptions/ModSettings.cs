namespace MoreBlockDamageOptions;

public static class ModSettings
{
    public static string LogPrefix = "[MoreBlockDamageOptions] ";

    public static ZombieBlockDamageMode ZombieBlockDamageMode = ZombieBlockDamageMode.None;
    public static int ZombieBlockDamageDistance = 5;
    public static bool AlwaysAllowDoorDamage = false;
    public static bool EnableDestroyAreaMode = true;

    public static int ExplosionBlockDamagePercentage = 25;
    public static bool EnableExplosionBlockDamage = true;

    public static bool VerboseLogging = true;
}

public enum ZombieBlockDamageMode
{
    None,
    Distance,
    Vanilla,
}
