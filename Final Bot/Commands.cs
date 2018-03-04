using Discord;
using Discord.WebSocket;
using Final_Bot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Final_Bot
{
    partial class Commands
    {
        public static async Task DoCommand(SocketMessage message)
        {
            String[] content = message.Content.Split(' ');
            String command = content[0].ToLower();
            String[] args = null;

            if (content.Length > 1) args = content.Skip(1).ToArray();

            switch (command)
            {
                case "!help":
                    await help(message);
                    break;
                case "!pic":
                    await pic(message, args[0]);
                    break;
                case "!gif":
                    await gif(message, args[0]);
                    break;
                case "!delete":
                    await delete(message);
                    break;
                case "!addpic":
                    addpic(message, args);
                    break;
                case "!addgif":
                    addgif(message, args);
                    break;
            }
        }

        private static void addpic(SocketMessage msg, string[] args)
        {
            XDocument xmlDoc = XDocument.Load(".\\resources\\images.xml");
            XElement temp = new XElement("img", new XAttribute("name", args[0]));
            xmlDoc.Element("images").Add(temp);
            temp.Add(new XElement("url", args[1]));
            xmlDoc.Save(".\\resources\\images.xml");

            delComm(msg);
        }

        private static void addgif(SocketMessage msg, string[] args)
        {
            XDocument xmlDoc = XDocument.Load(".\\resources\\images.xml");
            XElement temp = new XElement("gif", new XAttribute("name", args[0]));
            xmlDoc.Element("images").Add(temp);
            temp.Add(new XElement("url", args[1]));
            xmlDoc.Save(".\\resources\\images.xml");

            delComm(msg);
        }

        private static async Task help(SocketMessage msg)
        {
            delComm(msg);
            await (await msg.Author.GetOrCreateDMChannelAsync()).SendMessageAsync("Comandos: !help, !pic, !gif, !delete, !addpic, !addgif.");
        }

        private static async Task pic(SocketMessage msg, string file = null)
        {
            if (file == null) await msg.Channel.SendMessageAsync("Wich File?");
            else
            {
                delComm(msg);
                string fpath = GetFile(file, "img");
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(fpath, file + ".jpeg");
                }

                await msg.Channel.SendFileAsync(".\\" + file + ".jpeg");
                File.Delete(".\\" + file + ".jpeg");
            }
        }

        private static async Task gif(SocketMessage msg, string file = null)
        {
            if (file == null) await msg.Channel.SendMessageAsync("Wich File?");
            else
            {
                string fpath = GetFile(file, "gif");
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadFile(fpath, file + ".gif");
                }

                await msg.Channel.SendFileAsync(".\\" + file + ".gif");
                File.Delete(".\\" + file + ".gif");
            }
            delComm(msg);
        }

        private static async Task delete(SocketMessage msg)
        {
            var user = msg.Author as IGuildUser;

            if (user.hasRole("Admins"))
            {
                var deleteMsg = await msg.Channel.GetMessagesAsync(100).Flatten();
                await msg.Channel.DeleteMessagesAsync(deleteMsg);
                string logLine = " -- The user " + user.Username + " has used the delete command on channel " + msg.Channel.Name + ". 100 messages have been deleted.";
            }
            else
            {
                await msg.Channel.SendMessageAsync(user.Mention + " You can't do that");
                string logLine = " -- The user " + user.Username + " has used the delete command on channel " + msg.Channel.Name + ". Not executed due to lack of permisions.";
            }
        }

        //NOT COMMANDS
        static async void delComm(SocketMessage message)
        {
            try
            {
                await message.DeleteAsync();
            }
            catch (Exception e)
            {

            }
        }

        static string GetFile(string name, string type)
        {
            string file = "";
            using (StreamReader sr = new StreamReader(".\\resources\\images.xml", true))
            {
                XmlDocument xmlDoc1 = new XmlDocument();
                xmlDoc1.Load(sr);
                XmlNodeList itemNodes = xmlDoc1.GetElementsByTagName(type);
                if (itemNodes.Count > 0)
                {
                    foreach (XmlElement node in itemNodes)
                    {
                        if (node.Attributes["name"].Value.ToString() == name.Trim())
                        {
                            file = node.SelectSingleNode("url").InnerText;
                            break;
                        }

                    }
                }
            }
            return file;
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



