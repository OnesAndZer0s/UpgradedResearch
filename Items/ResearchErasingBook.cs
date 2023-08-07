// using On.Terraria.GameContent.Creative;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using UpgradedResearch.Common;
using static Terraria.GameContent.Creative.CreativeUI;
using static Terraria.GameContent.Creative.On_CreativeUI;

namespace UpgradedResearch.Items;

public class ResearchErasingBook : ModItem
{
    public override void SetStaticDefaults()
    {
        //IL_002e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0038: Expected O, but got Unknown
        // this.DisplayName.SetDefault("Research Erasing Book");
        // this.Tooltip.SetDefault("A book that will make you forget all that you researched. Right-click it to forget.");
        this.Item.ResearchUnlockCount = 1;
        SacrificeItem_refItem_refInt32_bool += OnSacrifice;
    }

    private ItemSacrificeResult OnSacrifice(orig_SacrificeItem_refItem_refInt32_bool orig, ref Item item, out int amountWeSacrificed, bool returnRemainderToPlayer)
    {
        //IL_003e: Unknown result type (might be due to invalid IL or missing references)
        if (item.type == ((ModItem)this).Item.type && ResearchGlobalItem.IsResearched(item))
        {
            item.ModItem.RightClick(Main.player[Main.myPlayer]);
            amountWeSacrificed = 0;
            return (ItemSacrificeResult)2;
        }
        return orig.Invoke(ref item, out amountWeSacrificed, returnRemainderToPlayer);
    }

    public override void SetDefaults()
    {
        ((Entity)((ModItem)this).Item).width = 32;
        ((Entity)((ModItem)this).Item).height = 32;
        ((ModItem)this).Item.value = 0;
        ((ModItem)this).Item.maxStack = 1;
    }

    public override bool CanRightClick()
    {
        return true;
    }

    public override void RightClick(Player player)
    {
        //IL_002c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0037: Unknown result type (might be due to invalid IL or missing references)
        if (!Main.ServerSideCharacter)
        {
            Main.LocalPlayerCreativeTracker.ItemSacrifices.Reset();
            player.GetModPlayer<ResearchPlayer>().resetItems();
        }
        CreativeUI.ResearchItem(((ModItem)this).Item.type);
        CreativeUI.ResearchItem(ModContent.ItemType<ResearchSharingBook>());
        Item item = ((ModItem)this).Item;
        item.stack--;
    }
}
