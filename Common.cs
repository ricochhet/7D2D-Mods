namespace MoreBlockDamageOptions;

public static class Common
{
    public static void TryParseInt(string value, ref int into, string name)
    {
        VerboseLogging_TryParse(value, into, name);
        if (int.TryParse(value, out int result))
        {
            into = result;
        }
    }

    public static void TryParseFloat(string value, ref float into, string name)
    {
        VerboseLogging_TryParse(value, into, name);
        if (float.TryParse(value, out float result))
        {
            into = result;
        }
    }

    public static void TryParseBool(string value, ref bool into, string name)
    {
        VerboseLogging_TryParse(value, into, name);
        if (bool.TryParse(value, out bool result))
        {
            into = result;
        }
    }

    private static void VerboseLogging_TryParse<T>(string value, T into, string name)
    {
        if (ModSettings.VerboseLogging)
        {
            Log.Out(ModSettings.LogPrefix + $"{name}={into} --> {name}={value}");
        }
    }
}
