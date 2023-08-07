using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UpgradedResearch.Buffs;
using UpgradedResearch.Common;
using UpgradedResearch.Configs;

namespace UpgradedResearch.Items;

public class OmniPotion : ModItem
{
    public override void SetStaticDefaults()
    {
        this.Item.ResearchUnlockCount= 1;
    }

    public override void SetDefaults()
    {
        //IL_0044: Unknown result type (might be due to invalid IL or missing references)
        ((Entity)((ModItem)this).Item).width = 32;
        ((Entity)((ModItem)this).Item).height = 32;
        ((ModItem)this).Item.value = 0;
        ((ModItem)this).Item.maxStack = 1;
        ((ModItem)this).Item.consumable = false;
        ((ModItem)this).Item.UseSound = SoundID.Item3;
        ((ModItem)this).Item.useStyle = 2;
        ((ModItem)this).Item.useTurn = true;
        ((ModItem)this).Item.useAnimation = 17;
        ((ModItem)this).Item.useTime = 17;
    }

    public override bool? UseItem(Player player)
    {
        this.RightClick(player);
        return true;
    }

    public override bool CanRightClick()
    {
        return true;
    }

    public override void AddRecipes()
    {
        base.CreateRecipe(1).AddIngredient(31, 1).AddIngredient(ModContent.ItemType<ResearchSharingBook>(), 1)
            .AddTile(13)
            .Register();
    }

    public override void RightClick(Player player)
    {
        if (Main.netMode == 2)
        {
            return;
        }
        if (!ModContent.GetInstance<Config>().omniPotion)
        {
            Main.NewText("Enable it in the configs for using its effects.", (byte)255, (byte)255, (byte)255);
        }
        ResearchPlayer modPlayer = player.GetModPlayer<ResearchPlayer>();
        if (((Entity)player).whoAmI == Main.myPlayer)
        {
            modPlayer.allBuffsIndexes.Clear();
            modPlayer.allDebuffsIndexes.Clear();
            bool favouritedOmniPotion = ModContent.GetInstance<Config>().favouritedOmniPotion;
            for (int i = 0; i < modPlayer.GetFavouriteArray().Length; i++)
            {
                if (!ResearchGlobalItem.IsResearched(i) || modPlayer.GetFavouriteArray()[i] != favouritedOmniPotion)
                {
                    continue;
                }
                Item val = ContentSamples.ItemsByType[i];
                if (val.buffType > 0 && val.buffTime > 0 && !Main.vanityPet[val.buffType] && !Main.lightPet[val.buffType] && val.buffType != 21 && val.buffType != 94)
                {
                    if (Main.debuff[val.buffType])
                    {
                        modPlayer.allDebuffsIndexes.Add(val.buffType);
                    }
                    else
                    {
                        modPlayer.allBuffsIndexes.Add(val.buffType);
                    }
                }
            }
        }
        player.AddBuff(ModContent.BuffType<AlphaBuff>(), 18000, true, false);
        player.AddBuff(ModContent.BuffType<OmegaBuff>(), 18000, true, false);
        if (((Entity)player).whoAmI == Main.myPlayer && Main.netMode != 0)
        {
            ((UpgradedResearch)(object)((ModType)this).Mod).SendRequestBuffsReplyPacket(255, ((Entity)player).whoAmI, modPlayer);
        }
    }
}
