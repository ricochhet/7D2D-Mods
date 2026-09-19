using System;
using System.Reflection;
using HarmonyLib;
using ResetPoiKeepBlocks.Registry;
using UnityEngine.Scripting;

namespace ResetPoiKeepBlocks;

[Preserve]
public class ModApi : IModApi
{
    public void InitMod(Mod _modInstance)
    {
        Log.Out(ModSettings.LogPrefix + "Init starting.");
        Harmony harmony = new("com.ricochet.resetpoikeepblocks");
        try
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            ModEvents.GameStartDone.RegisterHandler(OnGameStartDone);
            ModEvents.SavePlayerData.RegisterHandler(OnSavePlayerData);
            ModEvents.WorldShuttingDown.RegisterHandler(OnWorldShuttingDown);
            Log.Out(ModSettings.LogPrefix + "Init completed.");
        }
        catch (Exception ex)
        {
            Log.Error(ModSettings.LogPrefix + "patching failed: " + ex.GetBaseException().Message);
            Log.Exception(ex);
        }
    }

    private void OnGameStartDone(ref ModEvents.SGameStartDoneData _data) => BlockRegistry.Load();

    private void OnSavePlayerData(ref ModEvents.SSavePlayerDataData _data) => BlockRegistry.SaveIfDirty();

    private void OnWorldShuttingDown(ref ModEvents.SWorldShuttingDownData _data)
    {
        BlockRegistry.SaveIfDirty();
        BlockRegistry.Reset();
    }
}
