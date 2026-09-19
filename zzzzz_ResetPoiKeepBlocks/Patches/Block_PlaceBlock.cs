using HarmonyLib;
using ResetPoiKeepBlocks.Registry;

namespace ResetPoiKeepBlocks.Patches;

[HarmonyPatch(typeof(Block), nameof(Block.PlaceBlock))]
public static class Block_PlaceBlock
{
    public static void Postfix(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
    {
        if (_ea is not EntityPlayer)
            return;

        if (_world is not World world)
            return;

        if (world.GetPOIAtPosition(_result.blockPos, null, null) == null)
            return;

        BlockRegistry.MarkPlaced(_result.blockPos);
    }
}
