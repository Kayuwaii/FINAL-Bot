using Discord;
using Discord.WebSocket;
using Final_Bot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


public class Bot
{
    public async Task MainAsync()
    {
        var client = new DiscordSocketClient();

        client.Log += Log;
        client.MessageReceived += MessageReceived;

        string token = "MjU0NjQyNDI5Nzk2Mjg2NDc0.DX2d2g.VDOJWxkCs37GNgavXOd5j7Yze7A"; // Remember to keep this private!
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private async Task MessageReceived(SocketMessage message)
    {
        if (message.Content.StartsWith("!"))
        {
            await Commands.DoCommand(message);
        }
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}
