using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using UpgradedResearch.Common;

namespace UpgradedResearch.Buffs;

public class OmegaBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[((ModBuff)this).Type] = true;
        Main.debuff[((ModBuff)this).Type] = false;
        Main.pvpBuff[((ModBuff)this).Type] = true;
        Main.buffNoSave[((ModBuff)this).Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.buffTime[buffIndex] = 18000;
        ResearchPlayer modPlayer = player.GetModPlayer<ResearchPlayer>();
        if (player.buffTime[buffIndex] <= 0)
        {
            return;
        }
        int[] array = new int[player.buffTime.Length];
        System.Array.Copy((System.Array)player.buffTime, (System.Array)array, array.Length);
        int[] array2 = new int[player.buffType.Length];
        System.Array.Copy((System.Array)player.buffType, (System.Array)array2, array2.Length);
        int num;
        for (int i = 0; i < modPlayer.allDebuffsIndexes.Count; i += num)
        {
            num = Math.Min(array2.Length, modPlayer.allDebuffsIndexes.Count - i);
            for (int j = 0; j < player.buffType.Length; j++)
            {
                if (j < num)
                {
                    player.buffType[j] = modPlayer.allBuffsIndexes[i + j];
                    player.buffTime[j] = 18000;
                }
                else
                {
                    player.buffTime[j] = (player.buffTime[j] = -1);
                }
            }
            ((ModPlayer)modPlayer).Player.UpdateBuffs(((Entity)player).whoAmI);
        }
        System.Array.Copy((System.Array)array, (System.Array)player.buffTime, array.Length);
        System.Array.Copy((System.Array)array2, (System.Array)player.buffType, array2.Length);
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
        tip = "";
        tip = tip + ((ModBuff)this).Description.Value + modPlayer.allDebuffsIndexes.Count + "\n";
    }
}
