using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using UpgradedResearch.Common;
using UpgradedResearch.Configs;


namespace UpgradedResearch.Items;

public class ResearchSharingBook : ModItem
{
    public List<int> knowledge;

    public string playerName;

    public override void SetStaticDefaults()
    {
        //IL_002e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0038: Expected O, but got Unknown
        // ((ModItem)this).DisplayName.SetDefault("Research Sharing Book");
        // ((ModItem)this).Tooltip.SetDefault("Research this book once to store all your knowledge in it. Then share it with a friend!\nResearching a full book will give you all its knowledge and reset it to a blank book.");
        ((ModItem)this).Item.ResearchUnlockCount = 1;
        On_CreativeUI.SacrificeItem_refItem_refInt32_bool += OnSacrificeItem;
    }

    public override void SetDefaults()
    {
        ((Entity)((ModItem)this).Item).width = 32;
        ((Entity)((ModItem)this).Item).height = 32;
        ((ModItem)this).Item.value = 0;
        ((ModItem)this).Item.maxStack = 1;
    }

    public override bool CanUseItem(Player player)
    {
        return true;
    }

    private CreativeUI.ItemSacrificeResult OnSacrificeItem(On_CreativeUI.orig_SacrificeItem_refItem_refInt32_bool orig, ref Item item, out int amountWeSacrificed, bool returnRemainderToPlayer)
    {
        //IL_002c: Unknown result type (might be due to invalid IL or missing references)
        amountWeSacrificed = 0;
        if (item.type != ((ModItem)this).Type || !ResearchGlobalItem.IsResearched(((ModItem)this).Item))
        {
            int stack = item.stack;
            CreativeUI.ItemSacrificeResult result = orig.Invoke(ref item, out amountWeSacrificed, returnRemainderToPlayer);
            if (ModContent.GetInstance<SacrificeCountConfig>().asItemChecklist)
            {
                item.stack = stack;
            }
            return result;
        }
        ResearchSharingBook researchSharingBook = item.ModItem as ResearchSharingBook;
        ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
        if (researchSharingBook.playerName == null || researchSharingBook.knowledge == null)
        {
            Main.NewText("Collecting information...", (byte)255, (byte)255, (byte)255);
            CollectKnowledge(ref item);
            Main.NewText(string.Concat(new string[7]
            {
                "This book from ",
                researchSharingBook.playerName,
                " contains ",
                modPlayer.knownItems.ToString(),
                " fully researched items in ",
                researchSharingBook.knowledge.Count.ToString(),
                " entries."
            }), (byte)255, (byte)255, (byte)255);
            return (CreativeUI.ItemSacrificeResult)0;
        }
        Main.NewText(string.Concat(new string[7]
        {
            "This book from ",
            researchSharingBook.playerName,
            " contains ",
            modPlayer.knownItems.ToString(),
            " fully researched items in ",
            researchSharingBook.knowledge.Count.ToString(),
            " entries."
        }), (byte)255, (byte)255, (byte)255);
        if (Main.player[Main.myPlayer].name.Equals(researchSharingBook.playerName))
        {
            if (Main.netMode != 2)
            {
                rebuildResearch(ref item);
            }
        }
        else if (Main.netMode != 2)
        {
            addKnowledge(ref item);
        }
        return (CreativeUI.ItemSacrificeResult)2;
    }

    private void addKnowledge(ref Item item)
    {
        //IL_006e: Unknown result type (might be due to invalid IL or missing references)
        ResearchSharingBook researchSharingBook = item.ModItem as ResearchSharingBook;
        Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
        Main.NewText("Adding new researched items...", (byte)255, (byte)255, (byte)255);
        int num = 0;
        bool[] array = ResearchPlayer.convertListIntoValidatedItems(researchSharingBook.knowledge);
        for (int i = 1; i < ItemLoader.ItemCount && i < array.Length; i++)
        {
            Item itm = ContentSamples.ItemsByType[i];
            if (array[i] && ResearchGlobalItem.IsResearcheable(itm) && !ResearchGlobalItem.IsResearched(itm))
            {
                CreativeUI.ResearchItem(i);
                num++;
            }
        }
        if (num > 0)
        {
            Main.NewText("The book contained " + num + " unknown Items!", (byte)255, (byte)255, (byte)255);
        }
        researchSharingBook.knowledge = null;
        researchSharingBook.playerName = null;
    }

    private void rebuildResearch(ref Item item)
    {
        //IL_0054: Unknown result type (might be due to invalid IL or missing references)
        //IL_0059: Unknown result type (might be due to invalid IL or missing references)
        //IL_009e: Unknown result type (might be due to invalid IL or missing references)
        //IL_00a3: Unknown result type (might be due to invalid IL or missing references)
        //IL_00cb: Unknown result type (might be due to invalid IL or missing references)
        ResearchSharingBook researchSharingBook = item.ModItem as ResearchSharingBook;
        Main.NewText("Rebuilding research... This may take a while...", (byte)255, (byte)255, (byte)255);
        Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>().resetAndGatherExistingItemsAndTiles();
        if (ModContent.GetInstance<Config>().researchGroups)
        {
            Dictionary<int, RecipeGroup>.ValueCollection.Enumerator enumerator = RecipeGroup.recipeGroups.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    RecipeGroup current = enumerator.Current;
                    if (!Enumerable.ToList<int>((System.Collections.Generic.IEnumerable<int>)current.ValidItems).Exists((Predicate<int>)((int x) => Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>().validatedItem[x])))
                    {
                        continue;
                    }
                    HashSet<int>.Enumerator enumerator2 = current.ValidItems.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            int current2 = enumerator2.Current;
                            if (!Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>().validatedItem[current2])
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
        ((GlobalItem)ModContent.GetInstance<ResearchGlobalItem>()).OnResearched(((ModItem)this).Item, true);
        Main.NewText("Done.", (byte)255, (byte)255, (byte)255);
        researchSharingBook.knowledge = null;
        researchSharingBook.playerName = null;
    }

    public override ModItem Clone(Item item)
    {
        ResearchSharingBook obj = this.MemberwiseClone() as ResearchSharingBook;
        obj.knowledge = knowledge;
        obj.playerName = playerName;
        return (ModItem)(object)obj;
    }

    public override void SaveData(TagCompound tag)
    {
        if (playerName != null)
        {
            tag.Set("n", (object)playerName, false);
        }
        if (knowledge != null)
        {
            tag.Set("r", (object)knowledge, false);
        }
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey("n"))
        {
            playerName = tag.GetString("n");
        }
        if (tag.ContainsKey("r"))
        {
            knowledge = new List<int>();
            knowledge.AddRange((System.Collections.Generic.IEnumerable<int>)tag.GetList<int>("r"));
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        //IL_0000: Unknown result type (might be due to invalid IL or missing references)
        //IL_0006: Expected O, but got Unknown
        TagCompound val = new TagCompound();
        ((ModItem)this).SaveData(val);
        TagIO.Write(val, writer);
        base.NetSend(writer);
    }

    public override void NetReceive(BinaryReader reader)
    {
        ((ModItem)this).LoadData(TagIO.Read(reader));
        base.NetReceive(reader);
    }

    private void CollectKnowledge(ref Item item)
    {
        ResearchSharingBook researchSharingBook = item.ModItem as ResearchSharingBook;
        if (Main.netMode != 2)
        {
            researchSharingBook.playerName = Main.player[Main.myPlayer].name;
            ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
            modPlayer.resetAndGatherExistingItemsAndTiles();
            researchSharingBook.knowledge = modPlayer.convertValidatedItemsIntoList();
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        if (playerName != null)
        {
            tooltips.Find((Predicate<TooltipLine>)((TooltipLine tl) => tl.Name.Equals("ItemName"))).Text = playerName + "'s Research Sharing Book";
        }
    }
}
