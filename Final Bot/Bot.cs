using Discord;
using Discord.Commands;
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
    private CommandService commands;
    private DiscordSocketClient client;
    private IDependencyMap map;

    private List<string> q1 = new List<string>();
    private List<string> q2 = new List<string>();

    public async Task Start()
    {
        commands = new CommandService();
        client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            LogLevel = LogSeverity.Info

        });

        client.Log += (message) =>
        {
            Log(message);
            return Task.CompletedTask;
        };



        // Place the token of your bot account here
        string token = "MjU0NjQyNDI5Nzk2Mjg2NDc0.C4T9KA.Vr4vDBKSmHtPLBdbslZfV5yOWYA";

        map = new IDependencyMap();

        await InstallCommands();

        addQuestions();

        // Hook into the MessageReceived event on DiscordSocketClient
        client.MessageReceived += async (message) =>
        {
            if (message.Content.isInList(q1))
            {
                await message.Channel.SendMessageAsync("It's a fictitious buisness, it's a project from 3 Monlau students.");
            }
            else if (message.Content.isInList(q2))
            {
                await message.Channel.SendMessageAsync("You are in the Munngames FAQ discord chat. Feel free to ask whatever you" +
                    " want, and if I don't know the answer to your question, probably someone here will.");
            }
            else
            {

            }
        };
        // Configure the client to use a Bot token, and use our token
        await client.LoginAsync(TokenType.Bot, token);
        // Connect the client to Discord's gateway
        await client.ConnectAsync();

        // Block this task until the program is exited.
        await Task.Delay(-1);
    }

    public async Task InstallCommands()
    {
        // Hook the MessageReceived Event into our Command Handler
        client.MessageReceived += HandleCommand;
        // Discover all of the commands in this assembly and load them.
        await commands.AddModulesAsync(Assembly.GetEntryAssembly());
    }
    public async Task HandleCommand(SocketMessage messageParam)
    {
        // Don't process the command if it was a System Message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;
        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;
        // Determine if the message is a command, based on if it starts with '!' or a mention prefix
        if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
        // Create a Command Context
        var context = new CommandContext(client, message);
        // Execute the command. (result does not indicate a return value, 
        // rather an object stating if the command executed succesfully)
        var result = await commands.ExecuteAsync(context, argPos, map);
        if (!result.IsSuccess)
            await context.Channel.SendMessageAsync(result.ErrorReason);
    }

    private void Log(LogMessage msg)
    {
        string logLine = " -- " + msg.Source + " " + msg.Message;

        logToText(logLine);
    }

    public static void logToText(string logLine)
    {
        Console.WriteLine(DateTime.Now + logLine);
        FileStream fileStream = new FileStream("./log.txt", FileMode.Append);
        using (StreamWriter reader = new StreamWriter(fileStream))
        {
            reader.WriteLine(DateTime.Now + logLine);
        }
    }

    private void addQuestions()
    {
        #region Question 1: What is munngames?
        q1.Add("what is munngames?");
        q1.Add("what's munngames?");
        q1.Add("whats munngames?");
        q1.Add("what is mungames?");
        q1.Add("what's mungames?");
        q1.Add("whats mungames?");
        #endregion
        #region Question 2: What's this? (The Chat)
        q2.Add("what's this?");
        q2.Add("what's dis?");
        q2.Add("where am i?");
        q2.Add("what is this?");
        q2.Add("what is this?");
        q2.Add("da fuq is dis?");
        q2.Add("what the fuck is this?");
        #endregion

    }

}
