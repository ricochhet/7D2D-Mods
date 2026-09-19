using System.Collections;
using System.Collections.Generic;
using ResetPoiKeepBlocks.Registry;

namespace ResetPoiKeepBlocks.Commands;

public class ResetPoiKeepBlocks : ConsoleCmdAbstract
{
    public override string[] getCommands() => ["resetpoi"];

    public override string getDescription() => "Reset the current POI, keeping all placed blocks";

    public override string getHelp() => "Usage: resetpoi [dryrun]";

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        World world = GameManager.Instance?.World;
        if (world == null)
        {
            SdtdConsole.Instance.Output(ModSettings.LogPrefix + "No world is loaded");
            return;
        }

        EntityPlayer caller = ResolveCallingPlayer(world, _senderInfo);
        if (caller == null)
        {
            SdtdConsole.Instance.Output(ModSettings.LogPrefix + "Could not resolve command caller");
            return;
        }

        PrefabInstance prefabInstance = world.GetPOIAtPosition(caller.GetPosition(), null, null);
        if (prefabInstance == null)
        {
            SdtdConsole.Instance.Output(ModSettings.LogPrefix + "Not within POI boundaries");
            return;
        }

        Vector3i bbPos = prefabInstance.boundingBoxPosition;
        long initialChunkKey = WorldChunkCache.MakeChunkKey(bbPos.x >> 4, bbPos.z >> 4);

        bool dryRun =
            _params.Count > 0 && string.Equals(_params[0], "dryrun", System.StringComparison.OrdinalIgnoreCase);

        if (dryRun)
        {
            List<Vector3i> tracked = FindTrackedPositionsInBoundingBox(prefabInstance, world);
            SdtdConsole.Instance.Output(
                ModSettings.LogPrefix + $"DRY RUN: {prefabInstance.name} @ {bbPos}: {tracked.Count}"
            );
            foreach (Vector3i pos in tracked)
            {
                BlockValue current = world.GetBlock(pos);
                SdtdConsole.Instance.Output("  " + pos + " - " + current.Block.GetBlockName());
            }
            return;
        }

        SdtdConsole.Instance.Output(ModSettings.LogPrefix + $"Resetting: {prefabInstance.name} @ {bbPos}");
        ThreadManager.StartCoroutine(ResetKeepBlocks(prefabInstance, world, initialChunkKey));
    }

    private static IEnumerator ResetKeepBlocks(PrefabInstance prefabInstance, World world, long initialChunkKey)
    {
        List<TrackedBlock> tracked = SnapshotTrackedBlocks(prefabInstance, world);
        yield return prefabInstance.ResetBlocksAndRebuild(world, FastTags<TagGroup.Global>.none, initialChunkKey);
        int restoredCount = RestoreTrackedBlocks(world, tracked);

        SdtdConsole.Instance.Output(
            ModSettings.LogPrefix + $"Finished resetting: {prefabInstance.name} ({restoredCount})"
        );
    }

    private static List<Vector3i> FindTrackedPositionsInBoundingBox(PrefabInstance prefabInstance, World world)
    {
        List<Vector3i> result = [];
        Vector3i bbPos = prefabInstance.boundingBoxPosition;
        Vector3i bbSize = prefabInstance.boundingBoxSize;

        for (int wx = bbPos.x; wx < bbPos.x + bbSize.x; wx++)
        {
            for (int wy = bbPos.y; wy < bbPos.y + bbSize.y; wy++)
            {
                for (int wz = bbPos.z; wz < bbPos.z + bbSize.z; wz++)
                {
                    Vector3i worldPos = new(wx, wy, wz);
                    if (!BlockRegistry.IsPlayerPlaced(worldPos))
                        continue;

                    if (world.GetBlock(worldPos).isair)
                    {
                        BlockRegistry.MarkRemoved(worldPos);
                        continue;
                    }

                    result.Add(worldPos);
                }
            }
        }

        return result;
    }

    private static List<TrackedBlock> SnapshotTrackedBlocks(PrefabInstance prefabInstance, World world)
    {
        List<TrackedBlock> tracked = [];

        foreach (Vector3i worldPos in FindTrackedPositionsInBoundingBox(prefabInstance, world))
        {
            BlockValue currentBlock = world.GetBlock(worldPos);

            TileEntity tileEntityClone = null;
            IChunk ichunk = world.GetChunkFromWorldPos(worldPos);
            if (ichunk is Chunk chunk)
            {
                Vector3i chunkLocalPos = ToChunkLocal(worldPos);
                TileEntity existingTe = chunk.GetTileEntity(chunkLocalPos);
                if (existingTe != null)
                    tileEntityClone = existingTe.Clone();
            }

            tracked.Add(new TrackedBlock(worldPos, currentBlock, tileEntityClone));
        }

        return tracked;
    }

    private static int RestoreTrackedBlocks(World world, List<TrackedBlock> tracked)
    {
        foreach (TrackedBlock block in tracked)
        {
            world.SetBlockRPC(new BlockValueRef(block.WorldPos), block.BlockValue);

            if (block.TileEntityClone == null)
                continue;

            IChunk ichunk = world.GetChunkFromWorldPos(block.WorldPos);
            if (ichunk is not Chunk chunk)
                continue;

            TileEntity te = block.TileEntityClone;
            te.localChunkPos = ToChunkLocal(block.WorldPos);
            te.SetChunk(chunk);
            chunk.AddTileEntity(te);
            te.SetChunkModified();
        }

        return tracked.Count;
    }

    private static Vector3i ToChunkLocal(Vector3i worldPos) => new(worldPos.x & 15, worldPos.y, worldPos.z & 15);

    private static EntityPlayer ResolveCallingPlayer(World world, CommandSenderInfo senderInfo)
    {
        if (senderInfo.RemoteClientInfo != null)
            return world.GetEntity(senderInfo.RemoteClientInfo.entityId) as EntityPlayer;

        return world.GetPrimaryPlayer();
    }

    private sealed class TrackedBlock(Vector3i worldPos, BlockValue blockValue, TileEntity tileEntityClone)
    {
        public readonly Vector3i WorldPos = worldPos;
        public readonly BlockValue BlockValue = blockValue;
        public readonly TileEntity TileEntityClone = tileEntityClone;
    }
}
