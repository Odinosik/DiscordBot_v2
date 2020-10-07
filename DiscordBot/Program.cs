using System;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var bot = new Bot();
            await bot.RunAsync();
        }
    }
}
