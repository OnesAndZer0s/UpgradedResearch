using System.Collections.Generic;
using log4net;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace UpgradedResearch.Common;

public class CallHandler
{
    public static ILog Logger;

    public static object HandleCall(params object[] args)
    {
        if (args.Length == 0 || !(args[0] is string) || !(args[0] is string))
        {
            Logger.Info((object)"Mod call was Empty. This will display, in log, what functions are currently available. All function calls are case-insensitive, but arguments are not.");
            Logger.Info((object)"Player arguments accept the Player object or the Player.whoAmI (position in the Main.player array)");
            Logger.Info((object)"Item arguments take Item objects, ModItem object, the int Type of that item or the Item Tag of the item)");
            Logger.Info((object)"Int values accept int or its string representation");
            Logger.Info((object)"Functions available");
            Logger.Info((object)" - GetFavourites (Player)");
            Logger.Info((object)"Gets the list of items that are present in the player's favourites");
            Logger.Info((object)" - GetFavouritesAsBooleans (Player)");
            Logger.Info((object)"Gets the list of booleans that hold the player's favorites");
            Logger.Info((object)" - IsFavourite (Player, Item)");
            Logger.Info((object)"Returns true if the item is in this player's favourites, false otherwise");
            Logger.Info((object)" - GetAllResearchedItems(Player player)");
            Logger.Info((object)"Gets the list of items that were researched by the current main player, but only on Client. In Server, redirects the question to the client of the given player.");
            return null;
        }
        string text = args[0] as string;
        if (text.ToLower().Equals("getfavourites") || text.ToLower().Equals("getfavouritesasbooleans"))
        {
            if (args.Length < 2)
            {
                Logger.Error((object)("Error in ModCall " + text + ": Invalid parameter number (Player)"));
                return null;
            }
            Player playerFromObject = getPlayerFromObject(args[1]);
            if (playerFromObject == null)
            {
                Logger.Error((object)("Error in ModCall " + text + ": Player is null (means not really a Player or a player.whoAmI)"));
                return null;
            }
            ResearchPlayer modPlayer = playerFromObject.GetModPlayer<ResearchPlayer>();
            if (text.ToLower().Equals("getfavouritesasbooleans"))
            {
                return modPlayer.GetFavouriteArray();
            }
            return modPlayer.AllFavoritedItems();
        }
        if (text.ToLower().Equals("getallresearcheditems"))
        {
            if (args.Length < 2)
            {
                Logger.Error((object)("Error in ModCall " + text + ": Invalid parameter number (Player)"));
                return null;
            }
            Player playerFromObject2 = getPlayerFromObject(args[1]);
            if (playerFromObject2 == null)
            {
                Logger.Error((object)("Error in ModCall " + text + ": Player is null (means not really a Player or a player.whoAmI)"));
                return null;
            }
            return playerFromObject2.GetModPlayer<ResearchPlayer>().GetAllResearchedItems();
        }
        if (text.ToLower().Equals("isfavourite") || text.ToLower().Equals("isinfavourites"))
        {
            if (args.Length < 3)
            {
                Logger.Error((object)("Error in ModCall " + text + ": Invalid parameter number (Player, Item)"));
                return null;
            }
            Player playerFromObject3 = getPlayerFromObject(args[1]);
            if (playerFromObject3 == null)
            {
                Logger.Error((object)("Error in ModCall " + text + ": Player is null (means not really a Player or a player.whoAmI)"));
                return null;
            }
            int? itemTypeFromObject = getItemTypeFromObject(args[2]);
            if (!itemTypeFromObject.HasValue || !itemTypeFromObject.HasValue)
            {
                Logger.Error((object)("Error in ModCall " + text + ": Item is invalid"));
                return null;
            }
            bool[] favouriteArray = playerFromObject3.GetModPlayer<ResearchPlayer>().GetFavouriteArray();
            return (favouriteArray != null) ? new bool?(favouriteArray[itemTypeFromObject.Value]) : null;
        }
        Logger.Error((object)("Error in ModCall: function \"" + text + "\" does not exist."));
        return null;
    }

    public static List<string> getListOfStringFromObjects(object[] args, int v)
    {
        List<string> val = new List<string>();
        for (int i = v; i < args.Length; i++)
        {
            if (args[i] is System.Collections.Generic.ICollection<string>)
            {
                val.AddRange((System.Collections.Generic.IEnumerable<string>)(args[i] as System.Collections.Generic.ICollection<string>));
            }
            else if (args[i] is string[])
            {
                val.AddRange((System.Collections.Generic.IEnumerable<string>)(args[i] as string[]));
            }
            else if (args[i] is string)
            {
                val.Add(args[i] as string);
            }
        }
        return val;
    }

    public static int? getIntFromObject(object o)
    {
        if (o == null)
        {
            return null;
        }
        if ((o as int?).HasValue)
        {
            return o as int?;
        }
        int num = default(int);
        if (o is string && int.TryParse(o as string, out num))
        {
            return num;
        }
        return null;
    }

    public static int? getItemTypeFromObject(object o)
    {
        if (o == null)
        {
            return null;
        }
        if (o is int)
        {
            return (int)o;
        }
        if (o is Item)
        {
            return ((Item)((o is Item) ? o : null)).type;
        }
        if (o is ModItem)
        {
            return ((ModItem)((o is ModItem) ? o : null)).Item.type;
        }
        int num = default(int);
        if (o is string && ItemID.Search.TryGetId(o as string, out num))
        {
            return num;
        }
        return null;
    }

    public static Item getItemFromObject(object o)
    {
        if (o == null)
        {
            return null;
        }
        if (o is Item)
        {
            return (Item)((o is Item) ? o : null);
        }
        if (o is ModItem)
        {
            return ((ModItem)((o is ModItem) ? o : null)).Item;
        }
        int? itemTypeFromObject = getItemTypeFromObject(o);
        if (!itemTypeFromObject.HasValue || !itemTypeFromObject.HasValue)
        {
            return null;
        }
        return ContentSamples.ItemsByType[itemTypeFromObject.Value];
    }

    public static Player getPlayerFromObject(object o)
    {
        if (o == null)
        {
            return null;
        }
        if (o is int && (int)o < 256)
        {
            return Main.player[(int)o];
        }
        if (o is Player)
        {
            return (Player)((o is Player) ? o : null);
        }
        return null;
    }
}
