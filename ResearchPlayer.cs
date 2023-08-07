using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using UpgradedResearch.Buffs;
using UpgradedResearch.Configs;
using UpgradedResearch.Items;

namespace UpgradedResearch.Common;

public class ResearchPlayer : ModPlayer
{
    public bool[] favourited;

    public bool[] validatedItem;

    public bool[] researchedTiles;

    public List<int> adj;

    public int knownItems;

    public List<string> favorited_unloaded = new List<string>();

    public bool skipAutoResearchesForNow;

    public List<int> allBuffsIndexes;

    public List<int> allDebuffsIndexes;

    private Dictionary<int, int> originalResearchItemCounts;

    private List<Item> gotResearchedItems;

    private List<Item> gotResearchedItemsOld;

    private bool awaitingResearchRequest;

    public long timeout;

    private bool[] favouritedOld;

    private bool awaitingFavouriteRequest;

    public bool ignoreThisOnResearch;

    public bool todo;

    public override void ResetEffects()
    {
        //IL_011a: Unknown result type (might be due to invalid IL or missing references)
        //IL_011f: Unknown result type (might be due to invalid IL or missing references)
        if (allBuffsIndexes == null)
        {
            allBuffsIndexes = new List<int>();
            allDebuffsIndexes = new List<int>();
        }
        int num = ((ModPlayer)this).Player.FindBuffIndex(ModContent.BuffType<AlphaBuff>());
        int num2 = ((ModPlayer)this).Player.FindBuffIndex(ModContent.BuffType<OmegaBuff>());
        if (num < 0 || num2 < num + 1)
        {
            if (num >= 0)
            {
                ((ModPlayer)this).Player.buffTime[num] = 0;
                ((ModPlayer)this).Player.buffType[num] = 0;
                num = -1;
            }
            if (num2 >= 0)
            {
                ((ModPlayer)this).Player.buffTime[num2] = 0;
                ((ModPlayer)this).Player.buffType[num2] = 0;
                num2 = -1;
            }
            if (num <= -1 && num2 <= -1 && Main.netMode != 2 && Main.myPlayer == ((Entity)((ModPlayer)this).Player).whoAmI)
            {
                allBuffsIndexes.Clear();
                allDebuffsIndexes.Clear();
            }
        }
        if (gotResearchedItemsOld == null || timeout <= 0)
        {
            gotResearchedItems = null;
            if (Main.netMode != 2 && ((Entity)((ModPlayer)this).Player).whoAmI == Main.myPlayer)
            {
                List<Item> val = new List<Item>();
                List<int> val2 = new List<int>();

                // get all of CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId that is zero
                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Where(x => x.Value == 0).Select(x => x.Key).ToList().ForEach(x => val2.Add(x));
                
                // CreativeItemSacrificesCatalog.Instance.FillListOfItemsThatCanBeObtainedInfinitely(val2);
                List<int>.Enumerator enumerator = val2.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        int current = enumerator.Current;
                        val.Add(ContentSamples.ItemsByType[current]);
                    }
                }
                finally
                {
                    ((System.IDisposable)enumerator).Dispose();
                }
                gotResearchedItems = val;
                gotResearchedItemsOld = val;
                if (favourited == null)
                {
                    favourited = new bool[ItemLoader.ItemCount];
                }
                favouritedOld = favourited;
            }
            else
            {
                favourited = null;
                if (((ModPlayer)this).Player.team != 0 && ModContent.GetInstance<Config>().syncResearchWithTeam)
                {
                    GetAllResearchedItems();
                }
            }
            timeout = 120L;
        }
        if (timeout > 0 && !awaitingResearchRequest && !awaitingFavouriteRequest)
        {
            timeout--;
        }
    }

    public override void PreUpdate()
    {
        base.PreUpdate();
    }

    public List<int> convertValidatedItemsIntoList()
    {
        if (validatedItem == null)
        {
            resetAndGatherExistingItemsAndTiles();
        }
        List<int> val = new List<int>();
        for (int i = 1; i < validatedItem.Length; i += 32)
        {
            int num = 0;
            for (int j = 0; j < 32 && j + i < validatedItem.Length; j++)
            {
                num |= (validatedItem[j + i] ? (1 << j) : 0);
            }
            val.Add(num);
        }
        return val;
    }

    public List<Item> AllFavoritedItems()
    {
        return CommonUtils.ItemsFromBoolArray(GetFavouriteArray());
    }

    public static bool[] convertListIntoValidatedItems(List<int> ans)
    {
        bool[] array = new bool[1 + ans.Count * 32];
        for (int i = 1; i < array.Length; i += 32)
        {
            int num = ans[i / 32];
            for (int j = 0; j < 32; j++)
            {
                if (j + i >= array.Length)
                {
                    goto end_IL_004a;
                }
                array[j + i] = (num & (1 << j)) != 0;
            }
            continue;
            end_IL_004a:
            break;
        }
        return array;
    }

    public override void OnEnterWorld()
    {
//         //IL_0172: Unknown result type (might be due to invalid IL or missing references)
//         //IL_0177: Unknown result type (might be due to invalid IL or missing references)
//         //IL_01a5: Unknown result type (might be due to invalid IL or missing references)
//         //IL_01aa: Unknown result type (might be due to invalid IL or missing references)
//         //IL_03f9: Unknown result type (might be due to invalid IL or missing references)
//         //IL_0404: Unknown result type (might be due to invalid IL or missing references)
        if (Main.netMode != 2)
        {
            if (originalResearchItemCounts == null)
            {
                originalResearchItemCounts = Enumerable.ToDictionary<KeyValuePair<int, int>, int, int>((System.Collections.Generic.IEnumerable<KeyValuePair<int, int>>)CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId, (Func<KeyValuePair<int, int>, int>)((KeyValuePair<int, int> entry) => entry.Key), (Func<KeyValuePair<int, int>, int>)((KeyValuePair<int, int> entry) => entry.Value));
            }
            else
            {
                for (int i = 0; i < ItemLoader.ItemCount; i++)
                {
                    if (originalResearchItemCounts.ContainsKey(i))
                    {
                        if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.ContainsKey(i))
                        {
                            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[i] = originalResearchItemCounts[i];
                        }
                        else
                        {
                            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Add(i, originalResearchItemCounts[i]);
                        }
                    }
                    else if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.ContainsKey(i))
                    {
                        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Remove(i);
                    }
                }
            }
            if (ModContent.GetInstance<SacrificeCountConfig>().allResearchOne || ModContent.GetInstance<SacrificeCountConfig>().asItemChecklist)
            {
                for (int j = 1; j < ItemLoader.ItemCount; j++)
                {
                    if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.ContainsKey(j))
                    {
                        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[j] = 1;
                    }
                    else
                    {
                        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Add(j, 1);
                    }
                }
            }
            else
            {
                Dictionary<string, Dictionary<string, int>>.KeyCollection.Enumerator enumerator = ModContent.GetInstance<SacrificeCountConfig>().sacrifices.Keys.GetEnumerator();
                try
                {
                    int num = default(int);
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        Dictionary<string, int> val = ModContent.GetInstance<SacrificeCountConfig>().sacrifices[current];
                        if (val == null)
                        {
                            continue;
                        }
                        Dictionary<string, int>.KeyCollection.Enumerator enumerator2 = val.Keys.GetEnumerator();
                        try
                        {
                            while (enumerator2.MoveNext())
                            {
                                string current2 = enumerator2.Current;
                                if (!ItemID.Search.TryGetId(current2, out num) && !ItemID.Search.TryGetId(current + "/" + current2, out num))
                                {
                                    num = 0;
                                }
                                if (num == 0 || val[current2] < 0)
                                {
                                    continue;
                                }
                                if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.ContainsKey(num))
                                {
                                    if (val[current2] == 0)
                                    {
                                        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Remove(num);
                                    }
                                    else
                                    {
                                        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[num] = val[current2];
                                    }
                                }
                                else if (val[current2] != 0)
                                {
                                    CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Add(num, val[current2]);
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
                if (ModContent.GetInstance<SacrificeCountConfig>().itemMultPercent != 100)
                {
                    for (int k = 1; k < ItemLoader.ItemCount; k++)
                    {
                        if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.ContainsKey(k))
                        {
                            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[k] = (int)Math.Round((double)((float)(CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[k] * ModContent.GetInstance<SacrificeCountConfig>().itemMultPercent) / 100f));
                            if (CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[k] <= 0)
                            {
                                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[k] = 1;
                            }
                        }
                    }
                }
                if (ModContent.GetInstance<SacrificeCountConfig>().maxStackResearch)
                {
                    int num2 = default(int);
                    for (int l = 1; l < ItemLoader.ItemCount; l++)
                    {
                        if (ContentSamples.ItemsByType.ContainsKey(l) && ContentSamples.ItemsByType[l] != null && CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.TryGetValue(l, out num2))
                        {
                            if (num2 > 0)
                            {
                                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[l] = Math.Min(num2, ContentSamples.ItemsByType[l].maxStack);
                            }
                            else
                            {
                                CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Remove(l);
                            }
                        }
                    }
                }
            }
            resetItems();
            // if (!ResearchGlobalItem.IsResearched(ContentSamples.ItemsByType[ModContent.ItemType<ResearchErasingBook>()]))
            // {
                // CreativeUI.ResearchItem(ModContent.ItemType<ResearchSharingBook>());
                // CreativeUI.ResearchItem(ModContent.ItemType<ResearchErasingBook>());
            // }
            if (favourited == null)
            {
                favourited = new bool[ItemLoader.ItemCount];
            }
        }
        base.OnEnterWorld();
    }

    public List<Item> GetAllResearchedItems()
    {
        //IL_0032: Unknown result type (might be due to invalid IL or missing references)
        //IL_0037: Unknown result type (might be due to invalid IL or missing references)
        if (Main.netMode != 2 && ((Entity)((ModPlayer)this).Player).whoAmI == Main.myPlayer)
        {
            List<Item> val = new List<Item>();
            List<int> val2 = new List<int>();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId.Where(x => x.Value == 0).Select(x => x.Key).ToList().ForEach(x => val2.Add(x));

            // CreativeItemSacrificesCatalog.Instance.FillListOfItemsThatCanBeObtainedInfinitely(val2);
            List<int>.Enumerator enumerator = val2.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    int current = enumerator.Current;
                    val.Add(ContentSamples.ItemsByType[current]);
                }
            }
            finally
            {
                ((System.IDisposable)enumerator).Dispose();
            }
            gotResearchedItems = val;
            gotResearchedItemsOld = val;
            return gotResearchedItems;
        }
        if (!awaitingResearchRequest && (gotResearchedItemsOld == null || timeout <= 0))
        {
            ((UpgradedResearch)(object)((ModType)this).Mod).SendRequestResearchPacket((Main.netMode != 2) ? Main.myPlayer : 255, ((Entity)((ModPlayer)this).Player).whoAmI);
            awaitingResearchRequest = true;
        }
        return gotResearchedItemsOld;
    }

    public void SetFoundResearch(List<Item> research)
    {
        //IL_007d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0082: Unknown result type (might be due to invalid IL or missing references)
        //IL_009b: Unknown result type (might be due to invalid IL or missing references)
        gotResearchedItemsOld = research;
        gotResearchedItems = research;
        if (ModContent.GetInstance<Config>().syncResearchWithTeam && research != null && Main.netMode == 1 && ((Entity)((ModPlayer)this).Player).whoAmI != Main.myPlayer && Main.player[Main.myPlayer].team != 0 && ((Entity)((ModPlayer)this).Player).active && ((ModPlayer)this).Player.team == Main.player[Main.myPlayer].team)
        {
            List<Item>.Enumerator enumerator = research.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Item current = enumerator.Current;
                    if (!ResearchGlobalItem.IsResearched(current))
                    {
                        CreativeUI.ResearchItem(current.type);
                    }
                }
            }
            finally
            {
                ((System.IDisposable)enumerator).Dispose();
            }
        }
        awaitingResearchRequest = false;
    }

    public bool[] GetFavouriteArray()
    {
        if (Main.netMode != 2 && ((Entity)((ModPlayer)this).Player).whoAmI == Main.myPlayer)
        {
            favouritedOld = favourited;
            return favourited;
        }
        if (!awaitingFavouriteRequest && (favouritedOld == null || timeout <= 0))
        {
            ((UpgradedResearch)(object)((ModType)this).Mod).SendRequestFavoritesPacket((Main.netMode != 2) ? Main.myPlayer : 255, ((Entity)((ModPlayer)this).Player).whoAmI);
            awaitingFavouriteRequest = true;
        }
        return favouritedOld;
    }

    public void SetFoundFavorites(bool[] favorites)
    {
        favourited = favorites;
        favouritedOld = favourited;
        awaitingFavouriteRequest = false;
    }

    public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
    {
        //IL_008b: Unknown result type (might be due to invalid IL or missing references)
        //IL_00a4: Unknown result type (might be due to invalid IL or missing references)
        //IL_00aa: Invalid comparison between Unknown and I4
        //IL_00ba: Unknown result type (might be due to invalid IL or missing references)
        //IL_012b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0144: Unknown result type (might be due to invalid IL or missing references)
        if (!ResearchGlobalItem.IsResearcheable(inventory[slot]))
        {
            return base.ShiftClickSlot(inventory, context, slot);
        }
        if (context == 0 && Main.CreativeMenu.IsShowingResearchMenu() && Main.cursorOverride == 9)
        {
            if (ModContent.GetInstance<Config>().shiftTrashResearched && ResearchGlobalItem.IsResearched(inventory[slot]))
            {
                Main.player[Main.myPlayer].trashItem = inventory[slot];
                inventory[slot].TurnToAir();
                return true;
            }
            Item itm = inventory[slot];
            Main.CreativeMenu.SwapItem(ref inventory[slot]);
            SoundEngine.PlaySound(in SoundID.Grab, (Vector2?)null);
            if (ModContent.GetInstance<Config>().autoShiftResearch)
            {
                int num = default(int);
                if ((int)Main.CreativeMenu.SacrificeItem(out num) == 2)
                {
                    SoundEngine.PlaySound(in SoundID.ResearchComplete, (Vector2?)null);
                }
                if (ModContent.GetInstance<Config>().shiftTrashResearched && ResearchGlobalItem.IsResearched(itm))
                {
                    Main.player[Main.myPlayer].trashItem.TurnToAir();
                    Main.CreativeMenu.SwapItem(ref Main.player[Main.myPlayer].trashItem);
                }
            }
            return true;
        }
        if (context == 30 && Main.CreativeMenu.IsShowingResearchMenu())
        {
            inventory[slot] = Main.player[Main.myPlayer].GetItem(Main.myPlayer, inventory[slot], GetItemSettings.InventoryEntityToPlayerInventorySettings);
            SoundEngine.PlaySound(in SoundID.MenuTick, (Vector2?)null);
            return true;
        }
        return false;
    }

    public void resetItems()
    {
        if (Main.netMode != 2)
        {
            while (todo || ignoreThisOnResearch)
            {
            }
        }
        validatedItem = null;
        researchedTiles = null;
    }

    public void resetAndGatherExistingItemsAndTiles()
    {
        validatedItem = new bool[ItemLoader.ItemCount];
        knownItems = 0;
        researchedTiles = new bool[TileLoader.TileCount];
        for (int i = 1; i < ItemLoader.ItemCount; i++)
        {
            Item itm = ContentSamples.ItemsByType[i];
            ValidateItem(itm);
        }
    }

    public bool ValidateItem(Item itm)
    {
        //IL_00af: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b4: Unknown result type (might be due to invalid IL or missing references)
        bool flag = validatedItem[itm.type];
        validatedItem[itm.type] = ResearchGlobalItem.IsResearcheable(itm) && ResearchGlobalItem.IsResearched(itm);
        if (validatedItem[itm.type] && itm.createTile >= 0)
        {
            int num = ((!researchedTiles[itm.createTile]) ? 1 : 0);
            researchedTiles[itm.createTile] = true;
            if (adj == null)
            {
                adj = new List<int>();
            }
            if (!adj.Contains(itm.createTile))
            {
                adj.Add(itm.createTile);
            }
            List<int>.Enumerator enumerator = AdjTiles(itm.createTile).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    int current = enumerator.Current;
                    num += ((!researchedTiles[current]) ? 1 : 0);
                    researchedTiles[current] = true;
                    if (!adj.Contains(current))
                    {
                        adj.Add(current);
                    }
                }
            }
            finally
            {
                ((System.IDisposable)enumerator).Dispose();
            }
            todo = todo || num > 0;
        }
        if (flag != validatedItem[itm.type])
        {
            knownItems++;
        }
        return flag != validatedItem[itm.type];
    }

    public List<int> AdjTiles(int type)
    {
        //IL_00bc: Unknown result type (might be due to invalid IL or missing references)
        //IL_00c1: Unknown result type (might be due to invalid IL or missing references)
        if (type < 0)
        {
            return new List<int>();
        }
        List<int> obj = new List<int>();
        obj.Add(type);
        List<int> val = obj;
        if (!researchedTiles[type])
        {
            researchedTiles[type] = true;
        }
        if (type == 302 || type == 77 || type == 133)
        {
            val.AddRange((System.Collections.Generic.IEnumerable<int>)AdjTiles(17));
        }
        if (type == 133)
        {
            val.AddRange((System.Collections.Generic.IEnumerable<int>)AdjTiles(77));
        }
        if (type == 134)
        {
            val.AddRange((System.Collections.Generic.IEnumerable<int>)AdjTiles(16));
        }
        if (type == 354 || type == 469 || type == 355)
        {
            val.AddRange((System.Collections.Generic.IEnumerable<int>)AdjTiles(14));
        }
        if (type == 355)
        {
            val.AddRange((System.Collections.Generic.IEnumerable<int>)AdjTiles(13));
        }
        List<int> val2 = new List<int>();
        List<int>.Enumerator enumerator = val.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                if (!val2.Contains(current))
                {
                    val2.Add(current);
                }
            }
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
        val = val2;
        ModTile tile = TileLoader.GetTile(type);
        if (tile != null)
        {
            int[] adjTiles = tile.AdjTiles;
            foreach (int num in adjTiles)
            {
                if (!val.Contains(num))
                {
                    val.Add(num);
                }
                if (!researchedTiles[num])
                {
                    researchedTiles[num] = true;
                }
            }
        }
        Func<int, int[]>[] array = (Func<int, int[]>[])typeof(TileLoader).GetField("HookAdjTiles", (BindingFlags)40).GetValue((object)null);
        for (int j = 0; j < array.Length; j++)
        {
            int[] adjTiles = array[j].Invoke(type);
            foreach (int num2 in adjTiles)
            {
                if (!val.Contains(num2))
                {
                    val.Add(num2);
                }
                if (!researchedTiles[num2])
                {
                    researchedTiles[num2] = true;
                }
            }
        }
        return val;
    }

    public bool AllItemsResearched(Recipe recipe)
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_000b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0043: Unknown result type (might be due to invalid IL or missing references)
        //IL_0048: Unknown result type (might be due to invalid IL or missing references)
        List<Item>.Enumerator enumerator = recipe.requiredItem.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Item current = enumerator.Current;
                if (!MatchesItemInRecipe(recipe, current))
                {
                    return false;
                }
            }
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
        List<int>.Enumerator enumerator2 = recipe.requiredTile.GetEnumerator();
        try
        {
            while (enumerator2.MoveNext())
            {
                int current2 = enumerator2.Current;
                if (!researchedTiles[current2])
                {
                    return false;
                }
            }
        }
        finally
        {
            ((System.IDisposable)enumerator2).Dispose();
        }
        return true;
    }

    public bool CanBeReplacedByResearchedItem(Recipe recipe, Item itm)
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_000b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0039: Unknown result type (might be due to invalid IL or missing references)
        //IL_003e: Unknown result type (might be due to invalid IL or missing references)
        List<int>.Enumerator enumerator = recipe.acceptedGroups.GetEnumerator();
        try
        {
            RecipeGroup val = default(RecipeGroup);
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                if (!RecipeGroup.recipeGroups.TryGetValue(current, out val) || !val.ContainsItem(itm.type))
                {
                    continue;
                }
                HashSet<int>.Enumerator enumerator2 = val.ValidItems.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        int current2 = enumerator2.Current;
                        if (validatedItem[current2])
                        {
                            return true;
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
        return false;
    }

    public bool MatchesItemInRecipe(Recipe recipe, Item itm)
    {
        if (itm.IsAir)
        {
            return true;
        }
        if (validatedItem[itm.type])
        {
            return true;
        }
        return CanBeReplacedByResearchedItem(recipe, itm);
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        if (Main.netMode != 0)
        {
            if (((Entity)((ModPlayer)this).Player).whoAmI != Main.myPlayer || Main.netMode == 2)
            {
                ((UpgradedResearch)(object)((ModType)this).Mod).SendRequestBuffsPacket((Main.netMode != 2) ? Main.myPlayer : 255, ((Entity)((ModPlayer)this).Player).whoAmI);
            }
            base.SyncPlayer(toWho, fromWho, newPlayer);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        List<int> val = new List<int>();
        List<string> val2 = new List<string>();
        if (favourited == null && favouritedOld != null)
        {
            favourited = favouritedOld;
        }
        if (favourited != null)
        {
            string text = default(string);
            for (int i = 0; i < favourited.Length; i++)
            {
                if (favourited[i])
                {
                    if (i < 5125)
                    {
                        val.Add(i);
                    }
                    else if (ItemID.Search.TryGetName(i, out text))
                    {
                        val2.Add(text);
                    }
                }
            }
            int num = val.Count + val2.Count + favorited_unloaded.Count;
            tag.Set("fav_s", (object)num, false);
            int num2 = 0;
            for (int j = 0; j < val.Count; j++)
            {
                tag.Set("fav_i" + num2, (object)val[j], false);
                num2++;
            }
            for (int k = 0; k < val2.Count; k++)
            {
                tag.Set("fav_m" + num2, (object)val2[k], false);
                num2++;
            }
            for (int l = 0; l < favorited_unloaded.Count; l++)
            {
                tag.Set("fav_m" + num2, (object)favorited_unloaded[l], false);
                num2++;
            }
        }
        base.SaveData(tag);
    }

    public override void LoadData(TagCompound tag)
    {
        if (favourited == null)
        {
            favourited = new bool[ItemLoader.ItemCount];
        }
        if (!tag.ContainsKey("fav_s"))
        {
            return;
        }
        int asInt = tag.GetAsInt("fav_s");
        if (asInt <= 0)
        {
            return;
        }
        for (int i = 0; i < asInt; i++)
        {
            if (tag.ContainsKey("fav_i" + i))
            {
                int asInt2 = tag.GetAsInt("fav_i" + i);
                if (favourited.Length < asInt2)
                {
                    System.Array.Resize<bool>(ref favourited, asInt2 + 1);
                }
                favourited[tag.GetAsInt("fav_i" + i)] = true;
            }
            else
            {
                if (!tag.ContainsKey("fav_m" + i))
                {
                    continue;
                }
                int num = 0;
                if (ItemID.Search.TryGetId(tag.GetString("fav_m" + i), out num))
                {
                    if (favourited.Length < num)
                    {
                        System.Array.Resize<bool>(ref favourited, num + 1);
                    }
                    favourited[num] = true;
                }
                else
                {
                    favorited_unloaded.Add(tag.GetString("fav_m" + i));
                }
            }
        }
        SetFoundFavorites(favourited);
        base.LoadData(tag);
    }
}
