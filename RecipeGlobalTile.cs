using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using UpgradedResearch.Configs;

namespace UpgradedResearch.Common.Globals;

internal class RecipeGlobalTile : GlobalTile
{
    public override int[] AdjTiles(int type)
    {
        if (ModContent.GetInstance<Config>().allowCraftFromResearch && ((Entity)Main.player[Main.myPlayer]).active)
        {
            ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
            if (modPlayer.adj == null)
            {
                modPlayer.adj = new List<int>();
            }
            modPlayer.adj.Sort();
            return modPlayer.adj.ToArray();
        }
        return base.AdjTiles(type);
    }
}
