using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria.UI;


using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using UpgradedResearch.Common;
using UpgradedResearch.Configs;

namespace UpgradedResearch.Items;

public class CreativeMenuGlobalItem : GlobalItem
{
    public static Asset<Texture2D> favourite_overlay = ModContent.Request<Texture2D>("UpgradedResearch/Items/MainSlotFrame", (AssetRequestMode)1);

    public static string[] SearchBarContents = new string[1] { "" };

    public static string OriginalSearchString = "";

    private static UISearchBar searchBar;

    private static bool ignoreSetBar = false;

    private static bool searchBarContentsHasFilter = false;

    private static bool drawingInCreative = false;

    private static Vector2 SlotPosition;

    public override void Load()
    {
        //IL_0007: Unknown result type (might be due to invalid IL or missing references)
        //IL_0011: Expected O, but got Unknown
        //IL_0018: Unknown result type (might be due to invalid IL or missing references)
        //IL_0022: Expected O, but got Unknown
        //IL_0029: Unknown result type (might be due to invalid IL or missing references)
        //IL_0033: Expected O, but got Unknown
        //IL_003a: Unknown result type (might be due to invalid IL or missing references)
        //IL_0044: Expected O, but got Unknown
        //IL_004b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0055: Expected O, but got Unknown
        //IL_005c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0066: Expected O, but got Unknown
        //IL_006d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0077: Expected O, but got Unknown
        On_ItemSlot.OverrideHover_refItem_int += ItemSlot_OverrideHover_refItem_int;
        On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
        On_UICreativeInfiniteItemsDisplay.UpdateContents += UICreativeInfiniteItemsDisplay_UpdateContents;
        On_UIDynamicItemCollection.SetContentsToShow += UIDynamicItemCollection_SetContentsToShow;
        On_UISearchBar.SetContents += UISearchBar_SetContents;
        On_ItemSlot.RightClick_refItem_int += ItemSlot_RightClick_refItem_int;
        On_ItemSlot.LeftClick_refItem_int += ItemSlot_LeftClick_refItem_int;
        base.Load();
    }

    private void ItemSlot_RightClick_refItem_int(On_ItemSlot.orig_RightClick_refItem_int orig, ref Item inv, int context)
    {
        if (context != 29 || !ModContent.GetInstance<SacrificeCountConfig>().asItemChecklist)
        {
            orig.Invoke(ref inv, context);
        }
    }

    private void ItemSlot_LeftClick_refItem_int(On_ItemSlot.orig_LeftClick_refItem_int orig, ref Item inv, int context)
    {
        if (context != 29 || !ModContent.GetInstance<SacrificeCountConfig>().asItemChecklist)
        {
            orig.Invoke(ref inv, context);
        }
    }

    private void UISearchBar_SetContents(On_UISearchBar.orig_SetContents orig, UISearchBar self, string contents, bool forced)
    {
        //IL_00a0: Unknown result type (might be due to invalid IL or missing references)
        //IL_00c5: Unknown result type (might be due to invalid IL or missing references)
        if (((KeyboardState)(Main.inputText)).IsKeyDown((Keys)27))
        {
            self.ToggleTakingText();
            Main.LocalPlayer.ToggleInv();
            return;
        }
        if (ignoreSetBar)
        {
            FieldInfo field = ((object)self).GetType().GetField("actualContents", (BindingFlags)36);
            FieldInfo field2 = ((object)self).GetType().GetField("_text", (BindingFlags)36);
            FieldInfo field3 = ((object)self).GetType().GetField("_textToShowWhenEmpty", (BindingFlags)36);
            FieldInfo field4 = ((object)self).GetType().GetField("_textScale", (BindingFlags)36);
            object value = field2.GetValue((object)searchBar);
            UITextBox val = (UITextBox)((value is UITextBox) ? value : null);
            object value2 = field3.GetValue((object)self);
            LocalizedText val2 = (LocalizedText)((value2 is LocalizedText) ? value2 : null);
            field.SetValue((object)self, (object)contents);
            if (string.IsNullOrEmpty(contents))
            {
                ((UITextPanel<string>)(object)val).TextColor = Color.Gray;
                ((UITextPanel<string>)(object)val).SetText(val2.Value, (float)field4.GetValue((object)self), false);
            }
            else
            {
                ((UITextPanel<string>)(object)val).TextColor = Color.White;
                ((UITextPanel<string>)(object)val).SetText(contents);
            }
            return;
        }
        if (SearchBarContents == null || SearchBarContents.Length == 0)
        {
            SearchBarContents = new string[1];
        }
        OriginalSearchString = (SearchBarContents[0] = ((contents == null) ? "" : contents));
        if (SearchBarContents[0] == null || SearchBarContents[0].Equals(""))
        {
            searchBarContentsHasFilter = false;
            orig.Invoke(self, contents, forced);
            return;
        }
        SearchBarContents = SearchBarContents[0].Trim().Split((char[])null, (StringSplitOptions)3);
        searchBar = self;
        if (SearchBarContents == null || SearchBarContents.Length == 0)
        {
            searchBarContentsHasFilter = false;
            orig.Invoke(self, contents, forced);
            return;
        }
        string text = SearchBarContents[0];
        switch (CommonUtils.ComputeStringHash(text))
        {
            case 3305896031u:
                if (text == "@")
                {
                    goto IL_02aa;
                }
                goto default;
            case 638357778u:
                if (text == "#")
                {
                    goto IL_02aa;
                }
                goto default;
            case 3176739326u:
                if (text == ".fav")
                {
                    goto IL_02aa;
                }
                goto default;
            case 2758346563u:
                if (text == ".favo")
                {
                    goto IL_02aa;
                }
                goto default;
            case 39211555u:
                if (text == ".favor")
                {
                    goto IL_02aa;
                }
                goto default;
            case 4158884478u:
                if (text == ".favori")
                {
                    goto IL_02aa;
                }
                goto default;
            case 1160924606u:
                if (text == ".favorit")
                {
                    goto IL_02aa;
                }
                goto default;
            case 3375402945u:
                if (text == ".favorite")
                {
                    goto IL_02aa;
                }
                goto default;
            case 1520730416u:
                if (text == ".favorties")
                {
                    goto IL_02aa;
                }
                goto default;
            default:
                {
                    searchBarContentsHasFilter = false;
                    break;
                }
                IL_02aa:
                searchBarContentsHasFilter = true;
                break;
        }
        orig.Invoke(self, contents, forced);
    }

    private void UICreativeInfiniteItemsDisplay_UpdateContents(On_UICreativeInfiniteItemsDisplay.orig_UpdateContents orig, UICreativeInfiniteItemsDisplay self)
    {
        ignoreSetBar = true;
        string text = "";
        if (searchBarContentsHasFilter)
        {
            for (int i = 1; i < SearchBarContents.Length; i++)
            {
                text = text + SearchBarContents[i] + " ";
            }
            searchBar.SetContents(text.Trim(), false);
            orig.Invoke(self);
            text = "";
            for (int j = 0; j < SearchBarContents.Length; j++)
            {
                text = text + SearchBarContents[j] + " ";
            }
            searchBar.SetContents(OriginalSearchString, false);
        }
        else
        {
            orig.Invoke(self);
        }
        ignoreSetBar = false;
    }

    private void UIDynamicItemCollection_SetContentsToShow(On_UIDynamicItemCollection.orig_SetContentsToShow orig, UIDynamicItemCollection self, List<int> itemIdsToShow)
    {
        //IL_0391: Unknown result type (might be due to invalid IL or missing references)
        //IL_0398: Expected O, but got Unknown
        //IL_0489: Unknown result type (might be due to invalid IL or missing references)
        //IL_0490: Expected O, but got Unknown
        //IL_0564: Unknown result type (might be due to invalid IL or missing references)
        //IL_056b: Expected O, but got Unknown
        ResearchPlayer rp = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
        if (SearchBarContents == null || SearchBarContents.Length == 0)
        {
            orig.Invoke(self, itemIdsToShow);
            return;
        }
        string text = SearchBarContents[0];
        int num = default(int);
        List<Item> val;
        int num2 = default(int);
        bool flag;
        switch (CommonUtils.ComputeStringHash(text))
        {
            case 3196928671u:
                if (text == ".mod")
                {
                    goto IL_0286;
                }
                goto default;
            case 3305896031u:
                if (text == "@")
                {
                    goto IL_0286;
                }
                goto default;
            case 638357778u:
                if (!(text == "#"))
                {
                    goto default;
                }
                orig.Invoke(self, itemIdsToShow);
                break;
            case 1296938107u:
                if (!(text == ".class"))
                {
                    goto default;
                }
                if (SearchBarContents.Length >= 2)
                {
                    itemIdsToShow.Clear();
                    val = new List<Item>();
                    for (int num3 = 0; num3 < ItemLoader.ItemCount; num3++)
                    {
                        if (!ResearchGlobalItem.IsResearched(num3))
                        {
                            continue;
                        }
                        Item val4 = new Item();
                        try
                        {
                            val4.SetDefaults(num3);
                        }
                        catch (System.Exception)
                        {
                            continue;
                        }
                        if (val4 == null || val4.IsAir)
                        {
                            continue;
                        }
                        for (int num4 = 1; num4 < SearchBarContents.Length; num4++)
                        {
                            if (((ModType)val4.DamageType).Name.StartsWith(SearchBarContents[num4]) || val4.DamageType.DisplayName.Value.StartsWith(SearchBarContents[num4]))
                            {
                                val.Add(val4);
                            }
                        }
                    }
                    val.Sort((Comparison<Item>)((Item x, Item y) => (x.damage.CompareTo(y.damage) != 0) ? (-x.damage.CompareTo(y.damage)) : (-x.defense.CompareTo(y.defense))));
                    for (int num5 = 0; num5 < val.Count; num5++)
                    {
                        itemIdsToShow.Add(val[num5].type);
                    }
                    orig.Invoke(self, itemIdsToShow);
                }
                else
                {
                    orig.Invoke(self, itemIdsToShow);
                }
                break;
            case 3305920627u:
                if (text == ".dmg")
                {
                    goto IL_046f;
                }
                goto default;
            case 2325234716u:
                if (text == ".damage")
                {
                    goto IL_046f;
                }
                goto default;
            case 4095748648u:
                if (text == ".def")
                {
                    goto IL_054a;
                }
                goto default;
            case 1680514535u:
                if (text == ".defense")
                {
                    goto IL_054a;
                }
                goto default;
            case 3176739326u:
                if (text == ".fav")
                {
                    goto IL_0625;
                }
                goto default;
            case 2758346563u:
                if (text == ".favo")
                {
                    goto IL_0625;
                }
                goto default;
            case 39211555u:
                if (text == ".favor")
                {
                    goto IL_0625;
                }
                goto default;
            case 4158884478u:
                if (text == ".favori")
                {
                    goto IL_0625;
                }
                goto default;
            case 1160924606u:
                if (text == ".favorit")
                {
                    goto IL_0625;
                }
                goto default;
            case 3375402945u:
                if (text == ".favorite")
                {
                    goto IL_0625;
                }
                goto default;
            case 1520730416u:
                if (text == ".favorties")
                {
                    goto IL_0625;
                }
                goto default;
            default:
                {
                    orig.Invoke(self, itemIdsToShow);
                    break;
                }
                IL_0625:
                if (Enumerable.Count<int>((System.Collections.Generic.IEnumerable<int>)itemIdsToShow) == 0)
                {
                    itemIdsToShow.AddRange((System.Collections.Generic.IEnumerable<int>)CommonUtils.ItemListToIntList(rp.AllFavoritedItems()));
                }
                else
                {
                    itemIdsToShow = itemIdsToShow.FindAll((Predicate<int>)((int x) => rp.favourited[x]));
                }
                orig.Invoke(self, itemIdsToShow);
                break;
                IL_046f:
                itemIdsToShow.Clear();
                val = new List<Item>();
                for (int i = 0; i < ItemLoader.ItemCount; i++)
                {
                    if (!ResearchGlobalItem.IsResearched(i))
                    {
                        continue;
                    }
                    Item val2 = new Item();
                    try
                    {
                        val2.SetDefaults(i);
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                    if (val2 == null || val2.IsAir)
                    {
                        continue;
                    }
                    for (int j = 1; j < SearchBarContents.Length; j++)
                    {
                        if (int.TryParse(SearchBarContents[j], out num) && num <= val2.damage)
                        {
                            val.Add(val2);
                        }
                    }
                }
                val.Sort((Comparison<Item>)((Item x, Item y) => (x.damage.CompareTo(y.damage) != 0) ? (-x.damage.CompareTo(y.damage)) : (-x.defense.CompareTo(y.defense))));
                for (int k = 0; k < val.Count; k++)
                {
                    itemIdsToShow.Add(val[k].type);
                }
                orig.Invoke(self, itemIdsToShow);
                break;
                IL_054a:
                itemIdsToShow.Clear();
                val = new List<Item>();
                for (int l = 0; l < ItemLoader.ItemCount; l++)
                {
                    if (!ResearchGlobalItem.IsResearched(l))
                    {
                        continue;
                    }
                    Item val3 = new Item();
                    try
                    {
                        val3.SetDefaults(l);
                    }
                    catch (System.Exception)
                    {
                        continue;
                    }
                    if (val3 == null || val3.IsAir)
                    {
                        continue;
                    }
                    for (int m = 1; m < SearchBarContents.Length; m++)
                    {
                        if (int.TryParse(SearchBarContents[m], out num2) && num2 <= val3.defense)
                        {
                            val.Add(val3);
                        }
                    }
                }
                val.Sort((Comparison<Item>)((Item x, Item y) => (x.defense.CompareTo(y.defense) != 0) ? (-x.defense.CompareTo(y.defense)) : (-x.damage.CompareTo(y.damage))));
                for (int n = 0; n < val.Count; n++)
                {
                    itemIdsToShow.Add(val[n].type);
                }
                orig.Invoke(self, itemIdsToShow);
                break;
                IL_0286:
                itemIdsToShow.Clear();
                flag = false;
                for (int num6 = 1; num6 < SearchBarContents.Length; num6++)
                {
                    if ("terraria".StartsWith(SearchBarContents[num6]))
                    {
                        flag = true;
                        break;
                    }
                }
                for (int num7 = 0; num7 < ItemLoader.ItemCount; num7++)
                {
                    if (!ResearchGlobalItem.IsResearched(num7))
                    {
                        continue;
                    }
                    ModItem item = ItemLoader.GetItem(num7);
                    if (item == null && flag)
                    {
                        itemIdsToShow.Add(num7);
                    }
                    else
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        for (int num8 = 1; num8 < SearchBarContents.Length; num8++)
                        {
                            if (((ModType)item).Mod.Name.StartsWith(SearchBarContents[num8]) || ((ModType)item).Mod.DisplayName.StartsWith(SearchBarContents[num8]))
                            {
                                itemIdsToShow.Add(num7);
                                break;
                            }
                        }
                    }
                }
                orig.Invoke(self, itemIdsToShow);
                break;
        }
    }

    private void ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
    {
        //IL_000d: Unknown result type (might be due to invalid IL or missing references)
        //IL_000f: Unknown result type (might be due to invalid IL or missing references)
        //IL_001d: Unknown result type (might be due to invalid IL or missing references)
        //IL_001f: Unknown result type (might be due to invalid IL or missing references)
        //IL_0024: Unknown result type (might be due to invalid IL or missing references)
        //IL_0029: Unknown result type (might be due to invalid IL or missing references)
        //IL_0035: Unknown result type (might be due to invalid IL or missing references)
        //IL_0037: Unknown result type (might be due to invalid IL or missing references)
        if (context != 29)
        {
            orig.Invoke(spriteBatch, inv, context, slot, position, lightColor);
            return;
        }
        drawingInCreative = true;
        SlotPosition = position + Vector2.Zero;
        orig.Invoke(spriteBatch, inv, context, slot, position, lightColor);
    }

    public override void Unload()
    {
        base.Unload();
    }

    private void ItemSlot_OverrideHover_refItem_int(On_ItemSlot.orig_OverrideHover_refItem_int orig, ref Item inv, int context)
    {
        if (context != 29)
        {
            orig.Invoke(ref inv, context);
            return;
        }
        ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
        if (Main.mouseMiddle && Main.mouseMiddleRelease)
        {
            if (inv.type > modPlayer.favourited.Length)
            {
                System.Array.Resize<bool>(ref modPlayer.favourited, inv.type + 1);
            }
            modPlayer.favourited[inv.type] = !modPlayer.favourited[inv.type];
        }
        orig.Invoke(ref inv, context);
    }

    public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        //IL_0058: Unknown result type (might be due to invalid IL or missing references)
        //IL_0066: Unknown result type (might be due to invalid IL or missing references)
        //IL_0072: Unknown result type (might be due to invalid IL or missing references)
        //IL_0078: Unknown result type (might be due to invalid IL or missing references)
        //IL_0092: Unknown result type (might be due to invalid IL or missing references)
        //IL_0093: Unknown result type (might be due to invalid IL or missing references)
        //IL_0095: Unknown result type (might be due to invalid IL or missing references)
        //IL_0097: Unknown result type (might be due to invalid IL or missing references)
        //IL_0099: Unknown result type (might be due to invalid IL or missing references)
        if (drawingInCreative)
        {
            ResearchPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<ResearchPlayer>();
            if (item.type > modPlayer.favourited.Length)
            {
                System.Array.Resize<bool>(ref modPlayer.favourited, item.type + 1);
            }
            if (modPlayer.favourited[item.type])
            {
                spriteBatch.Draw(favourite_overlay.Value, SlotPosition, (Rectangle?)null, Color.White, 0f, default(Vector2), Main.inventoryScale, (SpriteEffects)0, 0f);
            }
            drawingInCreative = false;
        }
        base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }
}
