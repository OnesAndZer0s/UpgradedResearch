using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace UpgradedResearch.Configs;

public class Config : ModConfig
{
    [Label("Draw Unresearched Items overlay on World items")]
    [Tooltip("Any researcheable item you have not researched fully yet that are dropped in the world show up with a magnifying glass over them, to easily identify them.")]
    [DefaultValue(true)]
    public bool unresearchedOverlay;

    [Label("Research all craftables at once")]
    [Tooltip("When you research an item or crafting table, learns all items that can be crafted with it and the available researched items/crafting tables as well.")]
    [DefaultValue(true)]
    public bool researchRecipes;

    [Label("Research all Items in the same RecipeGroup at once")]
    [Tooltip("When you research an item, it will automatically research all of the items that belong to the same group at once (Warning: Pearlwood is part of the Any Wood Group)")]
    [DefaultValue(false)]
    public bool researchGroups;

    [Label("Research when shift-clicked")]
    [Tooltip("When you shift-click an item into the Research slot, automatically research it, instead of having to manually click the research button.")]
    [DefaultValue(false)]
    public bool autoShiftResearch;

    [Label("Trash when shift-clicked and already researched")]
    [Tooltip("When you shift-click an item, it will only go into the Research slot if it's not fully researched, else goes to the trash slot.")]
    [DefaultValue(true)]
    public bool shiftTrashResearched;

    [Label("Craft from Researched stations")]
    [Tooltip("If true, will allow use of the researched crafting stations as if they were around the player.")]
    [DefaultValue(true)]
    public bool allowCraftFromResearch;

    [Label("Share Research With Team members")]
    [Tooltip("If true, will share whatever you research from that point onwards with your teammates, until you leave the team. Will not share previously researched items.")]
    [DefaultValue(false)]
    public bool shareResearchWithTeam;

    [Label("Sync Research With Team members")]
    [Tooltip("If true, will share whatever you have researched with your teammates, until you leave the team.")]
    [DefaultValue(false)]
    public bool syncResearchWithTeam;

    [Label("Researched Ammo is infinite")]
    [Tooltip("If you have researched a type of ammo, that ammo is considered infinte, and will not be consumed.")]
    [DefaultValue(true)]
    public bool infiniteAmmo;

    [Label("Researched Consumables are infinite")]
    [Tooltip("If you have researched a type of consumable item (such as blocks, potions, furniture, etc...), that item and will not be consumed when used.")]
    [DefaultValue(true)]
    public bool infiniteItems;

    [Label("Display Research amount on item tooltip")]
    [Tooltip("Shows how much research is required for the selected item to be fully researched.")]
    [DefaultValue(true)]
    public bool showResearch;

    [Label("Display Researched! tooltip")]
    [Tooltip("Shows \"Researched!\" on the item if it was already researched. Make false to ignore it.")]
    [DefaultValue(false)]
    public bool showResearched;

    [Label("Display ItemTag on item tooltip")]
    [Tooltip("Shows the mod and item internal name below the item tooltip. Use this if you want to find out how to add an exception to the research value to an item.")]
    [DefaultValue(false)]
    public bool showTag;

    [Label("Allow OmniPotion")]
    [Tooltip("If the OmniPotion item should allow you to obtain the buffs from all researched items categorized as \"Food and Potions\".")]
    [DefaultValue(true)]
    public bool omniPotion;

    [Label("OmniPotion drinks favourited potions only")]
    [Tooltip("If the OmniPotion item, when allowed, should drink all \"Food and Potions\" in the favourite category, or off for drinking all potions reseached except the ones favourited.")]
    [DefaultValue(true)]
    public bool favouritedOmniPotion;

    public override ConfigScope Mode => (ConfigScope)0;
}
