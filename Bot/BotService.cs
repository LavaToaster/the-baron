using Discord;
using Discord.WebSocket;

namespace TheBaron.Bot;

public class BotService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly InteractionHandler _handler;
    private readonly ILogger<BotService> _logger;

    public BotService(DiscordSocketClient client, InteractionHandler handler, ILogger<BotService> logger,
        IConfiguration configuration)
    {
        _handler = handler;
        _logger = logger;
        _configuration = configuration;

        _client = client;
        _client.Log += LogMapper.GetFunc(logger);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _handler.InitializeAsync();

        var token = _configuration.GetValue<string>("Discord:Token");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.StopAsync();
    }
}