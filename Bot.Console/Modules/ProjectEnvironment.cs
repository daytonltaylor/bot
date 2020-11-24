using System;

namespace Bot.Console.Modules
{
    public static class ProjectEnvironment
    {
        public static string DiscordBot_LoginToken =>
            Environment.GetEnvironmentVariable("DiscordBot_LoginToken");
    }
}
