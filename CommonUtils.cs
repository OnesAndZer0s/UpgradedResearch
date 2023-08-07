using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace UpgradedResearch.Common;

public static class CommonUtils
{
    public static List<int> ItemListToIntList(List<Item> items)
    {
        //IL_000c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0011: Unknown result type (might be due to invalid IL or missing references)
        List<int> val = new List<int>();
        if (items == null)
        {
            return val;
        }
        List<Item>.Enumerator enumerator = items.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Item current = enumerator.Current;
                val.Add(current.type);
            }
            return val;
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
    }

    public static List<Item> IntListToItemList(List<int> items)
    {
        //IL_000c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0011: Unknown result type (might be due to invalid IL or missing references)
        List<Item> val = new List<Item>();
        if (items == null)
        {
            return val;
        }
        List<int>.Enumerator enumerator = items.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                val.Add(ContentSamples.ItemsByType[current]);
            }
            return val;
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
    }

    public static bool[] ItemsIntoBoolArray(List<Item> items)
    {
        //IL_0011: Unknown result type (might be due to invalid IL or missing references)
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        bool[] array = new bool[ItemLoader.ItemCount];
        if (items == null)
        {
            return array;
        }
        List<Item>.Enumerator enumerator = items.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Item current = enumerator.Current;
                if (!current.IsAir)
                {
                    array[current.type] = true;
                }
            }
            return array;
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
    }

    public static bool[] ItemsIntoBoolArray(List<int> items)
    {
        //IL_000c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0011: Unknown result type (might be due to invalid IL or missing references)
        bool[] array = new bool[ItemLoader.ItemCount];
        List<int>.Enumerator enumerator = items.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                array[current] = true;
            }
            return array;
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
    }

    public static int[] CompactBoolArray(bool[] validatedItem)
    {
        int[] array = new int[1 + validatedItem.Length / 32];
        for (int i = 1; i < validatedItem.Length; i += 32)
        {
            int num = 0;
            for (int j = 0; j < 32 && j + i < validatedItem.Length; j++)
            {
                num |= (validatedItem[j + i] ? (1 << j) : 0);
            }
            array[i / 32] = num;
        }
        return array;
    }

    public static bool[] UncompactBoolArray(int[] val, int withLength = -1)
    {
        int num = ((withLength < 0) ? (val.Length * 32) : withLength);
        bool[] array = new bool[num];
        for (int i = 1; i < num && i < val.Length * 32; i += 32)
        {
            for (int j = 0; j < 32; j++)
            {
                if (j + i >= num)
                {
                    return array;
                }
                array[j + i] = (val[i / 32] & (1 << j)) != 0;
            }
        }
        return array;
    }

    public static TagCompound CompactedBoolArrayToTagCompound(int[] compactedArray)
    {
        //IL_0000: Unknown result type (might be due to invalid IL or missing references)
        //IL_0006: Expected O, but got Unknown
        TagCompound val = new TagCompound();
        val.Set("c", (object)compactedArray.Length, false);
        for (int i = 0; i < compactedArray.Length; i++)
        {
            if (compactedArray[i] != 0)
            {
                val.Set(i.ToString() ?? "", (object)compactedArray[i], false);
            }
        }
        return val;
    }

    public static int[] CompactedBoolArrayFromTagCompound(TagCompound ans)
    {
        if (!ans.ContainsKey("c"))
        {
            return null;
        }
        int[] array = new int[ans.GetAsInt("c")];
        for (int i = 0; i < array.Length; i++)
        {
            if (ans.ContainsKey(i.ToString() ?? ""))
            {
                array[i] = ans.GetAsInt(i.ToString() ?? "");
            }
        }
        return array;
    }

    public static List<Item> ItemsFromBoolArray(bool[] array)
    {
        List<Item> val = new List<Item>();
        for (int i = 0; i < ItemLoader.ItemCount && i < array.Length; i++)
        {
            if (array[i])
            {
                val.Add(ContentSamples.ItemsByType[i]);
            }
        }
        return val;
    }

    public static TagCompound CompressItemListToTagCompound(List<Item> itemList)
    {
        //IL_0003: Unknown result type (might be due to invalid IL or missing references)
        //IL_0009: Expected O, but got Unknown
        if (itemList == null)
        {
            return new TagCompound();
        }
        return CompactedBoolArrayToTagCompound(CompactBoolArray(ItemsIntoBoolArray(itemList)));
    }

    public static TagCompound CompressItemListToTagCompound(List<int> itemList)
    {
        //IL_0003: Unknown result type (might be due to invalid IL or missing references)
        //IL_0009: Expected O, but got Unknown
        if (itemList == null)
        {
            return new TagCompound();
        }
        return CompactedBoolArrayToTagCompound(CompactBoolArray(ItemsIntoBoolArray(itemList)));
    }

    public static List<Item> UncompressItemListFromTagCompound(TagCompound ans)
    {
        if (ans == null || !ans.ContainsKey("c"))
        {
            return null;
        }
        return ItemsFromBoolArray(UncompactBoolArray(CompactedBoolArrayFromTagCompound(ans)));
    }

    public static TagCompound CompressBoolArrayToTagCompound(bool[] toCompress)
    {
        //IL_0003: Unknown result type (might be due to invalid IL or missing references)
        //IL_0009: Expected O, but got Unknown
        if (toCompress == null)
        {
            return new TagCompound();
        }
        return CompactedBoolArrayToTagCompound(CompactBoolArray(toCompress));
    }

    public static bool[] UncompressBoolArrayFromTagCompound(TagCompound ans)
    {
        if (ans == null || !ans.ContainsKey("c"))
        {
            return null;
        }
        return UncompactBoolArray(CompactedBoolArrayFromTagCompound(ans));
    }

    public static uint ComputeStringHash(string s)
    {
    uint num = default(uint);
    if (s != null)
    {
        num = 2166136261u;
        for (int i = 0; i < s.Length; i++)
        {
            num = (s[i] ^ num) * 16777619;
        }
    }
    return num;
}

}
