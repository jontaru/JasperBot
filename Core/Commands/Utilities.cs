﻿using Discord;
using Discord.WebSocket;
using JasperBot.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace JasperBot.Core.Commands
{
    public static class Utilities
    {
        public static CrafterRole RoleMapper(string id)
        {
            switch (id)
            {
                //ganksquad        
                case "<@&523919356850208788>": 
                    return CrafterRole.Scibe;

                case "<@&520853726886363136>": 
                    return CrafterRole.Scibe;

                case "<@&520854616485658625>":
                    return CrafterRole.Tailor;

                case "<@&520853780758134788>":
                    return CrafterRole.Blacksmith;

                case "<@&520853827654516757>":
                    return CrafterRole.Tamer;

                case "<@&520853913818103808>":
                    return CrafterRole.Carpentry;

                case "<@&520854030298120193>":
                    return CrafterRole.Alchemy;

                case "<@&520854111671943178>":
                    return CrafterRole.Chef;
            }

            return CrafterRole.Blacksmith;
        }

        public static string FormatRequest(CraftingRequest req, bool showCompleted = false)
        {
            if (req.status == OrderStatus.Completed && !showCompleted) return "";
            return $"[ID:{req.id}] {req.quantity}x {req.itemName} | Crafter: {req.assignedCrafter} | Status: {req.status} | Requester: {req.Requester} {Environment.NewLine}";
        }

        public static async void UpdateListing(ISocketMessageChannel channel)
        {
            string PostTitle = "Crafting Requests Listing";

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(Color.DarkOrange);
            Embed.WithTitle(PostTitle);

            string description = "";
            description += "Please type !request @ the role (@Alchemist, @Blacksmith, @Carpenter, @Chef, @Fabricator, @Scribe, @Tamer) for the amount and type of item you need. A post will be created with your order and crafter will be able to assign the order to themselves. Once completed the orders will be deleted (History will be kept)" + Environment.NewLine;
            description += "**Check basics-rules for full explanation.** *Instructions for pickup will be PMed to you. Thank you!*" + Environment.NewLine;
            description += Environment.NewLine;
            description += "If you are asking for a lot, or asking for high end/high tier gear then please be sure to donate the matierals or some similar equivalent materials (eg you can donate bars and ask for leather, etc.)" + Environment.NewLine;
            description += Environment.NewLine;
            description += "For Alchemist orders you must donate the materials, due to the nature of making potions." + Environment.NewLine;
            description += Environment.NewLine;
            description += "**Please post in this format to keep orders filled in a neat and orderly fashion: !request @Alchemist 50 Health_Potion Stamina_Potion**" + Environment.NewLine;
            description += "*Do not include anything other than the item name after the quantity. No please, emiticons or otherwise. (type !help for list of commands)*" + Environment.NewLine;
            description += Environment.NewLine;
            description += "**Order Statuses**" + Environment.NewLine;
            description += ":timer: = Your Order has been Read/Recieved by a Crafter" + Environment.NewLine;
            description += ":white_check_mark: = The Crafter is working on your order now" + Environment.NewLine;
            description += ":package: = Your Order is complete and ready for Pickup." + Environment.NewLine;
            description += ":baggage_claim: = Your Order has been picked up.";

            string scribeOrders = "";
            foreach (var req in Program.Requests)
            {
                if (req.role == CrafterRole.Scibe)
                    scribeOrders += Utilities.FormatRequest(req);
            }
            scribeOrders = (scribeOrders == "" ? "None" : scribeOrders);

            string tailorOrders = "";
            foreach (var req in Program.Requests)
            {
                if (req.role == CrafterRole.Tailor)
                    tailorOrders += Utilities.FormatRequest(req);
            }
            tailorOrders = (tailorOrders == "" ? "None" : tailorOrders);

            string tamerOrders = "";
            foreach (var req in Program.Requests)
            {
                if (req.role == CrafterRole.Tamer)
                    tamerOrders += Utilities.FormatRequest(req);
            }
            tamerOrders = (tamerOrders == "" ? "None" : tamerOrders);

            string blacksmithOrders = "";
            foreach (var req in Program.Requests)
            {
                if (req.role == CrafterRole.Blacksmith)
                    blacksmithOrders += Utilities.FormatRequest(req);
            }
            blacksmithOrders = (blacksmithOrders == "" ? "None" : blacksmithOrders);

            string carpentryOrders = "";
            foreach (var req in Program.Requests)
            {
                if (req.role == CrafterRole.Carpentry)
                    carpentryOrders += Utilities.FormatRequest(req);
            }
            carpentryOrders = (carpentryOrders == "" ? "None" : carpentryOrders);

            string alchemyOrders = "";
            foreach (var req in Program.Requests)
            {
                if (req.role == CrafterRole.Alchemy)
                    alchemyOrders += Utilities.FormatRequest(req);
            }
            alchemyOrders = (alchemyOrders == "" ? "None" : alchemyOrders);

            string cookingOrders = "";
            foreach (var req in Program.Requests)
            {
                if (req.role == CrafterRole.Chef)
                    cookingOrders += Utilities.FormatRequest(req);
            }
            cookingOrders = (cookingOrders == "" ? "None" : cookingOrders);
            
            //Page system instead of SafeLength
            Embed.AddField("Current Blacksmith Orders", SafeLength(blacksmithOrders));
            Embed.AddField("Current Tailor Orders", SafeLength(tailorOrders));
            Embed.AddField("Current Scibe Orders", SafeLength(scribeOrders));
            Embed.AddField("Current Carpentry Orders", SafeLength(carpentryOrders));
            Embed.AddField("Current Tamer Orders", SafeLength(tamerOrders));
            Embed.AddField("Current Alchemy Orders", SafeLength(alchemyOrders));
            Embed.AddField("Current Cooking Orders", SafeLength(cookingOrders));

            Embed.WithFooter($"last updated {DateTime.Now}");
            if (Program.Listing == null)
            {
                var items = await channel.GetMessagesAsync().Flatten();
                foreach (var item in items)
                {
                    if (item.Author.Username == "Jasper")
                    {
                        if (item.Embeds.Count > 0)
                        {
                            var enumerator = item.Embeds.GetEnumerator();
                            enumerator.MoveNext();
                            string title = enumerator.Current.Title;
                            if (title == PostTitle)
                            {
                                await item.DeleteAsync();
                                break;
                            }
                        }
                    }
                }

                Program.Listing = await channel.SendMessageAsync(description, false, Embed.Build());
            }
            else
            {
                await Program.Listing.ModifyAsync(msg => msg.Embed = Embed.Build());
                await Program.Listing.ModifyAsync(msg => msg.Content = description);
            }

            CleanChannel(channel);
        }

        private static object SafeLength(string value)
        {
            string ret = value;

            if (ret.Length > 1024)
            {
                ret = ret.Substring(0, 1020) + "...";
            }
            return ret;           
        }

        public static async void DeleteCMD(ISocketMessageChannel channel)
        {
            try
            {
                var items = await channel.GetMessagesAsync(1).Flatten();
                await channel.DeleteMessagesAsync(items);
            }
            catch { }
        }

        public static async void CleanChannel(ISocketMessageChannel channel)
        {
            string PostTitle = "Crafting Requests Listing";
            var items = await channel.GetMessagesAsync().Flatten();
            foreach (var item in items)
            {
                if (item.Author.Username == "Jasper")
                {
                    if (item.Embeds.Count > 0)
                    {
                        var enumerator = item.Embeds.GetEnumerator();
                        enumerator.MoveNext();
                        string title = enumerator.Current.Title;
                        if (title == PostTitle)
                        {
                            continue;
                        }
                    }
                }
                
                var local = item.CreatedAt.ToLocalTime().AddMinutes(5);
                if (DateTime.Now > local && Program.Requests.Find(r=>r.messageId ==  item.Id) == null)
                {
                    await item.DeleteAsync();
                }
            }
        }
    }
}
