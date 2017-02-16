namespace Final_Bot
{
    class Program
    {
        // Convert our sync-main to an async main method
        static void Main(string[] args) => new Bot().Start().GetAwaiter().GetResult();


    }

    
}