using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace MyBot
{
    public class Program
    {
        public static void Main(string[] args)
            => new Bot().MainAsync().GetAwaiter().GetResult();
    }
}