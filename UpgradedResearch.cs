using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using UpgradedResearch.Common;
using UpgradedResearch.Items;

namespace UpgradedResearch;

public class UpgradedResearch : Mod
{
    // public override void Unload()
    // {
    // }

    public override object Call(params object[] args)
    {
        return CallHandler.HandleCall(args);
    }

    public void SendRequestResearchPacket(int playerThatNeedsInfo, int playerWhoseInfoIsNeeded)
    {
        ModPacket packet = ((Mod)this).GetPacket(256);
        ((BinaryWriter)packet).Write((byte)1);
        ((BinaryWriter)packet).Write((byte)playerThatNeedsInfo);
        ((BinaryWriter)packet).Write((byte)playerWhoseInfoIsNeeded);
        if (Main.netMode == 2 && playerThatNeedsInfo < 255)
        {
            packet.Send(playerThatNeedsInfo, -1);
        }
        else
        {
            packet.Send(-1, -1);
        }
    }

    public void SendRequestFavoritesPacket(int playerThatNeedsInfo, int playerWhoseInfoIsNeeded)
    {
        ModPacket packet = ((Mod)this).GetPacket(256);
        ((BinaryWriter)packet).Write((byte)3);
        ((BinaryWriter)packet).Write((byte)playerThatNeedsInfo);
        ((BinaryWriter)packet).Write((byte)playerWhoseInfoIsNeeded);
        if (Main.netMode == 2 && playerThatNeedsInfo < 255)
        {
            packet.Send(playerThatNeedsInfo, -1);
        }
        else
        {
            packet.Send(-1, -1);
        }
    }

    public void SendRequestResearchReplyPacket(int playerThatNeedsInfo, int playerWhoseInfoIsNeeded, List<Item> research)
    {
        //IL_002d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0033: Expected O, but got Unknown
        ModPacket packet = ((Mod)this).GetPacket(256);
        ((BinaryWriter)packet).Write((byte)2);
        ((BinaryWriter)packet).Write((byte)playerThatNeedsInfo);
        ((BinaryWriter)packet).Write((byte)playerWhoseInfoIsNeeded);
        TagCompound val = CommonUtils.CompressItemListToTagCompound(research);
        if (val == null)
        {
            val = new TagCompound();
        }
        TagIO.Write(val, (BinaryWriter)(object)packet);
        if (Main.netMode == 2 && playerThatNeedsInfo < 255)
        {
            packet.Send(playerThatNeedsInfo, -1);
        }
        else
        {
            packet.Send(-1, -1);
        }
    }

    public void SendRequestFavoriteReplyPacket(int playerThatNeedsInfo, int playerWhoseInfoIsNeeded, bool[] fav)
    {
        //IL_002d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0033: Expected O, but got Unknown
        ModPacket packet = ((Mod)this).GetPacket(256);
        ((BinaryWriter)packet).Write((byte)4);
        ((BinaryWriter)packet).Write((byte)playerThatNeedsInfo);
        ((BinaryWriter)packet).Write((byte)playerWhoseInfoIsNeeded);
        TagCompound val = CommonUtils.CompressBoolArrayToTagCompound(fav);
        if (val == null)
        {
            val = new TagCompound();
        }
        TagIO.Write(val, (BinaryWriter)(object)packet);
        if (Main.netMode == 2 && playerThatNeedsInfo < 255)
        {
            packet.Send(playerThatNeedsInfo, -1);
        }
        else
        {
            packet.Send(-1, -1);
        }
    }

    public void SendRequestBuffsPacket(int playerThatNeedsInfo, int playerWhoseInfoIsNeeded)
    {
        ModPacket packet = ((Mod)this).GetPacket(256);
        ((BinaryWriter)packet).Write((byte)16);
        ((BinaryWriter)packet).Write((byte)playerThatNeedsInfo);
        ((BinaryWriter)packet).Write((byte)playerWhoseInfoIsNeeded);
        if (Main.netMode == 2 && playerThatNeedsInfo < 255)
        {
            packet.Send(playerThatNeedsInfo, -1);
        }
        else
        {
            packet.Send(-1, -1);
        }
    }

    public void SendRequestBuffsReplyPacket(int playerThatNeedsInfo, int playerWhoseInfoIsNeeded, ResearchPlayer answer)
    {
        //IL_0080: Unknown result type (might be due to invalid IL or missing references)
        //IL_0085: Unknown result type (might be due to invalid IL or missing references)
        //IL_00b6: Unknown result type (might be due to invalid IL or missing references)
        //IL_00bb: Unknown result type (might be due to invalid IL or missing references)
        ModPacket packet = ((Mod)this).GetPacket(256);
        ((BinaryWriter)packet).Write((byte)17);
        ((BinaryWriter)packet).Write((byte)playerThatNeedsInfo);
        ((BinaryWriter)packet).Write((byte)playerWhoseInfoIsNeeded);
        if (answer.allBuffsIndexes == null)
        {
            answer.allBuffsIndexes = new List<int>();
            answer.allDebuffsIndexes = new List<int>();
        }
        ((BinaryWriter)packet).Write((answer.allBuffsIndexes != null) ? answer.allBuffsIndexes.Count : 0);
        ((BinaryWriter)packet).Write((answer.allDebuffsIndexes != null) ? answer.allDebuffsIndexes.Count : 0);
        List<int>.Enumerator enumerator = answer.allBuffsIndexes.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                int current = enumerator.Current;
                ((BinaryWriter)packet).Write(current);
            }
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
        enumerator = answer.allDebuffsIndexes.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                int current2 = enumerator.Current;
                ((BinaryWriter)packet).Write(current2);
            }
        }
        finally
        {
            ((System.IDisposable)enumerator).Dispose();
        }
        if (Main.netMode == 2 && playerThatNeedsInfo < 255)
        {
            packet.Send(playerThatNeedsInfo, -1);
        }
        else
        {
            packet.Send(-1, -1);
        }
    }

    public void SendForceResearchPacket(int sender, Item item)
    {
        SendForceResearchPacket(sender, item.type);
    }

    public void SendForceResearchPacket(int sender, int item)
    {
        ModPacket packet = ((Mod)this).GetPacket(256);
        ((BinaryWriter)packet).Write((byte)0);
        ((BinaryWriter)packet).Write((byte)sender);
        ((BinaryWriter)packet).Write(item);
        packet.Send(-1, -1);
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        //IL_0079: Unknown result type (might be due to invalid IL or missing references)
        byte b = reader.ReadByte();
        switch (b)
        {
            case 0:
            {
                int num5 = reader.ReadByte();
                int num6 = reader.ReadInt32();
                if (Main.netMode == 2)
                {
                    SendForceResearchPacket(num5, num6);
                }
                else if (!ResearchGlobalItem.IsResearched(num6) && Main.player[num5] != null && ((Entity)Main.player[num5]).active && Main.player[num5].team == Main.player[Main.myPlayer].team && Main.player[Main.myPlayer].team != 0)
                {
                    CreativeUI.ResearchItem(num6);
                }
                return;
            }
            case 1:
            {
                int num3 = reader.ReadByte();
                int num4 = reader.ReadByte();
                List<Item> allResearchedItems = Main.player[num4].GetModPlayer<ResearchPlayer>().GetAllResearchedItems();
                if ((Main.netMode == 2 && num3 < 255) || (Main.netMode == 1 && num3 != Main.myPlayer))
                {
                    SendRequestResearchReplyPacket(num3, num4, allResearchedItems);
                }
                return;
            }
            case 2:
            {
                int num = reader.ReadByte();
                int num2 = reader.ReadByte();
                ResearchPlayer modPlayer = Main.player[num2].GetModPlayer<ResearchPlayer>();
                TagCompound val = TagIO.Read(reader);
                List<Item> foundResearch = CommonUtils.UncompressItemListFromTagCompound(val);
                modPlayer.SetFoundResearch(foundResearch);
                if (Main.netMode == 2 && num < 255)
                {
                    ModPacket packet = ((Mod)this).GetPacket(256);
                    ((BinaryWriter)packet).Write((byte)2);
                    ((BinaryWriter)packet).Write((byte)num);
                    ((BinaryWriter)packet).Write((byte)num2);
                    TagIO.Write(val, (BinaryWriter)(object)packet);
                    packet.Send(num, -1);
                }
                break;
            }
        }
        switch (b)
        {
            case 3:
            {
                int num9 = reader.ReadByte();
                int num10 = reader.ReadByte();
                bool[] favouriteArray = Main.player[num10].GetModPlayer<ResearchPlayer>().GetFavouriteArray();
                if ((Main.netMode == 2 && num9 < 255) || (Main.netMode == 1 && num9 != Main.myPlayer))
                {
                    SendRequestFavoriteReplyPacket(num9, num10, favouriteArray);
                }
                return;
            }
            case 4:
            {
                int num7 = reader.ReadByte();
                int num8 = reader.ReadByte();
                ResearchPlayer modPlayer2 = Main.player[num8].GetModPlayer<ResearchPlayer>();
                TagCompound val2 = TagIO.Read(reader);
                if (num7 != Main.myPlayer)
                {
                    num7 = num7;
                }
                bool[] foundFavorites = CommonUtils.UncompressBoolArrayFromTagCompound(val2);
                modPlayer2.SetFoundFavorites(foundFavorites);
                if (Main.netMode == 2 && num7 < 255)
                {
                    ModPacket packet2 = ((Mod)this).GetPacket(256);
                    ((BinaryWriter)packet2).Write((byte)4);
                    ((BinaryWriter)packet2).Write((byte)num7);
                    ((BinaryWriter)packet2).Write((byte)num8);
                    TagIO.Write(val2, (BinaryWriter)(object)packet2);
                    packet2.Send(num7, -1);
                }
                break;
            }
        }
        switch (b)
        {
            case 16:
            {
                int num15 = reader.ReadByte();
                int num16 = reader.ReadByte();
                if ((Main.netMode == 2 && num15 < 255) || (Main.netMode == 1 && num15 != Main.myPlayer))
                {
                    SendRequestBuffsReplyPacket(num15, num16, Main.player[num16].GetModPlayer<ResearchPlayer>());
                }
                break;
            }
            case 17:
            {
                int num11 = reader.ReadByte();
                int num12 = reader.ReadByte();
                ResearchPlayer modPlayer3 = Main.player[num12].GetModPlayer<ResearchPlayer>();
                int num13 = reader.ReadInt32();
                int num14 = reader.ReadInt32();
                modPlayer3.allBuffsIndexes.Clear();
                modPlayer3.allDebuffsIndexes.Clear();
                if (num11 != Main.myPlayer)
                {
                    num11 = num11;
                }
                if (modPlayer3.allBuffsIndexes == null)
                {
                    modPlayer3.allBuffsIndexes = new List<int>();
                    modPlayer3.allDebuffsIndexes = new List<int>();
                }
                for (int i = 0; i < num13; i++)
                {
                    modPlayer3.allBuffsIndexes.Add(reader.ReadInt32());
                }
                for (int j = 0; j < num14; j++)
                {
                    modPlayer3.allDebuffsIndexes.Add(reader.ReadInt32());
                }
                if (Main.netMode == 2)
                {
                    SendRequestBuffsReplyPacket(num11, num12, modPlayer3);
                }
                break;
            }
        }
    }
}
