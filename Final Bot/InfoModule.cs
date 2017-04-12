using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Final_Bot
{
    // Create a module with no prefix (STANDARD COMMANDS)
    public class Info : ModuleBase
    {
        [Command("help"), Summary("Shows some help")]
        public async Task help()
        {
            delComm();
            await (await Context.User.CreateDMChannelAsync()).SendMessageAsync("This are the commands: delete, say, userinfo, pic.");
            Bot.logToText(" -- " + Context.User.Username + " asked for help on channel " + Context.Channel.Name);
        }

        [Command("pic"), Summary("Sends a image from our list")]
        public async Task pic([Summary("The image to send")] string file = null)
        {
            if (file == null) await Context.Channel.SendMessageAsync("Wich File?");
            else
            {
                delComm();
                string fpath = ".\\images\\" + file + ".jpeg";
                await Context.Channel.SendFileAsync(fpath);
            }

        }

        [Command("gif"), Summary("Sends a image from our list")]
        public async Task gif([Summary("The image to send")] string file = null)
        {
            if (file == null) await Context.Channel.SendMessageAsync("Wich File?");
            else
            {
                delComm();
                string fpath = ".\\gifs\\" + file + ".gif";
                await Context.Channel.SendFileAsync(fpath);
            }

        }


        [Command("delete"), Summary("Deletes up to 100 messages")]
        public async Task delete()
        {
            var user = Context.User as IGuildUser;

            if (user.hasRole("admin"))
            {
                var deleteMsg = await Context.Channel.GetMessagesAsync(100).Flatten();
                await Context.Channel.DeleteMessagesAsync(deleteMsg);
                string logLine = " -- The user " + user.Username + " has used the delete command on channel " + Context.Channel.Name + ". 100 messages have been deleted.";

                Bot.logToText(logLine);
            }
            else
            {
                await Context.Channel.SendMessageAsync(user.Mention + " You can't do that");
                string logLine = " -- The user " + user.Username + " has used the delete command on channel " + Context.Channel.Name + ". Not executed due to lack of permisions.";

                Bot.logToText(logLine);
            }

        }

        [Command("say"), Summary("Echos a message.")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        [Command("userinfo"), Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] IUser user = null)
        {
            if (user == null)
            {
                await ReplyAsync("You missed one argument");
            }
            else
            {
                await ReplyAsync($"{user.Username}#{user.Discriminator}");
            }


        }




        //NOT COMMANDS
        async void delComm()
        {
            await Context.Message.DeleteAsync();
        }
    }

    public static class xtndMethods
    {
        public static bool hasRole(this IGuildUser usr, string role)
        {
            var ch = usr.Guild as IGuild;

            var roleID = ch.Roles.FirstOrDefault(x => x.Name == role).Id;
            if (usr.RoleIds.ToList().Contains(roleID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isInList(this string input, List<string> list)
        {
            return (list.Contains(input.ToLower()));

        }
    }


}
