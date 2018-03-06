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
            else args = new String[] { null };

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
                    await Task.Run(() => addpic(message, args));
                    break;
                case "!addgif":
                    await Task.Run(() => addgif(message, args));
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
            await (await msg.Author.GetOrCreateDMChannelAsync()).SendMessageAsync("Comandos: !help, !pic, !gif, !delete, !addpic, !addgif." +
                "\n\n     !pic *nombre*  (muestra la foto especificada, puedes ver una lista de fotos con !pic list)" +
                "\n\n     !gif *nombre*  (muestra el gif especificado, puedes ver una lista de gifs con !gif list)" +
                "\n\n     !delete  (borra los ultimos 100 mensajes, solo podrás usarlo si eres admin)" +
                "\n\n     !addpic *nombre* *enlaceDeLaFoto*  (añade la imagen desde el enlace a la lista de imagenes disponibles, una vez hecho puedes usar !pic *nombre* con el nombre que hayas especificado para mostrar la foto)" +
                "\n\n     !addgif *nombre* *enlaceDelGIF*  (añade el GIF desde el enlace a la lista de GIFs disponibles, una vez hecho puedes usar !gif *nombre* con el nombre que hayas especificado para mostrar el GIF)");
        }

        private static async Task pic(SocketMessage msg, string file = null)
        {
            if (file == null)
            {
                await msg.Channel.SendMessageAsync("BAAAKAAAAA!!! Como voy a saber que imagen quieres que ponga si no me lo dices?, seguro que querias que pusiera alguna foto mia en la playa... no es que no quiera enseñartelas... p-p-pero " + msg.Author.Mention + " no ecchi!");
                delComm(msg);
            }
            else if (file == "list")
            {
                await (await msg.Author.GetOrCreateDMChannelAsync()).SendMessageAsync("Lista de Imagenes:");
                using (StreamReader sr = new StreamReader(".\\resources\\images.xml", true))
                {
                    XmlDocument xmlDoc1 = new XmlDocument();
                    xmlDoc1.Load(sr);
                    XmlNodeList itemNodes = xmlDoc1.GetElementsByTagName("img");
                    if (itemNodes.Count > 0)
                    {
                        String all = "";
                        foreach (XmlElement node in itemNodes)
                        {
                            all += ("\n " + node.Attributes["name"].Value.ToString());
                        }
                        await (await msg.Author.GetOrCreateDMChannelAsync()).SendMessageAsync(all);
                    }
                }
                delComm(msg);
            }
            else
            {
                try
                {
                    string fpath = GetFile(file, "img");
                    using (var client = new System.Net.WebClient())
                    {
                        client.DownloadFile(fpath, file + ".jpeg");
                    }
                }
                catch (Exception ex)
                {
                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " esa imagen no existe, dejame que busque si está con la segunda temporada de NGNL... ahh, pues tampoco existe... ");
                    delComm(msg);
                }
                finally
                {
                    await msg.Channel.SendFileAsync(".\\" + file + ".jpeg");
                    File.Delete(".\\" + file + ".jpeg");
                    delComm(msg);
                }
            }
        }

        private static async Task gif(SocketMessage msg, string file = null)
        {
            if (file == null) await msg.Channel.SendMessageAsync("BAAAKAAAAA!!! Como voy a saber que gif quieres que ponga si no me lo dices?, seguro que querias que pusiera algun GIF d-d-de... lolis... " + msg.Author.Mention + " no ecchi!");
            else if (file == "list")
            {
                await (await msg.Author.GetOrCreateDMChannelAsync()).SendMessageAsync("Lista de GIFS:");
                using (StreamReader sr = new StreamReader(".\\resources\\images.xml", true))
                {
                    XmlDocument xmlDoc1 = new XmlDocument();
                    xmlDoc1.Load(sr);
                    XmlNodeList itemNodes = xmlDoc1.GetElementsByTagName("gif");
                    if (itemNodes.Count > 0)
                    {
                        String all = "";
                        foreach (XmlElement node in itemNodes)
                        {
                            all += ("\n " + node.Attributes["name"].Value.ToString());
                        }
                        await (await msg.Author.GetOrCreateDMChannelAsync()).SendMessageAsync(all);
                    }
                }
            }
            else
            {
                try
                {
                    string fpath = GetFile(file, "gif");
                    using (var client = new System.Net.WebClient())
                    {
                        client.DownloadFile(fpath, file + ".gif");
                    }
                }
                catch (Exception ex)
                {
                    await msg.Channel.SendMessageAsync(msg.Author.Mention + " Tu GIF esta en otro castillo! Okno, no existe, pero queria hacerte reir.... ");
                }
                finally
                {
                    await msg.Channel.SendFileAsync(".\\" + file + ".gif");
                    File.Delete(".\\" + file + ".gif");
                }
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
            }
            else
            {
                await msg.Channel.SendMessageAsync(user.Mention + " tu no puedes hacer eso... No es que yo quiera impedírtelo o algo... b-b-b-baka!");
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



