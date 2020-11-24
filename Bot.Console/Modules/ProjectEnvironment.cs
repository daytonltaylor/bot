using System;

namespace Bot.Console.Modules
{
    public static class ProjectEnvironment
    {
        public static string CommandPrefix_Default =>
            Environment.GetEnvironmentVariable("CommandPrefix_Default");

        public static string DiscordBot_LoginToken =>
            Environment.GetEnvironmentVariable("DiscordBot_LoginToken");
    }
}
