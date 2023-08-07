using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using UpgradedResearch.Common;
using UpgradedResearch.Configs;

namespace UpgradedResearch.Items;

public class ResearchGlobalItem : GlobalItem
{
    public static Asset<Texture2D> unresearched_overlay = ModContent.Request<Texture2D>("UpgradedResearch/Items/UnresearchedOverlay", (AssetRequestMode)1);

    public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        //IL_0041: Unknown result type (might be due to invalid IL or missing references)
        //IL_005d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0078: Unknown result type (might be due to invalid IL or missing references)
        //IL_0086: Unknown result type (might be due to invalid IL or missing references)
        //IL_0092: Unknown result type (might be due to invalid IL or missing references)
        //IL_0098: Unknown result type (might be due to invalid IL or missing references)
        //IL_00a9: Unknown result type (might be due to invalid IL or missing references)
        //IL_00aa: Unknown result type (might be due to invalid IL or missing references)
        if (ModContent.GetInstance<Config>().unresearchedOverlay && Main.player[Main.myPlayer].difficulty == 3 && IsResearcheable(item) && !IsResearched(item))
        {
            spriteBatch.Draw(unresearched_overlay.Value, new Vector2(((Entity)item).Center.X - Main.screenPosition.X - 38f, ((Entity)item).Center.Y - Main.screenPosition.Y - 38f), (Rectangle?)null, Color.White, 0f, default(Vector2), scale, (SpriteEffects)0, 0f);
        }
        base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
    }

    public override bool OnPickup(Item item, Player player)
    {
        //IL_0026: Unknown result type (might be due to invalid IL or missing references)
        if ((ModContent.GetInstance<SacrificeCountConfig>().autoResearchOne || ModContent.GetInstance<SacrificeCountConfig>().asItemChecklist) && OneToResearched(item))
        {
            CreativeUI.ResearchItem(item.type);
        }
        if (ModContent.GetInstance<TrashConfig>().autoTrashResearched && !ModContent.GetInstance<SacrificeCountConfig>().asItemChecklist && Main.netMode != 2 && ((Entity)player).whoAmI == Main.myPlayer && IsResearcheable(item) && IsResearched(item) && !isItemNotTrasheable(item, player))
        {
            item.TurnToAir();
            return true;
        }
        return base.OnPickup(item, player);
    }

    public override bool CanBeConsumedAsAmmo(Item weapon, Item ammo, Player player)
    {
        if (ModContent.GetInstance<Config>().infiniteAmmo && IsResearcheable(ammo))
        {
            return !IsResearched(ammo);
        }
        return true;
    }

    public override bool ConsumeItem(Item item, Player player)
    {
        if (!ModContent.GetInstance<Config>().infiniteItems || !IsResearcheable(item) || !IsResearched(item))
        {
            return base.ConsumeItem(item, player);
        }
        return false;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        //IL_003b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0041: Expected O, but got Unknown
        //IL_0042: Unknown result type (might be due to invalid IL or missing references)
        //IL_0089: Unknown result type (might be due to invalid IL or missing references)
        //IL_0093: Expected O, but got Unknown
        //IL_00bb: Unknown result type (might be due to invalid IL or missing references)
        //IL_00c5: Expected O, but got Unknown
        if (ModContent.GetInstance<Config>().showResearch && IsResearcheable(item))
        {
            if (IsResearched(item))
            {
                if (ModContent.GetInstance<Config>().showResearched)
                {
                    TooltipLine val = new TooltipLine(((ModType)this).Mod, "howMuchToResearch", "Researched!");
                    val.OverrideColor = Color.Orange;
                    tooltips.Add(val);
                }
            }
            else
            {
                tooltips.Add(new TooltipLine(((ModType)this).Mod, "howMuchToResearch", ((object)CreativeUI.GetSacrificesRemaining(item.type)).ToString() + " to full research!"));
            }
        }
        if (ModContent.GetInstance<Config>().showTag)
        {
            tooltips.Add(new TooltipLine(((ModType)this).Mod, "showInternalName", ItemID.Search.GetName(item.type)));
        }
        base.ModifyTooltips(item, tooltips);
    }

    public override void OnResearched(Item item, bool fullyResearched)
    {
        //IL_005d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0062: Unknown result type (might be due to invalid IL or missing references)
        //IL_0081: Unknown result type (might be due to invalid IL or missing references)
        //IL_0086: Unknown result type (might be due to invalid IL or missing references)
        //IL_009f: Unknown result type (might be due to invalid IL or missing references)
        //IL_015e: Unknown result type (might be due to invalid IL or missing references)
        if (!fullyResearched)
        {
            return;
        }
        ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
        if (modPlayer.validatedItem == null)
        {
            modPlayer.resetAndGatherExistingItemsAndTiles();
            modPlayer.todo = true;
        }
        else
        {
            modPlayer.todo = modPlayer.ValidateItem(item) || modPlayer.todo;
        }
        if (ModContent.GetInstance<Config>().researchGroups)
        {
            Dictionary<int, RecipeGroup>.ValueCollection.Enumerator enumerator = RecipeGroup.recipeGroups.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    RecipeGroup current = enumerator.Current;
                    if (!current.ContainsItem(item.type))
                    {
                        continue;
                    }
                    HashSet<int>.Enumerator enumerator2 = current.ValidItems.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            int current2 = enumerator2.Current;
                            if (!modPlayer.validatedItem[current2])
                            {
                                CreativeUI.ResearchItem(current2);
                            }
                        }
                    }
                    finally
                    {
                        ((System.IDisposable)enumerator2).Dispose();
                    }
                }
            }
            finally
            {
                ((System.IDisposable)enumerator).Dispose();
            }
        }
        if (fullyResearched && ModContent.GetInstance<Config>().researchRecipes && modPlayer.todo && !modPlayer.ignoreThisOnResearch)
        {
            modPlayer.ignoreThisOnResearch = true;
            int num;
            do
            {
                modPlayer.todo = false;
                num = 0;
                Recipe[] recipe = Main.recipe;
                foreach (Recipe val in recipe)
                {
                    if (IsResearcheable(val.createItem) && !IsResearched(val.createItem) && modPlayer.AllItemsResearched(val))
                    {
                        num++;
                        CreativeUI.ResearchItem(val.createItem.type);
                    }
                }
            }
            while (modPlayer.todo || num > 0);
            modPlayer.ignoreThisOnResearch = false;
        }
        if (Main.netMode != 1 || !ModContent.GetInstance<Config>().shareResearchWithTeam)
        {
            return;
        }
        for (int j = 0; j < Main.player.Length; j++)
        {
            if (j != Main.myPlayer && Main.player[Main.myPlayer].team != 0 && Main.player[j] != null && ((Entity)Main.player[j]).active && Main.player[j].team == Main.player[Main.myPlayer].team)
            {
                ((UpgradedResearch)(object)((ModType)this).Mod).SendForceResearchPacket(((Entity)((ModPlayer)modPlayer).Player).whoAmI, item);
            }
        }
    }

    public static bool IsResearcheable(Item itm)
    {
        return CreativeUI.GetSacrificesRemaining(itm.type).HasValue;
    }

    public static bool IsResearched(Item itm)
    {
        int? sacrificesRemaining = CreativeUI.GetSacrificesRemaining(itm.type);
        if (sacrificesRemaining.HasValue)
        {
            return sacrificesRemaining <= 0;
        }
        return false;
    }

    public static bool OneToResearched(Item itm)
    {
        int? sacrificesRemaining = CreativeUI.GetSacrificesRemaining(itm.type);
        if (sacrificesRemaining.HasValue)
        {
            return sacrificesRemaining == 1;
        }
        return false;
    }

    public static bool IsResearched(int type)
    {
        int? sacrificesRemaining = CreativeUI.GetSacrificesRemaining(type);
        if (sacrificesRemaining.HasValue)
        {
            return sacrificesRemaining <= 0;
        }
        return false;
    }

    public static bool OneToResearched(int itm)
    {
        int? sacrificesRemaining = CreativeUI.GetSacrificesRemaining(itm);
        if (sacrificesRemaining.HasValue)
        {
            return sacrificesRemaining == 1;
        }
        return false;
    }

    public virtual bool isItemNotTrasheable(Item item, Player p)
    {
        //IL_007c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0081: Unknown result type (might be due to invalid IL or missing references)
        if (item.type == ModContent.ItemType<ResearchSharingBook>())
        {
            return true;
        }
        if (item.type >= 71 && item.type <= 74)
        {
            return true;
        }
        if (item.type == 3822)
        {
            return true;
        }
        Item[] inventory = p.inventory;
        foreach (Item val in inventory)
        {
            if (val.type == item.type && val.stack + item.stack <= val.maxStack)
            {
                return true;
            }
        }
        List<string>.Enumerator enumerator = ModContent.GetInstance<TrashConfig>().exceptionList.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                try
                {
                    if (item.type == int.Parse(current))
                    {
                        return true;
                    }
                }
                catch (System.Exception)
                {
                    if (item.ModItem != null && ((ModType)item.ModItem).FullName.Equals(current.Trim(), (StringComparison)1))
                    {
                        return true;
                    }
                }
            }
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
        return false;
    }
}
