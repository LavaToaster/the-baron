using Discord.Interactions;
using Discord.WebSocket;

namespace TheBaron.Bot.Extensions;

public static class ServiceCollection
{
    public static IServiceCollection AddDiscord(this IServiceCollection services)
    {
        services.AddHostedService<BotService>();
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddSingleton<InteractionHandler>();

        return services;
    }
}