using System;
using HarmonyLib;

namespace MoreBlockDamageOptions.Patches;

[HarmonyPatch(typeof(EAIBreakBlock), nameof(EAIBreakBlock.CanExecute))]
public static class EAIBreakBlock_CanExecute
{
    public static void Postfix(EAIBreakBlock __instance, ref bool __result)
    {
        if (!__result)
            return;

        EntityAlive entityAlive = __instance.theEntity;
        if (entityAlive == null || entityAlive.world == null || entityAlive.moveHelper == null)
            return;

        if (ModSettings.AlwaysAllowDoorDamage)
        {
            Vector3i blockPos = entityAlive.moveHelper.HitInfo.hit.blockPos;
            BlockValue blockValue = entityAlive.world.GetBlock(blockPos);
            if (blockValue.Block != null)
            {
                Block block = blockValue.Block;
                if (block.HasTag(BlockTags.Door) || block.HasTag(BlockTags.ClosetDoor))
                    return;
            }
        }

        if (ModSettings.ZombieBlockDamageMode == ZombieBlockDamageMode.Vanilla)
            return;

        if (
            ModSettings.ZombieBlockDamageDistance <= 0
            || ModSettings.ZombieBlockDamageMode == ZombieBlockDamageMode.None
        )
        {
            __result = false;
            return;
        }

        bool flag = false;
        Vector3i entityPosition = new(entityAlive.position);
        for (int i = 0; i < entityAlive.world.Players.list.Count; i++)
        {
            EntityPlayer entityPlayer = entityAlive.world.Players.list[i];
            if (entityPlayer?.IsDead() == false)
            {
                Vector3i playerPosition = new(entityPlayer.position);
                int x = Math.Abs(entityPosition.x - playerPosition.x);
                int y = Math.Abs(entityPosition.y - playerPosition.y);
                int z = Math.Abs(entityPosition.z - playerPosition.z);
                if (
                    x <= ModSettings.ZombieBlockDamageDistance
                    && y <= ModSettings.ZombieBlockDamageDistance
                    && z <= ModSettings.ZombieBlockDamageDistance
                )
                {
                    flag = true;
                    break;
                }
            }
        }

        if (!flag)
            __result = false;
    }
}
