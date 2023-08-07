using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using UpgradedResearch.Common;

namespace UpgradedResearch.Configs;

public class SacrificeCountConfig : ModConfig
{
    [Label("Set all required research values to 1")]
    [Tooltip("Every item that can exist in the game will have it's research max set to 1.\nNOTE: this option is kinda one-way. Any item researched before this will also have their researched count set to one, so if you turn off this function later, all researched items will only have one has the research value.")]
    [DefaultValue(false)]
    public bool allResearchOne;

    [Label("Use Creative Panel as Item Checklist")]
    [Tooltip("Disables retrieving items from the Creative panel, and researched items will not disappear. All items will only cost 1, regardless of the option above on or off.")]
    [DefaultValue(false)]
    public bool asItemChecklist;

    [Label("Research Requirement Percentage Multiplier")]
    [Tooltip("Mutliplies all max items needed to research by this value/100. Not applied if either of the two previous features are turned on, but will apply to manually set values and categories.")]
    [DefaultValue(100)]
    [Range(0, 2147483647)]
    public int itemMultPercent;

    [Label("Limit Max Research to Max Stack")]
    [Tooltip("Regardless of item, one full stack is what you will need to research it. This option overides all manual sets as well")]
    [DefaultValue(true)]
    public bool maxStackResearch;

    [Label("When research value is 1, auto-research picked up items")]
    [Tooltip("When picking up an item, it will check if it only needs 1 item to research. If so, auto-researches the item.")]
    [DefaultValue(true)]
    public bool autoResearchOne;

    [Label("Manual Research Override - Items")]
    [Tooltip("Place the (internal) Item name of the items you want to set and their max values here. Grouped by mod, so set the mod name first, then the item name as shown in the tag.")]
    public Dictionary<string, Dictionary<string, int>> sacrifices = new Dictionary<string, Dictionary<string, int>>();

    public override ConfigScope Mode => (ConfigScope)1;

    public override void OnChanged()
    {
        if (Main.netMode != 2 && Main.player != null && ((Entity)Main.player[Main.myPlayer]).active)
        {
            ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
            if (modPlayer != null)
            {
                // ((ModPlayer)modPlayer).OnEnterWorld(Main.player[Main.myPlayer]);
                modPlayer.OnEnterWorld();

            }
        }
    }
}
