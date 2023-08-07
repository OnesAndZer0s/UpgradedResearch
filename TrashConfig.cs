using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace UpgradedResearch.Configs;

public class TrashConfig : ModConfig
{
    [Label("Auto-Trash picked up Researched items")]
    [Tooltip("Automatically deletes items that are researched when picked up. Pick up priority is Mod Name Based, so they may be caught up by other mods. Researched items with prefixes you have not researched yet will not be destroyed.")]
    [DefaultValue(true)]
    public bool autoTrashResearched;

    [Label("Auto-trash Blacklist")]
    [Tooltip("Place the tags of the items you don't want to trash in this list.")]
    public List<string> exceptionList = new List<string>();

    public override ConfigScope Mode => (ConfigScope)1;
}
