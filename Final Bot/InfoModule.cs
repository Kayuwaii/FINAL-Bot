using Discord;
using Discord.Commands;
using System;
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
            await (await Context.User.CreateDMChannelAsync()).SendMessageAsync("help");
            Bot.logToText(" -- " + Context.User.Username + " asked for help on channel " + Context.Channel.Name);
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
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }




        //NOT COMMANDS

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
    }


}
