using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TheBaron.Bot;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly InteractionService _handler;
    private readonly ILogger<InteractionHandler> _logger;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services,
        IConfiguration config, ILogger<InteractionHandler> logger)
    {
        _client = client;
        _handler = handler;
        _services = services;
        _configuration = config;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        // Process when the client is ready, so we can register our commands.
        _client.Ready += ReadyAsync;
        _handler.Log += LogMapper.GetFunc(_logger);

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;
    }

    private async Task ReadyAsync()
    {
        // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
        // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
# if DEBUG
        var guildId = _configuration.GetValue<ulong>("Discord:TestGuildId");

        await _handler.RegisterCommandsToGuildAsync(guildId, deleteMissing: true);
# else
            await _handler.RegisterCommandsGloballyAsync(true);
# endif
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            var result = await _handler.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync()
                    .ContinueWith(async msg => await msg.Result.DeleteAsync());
        }
    }
}